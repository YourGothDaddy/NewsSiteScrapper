namespace NewsWebsiteSiteScraper
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NewsWebSiteScraper.Services.News;
    using NewsWebSiteScraper.Data;
    using NewsWebSiteScraper.Services.Home;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<NewsWebSiteScraperDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContextFactory<NewsWebSiteScraperDbContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Singleton);


            builder.Services.AddHostedService<BackgroundScraperService>();
            builder.Services.AddTransient<INewsService, NewsService>();
            builder.Services.AddTransient<IHomeService, HomeService>();

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options => options.IncludeScopes = true);
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.None);
            builder.Logging.SetMinimumLevel(LogLevel.Error);


            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
            app.Run();
        }

    }
}