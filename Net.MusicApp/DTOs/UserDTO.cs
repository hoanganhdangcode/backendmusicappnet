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
    public class SingerTrendingDto
    {
        public int SingerId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public int TotalListens { get; set; } // Tổng lượt nghe
        public int SongCount { get; set; }    // Số lượng bài hát (hiển thị thêm cho oai)
    }

    public class SongDetailDto
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
        public int ListenCount { get; set; } // Hiển thị lượt nghe
        public DateTime CreatedAt { get; set; }

        // Thông tin Ca sĩ
        public int SingerId { get; set; }
        public string SingerName { get; set; }
        public string? SingerAvatar { get; set; }

        // Thông tin Thể loại
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }

    public class SingerDetailDto
    {
        public int SingerId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int SongCount { get; set; } // Tổng số bài hát
        public int TotalListens { get; set; } // Tổng lượt nghe của ca sĩ
    }

}
