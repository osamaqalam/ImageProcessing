using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.App.Models.Imaging;

namespace ImageProcessing.App.Models.Flowchart
{
    public class LoadImageNode
    {
        public string? ImagePath { get; set; }
        public ImageData? LoadedImage { get; set; }
    }
}
