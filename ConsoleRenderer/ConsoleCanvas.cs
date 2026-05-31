using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
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
        private Pixel[] _pixels;
        private Pixel[] _previous;

        // Cached foregrounds and backgrounds to reduce writing ansi codes for repeated colors.
        private int _lastFg;
        private int _lastBg;

        /// <summary>
        ///     Shared across all instances so that every canvas renders to the same console output.
        /// </summary>
        private Stream? _outputStream;

        private ArrayBufferWriter<byte>? _frameBuffer;

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

            _pixels = new Pixel[Width * Height];
            _previous = new Pixel[Width * Height];

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
            var pixel = new Pixel
            {
                Character = character,
                Foreground = foreground,
                Background = background
            };
            for (int i = 0; i < _pixels.Length; i++)
                _pixels[i] = pixel;

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
            return CreateBorder(startX, startY, width, height, character, DefaultForegroundColor,
                DefaultBackgroundColor);
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
        public ConsoleCanvas CreateBorder(int startX, int startY, int width, int height, char? character,
            ConsoleColor foreground, ConsoleColor background)
        {
            int endX = startX + width - 1;
            int endY = startY + height - 1;

            var pixel = new Pixel
            {
                Foreground = foreground,
                Background = background,
                Character = character ?? _emptyCharacter
            };

            for (int y = startY; y <= endY && y < Height; y++)
            {
                for (int x = startX; x <= endX && x < Width; x++)
                {
                    bool onTop = y == startY;
                    bool onBottom = y == endY;
                    bool onLeft = x == startX;
                    bool onRight = x == endX;
                    if (!onTop && !onBottom && !onLeft && !onRight)
                        continue;

                    pixel.Character = character ?? GetBorderChar(x, y, startX, startY, endX, endY);
                    _pixels[y * Width + x] = pixel;
                }
            }

            return this;
        }

        private static char GetBorderChar(int x, int y, int startX, int startY, int endX, int endY)
        {
            bool onTop = y == startY;
            bool onBottom = y == endY;
            bool onLeft = x == startX;
            bool onRight = x == endX;

            if (onTop)
            {
                if (onLeft) return '╔';
                if (onRight) return '╗';
                return '═';
            }

            if (onBottom)
            {
                if (onLeft) return '╚';
                if (onRight) return '╝';
                return '═';
            }

            if (onLeft || onRight) return '║';
            return _emptyCharacter;
        }

        /// <summary>
        /// Creates a rectangle on the canvas 
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to fill the rectangle with</param>
        public ConsoleCanvas CreateRectangle(int startX, int startY, int width, int height,
            char character = _defaultCharacter)
        {
            return CreateRectangle(startX, startY, width, height, character, DefaultForegroundColor,
                DefaultBackgroundColor);
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
        public ConsoleCanvas CreateRectangle(int startX, int startY, int width, int height, char character,
            ConsoleColor foreground, ConsoleColor background)
        {
            int yMin = Math.Max(0, startY);
            int yMax = Math.Min(Height, startY + height);
            int xMin = Math.Max(0, startX);
            int xMax = Math.Min(Width, startX + width);
            var pixel = new Pixel
            {
                Character = character,
                Foreground = foreground,
                Background = background
            };

            for (int y = yMin; y < yMax; y++)
            {
                int rowStart = y * Width;
                for (int x = xMin; x < xMax; x++)
                    _pixels[rowStart + x] = pixel;
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
            bool isSizeChanged = _previousWidth != windowWidth || _previousHeight != windowHeight;

            if (isSizeChanged)
            {
                if (AutoResize) Resize(windowWidth, windowHeight);

                ClearPixelCache();
                _previousWidth = windowWidth;
                _previousHeight = windowHeight;
            }

            int effectiveWidth = Math.Min(Width, windowWidth);
            int effectiveHeight = Math.Min(Height, windowHeight);

            ArrayBufferWriter<byte> buffer = _frameBuffer!;
            buffer.Clear();

            buffer.Write("\x1b[1;1H"u8);
            // We do not write 1;1 here. RenderCluster handles its own positioning.
            // Perform sparse update
            for (int y = 0; y < effectiveHeight; y++)
            {
                bool skipRow =
                    Interlaced && ((_oddRows && y % 2 == 0) || (!_oddRows && y % 2 != 0));
                if (skipRow) continue;

                for (int x = 0; x < effectiveWidth; x++)
                {
                    int idx = y * Width + x;
                    if (_pixels[idx] != _previous[idx])
                    {
                        // Found a 'dirty' pixel.
                        // Start a cluster: look ahead in this row to see whether we should over-draw
                        // or jump to the next cluster.
                        x = RenderCluster(effectiveWidth, x, y, buffer);
                    }
                }
            }

            _outputStream!.Write(buffer.WrittenSpan);
            _outputStream.Flush();
            // After rendering, sync the buffers so we know what to diff next time
            _pixels.CopyTo(_previous);

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
            _pixels = new Pixel[Width * Height];
            _previous = new Pixel[Width * Height];

            var defaultPixel = new Pixel
            {
                Background = DefaultBackgroundColor,
                Foreground = DefaultForegroundColor,
                Character = _emptyCharacter
            };

            Array.Fill(_pixels, defaultPixel);
            Array.Fill(_previous, defaultPixel);

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
                _pixels[y * Width + x] = pixel;

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
            if (y < 0 || y >= Height || pixels.Length == 0)
                return this;
            int start = Math.Max(0, -x);
            int count = Math.Min(pixels.Length - start, Width - (x + start));
            if (count <= 0)
                return this;
            int rowStart = y * Width;
            for (int i = 0; i < count; i++)
                _pixels[rowStart + x + start + i] = pixels[start + i];

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
            if (y < 0 || y >= Height || pixels.Count == 0)
                return this;
            int start = Math.Max(0, -x);
            int count = Math.Min(pixels.Count - start, Width - (x + start));
            if (count <= 0)
                return this;
            int rowStart = y * Width;
            for (int i = 0; i < count; i++)
                _pixels[rowStart + x + start + i] = pixels[start + i];

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
        public ConsoleCanvas Text(int x, int y, string text, bool centered = false, ConsoleColor? foreground = null,
            ConsoleColor? background = null)
        {
            // If the text should be centered, deduct half the text length from the x coordinate
            int startX = centered ? x - (int)Math.Floor(text.Length / 2d) : x;
            var fg = foreground ?? DefaultForegroundColor;
            var bg = background ?? DefaultBackgroundColor;

            if (y < 0 || y >= Height)
                return this;
            int rowStart = y * Width;
            var pixel = new Pixel { Foreground = fg, Background = bg };
            for (int t = 0; t < text.Length; t++)
            {
                int px = startX + t;
                if (px >= 0 && px < Width)
                {
                    pixel.Character = text[t];
                    _pixels[rowStart + px] = pixel;
                }
            }

            return this;
        }

        /// <summary>
        /// Returns the <see cref="Pixel"/> at the given <paramref name="x"/>,<paramref name="y"/> coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="backBuffer">Whether to return the pixel as it was last drawn (<see cref="true"/>), or the
        /// one that will be drawn at the next call to <see cref="Render"/></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Pixel Get(int x, int y, bool backBuffer = true)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                throw new IndexOutOfRangeException(
                    $"The coordinates {x},{y} need to be positive and less than {Width} and {Height}");
            }

            int index = y * Width + x;
            return backBuffer ? _previous[index] : _pixels[index];
        }

        private void ClearPixelCache()
        {
            var defaultPixel = new Pixel
            {
                Background = DefaultBackgroundColor,
                Foreground = DefaultForegroundColor,
                Character = _emptyCharacter
            };

            for (int i = 0; i < _previous.Length; i++)
                _previous[i] = defaultPixel;
        }

        private void EnsureStreamInitialized()
        {
            if (_outputStream != null && _frameBuffer != null)
                return;

            WindowsAnsi.EnsureAnsiSupport();

            if (_outputStream == null)
                _outputStream = Console.OpenStandardOutput();

            if (_frameBuffer == null)
                _frameBuffer = new ArrayBufferWriter<byte>();
        }

        /// <summary>
        /// Renders a cluster of changed pixels.
        /// If there is a gap of a clean pixels in-between dirty pixels, one of two options will happen:
        /// - re-render it if the gap is sufficiently small
        /// - finish the cluster, skip the gap and start a new cluster if the clean gap is too small.
        /// </summary>
        /// <param name="effectiveWidth">Width of the window to render.</param>
        /// <param name="startX">Current position in the row to render.</param>
        /// <param name="y">Row.</param>
        /// <param name="source">Source buffer to draw from.</param>
        /// <param name="buffer">Writer to stream the Ansi code bytes to.</param>
        /// <returns>New position of X after the cluster has been rendered.</returns>
        private int RenderCluster(
            int effectiveWidth,
            int startX,
            int y,
            in ArrayBufferWriter<byte> buffer)
        {
            int currentX = startX;
            int rowOffset = y * Width;
            _lastFg = -1;
            _lastBg = -1;

            // 1. Move Cursor (Note the +1 for 1-based terminal coords)
            MoveCursorAnsi(currentX, y, buffer);

            while (currentX < effectiveWidth)
            {
                int index = rowOffset + currentX;
                Pixel p = _pixels[index];
                WritePixelAnsi(p, buffer);

                currentX++;
                if (currentX >= effectiveWidth) continue;
                
                // 4. Look Ahead Logic
                int nextIndex = rowOffset + currentX;
                    
                if (_pixels[nextIndex] != _previous[nextIndex]) continue;
                // If the next pixel is clean (unchanged), then start counting the gap
                int gapSize = CountCleanGap(currentX, rowOffset, effectiveWidth);
                // If gap is too big, stop this cluster and jump later
                if (gapSize > 6) break;

                // Otherwise, the loop just continues naturally.
                // The next iteration will "overdraw" the clean pixels.
            }

            return currentX;
        }

        private static void MoveCursorAnsi(int x, int y, in ArrayBufferWriter<byte> buffer)
        {
            Span<byte> ansiBuffer = stackalloc byte[32];
            int written = Console.OutputEncoding.GetBytes($"\x1b[{y + 1};{x + 1}H", ansiBuffer);
            buffer.Write(ansiBuffer[..written]);
        }

        private void WritePixelAnsi(Pixel p, in ArrayBufferWriter<byte> buffer)
        {
            // 2. State-aware Color Update
            int fg = AnsiForeground[(int)p.Foreground];
            int bg = AnsiBackground[(int)p.Background];

            if (fg != _lastFg)
            {
                WriteSgr(buffer, fg);
                _lastFg = fg;
            }

            if (bg != _lastBg)
            {
                WriteSgr(buffer, bg);
                _lastBg = bg;
            }

            // 3. Write the character
            WriteEncodedChar(buffer, p.Character);
        }

        private int CountCleanGap(int currentX, int rowOffset, int effectiveWidth)
        {
            int gapSize = 0;
            for (int x = currentX; x < effectiveWidth; x++)
            {
                int index = rowOffset + x;
                if (_pixels[index] != _previous[index])
                {
                    return gapSize;
                }

                gapSize++;
            }

            return gapSize;
        }

        // ANSI SGR codes for ConsoleColor (index = (int)ConsoleColor): 30-37, 90-97 fg; 40-47, 100-107 bg
        private static readonly int[] AnsiForeground =
            { 30, 34, 32, 36, 31, 35, 33, 37, 90, 94, 92, 96, 91, 95, 93, 97 };

        private static readonly int[] AnsiBackground =
            { 40, 44, 42, 46, 41, 45, 43, 47, 100, 104, 102, 106, 101, 105, 103, 107 };

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

        private static void WriteEncodedChar(ArrayBufferWriter<byte> buffer, char c)
        {
            Span<char> cSpan = stackalloc char[1] { c };
            Span<byte> dest = buffer.GetSpan(4);
            int written = Console.OutputEncoding.GetBytes(cSpan, dest);
            buffer.Advance(written);
        }
    }
}