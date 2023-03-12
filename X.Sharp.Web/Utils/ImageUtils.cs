using Microsoft.Extensions.Options;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace X.Sharp.Web.Utils
{
    public static class ImageUtils
    {
        public static byte[] GenerateVerifyCode(string text,int width=120,int height=50)
        {
            return null;
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="imagePath">原始图片路径</param>
        /// <param name="width">宽 默认80</param>
        /// <param name="height">高 默认80</param>
        /// <returns>返回缩略图路径</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Thumb(string imagePath, int width = 80, int height = 80)
        {
            if (!System.IO.File.Exists(imagePath))
                throw new ArgumentException("图片不存在，请检查ImagePath参数");
            using var image = Image.Load(imagePath);
            image.Mutate(i =>
            {
                i.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Compand = true
                });
            });
            var extension = Path.GetExtension(imagePath);
            var savePath = Path.Combine(Path.GetDirectoryName(imagePath)!,
                DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension);
            image.Save(savePath);
            return savePath;
        }

        /// <summary>
        /// 图片水印
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Watermark(ImageWatermarkOptions options)
        {
            if (!System.IO.File.Exists(options.ImagePath))
                throw new ArgumentException("图片不存在，请检查ImagePath参数");
            if (!System.IO.File.Exists(options.WatermarkImagePath))
                throw new ArgumentException("水印图片不存在，请检查WatermarkImagePath参数");
            using var image = Image.Load(options.ImagePath);
            using var wmImage = Image.Load(options.WatermarkImagePath);

            var wmWidth = wmImage.Width;
            var wmHeight = wmImage.Height;
            var padding = 10;
            Point point = new Point(image.Width - padding - wmWidth, image.Height - padding - wmHeight);
            switch (options.Position)
            {
                case WatermarkPosition.Center:
                    point.X = image.Width / 2 - wmWidth;
                    point.Y = image.Height / 2 - wmHeight;
                    break;
                case WatermarkPosition.LeftTop:
                    point.X = padding;
                    point.Y = padding;
                    break;
                case WatermarkPosition.RightTop:
                    point.X = image.Width - wmWidth - padding;
                    point.Y = padding;
                    break;
                case WatermarkPosition.LeftBottom:
                    point.X = padding;
                    point.Y = image.Height - wmHeight - padding;
                    break;
                case WatermarkPosition.RightBottom:
                    point.X = image.Width - wmWidth - padding;
                    point.Y = image.Height - wmHeight - padding;
                    break;
            }

            var opactity = 0.5F;
            image.Mutate(i => { i.DrawImage(wmImage, point, opactity); });
            var extension = Path.GetExtension(options.ImagePath);
            var savePath = Path.Combine(Path.GetDirectoryName(options.ImagePath)!,
                DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension);
            image.Save(savePath);
            return savePath;
        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Watermark(TextWatermarkOptions options)
        {
            if (!System.IO.File.Exists(options.ImagePath))
                throw new ArgumentException("图片不存在，请检查ImagePath参数");
            if (string.IsNullOrWhiteSpace(options.Text))
                throw new ArgumentException("文字不能为空字符，请检查Text参数");
            using var image = Image.Load(options.ImagePath);
            var padding = 10;
            // Microsoft YaHei
            var fontName = string.IsNullOrWhiteSpace(options.FontName) ? "Arial" : options.FontName;
            var font = SystemFonts.CreateFont(fontName, options.FontSize);
            var size = TextMeasurer.Measure(options.Text, new TextOptions(font));
            var wmWidth = size.Width;
            var wmHeight = size.Height;
            PointF point = new PointF(image.Width - padding - wmWidth, image.Height - padding - wmHeight);
            switch (options.Position)
            {
                case WatermarkPosition.Center:
                    point.X = image.Width / 2 - wmWidth;
                    point.Y = image.Height / 2 - wmHeight;
                    break;
                case WatermarkPosition.LeftTop:
                    point.X = padding;
                    point.Y = padding;
                    break;
                case WatermarkPosition.RightTop:
                    point.X = image.Width - wmWidth - padding;
                    point.Y = padding;
                    break;
                case WatermarkPosition.LeftBottom:
                    point.X = padding;
                    point.Y = image.Height - wmHeight - padding;
                    break;
                case WatermarkPosition.RightBottom:
                    point.X = image.Width - wmWidth - padding;
                    point.Y = image.Height - wmHeight - padding;
                    break;
            }

            var textOptions = new TextOptions(font)
            {
                Origin = point,
                Dpi = 144,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (!Color.TryParseHex(options.HexColor, out var color))
                color = Color.White;
            image.Mutate(i => { i.DrawText(textOptions, options.Text, color); });
            var extension = Path.GetExtension(options.ImagePath);
            var savePath = Path.Combine(Path.GetDirectoryName(options.ImagePath)!,
                DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension);
            image.Save(savePath);
            return savePath;
        }
    }


    #region Options Object

    public abstract class WatermarkOptions
    {
        /// <summary>
        /// 水印位置
        /// </summary>
        public WatermarkPosition Position { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; }
    }

    public enum WatermarkPosition
    {
        LeftTop = 1,
        RightTop = 2,
        LeftBottom = 3,
        RightBottom = 4,
        Center = 5
    }

    public class TextWatermarkOptions : WatermarkOptions
    {
        /// <summary>
        /// 水印文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 文字颜色 默认 #FFFFFF
        /// </summary>
        public string HexColor { get; set; }

        /// <summary>
        /// 字体名称 默认 Arial
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// 字体大小 默认 10
        /// </summary>

        public int FontSize { get; set; } = 10;
    }


    public class ImageWatermarkOptions : WatermarkOptions
    {
        /// <summary>
        /// 水印图片路径
        /// </summary>
        public string WatermarkImagePath { get; set; }
    }

    #endregion
}