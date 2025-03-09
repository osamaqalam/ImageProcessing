using ImageProcessing.App.Models.Imaging;
using ImageProcessing.App.Services.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace ImageProcessing.App.Services.Imaging;

public class ImageService : IImageService
{
    public ImageData LoadImage(string path)
    {
        using var image = Image.Load(path);
        using var stream = new MemoryStream();

        // Save to PNG format (WPF can read this)
        image.Save(stream, new PngEncoder());

        return new ImageData(
            pixelData: stream.ToArray(),
            width: image.Width,
            height: image.Height,
            format: "Png");
    }
}
