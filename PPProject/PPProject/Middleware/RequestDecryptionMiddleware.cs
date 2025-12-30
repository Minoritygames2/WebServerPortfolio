using PPProject.Common;
using System.Security.Cryptography;
using System.Text;

namespace PPProject.Middleware
{
    public class RequestDecryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secret;

        public RequestDecryptionMiddleware(RequestDelegate next,
            IConfiguration config)
        {
            _next = next;
            _secret = config["PacketSecurity:Secret"] 
                ?? throw new ArgumentNullException("PacketSecurity:Secret 설정이 필요합니다.");
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.RequestServices
                .GetRequiredService<IHostEnvironment>()
                .IsProduction())
            {
                await _next(context);
                return;
            }

            if (context.Request.Method != HttpMethods.Post || context.Request.ContentLength is null or 0)
            {
                await _next(context);
                return;
            }

            context.Request.EnableBuffering();
            try
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen:true);
                //Body를 복호화
                var body = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0;

                var decryptedBody = Decript(body);

                //복호화된 Body로 교체
                var byteArray = Encoding.UTF8.GetBytes(decryptedBody);
                context.Request.Body = new MemoryStream(byteArray);

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Decript Error");
            }
        }

        private byte[] DeriveKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: SecretDefine.ITERACTIONS,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: SecretDefine.KEY_SIZE
            );
        }

        private string Decript(string encryptedText)
        {
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(encryptedText.Trim());
            }
            catch (Exception ex)
            {
                throw new CryptographicException("invalid base64 payload");
            }


            if (bytes.Length < SecretDefine.SALT_SIZE + SecretDefine.IV_SIZE)
            {
                throw new CryptographicException("invalid base64 payload length");
            }

            var salt = new byte[SecretDefine.SALT_SIZE];
            var iv = new byte[SecretDefine.IV_SIZE];
            var cipherBytes = new byte[bytes.Length - SecretDefine.SALT_SIZE - SecretDefine.IV_SIZE];

            Buffer.BlockCopy(bytes, 0, salt, 0, SecretDefine.SALT_SIZE);
            Buffer.BlockCopy(bytes, SecretDefine.SALT_SIZE, iv, 0, SecretDefine.IV_SIZE);
            Buffer.BlockCopy(bytes, SecretDefine.SALT_SIZE + SecretDefine.IV_SIZE, cipherBytes, 0, cipherBytes.Length);

            //키 생성
            var key = DeriveKey(_secret, salt);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            try
            {
                using var decryptor = aes.CreateDecryptor();
                var resultBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(resultBytes);
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Decrypt failed");
            }
            

            
        }
    }
}
