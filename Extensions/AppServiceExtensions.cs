using ApiPeliculasIdentity.Data;
using ApiPeliculasIdentity.Feats.Categories.Repository;
using ApiPeliculasIdentity.Feats.Movies.Repository;
using ApiPeliculasIdentity.Feats.Users.Repository;
using ApiPeliculasIdentity.Shared;
using Microsoft.AspNetCore.Identity;

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

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<IdentityOptions>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.Password.RequireUppercase = true;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            opt.Lockout.MaxFailedAccessAttempts = 6;
        });

        services.ConfigureApplicationCookie(opt =>
        {
            opt.ExpireTimeSpan = TimeSpan.FromMinutes(4320);
        });
    }
}
