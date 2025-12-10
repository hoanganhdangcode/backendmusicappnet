using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Net.MusicApp.APIs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    option => { 
    option.SwaggerDoc("v1", new() { Title = "MusicApp", Version = "v1", Description= "Backend NetMusicApp api swagger document" });
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
app.MapTestAPI();
app.MapAuthAPI();
app.MapAdminAPI();
app.MapUserAPI();
app.Run();
