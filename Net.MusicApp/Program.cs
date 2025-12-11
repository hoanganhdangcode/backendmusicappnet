using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Net.MusicApp.APIs;
using System.Security.Cryptography;
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
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Nhập JWT Token theo định dạng: **Bearer {token}**"
        });

        // 2. Định nghĩa Security Requirement (Áp dụng token cho các endpoint)
    //    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        new string[] {}
    //    }
    //});
    });
builder.Services.AddRateLimiter(_ => {
        _.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
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
var publicKeyPem = File.ReadAllText("Keys/rsa_public_key.pem");
var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyPem);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // Nếu bạn tự ký RSA thì dùng cấu hình dưới
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(
                rsa
                )
            
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(
    c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicApp v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    }
    );
app.UseAuthentication();
app.UseAuthorization();


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapGroupTest();
app.MapGroupAuth();
app.MapGroupAdmin();
app.MapGroupUser();


app.Run();
