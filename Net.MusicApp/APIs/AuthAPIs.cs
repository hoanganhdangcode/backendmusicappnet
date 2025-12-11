using Net.MusicApp.DTOs.AuthDTOs;
using Net.MusicApp.Services.AuthService;
namespace Net.MusicApp.APIs
{
    public static class AuthAPIs
    {

        public static void MapGroupAuth(this WebApplication app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth APIs");
            //group.MapGet("/get", () => "GET thanh cong");
            group.MapPost("/login", ( UserLoginDto dto) => { 
            return Results.Ok(
                new LoginResponseDto { 
                AccessToken = "sample",
                ExpiresInSeconds = 60,

                }
                );


            } );


        }
    }
}
