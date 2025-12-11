using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Net.MusicApp.APIs;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
});

ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    option => { 
    option.SwaggerDoc("v1", new() { Title = "MusicApp", Version = "v1", Description= "Backend NetMusicApp api swagger document" });
    });
builder.Services.AddRateLimiter(_ => {
        _.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    //_.AddPolicy("limit5per10", context =>
    //{
    //    var ipAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString()??"unknownIP";
    //    logger.LogInformation(ipAddress);
        
    //    return RateLimitPartition.GetFixedWindowLimiter(
    //        partitionKey: ipAddress, 
    //        factory: partition => new FixedWindowRateLimiterOptions
    //        {  
    //            PermitLimit = 5,
    //            Window = TimeSpan.FromSeconds(10),
    //            QueueLimit = 0
    //        });
    //});
    _.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
      
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress, 
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,                     
                Window = TimeSpan.FromSeconds(10),     
                QueueLimit = 0
            });
    });
});


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(
    c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicApp v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    }
    );


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapGroupTest();
app.MapGroupAuth();
app.MapGroupAdmin();
app.MapGroupUser();


app.Run();
