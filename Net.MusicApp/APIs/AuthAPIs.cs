using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.MusicApp.Data;
using Net.MusicApp.DTOs;
using Net.MusicApp.Entities;
using Net.MusicApp.Services;
using System.Security.Claims;

namespace Net.MusicApp.APIs
{
    public static class AuthAPIs
    {

        public static void MapGroupAuth(this WebApplication app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth APIs");
            #region Đăng nhập
            group.MapPost("/login", async ([FromBody]LoginDto dto, MusicAppDBContext db) =>
            {
                //var emailencrypt = CryptoHelper.EncryptAES256(dto.Email);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null) return Results.Unauthorized();

                var hash = CryptoHelper.HashSHA256(dto.Password);
                if (hash != user.Password) return Results.Unauthorized();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                };
                var token = JWTHelper.GenerateToken(claims, TimeSpan.FromSeconds(20));
                var info = new UserInfoResponseDto
                {
                    UserId = user.UserId,
                    Name = user.Name != null ? CryptoHelper.DecryptAES256(user.Name) : "",
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl!=null ? CryptoHelper.DecryptAES256(user.AvatarUrl) : "",
                    Role = (int)user.Role
                };

                return Results.Ok(new LoginResponseDto
                {
                    AccessToken = token,
                    User = info
                });

            }).WithName("Login").WithDescription("API for user login");
            #endregion
            #region Đăng kí
            group.MapPost("/register",async ([FromBody] RegisterDto dto, MusicAppDBContext db) => {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user != null) { return Results.Conflict("Email đã tồn tại"); }
                var useradd = new User
                {
                  Name = dto.Name != null ? CryptoHelper.EncryptAES256(dto.Name):"",
                    Email = dto.Email,
                    Password = CryptoHelper.HashSHA256(dto.Password)
                };
                db.Users.Add(useradd);
                await db.SaveChangesAsync(); // Hoặc SaveChangesAsync


                return Results.Created();


            });

            #endregion



        }
    }
}
