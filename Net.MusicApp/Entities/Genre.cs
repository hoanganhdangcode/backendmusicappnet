using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net.MusicApp.Entities
{
    public class Genre
    {
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string? imageurl { get; set; }= "";
        public DateTime CreatedAt { get; set; }

       public ICollection<Song> Songs { get; set; } = [];


    }
}
