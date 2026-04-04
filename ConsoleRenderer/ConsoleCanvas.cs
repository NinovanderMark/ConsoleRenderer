using System;
using System.Collections.Generic;

namespace ConsoleRenderer
{
    public class ConsoleCanvas : ICanvas
    {
        /// <summary>
        /// Width of the canvas
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the canvas
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Foreground color to use if none was specified for operations updating the render buffer
        /// </summary>
        public ConsoleColor DefaultForegroundColor { get; set; }

        /// <summary>
        /// Background color to use if none was specified for operations updating the render buffer
        /// </summary>
        public ConsoleColor DefaultBackgroundColor { get; set; }

        /// <summary>
        /// Whether the <see cref="ConsoleCanvas"/> dimensions should automatically update to match the terminal's dimensions
        /// </summary>
        public bool AutoResize { get; set; }

        /// <summary>
        /// Interlaced mode alternates between rendering only odd or even rows to the screen each time <see cref="Render"/> is called
        /// </summary>
        public bool Interlaced { get; set; }

        private const char _defaultCharacter = '*';
        private const char _emptyCharacter = ' ';

        private int _previousWidth;
        private int _previousHeight;
        private bool _oddRows;
        private List<List<Pixel>> _pixels;
        private List<List<Pixel>> _previous;

        public ConsoleCanvas(int width, int height, bool interlaced = false, bool autoResize = false)
        {
            Width = width;
            Height = height;
            Interlaced = interlaced;
            AutoResize = autoResize;

            DefaultForegroundColor = Console.ForegroundColor;
            DefaultBackgroundColor = Console.BackgroundColor;

            _pixels = new List<List<Pixel>>();
            _previous = new List<List<Pixel>>();

            Resize(width, height);
        }

        public ConsoleCanvas(bool interlaced = false, bool autoResize = false) 
            : this(Console.WindowWidth, Console.WindowHeight, interlaced, autoResize)
        {
        }

        public ICanvas Clear()
        {
            return Fill(_emptyCharacter, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public ICanvas Fill(char character, ConsoleColor foreground, ConsoleColor background)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, character, foreground, background);

            return this;
        }

        public ICanvas CreateBorder(char? character = null)
        {
            return CreateBorder(character, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public ICanvas CreateBorder(char? character, ConsoleColor foreground, ConsoleColor background)
        {
            return CreateBorder(0, 0, Width, Height, character, foreground, background);
        }

        public ICanvas CreateBorder(int startX, int startY, int width, int height, char? character = null)
        {
            return CreateBorder(startX, startY, width, height, character, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public ICanvas CreateBorder(int startX, int startY, int width, int height, char? character, ConsoleColor foreground, ConsoleColor background)
        {
            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    if ( y != startY && y + 1 != startY + height && x != startX && x + 1 != startX + width)
                    {
                        continue;
                    }

                    char fallback = ' ';
                    if ( y == startY )
                    {
                        if ( x == startX )
                        {
                            fallback = '╔';
                        }
                        else if ( x + 1 == startX + width)
                        {
                            fallback = '╗';
                        }
                        else
                        {
                            fallback = '═';
                        }
                        
                    }
                    else if (y + 1 == startY + height)
                    {
                        if (x == startX)
                        {
                            fallback = '╚';
                        }
                        else if (x + 1 == startX + width)
                        {
                            fallback = '╝';
                        }
                        else
                        {
                            fallback = '═';
                        }
                    }
                    else if (x == startX || x + 1 == startX + width)
                    {
                        fallback = '║';
                    }

                    Set(x, y, character ?? fallback, foreground, background);
                }
            }

            return this;
        }

        public ICanvas CreateRectangle(int startX, int startY, int width, int height, char character = _defaultCharacter)
        {
            return CreateRectangle(startX, startY, width, height, character, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public ICanvas CreateRectangle(int startX, int startY, int width, int height, char character, ConsoleColor foreground, ConsoleColor background)
        {
            for (int y = startY; y < Height && y-startY < height; y++)
            {
                for (int x = startX; x < Width && x - startX < width; x++)
                    Set(x, y, character, foreground, background);
            }

            return this;
        }

        public ICanvas Render()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            // Temporary variables to track Console attributes like size, position and color
            int cursorTop = 0;
            int cursorLeft = 0;
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            ConsoleColor backgroundColor = Console.BackgroundColor;

            if (_previousWidth != windowWidth || _previousHeight != windowHeight)
            {
                if ( AutoResize )
                {
                    Resize(windowWidth, windowHeight);
                }

                ClearPixelCache();

                _previousWidth = windowWidth;
                _previousHeight = windowHeight;
            }

            int leftOperations = 0;
            int backgroundOperations = 0;

            for (int y = 0; y < Height; y++)
            {
                // See if this is one of the rows we should skip in Interlaced mode
                if ( Interlaced && ((_oddRows && y % 2 == 0) || (!_oddRows && y % 2 != 0)) )
                    continue;

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
                        try
                        {
                            Console.CursorLeft = x;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return Render();
                        }

                        cursorLeft = x;
                        leftOperations++;
                    }

                    if (cursorTop != y)
                    {
                        try
                        {
                            Console.CursorTop = y;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return Render();
                        }

                        cursorTop = y;
                    }

                    if (_pixels[y][x].Character != ' ' && _pixels[y][x].Foreground != foregroundColor)
                    {
                        Console.ForegroundColor = _pixels[y][x].Foreground;
                        foregroundColor = _pixels[y][x].Foreground;
                    }

                    if (_pixels[y][x].Background != backgroundColor)
                    {
                        Console.BackgroundColor = _pixels[y][x].Background;
                        backgroundColor = _pixels[y][x].Background;
                        backgroundOperations++;
                    }

                    Console.Write(_pixels[y][x].Character);
                    cursorLeft++;

                    _previous[y][x] = _pixels[y][x];

                    // After writing the last character on the bottom right, reposition the cursor to prevent
                    // an unintended newline, which may shift the screen downwards, causing jitter
                    if ( cursorLeft == windowWidth && cursorTop == windowHeight - 1)
                    {
                        Console.CursorLeft = 0;
                        Console.CursorTop = 0;
                        cursorLeft = 0;
                        cursorTop = 0;
                    }
                }
            }

            // Swap whether we render odd or even rows next frame
            _oddRows = !_oddRows;
            return this;
        }

        public ICanvas Resize(int width, int height)
        {
            Width = width;
            Height = height;
            _pixels = new List<List<Pixel>>();
            _previous = new List<List<Pixel>>();

            for (int y = 0; y < Height; y++)
            {
                var row = new List<Pixel>();
                var previousRow = new List<Pixel>();
                for (int x = 0; x < Width; x++)
                {
                    var pixel = new Pixel
                    {
                        Character = _emptyCharacter,
                        Foreground = DefaultForegroundColor,
                        Background = DefaultBackgroundColor
                    };

                    row.Add(pixel);
                    previousRow.Add(pixel);
                }

                _pixels.Add(row);
                _previous.Add(previousRow);
            }

            return this;
        }

        public ICanvas Set(int x, int y, char character = _defaultCharacter)
        {
            return Set(x, y, character, DefaultForegroundColor);
        }

        public ICanvas Set(int x, int y, ConsoleColor color)
        {
            return Set(x, y, _defaultCharacter, color);
        }

        public ICanvas Set(int x, int y, char character, ConsoleColor color)
        {
            return Set(x, y, character, color, DefaultBackgroundColor);
        }

        public ICanvas Set(int x, int y, char character, ConsoleColor foreground, ConsoleColor background)
        {
            return Set(x, y, new Pixel
            {
                Character = character,
                Foreground = foreground,
                Background = background,
            });
        }

        public ICanvas Set(int x, int y, Pixel pixel)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                _pixels[y][x] = pixel;

            return this;
        }

        public ICanvas Set(int x, int y, Pixel[] pixels)
        {
            for (int t = 0; t < pixels.Length; t++)
            {
                Set(x+t, y, pixels[t]);
            }

            return this;
        }

        public ICanvas Set(int x, int y, List<Pixel> pixels)
        {
            for (int t = 0; t < pixels.Count; t++)
            {
                Set(x + t, y, pixels[t]);
            }

            return this;
        }

        public ICanvas Text(int x, int y, string text, bool centered = false, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            // If the text should be centered, deduct half the text length from the x coordinate
            int startX = centered ? x - (int) Math.Floor(text.Length/2d) : x;

            for (int t = 0; t < text.Length && t < Width; t++)
            {
                Set(startX + t, y, new Pixel
                {
                    Character = text[t],
                    Foreground = foreground ?? DefaultForegroundColor,
                    Background = background ?? DefaultBackgroundColor
                });
            }

            return this;
        }

        public Pixel Get(int x, int y, bool backBuffer = true)
        {
            if ( x < 0 || y < 0 || x >= Width || y >= Height)
            {
                throw new IndexOutOfRangeException($"The coordinates {x},{y} need to be positive and less than {Width} and {Height}");
            }

            return backBuffer ? _previous[y][x] : _pixels[y][x];
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