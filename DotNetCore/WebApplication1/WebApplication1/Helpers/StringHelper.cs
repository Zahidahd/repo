using System.Text;
using System.Security.Cryptography;

namespace WebApplication1.Helpers
{
    public class StringHelper
    {
        public static byte[] StringToByteArray(string password)
        {
            SHA256 sha256 = SHA256.Create();
            UTF8Encoding objUtf8 = new();
            byte[] hashValuePassword = sha256.ComputeHash(objUtf8.GetBytes(password));
            return hashValuePassword;
        }
    }
}
