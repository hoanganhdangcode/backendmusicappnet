using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.MusicApp.Data;
using Net.MusicApp.DTOs;
using Net.MusicApp.Entities;
using Net.MusicApp.Services;


namespace Net.MusicApp.APIs
{
    public static class AdminAPIs
    {
        public static void MapGroupAdmin(this WebApplication app) {
            var group= app.MapGroup("/admin").WithTags("Admin APIs");
            group.MapGet("/status", () => "Admin API is running");
            group.MapPost("/singer/add", async ([FromBody] CreateSingerDto dto, MusicAppDBContext dbContext) =>
            {
                // 1. Chuyển đổi từ DTO sang Entity
                var newSinger = new Singer
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                };

                // 2. Thêm vào DBSet
                dbContext.Singers.Add(newSinger);

                // 3. Lưu thay đổi vào Database
                await dbContext.SaveChangesAsync();

                // 4. Trả về mã 201 Created và thông tin vừa tạo
                return Results.Created();
            });
            group.MapPost("/genre/add", async ([FromBody] CreateGenreDto dto, MusicAppDBContext dbContext) =>
            {
                var newGenre = new Genre
                {
                    Name = dto.Name,
                    // Map đúng tên thuộc tính trong Entity Genre
                    imageurl = dto.ImageUrl,
                };

                dbContext.Genres.Add(newGenre);
                await dbContext.SaveChangesAsync();

                return Results.Created();
            });
            group.MapPost("/song/add", async ([FromBody] CreateSongDto dto, MusicAppDBContext dbContext) =>
            {
                // [Optional] Kiểm tra khóa ngoại (Ca sĩ có tồn tại không?)
                var singerExists = await dbContext.Singers.FindAsync(dto.SingerId);
                if (singerExists == null)
                {
                    return Results.BadRequest("Ca sĩ không tồn tại.");
                }

                // [Optional] Kiểm tra khóa ngoại (Thể loại có tồn tại không?)
                var genreExists = await dbContext.Genres.FindAsync(dto.GenreId);
                if (genreExists == null)
                {
                    return Results.BadRequest("Thể loại không tồn tại.");
                }

                // Chuyển đổi DTO -> Entity
                var newSong = new Song
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    AudioUrl = dto.AudioUrl,
                    ImageUrl = dto.ImageUrl,
                    SingerId = dto.SingerId, // Gán khóa ngoại
                    GenreId = dto.GenreId,   // Gán khóa ngoại
                };

                dbContext.Songs.Add(newSong);
                await dbContext.SaveChangesAsync();

                return Results.Created();
            });
            // --- SỬA CA SĨ ---
            group.MapPut("/singer/update/{id}", async (int id, [FromBody] UpdateSingerDto dto, MusicAppDBContext dbContext) =>
            {
                var singer = await dbContext.Singers.FindAsync(id);
                if (singer == null) return Results.NotFound("Không tìm thấy ca sĩ.");

                // Cập nhật thông tin
                singer.Name = dto.Name;
                singer.Description = dto.Description;
                singer.ImageUrl = dto.ImageUrl;
                // Lưu ý: Không cập nhật CreatedAt

                await dbContext.SaveChangesAsync();
                return Results.Ok(singer); // Trả về đối tượng đã sửa
            });

            // --- SỬA THỂ LOẠI ---
            group.MapPut("/genre/update/{id}", async (int id, [FromBody] UpdateGenreDto dto, MusicAppDBContext dbContext) =>
            {
                var genre = await dbContext.Genres.FindAsync(id);
                if (genre == null) return Results.NotFound("Không tìm thấy thể loại.");

                genre.Name = dto.Name;
                genre.imageurl = dto.ImageUrl; // Map đúng thuộc tính imageurl (viết thường) trong entity

                await dbContext.SaveChangesAsync();
                return Results.Ok(genre);
            });

            // --- SỬA BÀI HÁT ---
            group.MapPut("/song/update/{id}", async (int id, [FromBody] UpdateSongDto dto, MusicAppDBContext dbContext) =>
            {
                var song = await dbContext.Songs.FindAsync(id);
                if (song == null) return Results.NotFound("Không tìm thấy bài hát.");
                var singerExists = await dbContext.Singers.FindAsync(dto.SingerId);
                if (singerExists == null)
                {
                    return Results.BadRequest("Ca sĩ không tồn tại.");
                }

                // [Optional] Kiểm tra khóa ngoại (Thể loại có tồn tại không?)
                var genreExists = await dbContext.Genres.FindAsync(dto.GenreId);
                if (genreExists == null)
                {
                    return Results.BadRequest("Thể loại không tồn tại.");
                }
                song.Name = dto.Name;
                song.Description = dto.Description;
                song.AudioUrl = dto.AudioUrl;
                song.ImageUrl = dto.ImageUrl;
                song.SingerId = dto.SingerId; // Cho phép đổi ca sĩ
                song.GenreId = dto.GenreId;   // Cho phép đổi thể loại

                await dbContext.SaveChangesAsync();
                return Results.Ok();
            });
            // --- XÓA CA SĨ ---
            group.MapDelete("/singer/delete/{id}", async (int id, MusicAppDBContext dbContext) =>
            {
                //  Tìm ca sĩ
                var singer = await dbContext.Singers.FindAsync(id);
                if (singer == null) return Results.NotFound("Không tìm thấy ca sĩ.");

                //  Có bài hát nào của ca sĩ này không?
                bool hasSongs = await dbContext.Songs.AnyAsync(s => s.SingerId == id);

                if (hasSongs)
                {
                    return Results.Conflict($"Ca sĩ '{singer.Name}' đang có bài hát trong hệ thống. Vui lòng xóa bài hát trước.");
                }

                dbContext.Singers.Remove(singer);
                await dbContext.SaveChangesAsync();

                return Results.Ok("Đã xóa ca sĩ thành công.");
            });

            // --- XÓA THỂ LOẠI ---
            group.MapDelete("/genre/delete/{id}", async (int id, MusicAppDBContext dbContext) =>
            {
                // 1. Tìm thể loại
                var genre = await dbContext.Genres.FindAsync(id);
                if (genre == null) return Results.NotFound("Không tìm thấy thể loại.");

                // 2. KIỂM TRA: Có bài hát nào thuộc thể loại này không?
                bool hasSongs = await dbContext.Songs.AnyAsync(s => s.GenreId == id);

                if (hasSongs)
                {
                    return Results.Conflict($"Thể loại '{genre.Name}' đang chứa bài hát. Không thể xóa ngay lập tức.");
                }

                // 3. Nếu an toàn -> Xóa
                dbContext.Genres.Remove(genre);
                await dbContext.SaveChangesAsync();

                return Results.Ok("Đã xóa thể loại thành công.");
            });

            group.MapDelete("/song/delete/{id}", async (int id, MusicAppDBContext dbContext) =>
            {
                var song = await dbContext.Songs.FindAsync(id);
                if (song == null) return Results.NotFound("Không tìm thấy bài hát.");

                dbContext.Songs.Remove(song);
                await dbContext.SaveChangesAsync();

                return Results.Ok("Đã xóa bài hát.");
            });

            group.MapGet("/songs", async (MusicAppDBContext db, string? keyword) =>
            {
                var query = db.Songs.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(s => s.Name.Contains(keyword));
                }

                var items = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new SongDto
                    {
                        SongId = s.SongId,
                        Name = s.Name,
                        Description = s.Description,
                        AudioUrl = s.AudioUrl,
                        ImageUrl = s.ImageUrl,
                        CreatedAt = s.CreatedAt,
                        SingerId = s.SingerId,
                        SingerName = s.Singer.Name,
                        GenreId = s.GenreId,
                        GenreName = s.Genre.Name
                    })
                    .ToListAsync();

                return Results.Ok(items);
            });


            group.MapGet("/singers", async (MusicAppDBContext db, string? keyword) =>
            {
                var query = db.Singers.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(s => s.Name.Contains(keyword));
                }

                var items = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new SingerDto
                    {
                        SingerId = s.SingerId,
                        Name = s.Name,
                        Description = s.Description,
                        ImageUrl = s.ImageUrl,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(items);
            });
            group.MapGet("/genres", async (MusicAppDBContext db, string? keyword) =>
            {
                var query = db.Genres.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(g => g.Name.Contains(keyword));
                }

                var items = await query
                    .OrderByDescending(g => g.CreatedAt)
                    .Select(g => new GenreDto
                    {
                        GenreId = g.GenreId,
                        Name = g.Name,
                        ImageUrl = g.imageurl, 
                        CreatedAt = g.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(items);
            });
            group.MapGet("/users", async (MusicAppDBContext db, string? keyword) =>
            {
                var query = db.Users.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(u =>
                        u.Name.Contains(keyword) ||
                        u.Email.Contains(keyword));
                }

                var items = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UserDto
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Email = u.Email,
                        AvatarUrl = u.AvatarUrl,
                        Role = u.Role == 0 ? "User" : "Admin",
                        CreatedAt = u.CreatedAt
                    })
                    .ToListAsync();

                foreach (var u in items)
                {
                    if (!string.IsNullOrEmpty(u.Name))
                        u.Name = CryptoHelper.DecryptAES256(u.Name);

                    if (!string.IsNullOrEmpty(u.AvatarUrl))
                        u.AvatarUrl = CryptoHelper.DecryptAES256(u.AvatarUrl);
                }

                return Results.Ok(items);
            });
            group.MapGet("/upload/signature", (CloudinaryService cloudinary, IConfiguration configuration) =>
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var parameters = new Dictionary<string, object>
    {
        { "timestamp", timestamp },
        { "folder", "musicapp" }
    };

                var signature = cloudinary.CreateSignature(parameters);

                var cloud = configuration.GetSection("Cloudinary");

                return Results.Ok(new
                {
                    cloudName = cloud["CloudName"],
                    apiKey = cloud["ApiKey"],
                    timestamp,
                    signature,
                    folder = "musicapp"
                });
            });






        }

    }
       
    }

