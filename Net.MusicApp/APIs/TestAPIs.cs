using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Net.MusicApp.APIs
{
    public static class TestAPIs
    {
        public static void MapGroupTest(this WebApplication app) {
            var group = app.MapGroup("/test").WithTags("Test APIs");
            group.MapGet("testjwt",()=>"OK").RequireAuthorization();

            group.MapGet("/get", ([FromQuery] string q) =>
            $"GET thanh cong: {q}"
);

            group.MapPost("/post", ([FromBody] string bodySample) =>
                $"POST thanh cong: {bodySample}"
            );

            group.MapPatch("/patch", ([FromBody] string bodySample) =>
                $"PATCH thanh cong: {bodySample}"
            );

            group.MapPut("/put", ([FromBody] string bodySample) =>
                $"PUT thanh cong: {bodySample}"
            );

            group.MapDelete("/delete", ([FromBody] string bodySample) =>
                $"DELETE thanh cong: {bodySample}"
            );
        }
    }
}
