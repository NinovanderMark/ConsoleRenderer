using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    public interface ICanvas
    {
        /// <summary>
        /// Width of the canvas
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the canvas
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Foreground color to use if none was specified for operations updating the render buffer
        /// </summary>
        ConsoleColor DefaultForegroundColor { get; set; }

        /// <summary>
        /// Background color to use if none was specified for operations updating the render buffer
        /// </summary>
        ConsoleColor DefaultBackgroundColor { get; set; }

        /// <summary>
        /// Whether the <see cref="ICanvas"/> dimensions should automatically update to match the terminal's dimensions
        /// </summary>
        bool AutoResize { get; set; }

        /// <summary>
        /// Interlaced mode alternates between rendering only odd or even rows to the screen each time <see cref="Render"/> is called
        /// </summary>
        bool Interlaced { get; set; }

        /// <summary>
        /// Clears the canvas of all characters, using the default fore- and background colors
        /// </summary>
        /// <returns>A reference to this instance of <see cref="ICanvas"/></returns>
        ICanvas Clear();

        /// <summary>
        /// Creates a border on the edges of the canvas with the default fore- and background colors
        /// </summary>
        /// <param name="character">Character to draw the border with</param>
        ICanvas CreateBorder(char? character = null);

        /// <summary>
        /// Creates a border on the edges of the canvas with the specified character and colors
        /// </summary>
        /// <param name="character">Character to draw the border with, or <see cref="null"/> to use default pretty borders</param>
        /// <param name="foreground">Color to draw the border with</param>
        /// <param name="background">Color to draw the border with</param>
        ICanvas CreateBorder(char? character, ConsoleColor foreground, ConsoleColor background);

        /// <summary>
        /// Creates a border on the edges of the canvas with the specified character and the default fore- and background colors
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to draw the border with, or <see cref="null"/> to use default pretty borders</param>
        ICanvas CreateBorder(int startX, int startY, int width, int height, char? character = null);

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
        ICanvas CreateBorder(int startX, int startY, int width, int height, char? character, ConsoleColor foreground, ConsoleColor background);

        /// <summary>
        /// Creates a rectangle on the canvas 
        /// </summary>
        /// <param name="startX">Left edge of the rectangle</param>
        /// <param name="startY">Top edge of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="character">Character to fill the rectangle with</param>
        ICanvas CreateRectangle(int startX, int startY, int width, int height, char character = '*');

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
        ICanvas CreateRectangle(int startX, int startY, int width, int height, char character, ConsoleColor foreground, ConsoleColor background);

        /// <summary>
        /// Fills the canvas with the specified character in the given colors
        /// </summary>
        /// <param name="character">Character to fill the canvas with</param>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        /// <returns></returns>
        ICanvas Fill(char character, ConsoleColor foreground, ConsoleColor background);

        /// <summary>
        /// Returns the <see cref="Pixel"/> at the given <paramref name="x"/>,<paramref name="y"/> coordinates
        /// </summary>
        /// <param name="backBuffer">Whether to return the pixel as it was last drawn (<see cref="true"/>), or the
        /// one that will be drawn at the next call to <see cref="Render"/></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        Pixel Get(int x, int y, bool backBuffer = true);

        /// <summary>
        /// Resizes the canvas to match the new dimensions
        /// </summary>
        /// <param name="width">The new <see cref="Width"/> of the <see cref="ICanvas"/></param>
        /// <param name="height">The new <see cref="Height"/> of the <see cref="ICanvas"/></param>
        /// <returns>A reference to this instance of <see cref="ICanvas"/></returns>
        ICanvas Resize(int width, int height);

        /// <summary>
        /// Renders all the pixels on the canvas
        /// </summary>
        /// <returns>A reference to this instance of <see cref="ICanvas"/></returns>
        ICanvas Render();

        /// <summary>
        /// Set a particular pixel on the canvas to the specified character with the default fore- and background colors
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="character">Character to set the pixel to</param>
        ICanvas Set(int x, int y, char character = '*');

        /// <summary>
        /// Set a particular pixel on the canvas to the specified foreground color, with the default background color
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        ICanvas Set(int x, int y, ConsoleColor color);

        /// <summary>
        /// Set a particular pixel on the canvas to the specified character with a given color and the default background color
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="character">Character to set the pixel to</param>
        /// <param name="color">Color to draw the character with</param>
        ICanvas Set(int x, int y, char character, ConsoleColor color);

        /// <summary>
        /// Set a particular pixel on the canvas to the specified character with a given background and foreground color
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="character">Character to set the pixel to</param>
        /// <param name="foreground">Foreground color to draw the character with</param>
        /// <param name="background">Background color to draw the character with</param>
        ICanvas Set(int x, int y, char character, ConsoleColor foreground, ConsoleColor background);

        /// <summary>
        /// Set a particular pixel on the canvas to the specified <see cref="Pixel"/>
        /// </summary>
        /// <param name="x">X Coordinate of the pixel</param>
        /// <param name="y">Y Coordinate of the pixel</param>
        /// <param name="pixel">Pixel to set at the specified coordinates</param>
        ICanvas Set(int x, int y, Pixel pixel);

        /// <summary>
        /// Draws a series of <see cref="Pixel"/>s to the screen, starting at the specified <paramref name="x"/> and <paramref name="y"/>
        /// coordinates
        /// </summary>
        /// <param name="x">Starting x coordinate of the pixels, each consecutive one will be drawn to the right of the last</param>
        /// <param name="y">Y Coordinate of the pixels</param>
        /// <param name="pixels">Pixels to set at the specified coordinates</param>
        ICanvas Set(int x, int y, Pixel[] pixels);

        /// <summary>
        /// Draws a series of <see cref="Pixel"/>s to the screen, starting at the specified <paramref name="x"/> and <paramref name="y"/>
        /// coordinates
        /// </summary>
        /// <param name="x">Starting x coordinate of the pixels, each consecutive one will be drawn to the right of the last</param>
        /// <param name="y">Y Coordinate of the pixels</param>
        /// <param name="pixels">Pixels to set at the specified coordinates</param>
        ICanvas Set(int x, int y, List<Pixel> pixels);

        /// <summary>
        /// Draws the given <paramref name="text"/> to the canvas, starting at the <paramref name="x"/> and <paramref name="y"/> coordinates
        /// </summary>
        /// <param name="x">X Coordinate of the first character of the string</param>
        /// <param name="y">Y Coordinate of the string</param>
        /// <param name="text">The text to draw</param>
        /// <param name="centered">Whether the text should be centered around the <paramref name="x"/> coordinate</param>
        /// <param name="foreground">Foreground color to draw the string with, or <see cref="DefaultForegroundColor"/> if <see cref="null"/></param>
        /// <param name="background">Background color to draw the string with, or <see cref="DefaultBackgroundColor"/> if <see cref="null"/></param>
        ICanvas Text(int x, int y, string text, bool centered = false, ConsoleColor? foreground = null, ConsoleColor? background = null);
    }
}
