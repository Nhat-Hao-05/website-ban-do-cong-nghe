using System.ComponentModel.DataAnnotations;

namespace MyEStore.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc Tên đăng nhập")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}