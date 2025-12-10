namespace Net.MusicApp.APIs
{
    public static class AuthAPIs
    {

        public static void MapAuthAPI(this WebApplication app)
        {
            var group = app.MapGroup("/auth").WithTags("Auth APIs");
            group.MapGet("/get", () => "GET thanh cong");


        }
    }
}
