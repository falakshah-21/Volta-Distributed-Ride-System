using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Volta.Services.Auth.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        // 32-byte Key for AES-256 (Hardcoded for demo)
        private static readonly string AesKey = "12345678901234567890123456789012";

        [HttpPost("encrypt-sensitive-data")]
        public IActionResult EncryptData([FromBody] string plainText)
        {
            // Fulfills Requirement: AES Encryption for sensitive user/driver data
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AesKey);
                aes.IV = new byte[16]; // Zero IV for simplicity in demo

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        return Ok(Convert.ToBase64String(ms.ToArray()));
                    }
                }
            }
        }
    }
}