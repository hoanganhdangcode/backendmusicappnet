namespace Net.MusicApp.DTOs
{
    // DTO để tạo mới
    public class CreatePlaylistDto
    {
        public string Name { get; set; } // Tên playlist (VD: Nhạc buồn, Nhạc tập gym)
        public int UserId { get; set; }  // Playlist này của ai?
    }

    // DTO để sửa tên
    public class UpdatePlaylistDto
    {
        public string Name { get; set; }
    }

    // DTO trả về kết quả tìm kiếm (Hiển thị)
    public class PlaylistDto
    {
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } // Hiển thị thêm tên người tạo cho xịn
        public DateTime CreatedAt { get; set; }
    }

}
