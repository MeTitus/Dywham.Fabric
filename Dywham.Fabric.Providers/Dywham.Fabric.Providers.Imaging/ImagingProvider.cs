using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Dywham.Fabric.Providers.Imaging
{
    public class ImagingProvider : IImagingProvider
    {
        public Bitmap ResizeImage(string file, int reductionPercentage)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            return ResizeImage(Image.FromFile(file), reductionPercentage);
#pragma warning restore CA1416 // Validate platform compatibility
        }

        public Bitmap ResizeImage(Image image, int reductionPercentage)
        {
            Size ResizeKeepAspect(Size size, int maxWidth, int maxHeight)
            {
                maxWidth = Math.Min(maxWidth, size.Width);
                maxHeight = Math.Min(maxHeight, size.Height);

                var rnd = Math.Min(maxWidth / (decimal)size.Width, maxHeight / (decimal)size.Height);

                return new Size((int)Math.Round(size.Width * rnd), (int)Math.Round(size.Height * rnd));
            }

#pragma warning disable CA1416 // Validate platform compatibility

            var newSize = ResizeKeepAspect(image.Size, image.Width * reductionPercentage / 100, image.Height * reductionPercentage / 100);
            var destRect = new Rectangle(0, 0, newSize.Width, newSize.Height);
            var destImage = new Bitmap(newSize.Width, newSize.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
#pragma warning restore CA1416 // Validate platform compatibility
        public bool IsImageFile(string filePath)
        {
            var validFileHeaders = new List<byte[]>
            {
                new byte[] {0x42, 0x4D},
                new byte[] {0x47, 0x49, 0x46, 0x38, 0x37, 0x61},
                new byte[] {0x47, 0x49, 0x46, 0x38, 0x39, 0x61},
                new byte[] {0x89, 0x50, 0x4e, 0x47, 0x0D, 0x0A, 0x1A, 0x0A},
                new byte[] {0x49, 0x49, 0x2A, 0x00},
                new byte[] {0x4D, 0x4D, 0x00, 0x2A},
                new byte[] {0xFF, 0xD8, 0xFF}
            };
            var buffer = new byte[8];

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length > buffer.Length)
                    {
                        fs.Read(buffer, 0, buffer.Length);
                    }

                    fs.Close();
                }

                if (validFileHeaders.Any(x => ByteArrayStartsWith(buffer, x)))
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    using (Image.FromFile(filePath))
#pragma warning restore CA1416 // Validate platform compatibility
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        private static bool ByteArrayStartsWith(IReadOnlyList<byte> a, IReadOnlyCollection<byte> b)
        {
            if (a.Count < b.Count)
            {
                return false;
            }

            return !b.Where((t, i) => a[i] != t).Any();
        }
    }
}