using System.ComponentModel;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace ConsoleRenderer
{
    public class ConsoleCanvas
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public ConsoleColor DefaultForegroundColor { get; set; }
        public ConsoleColor DefaultBackgroundColor { get; set; }

        private const char _defaultCharacter = '*';
        private const char _emptyCharacter = ' ';

        private int _previousWidth;
        private int _previousHeight;
        private List<List<Pixel>> _pixels;
        private List<List<Pixel>> _previous;

        public ConsoleCanvas(int width, int height)
        {
            Width = width;
            Height = height;
            _pixels = new List<List<Pixel>>();
            _previous = new List<List<Pixel>>();

            DefaultForegroundColor = Console.ForegroundColor;
            DefaultBackgroundColor = Console.BackgroundColor;

            for (int y = 0; y < Height; y++)
            {
                var row = new List<Pixel>();
                for (int x = 0; x < Width; x++)
                    row.Add(new Pixel
                    {
                        Character = _emptyCharacter,
                        Foreground = DefaultForegroundColor,
                        Background = DefaultBackgroundColor
                    });

                _pixels.Add(row);
                _previous.Add(row.ToList());
            }
        }

        public ConsoleCanvas() : this(Console.WindowWidth, Console.WindowHeight)
        {
        }

        /// <summary>
        /// Clears the canvas of all characters, using the default fore- and background colors
        /// </summary>
        /// <returns></returns>
        public ConsoleCanvas Clear()
        {
            return Fill(_emptyCharacter, DefaultForegroundColor, DefaultBackgroundColor);
        }

        /// <summary>
        /// Fills the canvas with the specified character in the given colors
        /// </summary>
        /// <param name="character">Character to fill the canvas with</param>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        /// <returns></returns>
        public ConsoleCanvas Fill(char character, ConsoleColor foreground, ConsoleColor background)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, character, foreground, background);

            return this;
        }

        /// <summary>
        /// Creates a border on the edges of the canvas with the default fore- and background colors
        /// </summary>
        /// <param name="character">Character to draw the border with</param>
        public ConsoleCanvas CreateBorder(char character = _defaultCharacter)
        {
            return CreateBorder(character, DefaultForegroundColor, DefaultBackgroundColor);
        }

        /// <summary>
        /// Creates a border on the edges of the canvas with the specified character and colors
        /// </summary>
        /// <param name="character">Character to draw the border with</param>
        /// <param name="foreground">Color to draw the border with</param>
        /// <param name="background">Color to draw the border with</param>
        public ConsoleCanvas CreateBorder(char character, ConsoleColor foreground, ConsoleColor background)
        {
            return CreateBorder(0, 0, Width, Height, character, foreground, background);
        }

        /// <summary>
        /// Creates a border on the edges of a rectangle with the specified character and colors
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to draw the border with</param>
        /// <param name="foreground">Color to draw the border with</param>
        /// <param name="background">Color to draw the border with</param>
        /// <returns></returns>
        public ConsoleCanvas CreateBorder(int startX, int startY, int width, int height, char character, ConsoleColor foreground, ConsoleColor background)
        {
            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    if (y == startY || y + 1 == startY + height || x == startX || x + 1 == startX + width)
                        Set(x, y, character, foreground, background);
                }
            }

            return this;
        }

        /// <summary>
        /// Creates a rectangle on the canvas 
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to fill the rectangle with</param>
        public ConsoleCanvas CreateRectangle(int startX, int startY, int width, int height, char character = _defaultCharacter)
        {
            return CreateRectangle(startX, startY, width, height, character, DefaultForegroundColor, DefaultBackgroundColor);
        }

        /// <summary>
        /// Creates a rectangle on the canvas 
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to fill the rectangle with</param>
        /// <param name="foreground">Color to draw the character with</param>
        /// <param name="background">Color to draw the background with</param>
        public ConsoleCanvas CreateRectangle(int startX, int startY, int width, int height, char character, ConsoleColor foreground, ConsoleColor background)
        {
            for (int y = startY; y < Height && y-startY < height; y++)
            {
                for (int x = startX; x < Width && x - startX < width; x++)
                    Set(x, y, character, foreground, background);
            }

            return this;
        }

        /// <summary>
        /// Renders all the pixels on the canvas
        /// </summary>
        public ConsoleCanvas Render()
        {
            if (_previousWidth != Console.WindowWidth || _previousHeight != Console.WindowHeight)
            {
                ClearPixelCache();

                _previousWidth = Console.WindowWidth;
                _previousHeight = Console.WindowHeight;
            }

            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            // Temporary variables to track Console attributes like size, position and color
            int cursorTop = 0;
            int cursorLeft = 0;
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            ConsoleColor backgroundColor = Console.BackgroundColor;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (_pixels[y][x] == _previous[y][x])
                        continue;

                    if (x >= windowWidth)
                        continue;

                    if (y >= windowHeight)
                        continue;

                    if (cursorLeft != x)
                    {
                        Console.CursorLeft = x;
                        cursorLeft = x;
                    }

                    if (cursorTop != y)
                    {
                        Console.CursorTop = y;
                        cursorTop = y;
                    }

                    if (_pixels[y][x].Foreground != foregroundColor && _pixels[y][x].Character != ' ')
                    {
                        Console.ForegroundColor = _pixels[y][x].Foreground;
                        foregroundColor = _pixels[y][x].Foreground;
                    }

                    if (_pixels[y][x].Background != backgroundColor)
                    {
                        Console.BackgroundColor = _pixels[y][x].Background;
                        backgroundColor = _pixels[y][x].Background;
                    }

                    Console.Write(_pixels[y][x].Character);
                    cursorLeft++;

                    _previous[y][x] = _pixels[y][x];
                }
            }

            return this;
        }

        /// <summary>
        /// Set a particular pixel on the canvas to the specified character with the default fore- and background colors
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="character">Character to set the pixel to</param>
        public ConsoleCanvas Set(int x, int y, char character = _defaultCharacter)
        {
            return Set(x, y, character, DefaultForegroundColor);
        }

        /// <summary>
        /// Set a particular pixel on the canvas to the specified foreground color, with the default background color
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public ConsoleCanvas Set(int x, int y, ConsoleColor color)
        {
            return Set(x, y, _defaultCharacter, color);
        }

        /// <summary>
        /// Set a particular pixel on the canvas to the specified character with a given color and the default background color
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="character">Character to set the pixel to</param>
        /// <param name="color">Color to draw the character with</param>
        public ConsoleCanvas Set(int x, int y, char character, ConsoleColor color)
        {
            return Set(x, y, character, color, DefaultBackgroundColor);
        }

        /// <summary>
        /// Set a particular pixel on the canvas to the specified character with a given background and foreground color
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="character">Character to set the pixel to</param>
        /// <param name="foreground">Foreground color to draw the character with</param>
        /// <param name="background">Background color to draw the character with</param>
        public ConsoleCanvas Set(int x, int y, char character, ConsoleColor foreground, ConsoleColor background)
        {
            return Set(x, y, new Pixel
            {
                Character = character,
                Foreground = foreground,
                Background = background,
            });
        }

        /// <summary>
        /// Set a particular pixel on the canvas to the specified <see cref="Pixel"/>
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="pixel">Pixel to set at the specified coordinates</param>
        public ConsoleCanvas Set(int x, int y, Pixel pixel)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                _pixels[y][x] = pixel;

            return this;
        }

        private void ClearPixelCache()
        {
            var defaultPixel = new Pixel
            {
                Background = DefaultBackgroundColor,
                Foreground = DefaultForegroundColor,
                Character = '\u00A0'
            };

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    _previous[y][x] = defaultPixel;
        }
    }
}