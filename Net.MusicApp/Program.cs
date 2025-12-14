using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Net.MusicApp.APIs;
using Net.MusicApp.Services;
using System.Security.Cryptography;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;




var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
var version = new Version(9, 5, 0);
var serverVersion = new MySqlServerVersion(version);
builder.Services.AddDbContext<Net.MusicApp.Data.MusicAppDBContext>(
    options => options.UseMySql(connectionString,serverVersion)
    );

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
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer" }
                            }, new List<string>() }
                    });
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
var rsa = JWTHelper.LoadRsaPublicKeyFromPem("Keys/rsa_public_key.pem");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
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

app.UseHttpsRedirection();
app.UseRateLimiter();


app.MapGroupTest();
app.MapGroupAuth();
app.MapGroupAdmin();
app.MapGroupUser();


app.Run();
