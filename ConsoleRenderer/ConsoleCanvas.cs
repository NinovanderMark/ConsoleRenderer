using System.Drawing;
using System.Reflection.PortableExecutable;

namespace ConsoleRenderer
{
    public class ConsoleCanvas
    {
        public int Width { get => _width; }
        public int Height { get => _height; }
        public ConsoleColor DefaultColor { get => _defaultForegroundColor; }

        private int _width;
        private int _height;
        private List<List<Pixel>> _pixels;
        private List<List<Pixel>> _previous;

        private ConsoleColor _defaultForegroundColor;
        private ConsoleColor _defaultBackgroundColor;

        public ConsoleCanvas()
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;
            _pixels = new List<List<Pixel>>();
            _previous = new List<List<Pixel>>();

            _defaultForegroundColor = Console.ForegroundColor;
            _defaultBackgroundColor = Console.BackgroundColor;

            for (int y = 0; y < _height; y++)
            {
                var row = new List<Pixel>();
                for (int x = 0; x < _width; x++)
                    row.Add(new Pixel 
                    { 
                        Character = ' ', 
                        Foreground = _defaultForegroundColor,
                        Background = _defaultBackgroundColor
                    });

                _pixels.Add(row);
                _previous.Add(row.ToList());
            }

        }

        /// <summary>
        /// Clears all the pixels on the canvas
        /// </summary>
        public ConsoleCanvas Clear()
        {
            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                    _pixels[y][x] = new Pixel 
                    { 
                        Character = ' ', 
                        Foreground = _defaultForegroundColor,
                        Background = _defaultBackgroundColor
                    };

            return this;
        }

        /// <summary>
        /// Set a particular pixel to the provided character with the default color
        /// </summary>
        public ConsoleCanvas Set(int x, int y, char character = '*')
        {
            return Set(x, y, _defaultForegroundColor, character);
        }

        /// <summary>
        /// Set a particular pixel to a particular color with the provided character
        /// </summary>
        public ConsoleCanvas Set(int x, int y, ConsoleColor color, char character = '*')
        {
            return Set(x, y, color, _defaultBackgroundColor, character);
        }

        public ConsoleCanvas Set(int x, int y, ConsoleColor foreground, ConsoleColor background, char character = '*')
        {
            return Set(x, y, new Pixel
            {
                Character = character,
                Foreground = foreground,
                Background = background,
            });
        }

        /// <summary>
        /// Set a particular pixel on the screen
        /// </summary>
        public ConsoleCanvas Set(int x, int y, Pixel pixel)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new IndexOutOfRangeException("Character position out of bounds");

            _pixels[y][x] = pixel;
            return this;
        }

        /// <summary>
        /// Creates a border on the edges of the canvas with the default color
        /// </summary>
        public ConsoleCanvas CreateBorder()
        {
            return CreateBorder(_defaultForegroundColor);
        }

        /// <summary>
        /// Creates a border on the edges of the canvas with a particular color
        /// </summary>
        public ConsoleCanvas CreateBorder(ConsoleColor color)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (y == 0 || y + 1 == _height || x == 0 || x + 1 == _width)
                        _pixels[y][x] = new Pixel { Character = '*', Foreground = color }; // Replace with nice ASCII edges
                    else
                        _pixels[y][x] = new Pixel { Character = ' ', Foreground = color };
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
        /// <param name="character">Character to use for drawing the rectangle's borders</param>
        public ConsoleCanvas CreateRectangle(int startX, int startY, int width, int height, char character = '*')
        {
            return CreateRectangle(startX, startY, width, height, character, _defaultForegroundColor, _defaultBackgroundColor);
        }

        /// <summary>
        /// Creates a rectangle on the canvas 
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to use for drawing the rectangle's borders</param>
        /// <param name="border">Color to draw the border with</param>
        /// <param name="fill">Color to fill the rectangle with</param>
        public ConsoleCanvas CreateRectangle(int startX, int startY, int width, int height, char character, ConsoleColor border, ConsoleColor fill)
        {
            for (int y = startY; y < _height && y-startY < height; y++)
            {
                for (int x = startX; x < _width && x-startX < width; x++)
                {
                    if (y == startY || y + 1 == startY + height|| x == startX || x + 1 == startX + width)
                        _pixels[y][x] = new Pixel 
                        { 
                            Character = character, 
                            Foreground = border,
                            Background = _defaultBackgroundColor,
                        };
                    else
                        _pixels[y][x] = new Pixel 
                        { 
                            Character = ' ', 
                            Foreground = _defaultForegroundColor,
                            Background = fill
                        };
                }
            }

            return this;
        }

        /// <summary>
        /// Renders all the pixels on the canvas
        /// </summary>
        public ConsoleCanvas Render()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_pixels[y][x] == _previous[y][x])
                        continue;

                    if (Console.CursorLeft != x)
                        Console.CursorLeft = x;

                    if (Console.CursorTop != y)
                        Console.CursorTop = y;

                    if (_pixels[y][x].Foreground != Console.ForegroundColor)
                        Console.ForegroundColor = _pixels[y][x].Foreground;

                    if (_pixels[y][x].Background != Console.BackgroundColor)
                        Console.BackgroundColor = _pixels[y][x].Background;

                    Console.Write(_pixels[y][x].Character);

                    _previous[y][x] = _pixels[y][x];
                }

                if (y + 1 != _height)
                    Console.WriteLine();
            }

            return this;
        }
    }
}