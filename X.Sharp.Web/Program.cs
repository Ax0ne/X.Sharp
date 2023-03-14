// Copyright (c) Ax0ne.  All Rights Reserved

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Globalization;
using X.Sharp.Web.Extensions;
using X.Sharp.Web.Middleware;

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
                .AddJsonOptions(options =>
                {
                    //System.Text.Json.JsonSerializerDefaults.Web
                    options.JsonSerializerOptions.Encoder =
                        System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.NumberHandling =
                        System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
            //.AddRazorRuntimeCompilation(); //Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
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

            //ImageUtils.Watermark(new ImageWatermarkOptions
            //{
            //    ImagePath = Path.Combine(app.Environment.ContentRootPath, "ff.jpg"),
            //    Position= WatermarkPosition.RightBottom,
            //    WatermarkImagePath = Path.Combine(app.Environment.ContentRootPath, "1.png"),
            //}) ;
            //ImageUtils.Watermark(new TextWatermarkOptions
            //{
            //    Text = "≤‚ ‘÷–Œƒ◊÷ÃÂ",
            //    HexColor = "#BBBBBB",
            //    ImagePath = Path.Combine(app.Environment.ContentRootPath, "ff.jpg"),
            //    FontSize = 16,
            //    FontName= "Microsoft YaHei",
            //    Position = WatermarkPosition.RightBottom
            //});
            //ImageUtils.Thumb(Path.Combine(app.Environment.ContentRootPath, "11.png"));
            app.Run();
        }
    }
}