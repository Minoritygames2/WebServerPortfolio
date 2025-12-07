using System.Security.Cryptography;
using System.Text;

namespace PPProject.Middleware
{
    public class PacketEncryptionMiddleware
    {
        private readonly RequestDelegate _next;

        private const int SaltSize = 16;    
        private const int KeySize = 32;
        private const int IvSize = 16;
        private const int Iterations = 100000;
        private readonly string _secret;

        public PacketEncryptionMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _secret = config["PacketSecurity:Secret"] ?? throw new ArgumentNullException("PacketSecurity:Secret 설정이 필요합니다.");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(context.Request.Method == HttpMethods.Post && context.Request.ContentLength > 0)
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    //Body를 복호화
                    var body = await reader.ReadToEndAsync();
                    var decryptedBody = Decript(body);

                    //복호화된 Body로 교체
                    var byteArray = Encoding.UTF8.GetBytes(decryptedBody);
                    context.Request.Body = new MemoryStream(byteArray);
                }
            }

            //기존 응답 스트림을 저장
            var originalBodyStream = context.Response.Body;

            //새로운 메모리 스트림으로 응답 바디 교체
            using var newBodyStream = new MemoryStream();
            context.Response.Body = newBodyStream;

            //컨트롤러 응답 기다림
            await _next(context);

            //스트림 포인터를 초기위치로 이동
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            //컨트롤러가 만든 응답을 꺼내서 읽음
            var plainResponseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            //응답 암호화
            var encryptedResponseBody = Encrypt(plainResponseBody);
            var responseByteArray = Encoding.UTF8.GetBytes(encryptedResponseBody);

            //원래 응답 스트림으로 복원
            context.Response.Body = originalBodyStream;

            await context.Response.Body.WriteAsync(responseByteArray, 0, responseByteArray.Length);
        }


        private byte[] DeriveKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: KeySize
            );
        }

        private string Encrypt(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var iv = RandomNumberGenerator.GetBytes(IvSize);

            var key = DeriveKey(_secret, salt);

            //AES 암호화 객체 생성
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            //실제 암호화 객체 생성
            using var encryptor = aes.CreateEncryptor();

            //암호문
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var resultByte = new byte[SaltSize + IvSize + cipherBytes.Length];

            Buffer.BlockCopy(salt, 0, resultByte, 0, SaltSize);
            Buffer.BlockCopy(iv, 0, resultByte, SaltSize, IvSize);
            Buffer.BlockCopy(cipherBytes, 0, resultByte, SaltSize + IvSize, cipherBytes.Length);

            return Convert.ToBase64String(resultByte);
        }

        private string Decript(string encryptedText)
        {
            var bytes = Convert.FromBase64String(encryptedText);

            var salt = new byte[SaltSize];
            var iv = new byte[IvSize];
            var cipherBytes = new byte[bytes.Length - SaltSize - IvSize];

            Buffer.BlockCopy(bytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(bytes, SaltSize, iv, 0, IvSize);
            Buffer.BlockCopy(bytes, SaltSize + IvSize, cipherBytes, 0, cipherBytes.Length);

            //키 생성
            var key = DeriveKey(_secret, salt);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var resultBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(resultBytes);
        }
    }
}
