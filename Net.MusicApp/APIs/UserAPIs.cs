namespace Net.MusicApp.APIs
{
    public static class UserAPIs
    {
        public static void MapUserAPI(this WebApplication app)
        {
            var group = app.MapGroup("/user").WithTags("User APIs");
            group.MapGet("/get", () => "GET thanh cong");
         

        }
    }
}
