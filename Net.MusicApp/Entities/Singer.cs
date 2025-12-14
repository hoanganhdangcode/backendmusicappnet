using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.MusicApp.Entities
{
    public class Singer
    {
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SingerId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; } = "";

        public string? ImageUrl { get; set; }="";

        public DateTime CreatedAt { get; set; }

        // 1 Singer → nhiều Song
        public ICollection<Song> Songs { get; set; } = [];


    }
}
