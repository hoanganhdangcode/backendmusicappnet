using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.MusicApp.Entities
{
    public class Song
    {
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SongId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; } = "";

        public string AudioUrl { get; set; }
        public string? ImageUrl { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        // FK → Singer
        public int SingerId { get; set; }
        public Singer? Singer { get; set; }

        // FK → Genre
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }


        public ICollection<PlaylistSongs> PlaylistSongs { get; set; } = [];



    }


}
