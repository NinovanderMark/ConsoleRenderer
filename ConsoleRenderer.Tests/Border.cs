using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleRenderer.Tests
{
    public class Border
    {
        [Theory]
        [InlineData('x')]
        [InlineData('0')]
        public void Char(char character)
        {
            // Assemble
            const int width = 10;
            const int height = 10;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.CreateBorder(character);

            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if ( x == 0 || y == 0 || x == width-1 || y == height-1)
                    {
                        var pixel = canvas.Get(x, y, false);
                        Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                        Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
                        Assert.Equal(character, pixel.Character);
                    }
                }
            }
        }

        [Theory]
        [InlineData('x', ConsoleColor.Blue, ConsoleColor.Red)]
        [InlineData('-', ConsoleColor.Green, ConsoleColor.White)]
        public void Char_Foreground_Background(char character, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            const int width = 10;
            const int height = 10;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.CreateBorder(character, foreground, background);

            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    if (x == 0 || y == 0 || x == width-1 || y == height-1)
                    {
                        Assert.Equal(foreground, pixel.Foreground);
                        Assert.Equal(background, pixel.Background);
                        Assert.Equal(character, pixel.Character);
                    }
                    else
                    {
                        Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                        Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
                        Assert.Equal(' ', pixel.Character);
                    }
                }
            }
        }

        [Theory]
        [InlineData(2, 2, 4, 4)]
        [InlineData(4, 4, 8, 8)]
        public void X_Y_Width_Height(int startX, int startY, int width, int height)
        {
            // Assemble
            int canvasWidth = 5 + startX + width;
            int canvasHeight = 5 + startY + height;
            var canvas = new ConsoleCanvas(canvasWidth, canvasHeight);

            // Act
            canvas.CreateBorder(startX, startY, width, height);

            // Assert
            for (int x = 0; x < canvasWidth; x++)
            {
                for (int y = 0; y < canvasHeight; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                    Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);

                    if ((x >= startX && y >= startY && x < (startX + width) && y < (startY + height) ) &&
                        (x == startX || y == startY || x == (startX + width) - 1 || y == (startY + height) - 1))
                    {
                        Assert.Contains(pixel.Character, new char[] { '═', '║', '╔', '╗', '╚', '╝' });
                    }
                    else
                    {
                        Assert.Equal(' ', pixel.Character);
                    }
                }
            }
        }

        [Theory]
        [InlineData(2, 2, 4, 4, 'a')]
        [InlineData(4, 4, 8, 8, '-')]
        public void X_Y_Width_Height_Char(int startX, int startY, int width, int height, char character)
        {
            // Assemble
            int canvasWidth = 5 + startX + width;
            int canvasHeight = 5 + startY + height;
            var canvas = new ConsoleCanvas(canvasWidth, canvasHeight);

            // Act
            canvas.CreateBorder(startX, startY, width, height, character);

            // Assert
            for (int x = 0; x < canvasWidth; x++)
            {
                for (int y = 0; y < canvasHeight; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                    Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);

                    if ((x >= startX && y >= startY && x < (startX + width) && y < (startY + height)) &&
                        (x == startX || y == startY || x == (startX + width) - 1 || y == (startY + height) - 1))
                    {
                        Assert.Equal(character, pixel.Character);
                    }
                    else
                    {
                        Assert.Equal(' ', pixel.Character);
                    }
                }
            }
        }

        [Theory]
        [InlineData(2, 2, 4, 4, 'a', ConsoleColor.Red, ConsoleColor.Blue)]
        [InlineData(4, 4, 8, 8, '-', ConsoleColor.Green, ConsoleColor.Yellow)]
        public void X_Y_Width_Height_Char_Foreground_Background(int startX, int startY, int width, int height, char character, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            int canvasWidth = 5 + startX + width;
            int canvasHeight = 5 + startY + height;
            var canvas = new ConsoleCanvas(canvasWidth, canvasHeight);

            // Act
            canvas.CreateBorder(startX, startY, width, height, character, foreground, background);

            // Assert
            for (int x = 0; x < canvasWidth; x++)
            {
                for (int y = 0; y < canvasHeight; y++)
                {
                    var pixel = canvas.Get(x, y, false);

                    if ((x >= startX && y >= startY && x < (startX + width) && y < (startY + height)) &&
                        (x == startX || y == startY || x == (startX + width) - 1 || y == (startY + height) - 1))
                    {
                        Assert.Equal(foreground, pixel.Foreground);
                        Assert.Equal(background, pixel.Background);
                        Assert.Equal(character, pixel.Character);
                    }
                    else
                    {
                        Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                        Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
                        Assert.Equal(' ', pixel.Character);
                    }
                }
            }
        }

        [Theory]
        [InlineData(2, 2, 4, 4, ConsoleColor.Red, ConsoleColor.Blue)]
        [InlineData(4, 4, 8, 8, ConsoleColor.Yellow, ConsoleColor.Green)]
        public void X_Y_Width_Height_NullChar_Foreground_Background(int startX, int startY, int width, int height, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            int canvasWidth = 5 + startX + width;
            int canvasHeight = 5 + startY + height;
            var canvas = new ConsoleCanvas(canvasWidth, canvasHeight);

            // Act
            canvas.CreateBorder(startX, startY, width, height, null, foreground, background);

            // Assert
            for (int x = 0; x < canvasWidth; x++)
            {
                for (int y = 0; y < canvasHeight; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    if ((x >= startX && y >= startY && x < (startX + width) && y < (startY + height)) &&
                        (x == startX || y == startY || x == (startX + width) - 1 || y == (startY + height) - 1))
                    {
                        Assert.Equal(foreground, pixel.Foreground);
                        Assert.Equal(background, pixel.Background);
                        Assert.Contains(pixel.Character, new char[] { '═', '║', '╔', '╗', '╚', '╝' });
                    }
                    else
                    {
                        Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                        Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
                        Assert.Equal(' ', pixel.Character);
                    }
                }
            }
        }

        [Fact]
        public void NoArgs()
        {
            // Assemble
            const int width = 10;
            const int height = 10;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.CreateBorder();

            // Assert
            var allPixels = new List<Pixel>();
            for (int x = 1; x < width-1; x++)
            {
                var topRow = canvas.Get(x, 0, false);
                Assert.Equal('═', topRow.Character);
                allPixels.Add(topRow);

                var bottomRow = canvas.Get(x, height-1, false);
                Assert.Equal('═', topRow.Character);
                allPixels.Add(bottomRow);
            }

            for (int y = 1; y < height - 1; y++)
            {
                var leftRow = canvas.Get(0, y, false);
                Assert.Equal('║', leftRow.Character);
                allPixels.Add(leftRow);

                var rightRow = canvas.Get(width-1, y, false);
                Assert.Equal('║', leftRow.Character);
                allPixels.Add(rightRow);
            }

            // Check color for all pixels
            foreach (var pix in allPixels)
            {
                Assert.Equal(canvas.DefaultForegroundColor, pix.Foreground);
                Assert.Equal(canvas.DefaultBackgroundColor, pix.Background);
            }
        }
    }
}
