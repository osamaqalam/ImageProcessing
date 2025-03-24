using ImageProcessing.App.Models.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Windows.Media.Imaging;

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

    public BitmapImage ConvertToGrayscale(BitmapImage source)
    {
        // Convert BitmapImage to ImageSharp's Image<Rgba32>
        using var imageStream = new MemoryStream();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(imageStream);
        imageStream.Position = 0;

        using var image = Image.Load<Rgba32>(imageStream);
        image.Mutate(x => x.Grayscale()); // Apply grayscale

        // Convert back to BitmapImage
        var bitmapImage = new BitmapImage();
        using (var outputStream = new MemoryStream())
        {
            image.Save(outputStream, new PngEncoder());
            outputStream.Position = 0;

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = outputStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
        }
        return bitmapImage;
    }
}
