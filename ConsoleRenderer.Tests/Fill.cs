using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleRenderer.Tests
{
    public class Fill
    {
        [Theory]
        [InlineData('x', ConsoleColor.Red, ConsoleColor.Blue)]
        [InlineData('a', ConsoleColor.White, ConsoleColor.Gray)]
        public void X_Y(char character, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            const int width = 10;
            const int height = 10;
            var canvas = new ConsoleCanvas(width, height);

            // Act
            canvas.Fill(character, foreground, background);

            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    Assert.Equal(foreground, pixel.Foreground);
                    Assert.Equal(background, pixel.Background);
                    Assert.Equal(character, pixel.Character);
                }
            }
        }
    }
}
