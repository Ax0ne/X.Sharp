// Copyright (c) Ax0ne.  All Rights Reserved

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace X.Sharp.Web.Utils
{
    public static class ImageUtils
    {
        /// <summary>
        /// 生成简单的验证码
        /// </summary>
        /// <param name="text">验证文本，中英文。字体不支持汉字</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <returns></returns>
        public static byte[] GenerateVerifyCode(string text, int width = 120, int height = 50)
        {
            // 二维码生成用 QRCoder-ImageSharp
            var random = Random.Shared;
            using Image<Rgba32> img = new Image<Rgba32>(width, height);
            //var lightColor = Color.ParseHex(lightColorHex);
            var fontName = "Arial";
            var font = SystemFonts.CreateFont(fontName, 14);
            //var size = TextMeasurer.Measure(text, new TextOptions(font));
            var twidth = (int)(width * 0.8 / text.Length);
            var theight = (int)(height * 0.8);
            img.Mutate(ctx =>
            {
                //IPen pen = Pens.DashDot(Color.ParseHex(deepColorHex), 2);
                ctx.Fill(RandomColor());
                ctx.Glow(RandomColor());
                for (var i = 0; i < 8; i++)
                {
                    var drawLinePoints = new PointF[2];
                    for (var j = 0; j < 2; j++)
                    {
                        drawLinePoints[j].X = random.Next(0, width);
                        drawLinePoints[j].Y = random.Next(0, height);
                    }

                    ctx.DrawLines(RandomColor(), 2, drawLinePoints);
                }

                var textOptions = new TextOptions(font)
                {
                    Dpi = 144,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                for (var i = 0; i < text.Length; i++)
                {
                    var p = new Point(i * twidth, 2);
                    using var timg = new Image<Rgba32>(twidth, theight);
                    timg.Mutate(m =>
                        m.DrawText(textOptions, text[i].ToString(), RandomColor()).Rotate(random.Next(-45, 45)));
                    ctx.DrawImage(timg, p, 1);
                }

                ctx.GaussianBlur(0.4f);
            });
            using var ms = new MemoryStream();
            img.Save(ms, PngFormat.Instance);
            return ms.ToArray();

            Color RandomColor()
            {
                var random = Random.Shared;
                var r = random.Next(0, 256);
                var g = random.Next(0, 256);
                var b = random.Next(0, 256);
                return Color.FromRgb((byte)r, (byte)g, (byte)b);
            }
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