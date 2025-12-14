namespace Net.MusicApp.Entities
{
    public class PlaylistSongs
    {
        public int SongId { get; set; }
        public Song? Song { get; set; }

        public int PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
