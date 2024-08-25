using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleRenderer.Tests
{
    public class Text
    {
        [Theory]
        [InlineData(0, 0, "")]
        [InlineData(2, 2, "God")]
        [InlineData(4, 4, "Jesus")]
        [InlineData(6, 6, "Spirit")]
        public void X_Y_Text(int x, int y, string text)
        {
            // Assemble
            const int width = 16; 
            const int height = 16;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.Text(x, y, text);

            // Assert
            for (int xx = 0; xx < width; xx++)
            {
                for (int yy = 0; yy < height; yy++)
                {
                    var pixel = canvas.Get(xx, yy, false);
                    Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                    Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);

                    if ( yy == y && xx >= x && xx < x+text.Length)
                    {
                        var intendedChar = text.ToCharArray()[xx - x];
                        Assert.Equal(intendedChar, pixel.Character);
                    }
                    else
                    {
                        Assert.Equal(' ', pixel.Character);
                    }
                }
            }
        }

        [Theory]
        [InlineData(0, 0, "")]
        [InlineData(2, 2, "God")]
        [InlineData(4, 4, "Jesus")]
        [InlineData(6, 6, "Spirit")]
        public void X_Y_Text_Centered(int x, int y, string text)
        {
            // Assemble
            const int width = 16;
            const int height = 16;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.Text(x, y, text, true);

            // Assert
            for (int xx = 0; xx < width; xx++)
            {
                for (int yy = 0; yy < height; yy++)
                {
                    var pixel = canvas.Get(xx, yy, false);
                    Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                    Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);

                    int offset = (int)Math.Floor(text.Length / 2d);
                    if (yy == y && xx >= x - offset && xx < x - offset + text.Length)
                    {
                        var intendedChar = text.ToCharArray()[xx - x + offset];
                        Assert.Equal(intendedChar, pixel.Character);
                    }
                    else
                    {
                        Assert.Equal(' ', pixel.Character);
                    }
                }
            }
        }

        [Theory]
        [InlineData(0, 0, "", ConsoleColor.White, ConsoleColor.Black)]
        [InlineData(2, 2, "God", ConsoleColor.Red, ConsoleColor.Blue)]
        [InlineData(4, 4, "Jesus", ConsoleColor.Yellow, ConsoleColor.Green)]
        [InlineData(6, 6, "Spirit", ConsoleColor.Magenta, ConsoleColor.Cyan)]
        public void X_Y_Text_NotCentered_Foreground_Background(int x, int y, string text, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            const int width = 16;
            const int height = 16;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.Text(x, y, text, false, foreground, background);

            // Assert
            for (int xx = 0; xx < width; xx++)
            {
                for (int yy = 0; yy < height; yy++)
                {
                    var pixel = canvas.Get(xx, yy, false);
                    if (yy == y && xx >= x && xx < x + text.Length)
                    {
                        Assert.Equal(foreground, pixel.Foreground);
                        Assert.Equal(background, pixel.Background);

                        var intendedChar = text.ToCharArray()[xx - x];
                        Assert.Equal(intendedChar, pixel.Character);
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
        [InlineData(0, 0, "", ConsoleColor.White, ConsoleColor.Black)]
        [InlineData(2, 2, "God", ConsoleColor.Red, ConsoleColor.Blue)]
        [InlineData(4, 4, "Jesus", ConsoleColor.Yellow, ConsoleColor.Green)]
        [InlineData(6, 6, "Spirit", ConsoleColor.Magenta, ConsoleColor.Cyan)]
        public void X_Y_Text_Centered_Foreground_Background(int x, int y, string text, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            const int width = 16;
            const int height = 16;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.Text(x, y, text, true, foreground, background);

            // Assert
            for (int xx = 0; xx < width; xx++)
            {
                for (int yy = 0; yy < height; yy++)
                {
                    var pixel = canvas.Get(xx, yy, false);
                    int offset = (int)Math.Floor(text.Length / 2d);

                    if (yy == y && xx >= x - offset && xx < x - offset + text.Length)
                    {
                        Assert.Equal(foreground, pixel.Foreground);
                        Assert.Equal(background, pixel.Background);

                        var intendedChar = text.ToCharArray()[xx - x + offset];
                        Assert.Equal(intendedChar, pixel.Character);
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
