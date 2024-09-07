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
        /// Clears the canvas of all characters, using the default fore- and background colors
        /// </summary>
        /// <returns>A reference to this instance of <see cref="ICanvas"/></returns>
        ICanvas Clear();
    }
}
