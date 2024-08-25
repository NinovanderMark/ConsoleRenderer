using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleRenderer.Tests
{
    public class Rectangle
    {
        [Theory]
        [InlineData(2, 2, 4, 4)]
        [InlineData(4, 4, 8, 8)]
        public void X_Y_Width_Height(int startX, int startY, int w, int h)
        {
            // Assemble
            const int width = 20;
            const int height = 20;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.CreateRectangle(startX, startY, w, h);

            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                    Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);

                    if (x >= startX && x < startX + w && y >= startY && y < startY + h)
                    {
                        Assert.Equal('*', pixel.Character);
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
        [InlineData(4, 4, 8, 8, 'x')]
        public void X_Y_Width_Height_Char(int startX, int startY, int w, int h, char character)
        {
            // Assemble
            const int width = 20;
            const int height = 20;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.CreateRectangle(startX, startY, w, h, character);

            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                    Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);

                    if ( x >= startX && x < startX + w && y >= startY && y < startY + h)
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
        [InlineData(4, 4, 8, 8, 'x', ConsoleColor.Green, ConsoleColor.Yellow)]
        public void X_Y_Width_Height_Char_Foreground_Background(int startX, int startY, int w, int h, char character, 
            ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            const int width = 20;
            const int height = 20;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.CreateRectangle(startX, startY, w, h, character, foreground, background);

            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = canvas.Get(x, y, false);

                    if (x >= startX && x < startX + w && y >= startY && y < startY + h)
                    {
                        Assert.Equal(character, pixel.Character);
                        Assert.Equal(foreground, pixel.Foreground);
                        Assert.Equal(background, pixel.Background);
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
    }
}
