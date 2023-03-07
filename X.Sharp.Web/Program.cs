using System.Reflection;
using X.Sharp.Web.Middleware;
using X.Sharp.Web.Extensions;
using Microsoft.AspNetCore.Identity;

namespace X.Sharp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddInjectServices<Program>();
            builder.Logging.AddDebug();
            var app = builder.Build();

            app.UseMiddleware<CustomMiddleware>();
            //app.Use((context, next) =>
            //{
            //    var q = context.Request.Query;
            //    context.Response.WriteAsJsonAsync(q);
            //    return next();
            //});

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}