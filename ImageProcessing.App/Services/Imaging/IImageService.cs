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
    /// Binarize an image based on the given thresholding range and type
    /// </summary>
    /// <param name="source"> Source Image to binarize</param>
    /// <param name="thresholdingType">Thresholding type</param>
    /// <param name="rangeStart"> Start of the thresholding range </param>
    /// <param name="rangeEnd"> End of the thresholding range</param>
    /// <returns></returns>
    BitmapImage ConvertToBinary(BitmapImage source, String thresholdingType, int rangeStart, int rangeEnd);

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
    /// <param name="source"> Image to be resized </param>
    /// <param name="scale"> Scale of the resizing </param>
    /// <param name="interpolationMethod"> Choose between NearestNeighbor and Bilinear </param>
    /// <returns> resized image as BitmapImage</returns>
    public BitmapImage Resize(BitmapImage source, double scale, String interpolationMethod);

}