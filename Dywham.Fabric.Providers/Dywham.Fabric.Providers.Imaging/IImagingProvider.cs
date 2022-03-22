using System.Drawing;
using Dywham.Fabric.Providers;

namespace Dywham.Fabric.Providers.Imaging
{
    public  interface IImagingProvider : IProvider
    {
        bool IsImageFile(string filePath);

        Bitmap ResizeImage(string file, int reductionPercentage);

        Bitmap ResizeImage(Image image, int reductionPercentage);
    }
}
