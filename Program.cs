using ApiPeliculasIdentity.Data;
using ApiPeliculasIdentity.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//A�adimos logging
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection2"));
});

builder.Services.AddResponseCaching();

//Agregamos servicios de extensi�n
//Extensi�n services added
builder.Services.AddAppServices();
builder.Services.ConfigureCors();
builder.Services.ConfigureIdentity();

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("default", new CacheProfile() { Duration = 30 });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("cors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
