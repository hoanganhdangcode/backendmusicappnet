using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.MusicApp.Entities
{
    public class User
    {

       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]  
        public string Email { get; set; }
        public string? AvatarUrl { get; set; } = "";
        [Required]
        public string Password { get; set; }

        public Role Role { get; set; } = Role.User;

        public DateTime CreatedAt { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = [];

    }

    public enum Role { 
        User = 0,
        Admin = 1,
    }
}
