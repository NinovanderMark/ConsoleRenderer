using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleRenderer.Tests
{
    public class Clear
    {
        [Fact]
        public void NoArgs()
        {
            // Assemble
            const int width = 10;
            const int height = 10;

            var canvas = new ConsoleCanvas(width, height);
            canvas.Set(0, 0)
                .Set(2, 2)
                .Set(4, 4)
                .Set(8, 8);

            // Act
            canvas.Clear();

            // Assert
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = canvas.Get(x, y, false);
                    Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
                    Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
                    Assert.Equal(' ', pixel.Character);
                }
            }
        }
    }
}
