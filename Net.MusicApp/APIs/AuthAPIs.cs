using Net.MusicApp.DTOs.AuthDTOs;
using Net.MusicApp.Services.AuthService;
using Net.MusicApp.Services.Common;
using System.Security.Claims;

namespace Net.MusicApp.APIs
{
    public static class AuthAPIs
    {

        public static void MapGroupAuth(this WebApplication app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth APIs");
            //group.MapGet("/get", () => "GET thanh cong");
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, "hoanganh"),
    new Claim("role", "User")
};
            var token = JWTHelper.GenerateToken(claims);
            group.MapPost("/login", ( UserLoginDto dto) => { 
            return Results.Ok(
                new LoginResponseDto { 
                AccessToken = token,
                ExpiresInSeconds = 60,

                }
                );


            } );


        }
    }
}
