namespace Net.MusicApp.APIs
{
    public static class AdminAPIs
    {
        public static void MapGroupAdmin(this WebApplication app) {
            var group= app.MapGroup("/admin").WithTags("Admin APIs");
            group.MapGet("/status", () => "Admin API is running");
            group.MapPost("/login", () => "Đăng nhập thành công");



        }
       
    }
}
