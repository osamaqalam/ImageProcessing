using ImageProcessing.App.Models.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Numerics;
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

    /// <summary>
    /// Converts a colored image into grayscale image
    /// We use the BT.709 standard for HDTV in the luminance calculation.
    /// https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public BitmapImage ConvertToGrayscale(BitmapImage source)
    {
        // Convert BitmapImage to ImageSharp's Image<Rgba32>
        using var imageStream = new MemoryStream(); // store the image in bytes
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(imageStream);
        imageStream.Position = 0;

        using var image = Image.Load<Rgba32>(imageStream);
        //image.Mutate(x => x.Grayscale()); // the conversion using the library

        // Convert to grayscale without using library
        image.Mutate(c => c.ProcessPixelRowsAsVector4(row =>
        {
            for (int x = 0; x < row.Length; x++)
            {
                Vector4 pixel = row[x];
                Vector4 normalizedPixel = pixel / 255.0f; 
                float r = normalizedPixel.X; // Red
                float g = normalizedPixel.Y; // Green
                float b = normalizedPixel.Z; // Blue
                float a = pixel.W;           // Alpha (opacity)

                float gray = 0.2126f*r + 0.7152f*g + 0.0722f*b;
                gray *= 255.0f; 
                row[x] = new Vector4(gray, gray, gray, a); 
            }
        }

        ));

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

    /// <summary>
    /// Resizes the image to a new size based on the scale factor
    /// </summary>
    /// <param name="source"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public BitmapImage Resize(BitmapImage source, double scale)
    {
        using var imageStream = new MemoryStream(); // store the image in bytes
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(imageStream);
        imageStream.Position = 0;

        using var image = Image.Load<Rgba32>(imageStream);
        image.Mutate(x => x.Resize(
            new ResizeOptions
            {
                Size = new Size((int)(source.PixelWidth * scale), (int)(source.PixelHeight * scale)),
                Mode = ResizeMode.Max
            }));

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
