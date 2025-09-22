using System.ComponentModel.DataAnnotations;

namespace MiniBitMVC.ViewModels
{
    public class LoginFormViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        // để quay lại trang gốc sau khi login
        public string? ReturnUrl { get; set; }
    }
}
