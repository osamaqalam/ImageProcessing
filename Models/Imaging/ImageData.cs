using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;           
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessing.App.Models.Imaging;
public class ImageData
{
    public byte[] PixelData { get; }       // Raw pixel bytes
    public int Width { get; }              // Image width
    public int Height { get; }             // Image height
    public string Format { get; }          // Pixel format (e.g., "Bgra32")

    public ImageData(byte[] pixelData, int width, int height, string format)
    {
        PixelData = pixelData;
        Width = width;
        Height = height;
        Format = format;
    }
}
