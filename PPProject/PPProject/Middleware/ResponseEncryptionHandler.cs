using PPProject.Common;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PPProject.Middleware
{
    public class ResponseEncryptionHandler : IResponseHandler
    {
        private readonly IHostEnvironment _env;
        private readonly string _secret;

        public ResponseEncryptionHandler(IHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _secret = config["PacketSecurity:Secret"] 
                ?? throw new ArgumentNullException("PacketSecurity:Secret 설정이 필요합니다.");
        }

        public Task HandleAsync(HttpContext context, string plainResponse)
        {
            if (!context.RequestServices
                .GetRequiredService<IHostEnvironment>()
                .IsProduction())
                return Task.CompletedTask;

            var encrypted = Encrypt(plainResponse);

            context.Items[Define.FINAL_RES_BODY] = encrypted;

            return Task.CompletedTask;
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

        private string Encrypt(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            var salt = RandomNumberGenerator.GetBytes(SecretDefine.SALT_SIZE);
            var iv = RandomNumberGenerator.GetBytes(SecretDefine.IV_SIZE);

            var key = DeriveKey(_secret, salt);

            //AES 암호화 객체 생성
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            //실제 암호화 객체 생성
            using (var encryptor = aes.CreateEncryptor())
            {
                //암호문
                var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                var resultByte = new byte[SecretDefine.SALT_SIZE + SecretDefine.IV_SIZE + cipherBytes.Length];
                Buffer.BlockCopy(salt, 0, resultByte, 0, SecretDefine.SALT_SIZE);
                Buffer.BlockCopy(iv, 0, resultByte, SecretDefine.SALT_SIZE, SecretDefine.IV_SIZE);
                Buffer.BlockCopy(cipherBytes, 0, resultByte, SecretDefine.SALT_SIZE + SecretDefine.IV_SIZE, cipherBytes.Length);
                return Convert.ToBase64String(resultByte);
            }
        }
    }
}
