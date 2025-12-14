using Microsoft.AspNetCore.Mvc;
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



        }
    }
}
