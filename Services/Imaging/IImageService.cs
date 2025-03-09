using ImageProcessing.App.Models.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.App.Services.Imaging;

public interface IImageService
{
    /// <summary>
    /// Loads an image from a file path and converts it to our custom ImageData format
    /// </summary>
    ImageData LoadImage(string path);
}