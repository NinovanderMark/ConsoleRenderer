using Xunit;

namespace ConsoleRenderer.Tests
{
    public class Set
    {
        [Fact]
        public void X_Y()
        {
            // Assemble
            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0);

            // Assert
            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
            Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
            Assert.Equal('*', pixel.Character);
        }

        [Theory]
        [InlineData('x')]
        [InlineData('z')]
        public void X_Y_Char(char character)
        {
            // Assemble
            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0, character);

            // Assert
            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
            Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
            Assert.Equal(character, pixel.Character);
        }

        [Theory]
        [InlineData(ConsoleColor.Red)]
        [InlineData(ConsoleColor.Blue)]
        public void X_Y_Color(ConsoleColor color)
        {
            // Assemble
            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0, color);

            // Assert
            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(color, pixel.Foreground);
            Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
            Assert.Equal('*', pixel.Character);
        }

        [Theory]
        [InlineData('x', ConsoleColor.Red, ConsoleColor.Blue)]
        [InlineData('-', ConsoleColor.Yellow, ConsoleColor.Green)]
        public void X_Y_Pixel(char character, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            Pixel target = new Pixel { Background = background, Foreground = foreground, Character = character };
            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0, target);

            // Assert
            Assert.Equal(target, canvas.Get(0, 0, false));
        }

        [Fact]
        public void X_Y_PixelArray()
        {
            // Assemble
            Pixel[] target = new Pixel[]
            { 
                new() { Background = ConsoleColor.Yellow, Foreground = ConsoleColor.Green, Character = 'x' },
                new() { Background = ConsoleColor.Red, Foreground = ConsoleColor.Blue, Character = 'a' },
            };

            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0, target);

            // Assert
            Assert.Equal(target[0], canvas.Get(0, 0, false));
            Assert.Equal(target[1], canvas.Get(1, 0, false));
        }

        [Fact]
        public void X_Y_PixelList()
        {
            // Assemble
            List<Pixel> target = new()
            {
                new() { Background = ConsoleColor.Yellow, Foreground = ConsoleColor.Green, Character = 'x' },
                new() { Background = ConsoleColor.Red, Foreground = ConsoleColor.Blue, Character = 'a' }
            };

            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0, target);

            // Assert
            Assert.Equal(target[0], canvas.Get(0, 0, false));
            Assert.Equal(target[1], canvas.Get(1, 0, false));
        }

        [Theory]
        [InlineData('x', ConsoleColor.Red)]
        [InlineData('-', ConsoleColor.Yellow)]
        public void X_Y_Character_Foreground(char character, ConsoleColor color)
        {
            // Assemble
            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0, character, color);

            // Assert
            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(character, pixel.Character);
            Assert.Equal(color, pixel.Foreground);
        }

        [Theory]
        [InlineData('a', ConsoleColor.Red, ConsoleColor.Blue)]
        [InlineData('1', ConsoleColor.Yellow, ConsoleColor.Green)]
        public void X_Y_Character_Foreground_Background(char character, ConsoleColor foreground, ConsoleColor background)
        {
            // Assemble
            var canvas = new ConsoleCanvas(10, 10);

            // Act
            canvas.Set(0, 0, character, foreground, background);

            // Assert
            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(character, pixel.Character);
            Assert.Equal(foreground, pixel.Foreground);
            Assert.Equal(background, pixel.Background);
        }
    }
}