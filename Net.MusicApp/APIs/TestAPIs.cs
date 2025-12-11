using System.Net.WebSockets;

namespace Net.MusicApp.APIs
{
    public static class TestAPIs
    {
        public static void MapGroupTest(this WebApplication app) {
            var group = app.MapGroup("/test").WithTags("Test APIs");
            group.MapGet("/get", () => "GET thanh cong");
            group.MapGet("/getlimit", () => "GET co limit thanh cong");
            group.MapPost("/post", () => "POST thanh cong");
            group.MapPost("/postlimit", () => "POST co limit thanh cong");
            group.MapPatch("/patch", () => "PATCH thanh cong");
            group.MapPatch("/patchlimit", () => "PATCH co limit thanh cong");
            group.MapPut("/put", () => "PUT thanh cong");
            group.MapPut("/putlimit", () => "PUT co limit thanh cong");
            group.MapDelete("/delete", () => "POP thanh cong");
            group.MapDelete("/deletelimit", () => "POP co limit thanh cong");


        }
    }
}
