using System.Reflection;
using X.Sharp.Web.Middleware;
using X.Sharp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace X.Sharp.Web
{
    public class Globals
    {
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMvc(options => { options.OutputFormatters.Add(new XmlSerializerOutputFormatter()); })
                .AddJsonOptions(options=>
                {
                    //System.Text.Json.JsonSerializerDefaults.Web
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy= System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive= true;
                })
                .AddRazorRuntimeCompilation(); //Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
            // AddNewtonsoftJson //Microsoft.AspNetCore.Mvc.NewtonsoftJson
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddInjectServices<Program>();
            builder.Logging.AddDebug();
            builder.Logging.AddJsonConsole(options =>
            {
                options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions { Indented = true };
            });
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
            app.UseRequestLocalization(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN"),
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}