using Xunit;

namespace ConsoleRenderer.Tests
{
    public class Set
    {
        [Fact]
        public void Happy_XY()
        {
            // Assemble
            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0);

            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
            Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
            Assert.Equal('*', pixel.Character);
        }

        [Fact]
        public void Happy_XY_Char()
        {
            const char character = 'x';

            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0, character);

            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(canvas.DefaultForegroundColor, pixel.Foreground);
            Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
            Assert.Equal(character, pixel.Character);
        }

        [Fact]
        public void Happy_XY_Color()
        {
            const ConsoleColor color = ConsoleColor.Red;

            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0, color);

            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(color, pixel.Foreground);
            Assert.Equal(canvas.DefaultBackgroundColor, pixel.Background);
            Assert.Equal('*', pixel.Character);
        }

        [Fact]
        public void Happy_XY_Pixel()
        {
            Pixel target = new Pixel { Background = ConsoleColor.Yellow, Foreground = ConsoleColor.Green, Character = 'x' };

            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0, target);

            Assert.Equal(target, canvas.Get(0, 0, false));
        }

        [Fact]
        public void Happy_XY_PixelArray()
        {
            Pixel[] target = new Pixel[]
            { 
                new() { Background = ConsoleColor.Yellow, Foreground = ConsoleColor.Green, Character = 'x' },
                new() { Background = ConsoleColor.Red, Foreground = ConsoleColor.Blue, Character = 'a' },
            };

            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0, target);

            Assert.Equal(target[0], canvas.Get(0, 0, false));
            Assert.Equal(target[1], canvas.Get(1, 0, false));
        }

        [Fact]
        public void Happy_XY_PixelList()
        {
            List<Pixel> target = new()
            {
                new() { Background = ConsoleColor.Yellow, Foreground = ConsoleColor.Green, Character = 'x' },
                new() { Background = ConsoleColor.Red, Foreground = ConsoleColor.Blue, Character = 'a' }
            };

            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0, target);

            Assert.Equal(target[0], canvas.Get(0, 0, false));
            Assert.Equal(target[1], canvas.Get(1, 0, false));
        }

        [Fact]
        public void Happy_XY_Character_Foreground()
        {
            const char character = 'x';
            const ConsoleColor color = ConsoleColor.Yellow;

            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0, character, color);

            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(character, pixel.Character);
            Assert.Equal(color, pixel.Foreground);
        }

        [Fact]
        public void Happy_XY_Character_Foreground_Background()
        {
            const char character = 'x';
            const ConsoleColor foreground = ConsoleColor.Yellow;
            const ConsoleColor background = ConsoleColor.Magenta;

            var canvas = new ConsoleCanvas(10, 10);
            canvas.Set(0, 0, character, foreground, background);

            var pixel = canvas.Get(0, 0, false);
            Assert.Equal(character, pixel.Character);
            Assert.Equal(foreground, pixel.Foreground);
            Assert.Equal(background, pixel.Background);
        }
    }
}