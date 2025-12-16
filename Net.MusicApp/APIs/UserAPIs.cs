using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.MusicApp.Data;
using Net.MusicApp.DTOs;
using Net.MusicApp.Entities;

namespace Net.MusicApp.APIs
{
    public static class UserAPIs
    {
        public static void MapGroupUser(this WebApplication app)
        {
            var group = app.MapGroup("/user").WithTags("User APIs");
            group.MapPost("/playlist/add", async ([FromBody] CreatePlaylistDto dto, MusicAppDBContext dbContext) =>
            {
                // 1. Kiểm tra User có tồn tại không
                var user = await dbContext.Users.FindAsync(dto.UserId);
                if (user == null)
                {
                    return Results.BadRequest("Người dùng không tồn tại.");
                }

                // 2. Tạo Entity
                var newPlaylist = new Playlist
                {
                    Name = dto.Name,
                    UserId = dto.UserId,
                };

                dbContext.Playlists.Add(newPlaylist);
                await dbContext.SaveChangesAsync();

                return Results.Created($"/playlist/{newPlaylist.PlaylistId}", newPlaylist);
            });
            // Net.MusicApp.APIs/HomeAPIs.cs

            group.MapPut("/songs/{id}/listen", async (int id, MusicAppDBContext db) =>
            {
                var song = await db.Songs.FindAsync(id);
                if (song == null) return Results.NotFound("Bài hát không tồn tại");

                // Tăng lượt nghe lên 1
                song.listenCount++;

                // Lưu lại
                await db.SaveChangesAsync();

                // Trả về số lượt nghe mới nhất để FE cập nhật UI
                return Results.Ok(new { Id = id, NewListenCount = song.listenCount });
            });
            // Net.MusicApp.APIs/HomeAPIs.cs

            group.MapGet("/songs/trending", async (MusicAppDBContext db, int? limit) =>
            {
                int take = limit ?? 20; 

                var trendingSongs = await db.Songs
                    .AsNoTracking()
                    .OrderByDescending(s => s.listenCount) 
                    .Take(take)
                    .Select(s => new SongDto 
                    {
                        SongId = s.SongId,
                        Name = s.Name,
                        ImageUrl = s.ImageUrl,
                        AudioUrl = s.AudioUrl,
                        SingerName = s.Singer!=null?s.Singer.Name:"",
                        GenreName = s.Genre!=null?s.Genre.Name:"",
                        listenCount = s.listenCount, 
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(trendingSongs);
            });
            group.MapGet("/singers/trending", async (MusicAppDBContext db) =>
            {
                var trendingSingers = await db.Singers
                    .AsNoTracking()
                    // Quan trọng: Tính toán trực tiếp trong Database
                    .Select(s => new SingerTrendingDto
                    {
                        SingerId = s.SingerId,
                        Name = s.Name,
                        ImageUrl = s.ImageUrl,
                        // Cộng tổng ListenCount của tất cả bài hát thuộc ca sĩ này
                        TotalListens = s.Songs.Sum(song => song.listenCount),
                        SongCount = s.Songs.Count()
                    })
                    .OrderByDescending(s => s.TotalListens) // Sắp xếp giảm dần theo tổng lượt nghe
                    .Take(10) // Lấy top 10
                    .ToListAsync();

                return Results.Ok(trendingSingers);
            });
            // GET /api/home/song/{id}
            group.MapGet("/song/{id}", async (int id, MusicAppDBContext db) =>
            {
                var song = await db.Songs
                    .AsNoTracking() // Tối ưu hiệu suất đọc
                    .Where(s => s.SongId == id)
                    .Select(s => new SongDetailDto
                    {
                        SongId = s.SongId,
                        Name = s.Name,
                        Description = s.Description,
                        AudioUrl = s.AudioUrl,
                        ImageUrl = s.ImageUrl,
                        ListenCount = s.listenCount,
                        CreatedAt = s.CreatedAt,

                        // Map thông tin ca sĩ
                        SingerId = s.SingerId,
                        SingerName = s.Singer.Name,
                        SingerAvatar = s.Singer.ImageUrl,

                        // Map thông tin thể loại
                        GenreId = s.GenreId,
                        GenreName = s.Genre.Name
                    })
                    .FirstOrDefaultAsync();

                if (song == null) return Results.NotFound("Không tìm thấy bài hát.");

                return Results.Ok(song);
            });
            group.MapGet("/singer/{id}", async (int id, MusicAppDBContext db) =>
            {
                var singer = await db.Singers
                    .AsNoTracking()
                    .Where(s => s.SingerId == id)
                    .Select(s => new SingerDetailDto
                    {
                        SingerId = s.SingerId,
                        Name = s.Name,
                        Description = s.Description,
                        ImageUrl = s.ImageUrl,
                        // Đếm số lượng bài hát
                        SongCount = s.Songs.Count(),
                        // Tính tổng lượt nghe (nếu em đã làm tính năng đếm lượt nghe)
                        TotalListens = s.Songs.Sum(song => song.listenCount)
                    })
                    .FirstOrDefaultAsync();

                if (singer == null) return Results.NotFound("Không tìm thấy ca sĩ.");

                return Results.Ok(singer);
            });
            // GET /api/home/singer/{id}/songs
            group.MapGet("/singer/{id}/songs", async (int id, MusicAppDBContext db) =>
            {
                // Kiểm tra ca sĩ có tồn tại không (Tuỳ chọn, có thể bỏ qua để tối ưu)
                // var exists = await db.Singers.AnyAsync(s => s.SingerId == id);
                // if (!exists) return Results.NotFound("Ca sĩ không tồn tại.");

                var songs = await db.Songs
                    .AsNoTracking()
                    .Where(s => s.SingerId == id) // Lọc theo ID ca sĩ
                    .OrderByDescending(s => s.CreatedAt) // Bài mới nhất lên đầu
                    .Select(s => new SongDetailDto // Tái sử dụng DTO bài hát
                    {
                        SongId = s.SongId,
                        Name = s.Name,
                        AudioUrl = s.AudioUrl,
                        ImageUrl = s.ImageUrl,
                        ListenCount = s.listenCount,
                        CreatedAt = s.CreatedAt,

                        // Vẫn trả về tên ca sĩ/thể loại cho đầy đủ
                        SingerId = s.SingerId,
                        SingerName = s.Singer.Name,
                        GenreId = s.GenreId,
                        GenreName = s.Genre.Name
                    })
                    .ToListAsync();

                return Results.Ok(songs);
            });

        }
    }
}
