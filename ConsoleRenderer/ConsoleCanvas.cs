using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleRenderer
{
    public class ConsoleCanvas
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
        private static Stream? _outputStream;
        private ArrayBufferWriter<byte>? _frameBuffer;
        private static bool _ansiInitialized;

        public ConsoleCanvas(int width, int height, bool interlaced = false, bool autoResize = false)
        {
            Width = width;
            Height = height;
            Interlaced = interlaced;
            AutoResize = autoResize;

            DefaultForegroundColor = Console.ForegroundColor;
            DefaultBackgroundColor = Console.BackgroundColor;
            
            // In Linux Console.ForegroundColor is sometimes not defined (-1).
            // In that case set it manually.
            if ((int)DefaultForegroundColor == -1) DefaultForegroundColor = ConsoleColor.Gray;
            if ((int)DefaultBackgroundColor == -1) DefaultBackgroundColor = ConsoleColor.Black;

            _pixels = new List<List<Pixel>>();
            _previous = new List<List<Pixel>>();

            Resize(width, height);
        }

        public ConsoleCanvas(bool interlaced = false, bool autoResize = false) 
            : this(Console.WindowWidth, Console.WindowHeight, interlaced, autoResize)
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
        public ConsoleCanvas CreateBorder(char? character = null)
        {
            return CreateBorder(character, DefaultForegroundColor, DefaultBackgroundColor);
        }

        /// <summary>
        /// Creates a border on the edges of the canvas with the specified character and colors
        /// </summary>
        /// <param name="character">Character to draw the border with, or <see cref="null"/> to use default pretty borders</param>
        /// <param name="foreground">Color to draw the border with</param>
        /// <param name="background">Color to draw the border with</param>
        public ConsoleCanvas CreateBorder(char? character, ConsoleColor foreground, ConsoleColor background)
        {
            return CreateBorder(0, 0, Width, Height, character, foreground, background);
        }

        /// <summary>
        /// Creates a border on the edges of the canvas with the specified character and the default fore- and background colors
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to draw the border with, or <see cref="null"/> to use default pretty borders</param>
        public ConsoleCanvas CreateBorder(int startX, int startY, int width, int height, char? character = null)
        {
            return CreateBorder(startX, startY, width, height, character, DefaultForegroundColor, DefaultBackgroundColor);
        }

        /// <summary>
        /// Creates a border on the edges of a rectangle with the specified character and colors
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to draw the border with, or <see cref="null"/> to use default pretty borders</param>
        /// <param name="foreground">Color to draw the border with</param>
        /// <param name="background">Color to draw the border with</param>
        /// <returns></returns>
        public ConsoleCanvas CreateBorder(int startX, int startY, int width, int height, char? character, ConsoleColor foreground, ConsoleColor background)
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
            EnsureStreamInitialized();

            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            if (_previousWidth != windowWidth || _previousHeight != windowHeight)
            {
                if (AutoResize)
                {
                    Resize(windowWidth, windowHeight);
                }

                ClearPixelCache();

                _previousWidth = windowWidth;
                _previousHeight = windowHeight;
            }

            int effectiveWidth = Math.Min(Width, windowWidth);
            int effectiveHeight = Math.Min(Height, windowHeight);

            ArrayBufferWriter<byte> buffer = _frameBuffer!;
            buffer.Clear();

            // Cursor to top-left (1-based in ANSI)
            buffer.Write("\x1b[1;1H"u8);

            int lastFg = -1;
            int lastBg = -1;

            for (int y = 0; y < effectiveHeight; y++)
            {
                bool skipRow = Interlaced && ((_oddRows && y % 2 == 0) || (!_oddRows && y % 2 != 0));
                List<Pixel> sourceRow = skipRow ? _previous[y] : _pixels[y];

                for (int x = 0; x < effectiveWidth; x++)
                {
                    Pixel p = sourceRow[x];
                    int fg = AnsiForeground[(int)p.Foreground];
                    int bg = AnsiBackground[(int)p.Background];

                    if (fg != lastFg || bg != lastBg)
                    {
                        WriteSgr(buffer, fg);
                        WriteSgr(buffer, bg);
                        lastFg = fg;
                        lastBg = bg;
                    }

                    WriteUtf8Char(buffer, p.Character);
                }

                // Newline between rows only; skipping after last row prevents terminal scroll (first line cut off)
                if (y < effectiveHeight - 1)
                    buffer.Write("\n"u8);
                lastFg = -1;
                lastBg = -1;

                if (!skipRow)
                {
                    for (int x = 0; x < Width; x++)
                        _previous[y][x] = sourceRow[x];
                }
            }

            buffer.Write("\x1b[1;1H"u8);

            _outputStream!.Write(buffer.WrittenSpan);
            _outputStream.Flush();

            _oddRows = !_oddRows;
            return this;
        }

        /// <summary>
        /// Resizes the canvas to match the new dimensions
        /// </summary>
        /// <param name="width">The new <see cref="Width"/> of the <see cref="ConsoleCanvas"/></param>
        /// <param name="height">The new <see cref="Height"/> of the <see cref="ConsoleCanvas"/></param>
        /// <returns></returns>
        public ConsoleCanvas Resize(int width, int height)
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

        /// <summary>
        /// Draws a series of <see cref="Pixel"/>s to the screen, starting at the specified <paramref name="x"/> and <paramref name="y"/>
        /// coordinates
        /// </summary>
        /// <param name="x">Starting x coordinate of the pixels, each consecutive one will be drawn to the right of the last</param>
        /// <param name="y">Y Coordinate of the pixels</param>
        /// <param name="pixels">Pixels to set at the specified coordinates</param>
        public ConsoleCanvas Set(int x, int y, Pixel[] pixels)
        {
            for (int t = 0; t < pixels.Length; t++)
            {
                Set(x+t, y, pixels[t]);
            }

            return this;
        }

        /// <summary>
        /// Draws a series of <see cref="Pixel"/>s to the screen, starting at the specified <paramref name="x"/> and <paramref name="y"/>
        /// coordinates
        /// </summary>
        /// <param name="x">Starting x coordinate of the pixels, each consecutive one will be drawn to the right of the last</param>
        /// <param name="y">Y Coordinate of the pixels</param>
        /// <param name="pixels">Pixels to set at the specified coordinates</param>
        public ConsoleCanvas Set(int x, int y, List<Pixel> pixels)
        {
            for (int t = 0; t < pixels.Count; t++)
            {
                Set(x + t, y, pixels[t]);
            }

            return this;
        }

        /// <summary>
        /// Draws the given <paramref name="text"/> to the canvas, starting at the <paramref name="x"/> and <paramref name="y"/> coordinates
        /// </summary>
        /// <param name="x">X Coordinate of the first character of the string</param>
        /// <param name="y">Y Coordinate of the string</param>
        /// <param name="text">The text to draw</param>
        /// <param name="centered">Whether the text should be centered around the <paramref name="x"/> coordinate</param>
        /// <param name="foreground">Foreground color to draw the string with, or <see cref="DefaultForegroundColor"/> if <see cref="null"/></param>
        /// <param name="background">Background color to draw the string with, or <see cref="DefaultBackgroundColor"/> if <see cref="null"/></param>
        public ConsoleCanvas Text(int x, int y, string text, bool centered = false, ConsoleColor? foreground = null, ConsoleColor? background = null)
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

        /// <summary>
        /// Returns the <see cref="Pixel"/> at the given <paramref name="x"/>,<paramref name="y"/> coordinates
        /// </summary>
        /// <param name="backBuffer">Whether to return the pixel as it was last drawn (<see cref="true"/>), or the
        /// one that will be drawn at the next call to <see cref="Render"/></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
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

        private void EnsureStreamInitialized()
        {
            if (_outputStream != null && _frameBuffer != null)
                return;

            lock (typeof(ConsoleCanvas))
            {
                if (!_ansiInitialized)
                {
                    // Windows needs VT mode for ANSI; Linux and macOS terminals already support it
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        WindowsAnsiHelper.EnableVirtualTerminalProcessing();
                    Console.OutputEncoding = Encoding.UTF8;
                    _ansiInitialized = true;
                }

                if (_outputStream == null)
                    _outputStream = Console.OpenStandardOutput();

                if (_frameBuffer == null)
                    _frameBuffer = new ArrayBufferWriter<byte>();
            }
        }

        // ANSI SGR codes for ConsoleColor (index = (int)ConsoleColor): 30-37, 90-97 fg; 40-47, 100-107 bg
        private static readonly int[] AnsiForeground = { 30, 34, 32, 36, 31, 35, 33, 37, 90, 94, 92, 96, 91, 95, 93, 97 };
        private static readonly int[] AnsiBackground = { 40, 44, 42, 46, 41, 45, 43, 47, 100, 104, 102, 106, 101, 105, 103, 107 };

        private static void WriteSgr(ArrayBufferWriter<byte> buffer, int code)
        {
            buffer.Write("\x1b["u8);
            if (code >= 100)
            {
                buffer.Write("1"u8);
                buffer.Write(stackalloc byte[] { (byte)('0' + (code / 10 % 10)), (byte)('0' + (code % 10)) });
            }
            else if (code >= 10)
            {
                buffer.Write(stackalloc byte[] { (byte)('0' + (code / 10)), (byte)('0' + (code % 10)) });
            }
            else
            {
                buffer.Write(stackalloc byte[] { (byte)('0' + code) });
            }
            buffer.Write("m"u8);
        }

        private static void WriteSgrReset(ArrayBufferWriter<byte> buffer)
        {
            buffer.Write("\x1b[0m"u8);
        }

        private static void WriteUtf8Char(ArrayBufferWriter<byte> buffer, char c)
        {
            Span<char> cSpan = stackalloc char[1] { c };
            Span<byte> dest = buffer.GetSpan(4);
            int written = Encoding.UTF8.GetBytes(cSpan, dest);
            buffer.Advance(written);
        }

        private static class WindowsAnsiHelper
        {
            private const int StdOutputHandle = -11;
            private const uint VirtualTerminalProcessingFlag = 0x0004;

            public static void EnableVirtualTerminalProcessing()
            {
                var handle = GetStdHandle(StdOutputHandle);
                if (handle != IntPtr.Zero && GetConsoleMode(handle, out uint mode))
                {
                    SetConsoleMode(handle, mode | VirtualTerminalProcessingFlag);
                }
            }

            [DllImport("kernel32")]
            private static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32")]
            private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32")]
            private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        }
    }
}