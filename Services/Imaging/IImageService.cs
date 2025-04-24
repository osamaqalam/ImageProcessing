using ImageProcessing.App.Models.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageProcessing.App.Services.Imaging;

public interface IImageService
{
    /// <summary>
    /// Loads an image from a file path and converts it to our custom ImageData format
    /// </summary>
    ImageData LoadImage(string path);

    /// <summary>
    /// Converts a colored image into grayscale image
    /// </summary>
    /// <param name="source"> the color image</param>
    /// <returns></returns>
    BitmapImage ConvertToGrayscale(BitmapImage source);

    /// <summary>
    /// Resizes the image to a new size based on the scale factor
    /// </summary>
    /// <param name="source"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    BitmapImage Resize(BitmapImage source, double scale);

}