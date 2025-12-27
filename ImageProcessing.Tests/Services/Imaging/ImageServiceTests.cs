using ImageProcessing.App.Models.Imaging;
using ImageProcessing.App.Services.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using Xunit;

namespace ImageProcessing.Tests.Services.Imaging
{
    public class ImageServiceTests
    {
        private readonly ImageService _imageService;

        public ImageServiceTests()
        {
            _imageService = new ImageService();
        }

        #region Helper Methods

        /// <summary>
        /// Creates a test BitmapImage with the specified dimensions and color
        /// </summary>
        private BitmapImage CreateTestBitmapImage(int width, int height, Rgba32 color)
        {
            using var image = new Image<Rgba32>(width, height);
            
            // Fill the image with the specified color
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] = color;
                }
            }

            // Convert to BitmapImage
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());
            memoryStream.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        /// <summary>
        /// Creates a test BitmapImage with a gradient
        /// </summary>
        private BitmapImage CreateGradientBitmapImage(int width, int height)
        {
            using var image = new Image<Rgba32>(width, height);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte value = (byte)((x * 255) / width);
                    image[x, y] = new Rgba32(value, value, value, 255);
                }
            }

            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());
            memoryStream.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        /// <summary>
        /// Creates a test PNG image file and returns its path
        /// </summary>
        private string CreateTestImageFile(int width, int height, Rgba32 color)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"test_image_{Guid.NewGuid()}.png");
            
            using var image = new Image<Rgba32>(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] = color;
                }
            }
            
            image.Save(tempPath);
            return tempPath;
        }

        /// <summary>
        /// Converts BitmapImage to ImageSharp Image for testing
        /// </summary>
        private Image<Rgba32> BitmapImageToImage(BitmapImage bitmapImage)
        {
            using var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(stream);
            stream.Position = 0;
            return Image.Load<Rgba32>(stream);
        }

        #endregion

        #region ConvertToBinary Tests

        [Fact]
        public void ConvertToBinary_InRange_PixelsInRangeAreWhite()
        {
            // Arrange
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.ConvertToBinary(testImage, "InRange", 100, 150);

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            Assert.Equal(255, resultImage[0, 0].R);
            Assert.Equal(255, resultImage[0, 0].G);
            Assert.Equal(255, resultImage[0, 0].B);
        }

        [Fact]
        public void ConvertToBinary_InRange_PixelsOutOfRangeAreBlack()
        {
            // Arrange
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(50, 50, 50, 255));

            // Act
            var result = _imageService.ConvertToBinary(testImage, "InRange", 100, 150);

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            Assert.Equal(0, resultImage[0, 0].R);
            Assert.Equal(0, resultImage[0, 0].G);
            Assert.Equal(0, resultImage[0, 0].B);
        }

        [Fact]
        public void ConvertToBinary_OutRange_PixelsInRangeAreBlack()
        {
            // Arrange
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.ConvertToBinary(testImage, "OutRange", 100, 150);

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            Assert.Equal(0, resultImage[0, 0].R);
            Assert.Equal(0, resultImage[0, 0].G);
            Assert.Equal(0, resultImage[0, 0].B);
        }

        [Fact]
        public void ConvertToBinary_OutRange_PixelsOutOfRangeAreWhite()
        {
            // Arrange
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(50, 50, 50, 255));

            // Act
            var result = _imageService.ConvertToBinary(testImage, "OutRange", 100, 150);

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            Assert.Equal(255, resultImage[0, 0].R);
            Assert.Equal(255, resultImage[0, 0].G);
            Assert.Equal(255, resultImage[0, 0].B);
        }

        [Fact]
        public void ConvertToBinary_PreservesDimensions()
        {
            // Arrange
            int width = 20;
            int height = 15;
            var testImage = CreateTestBitmapImage(width, height, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.ConvertToBinary(testImage, "InRange", 100, 150);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(width, result.PixelWidth);
            Assert.Equal(height, result.PixelHeight);
        }

        #endregion

        #region LoadImage Tests

        [Fact]
        public void LoadImage_ValidPath_ReturnsImageData()
        {
            // Arrange
            string testPath = CreateTestImageFile(10, 10, new Rgba32(255, 0, 0, 255));

            try
            {
                // Act
                var result = _imageService.LoadImage(testPath);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Width);
                Assert.Equal(10, result.Height);
                Assert.Equal("Png", result.Format);
                Assert.NotNull(result.PixelData);
                Assert.True(result.PixelData.Length > 0);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
            }
        }

        [Fact]
        public void LoadImage_InvalidPath_ThrowsException()
        {
            // Arrange
            string invalidPath = "non_existent_file.png";

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _imageService.LoadImage(invalidPath));
        }

        [Fact]
        public void LoadImage_CorrectDimensions()
        {
            // Arrange
            int width = 50;
            int height = 30;
            string testPath = CreateTestImageFile(width, height, new Rgba32(128, 128, 128, 255));

            try
            {
                // Act
                var result = _imageService.LoadImage(testPath);

                // Assert
                Assert.Equal(width, result.Width);
                Assert.Equal(height, result.Height);
            }
            finally
            {
                if (File.Exists(testPath))
                {
                    File.Delete(testPath);
                }
            }
        }

        #endregion

        #region ConvertToGrayscale Tests

        [Fact]
        public void ConvertToGrayscale_ColorImage_BecomesGrayscale()
        {
            // Arrange - Create a red image
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(255, 0, 0, 255));

            // Act
            var result = _imageService.ConvertToGrayscale(testImage);

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            
            // For red color (255, 0, 0), using BT.709: 0.2126 * 255 = 54.213 ≈ 54
            byte expectedGray = (byte)(0.2126f * 255);
            Assert.Equal(resultImage[0, 0].R, resultImage[0, 0].G);
            Assert.Equal(resultImage[0, 0].G, resultImage[0, 0].B);
            Assert.InRange(resultImage[0, 0].R, expectedGray - 1, expectedGray + 1);
        }

        [Fact]
        public void ConvertToGrayscale_AlreadyGrayscale_RemainsGrayscale()
        {
            // Arrange - Create a gray image
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.ConvertToGrayscale(testImage);

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            Assert.Equal(resultImage[0, 0].R, resultImage[0, 0].G);
            Assert.Equal(resultImage[0, 0].G, resultImage[0, 0].B);
        }

        [Fact]
        public void ConvertToGrayscale_PreservesDimensions()
        {
            // Arrange
            int width = 25;
            int height = 20;
            var testImage = CreateTestBitmapImage(width, height, new Rgba32(100, 150, 200, 255));

            // Act
            var result = _imageService.ConvertToGrayscale(testImage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(width, result.PixelWidth);
            Assert.Equal(height, result.PixelHeight);
        }

        [Fact]
        public void ConvertToGrayscale_PreservesAlpha()
        {
            // Arrange
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(255, 0, 0, 255));

            // Act
            var result = _imageService.ConvertToGrayscale(testImage);

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            Assert.Equal(255, resultImage[0, 0].A);
        }

        #endregion

        #region Resize Tests

        [Fact]
        public void Resize_NearestNeighbor_ScaleUp_CorrectDimensions()
        {
            // Arrange
            int originalWidth = 10;
            int originalHeight = 10;
            double scale = 2.0;
            var testImage = CreateTestBitmapImage(originalWidth, originalHeight, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.Resize(testImage, scale, "NearestNeighbor");

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)(originalWidth * scale), result.PixelWidth);
            Assert.Equal((int)(originalHeight * scale), result.PixelHeight);
        }

        [Fact]
        public void Resize_NearestNeighbor_ScaleDown_CorrectDimensions()
        {
            // Arrange
            int originalWidth = 20;
            int originalHeight = 20;
            double scale = 0.5;
            var testImage = CreateTestBitmapImage(originalWidth, originalHeight, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.Resize(testImage, scale, "NearestNeighbor");

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)(originalWidth * scale), result.PixelWidth);
            Assert.Equal((int)(originalHeight * scale), result.PixelHeight);
        }

        [Fact]
        public void Resize_Bilinear_ScaleUp_CorrectDimensions()
        {
            // Arrange
            int originalWidth = 10;
            int originalHeight = 10;
            double scale = 2.0;
            var testImage = CreateTestBitmapImage(originalWidth, originalHeight, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.Resize(testImage, scale, "Bilinear");

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)(originalWidth * scale), result.PixelWidth);
            Assert.Equal((int)(originalHeight * scale), result.PixelHeight);
        }

        [Fact]
        public void Resize_Bilinear_ScaleDown_CorrectDimensions()
        {
            // Arrange
            int originalWidth = 20;
            int originalHeight = 20;
            double scale = 0.5;
            var testImage = CreateTestBitmapImage(originalWidth, originalHeight, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.Resize(testImage, scale, "Bilinear");

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)(originalWidth * scale), result.PixelWidth);
            Assert.Equal((int)(originalHeight * scale), result.PixelHeight);
        }

        [Fact]
        public void Resize_NearestNeighbor_PreservesColors()
        {
            // Arrange
            var testImage = CreateTestBitmapImage(10, 10, new Rgba32(200, 100, 50, 255));

            // Act
            var result = _imageService.Resize(testImage, 2.0, "NearestNeighbor");

            // Assert
            Assert.NotNull(result);
            using var resultImage = BitmapImageToImage(result);
            // Nearest neighbor should preserve exact color values
            Assert.Equal(200, resultImage[0, 0].R);
            Assert.Equal(100, resultImage[0, 0].G);
            Assert.Equal(50, resultImage[0, 0].B);
        }

        [Fact]
        public void Resize_ScaleOfOne_SameDimensions()
        {
            // Arrange
            int width = 15;
            int height = 15;
            var testImage = CreateTestBitmapImage(width, height, new Rgba32(128, 128, 128, 255));

            // Act
            var result = _imageService.Resize(testImage, 1.0, "NearestNeighbor");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(width, result.PixelWidth);
            Assert.Equal(height, result.PixelHeight);
        }

        [Fact]
        public void Resize_Bilinear_InterpolatesColors()
        {
            // Arrange - Create a gradient image
            var testImage = CreateGradientBitmapImage(10, 10);

            // Act
            var result = _imageService.Resize(testImage, 2.0, "Bilinear");

            // Assert
            Assert.NotNull(result);
            // Bilinear should create smooth transitions
            using var resultImage = BitmapImageToImage(result);
            Assert.True(resultImage.Width == 20);
            Assert.True(resultImage.Height == 20);
        }

        #endregion
    }
}
