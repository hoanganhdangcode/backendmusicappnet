using System.ComponentModel.DataAnnotations;

namespace Net.MusicApp.DTOs.AuthDTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)] // Gợi ý kiểu dữ liệu là mật khẩu
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 50 ký tự.")]
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public long ExpiresInSeconds { get; set; }
        public string? DisplayName { get; set; }
        public string? Role { get; set; }
    }
}
