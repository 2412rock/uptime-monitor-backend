using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OverflowBackend.Middleware;
using OverflowBackend.Services;
using OverflowBackend.Services.Implementantion;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using Uptime_Monitor_Backend.Models;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseKestrel(options =>
//{
//    options.Listen(IPAddress.Any, 4200, listenOptions =>
//    {
//    });
//});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(4600, listenOptions =>
    {
        //listenOptions.UseHttps("/app/backendcertificate.pfx");
    });
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalHost",
        builder =>
        {
            builder.WithOrigins("http://localhost")
                   .AllowAnyHeader()
                   .AllowAnyMethod().AllowCredentials();
        });
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });

    options.AddPolicy("AllowOVerflowOrigin",
        builder =>
        {
            builder.WithOrigins("https://overflowapp.xyz")
                   .AllowAnyHeader()
                   .AllowAnyMethod().AllowCredentials();
        });

});

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<PasswordHashService>();
builder.Services.AddTransient<MailService>();

builder.Services.AddSignalR();


var saPassword = Environment.GetEnvironmentVariable("SA_PASSWORD");
var env = builder.Environment.EnvironmentName;
string hostIp = Environment.GetEnvironmentVariable("DB_IP");

Console.WriteLine($"Connecting to DB IP {hostIp}");
builder.Services.AddDbContext<MonitorDBContext>(options =>
    options.UseSqlServer($"Server={hostIp},1438;Database=UptimeMonitorDB;User Id=sa;Password={saPassword};TrustServerCertificate=True"));
builder.Logging.ClearProviders();
var app = builder.Build();



app.UseCors("AllowAnyOrigin");
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<SessionValidationMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRouting();
app.MapControllers();
app.UseWebSockets();


app.Run();
