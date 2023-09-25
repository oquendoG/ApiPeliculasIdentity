using ApiPeliculasIdentity.Data;
using ApiPeliculasIdentity.Feats.Categories.Repository;
using ApiPeliculasIdentity.Feats.Movies.Repository;
using ApiPeliculasIdentity.Feats.Users.Repository;
using ApiPeliculasIdentity.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiPeliculasIdentity.Extensions;

public static class AppServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IMovieRepository, MovieRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("cors", build =>
        {
            build.WithOrigins().AllowAnyHeader().AllowAnyMethod();
        }));
    }

    public static void ConfigureAuthentication(this IServiceCollection services, string key)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    }
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
    }
}
