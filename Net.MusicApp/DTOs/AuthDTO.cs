using Net.MusicApp.Entities;

namespace Net.MusicApp.DTOs
{
    public class UserInfoResponseDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public int Role { get; set; }
    }
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public UserInfoResponseDto User { get; set; } = null!;
    }
    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class UpdateNameDto
    {
        public string Name { get; set; } = null!;
    }
    public class UpdatePasswordDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
    public class UpdateAvatarDto
    {
        public string AvatarUrl { get; set; } = null!;
    }

}
