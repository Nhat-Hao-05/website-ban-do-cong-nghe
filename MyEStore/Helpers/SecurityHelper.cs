using System.Security.Cryptography;
using System.Text;

namespace MyEStore.Helpers
{
    public class SecurityHelper
    {
        public static string MaHoaSHA512(string duLieuVao)
        {
            if (string.IsNullOrEmpty(duLieuVao)) return string.Empty;

            using (var sha512 = SHA512.Create())
            {
                // Ép kiểu Encoding UTF8 rõ ràng
                byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(duLieuVao));

                var chuoiKetQua = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    chuoiKetQua.Append(b.ToString("X2"));
                }

                return chuoiKetQua.ToString();
            }
        }
    }
}