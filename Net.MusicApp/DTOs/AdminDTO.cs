using System.ComponentModel.DataAnnotations;

namespace Net.MusicApp.DTOs
{
    // DTO dùng để trả dữ liệu về (GET)
    public class SingerDto
    {
        public int SingerId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO dùng cho việc Thêm mới (POST)
    public class CreateSingerDto
    {
        [Required(ErrorMessage = "Tên ca sĩ là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Thường ImageUrl sẽ được sinh ra sau khi upload file, 
        // nhưng nếu client gửi link trực tiếp thì dùng trường này.
        public string? ImageUrl { get; set; }
    }

    // DTO dùng cho việc Cập nhật (PUT)
    public class UpdateSingerDto
    {
        [Required(ErrorMessage = "Tên ca sĩ là bắt buộc.")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class GenreDto
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO thêm mới
    public class CreateGenreDto
    {
        [Required(ErrorMessage = "Tên thể loại là bắt buộc.")]
        public string Name { get; set; }

        public string? ImageUrl { get; set; }
    }

    // DTO cập nhật
    public class UpdateGenreDto
    {
        [Required(ErrorMessage = "Tên thể loại là bắt buộc.")]
        public string Name { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class SongDto
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin tóm tắt của quan hệ (tránh loop reference)
        public int SingerId { get; set; }
        public string SingerName { get; set; } // Trả về tên thay vì chỉ ID

        public int GenreId { get; set; }
        public string GenreName { get; set; }  // Trả về tên thay vì chỉ ID
    }

    // DTO thêm mới
    public class CreateSongDto
    {
        [Required(ErrorMessage = "Tên bài hát là bắt buộc.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Link bài hát là bắt buộc.")]
        [Url(ErrorMessage = "Đường dẫn Audio không hợp lệ.")]
        public string AudioUrl { get; set; }

        public string? ImageUrl { get; set; }

        // Bắt buộc phải chọn Ca sĩ và Thể loại khi tạo bài hát
        [Required]
        public int SingerId { get; set; }

        [Required]
        public int GenreId { get; set; }
    }

    // DTO cập nhật
    public class UpdateSongDto
    {
        [Required(ErrorMessage = "Tên bài hát là bắt buộc.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public string AudioUrl { get; set; }

        public string? ImageUrl { get; set; }

        public int SingerId { get; set; }
        public int GenreId { get; set; }
    }

}
