using ImageProcessing.App.Utilities;
using System;
using System.Globalization;
using Xunit;

namespace ImageProcessing.Tests.Utilities
{
    public class InverseBooleanConverterTests
    {
        private readonly InverseBooleanConverter _converter;

        public InverseBooleanConverterTests()
        {
            _converter = new InverseBooleanConverter();
        }

        [Fact]
        public void Convert_WhenTrue_ReturnsFalse()
        {
            // Arrange
            bool input = true;

            // Act
            var result = _converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void Convert_WhenFalse_ReturnsTrue()
        {
            // Arrange
            bool input = false;

            // Act
            var result = _converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void Convert_WhenNull_ReturnsTrue()
        {
            // Arrange
            object? input = null;

            // Act
            var result = _converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void Convert_WhenNonBoolean_ReturnsTrue()
        {
            // Arrange
            object input = "not a boolean";

            // Act
            var result = _converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() =>
                _converter.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture));
        }
    }
}
