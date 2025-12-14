using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.MusicApp.Entities

{
    public class Playlist
    {
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlaylistId { get; set; }

        public string? Name { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        // FK nối đến User
        public int UserId { get; set; }
        public User? User { get; set; }


        public ICollection<PlaylistSongs> PlaylistSongs { get; set; } = [];




    }
}
