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

    /// <summary>
    /// Binarize an image based on the given thresholding range and type
    /// </summary>
    /// <param name="source"> Source Image to binarize</param>
    /// <param name="thresholdingType">Thresholding type</param>
    /// <param name="rangeStart"> Start of the thresholding range </param>
    /// <param name="rangeEnd"> End of the thresholding range</param>
    /// <returns></returns>
    public BitmapImage ConvertToBinary(BitmapImage source, String thresholdingType, int rangeStart, int rangeEnd)
    {
        // Convert BitmapImage to ImageSharp's Image<Rgba32>
        using var imageStream = new MemoryStream(); // store the image in bytes
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(imageStream);
        imageStream.Position = 0;

        using var image = Image.Load<Rgba32>(imageStream);

        if (thresholdingType == "InRange")
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    // Assuming the image is grayscale so we can check any channel (R, G, or B)
                    image[i, j] = (image[i, j].R >= rangeStart && image[i, j].R <= rangeEnd) ? new Rgba32(255, 255, 255, 255) : new Rgba32(0, 0, 0, 255);
                }
            }
        }

        else if (thresholdingType == "OutRange")
        {
             for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    // Assuming the image is grayscale so we can check any channel (R, G, or B)
                    image[i, j] = (image[i, j].R >= rangeStart && image[i, j].R <= rangeEnd) ? new Rgba32(0, 0, 0, 255) : new Rgba32(255, 255, 255, 255);
                }
            }
        }
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
    /// <param name="source"> Image to be resized </param>
    /// <param name="scale"> Scale of the resizing </param>
    /// <param name="interpolationMethod"> Choose between NearestNeighbor and Bilinear </param>
    /// <returns> resized image as BitmapImage</returns>
    public BitmapImage Resize(BitmapImage source, double scale, String interpolationMethod = "NearestNeighbor")
    {
        using var imageStream = new MemoryStream(); // store the image in bytes
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(imageStream);
        imageStream.Position = 0;

        using var sourceImage = Image.Load<Rgba32>(imageStream);
        Image<Rgba32> resizedImage = new Image<Rgba32>((int)(source.Width*scale), (int)(source.Height*scale));

        if(interpolationMethod == "NearestNeighbor")
        for (int i = 0; i < resizedImage.Width; i++)
        {
            for(int j = 0; j < resizedImage.Height; j++)
            {
                int srcX = (int)(i * (sourceImage.Width / (float)resizedImage.Width));
                int srcY = (int)(j * (sourceImage.Height / (float)resizedImage.Height));

                resizedImage[i, j] = sourceImage[srcX, srcY];
            }
        }
        
        else if(interpolationMethod == "Bilinear")
        {
            for (int i = 0; i < resizedImage.Width; i++)
            {
                for (int j = 0; j < resizedImage.Height; j++)
                {
                    float srcX = (i * (sourceImage.Width / (float)resizedImage.Width));
                    float srcY = (j * (sourceImage.Height / (float)resizedImage.Height));

                    // Check if the coordinates it maps to already exists in source image since it is a whole number
                    if (srcX % 1 == 0 && srcY % 1 == 0)
                    {
                        resizedImage[i, j] = sourceImage[(int)srcX, (int)srcY];
                        continue;
                    }

                    int srcX1 = (int) Math.Floor(srcX);
                    int srcX2 = Math.Min((int) Math.Ceiling(srcX), sourceImage.Width - 1);
                    int srcY1 = (int) Math.Floor(srcY);
                    int srcY2 = Math.Min((int) Math.Ceiling(srcY), sourceImage.Height - 1);

                    var p11 = sourceImage[srcX1, srcY1];
                    var p21 = sourceImage[srcX2, srcY1];
                    var p12 = sourceImage[srcX1, srcY2];
                    var p22 = sourceImage[srcX2, srcY2];

                    // Calculate weights
                    float wx2 = srcX - srcX1;
                    float wx1 = 1.0f - wx2;
                    float wy2 = srcY - srcY1;
                    float wy1 = 1.0f - wy2;

                    // Interpolate each channel separately
                    byte r = (byte)(wx1 * wy1 * p11.R + wx2 * wy1 * p21.R + wx1 * wy2 * p12.R + wx2 * wy2 * p22.R);
                    byte g = (byte)(wx1 * wy1 * p11.G + wx2 * wy1 * p21.G + wx1 * wy2 * p12.G + wx2 * wy2 * p22.G);
                    byte b = (byte)(wx1 * wy1 * p11.B + wx2 * wy1 * p21.B + wx1 * wy2 * p12.B + wx2 * wy2 * p22.B);
                    byte a = (byte)(wx1 * wy1 * p11.A + wx2 * wy1 * p21.A + wx1 * wy2 * p12.A + wx2 * wy2 * p22.A);

                    resizedImage[i, j] = new Rgba32(r, g, b, a);

                }
            }
        }

        // library solutions
        //using var image = Image.Load<Rgba32>(imageStream);
        //image.Mutate(x => x.Resize(
        //    new ResizeOptions
        //    {
        //        Size = new Size((int)(source.PixelWidth * scale), (int)(source.PixelHeight * scale)),
        //        Mode = ResizeMode.Max
        //    }));

        // Convert back to BitmapImage
        var bitmapImage = new BitmapImage();
        using (var outputStream = new MemoryStream())
        {
            resizedImage.Save(outputStream, new PngEncoder());
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
