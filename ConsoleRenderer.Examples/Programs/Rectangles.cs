using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class Rectangles
    {
        private readonly ConsoleCanvas _canvas;

        internal Rectangles()
        {
            _canvas = new ConsoleCanvas();
        }

        public void Tick()
        {
            int left = Random.Shared.Next(0, _canvas.Width - 1);
            int top = Random.Shared.Next(0, _canvas.Height - 1);
            int width = Random.Shared.Next(1, _canvas.Width / 2);
            int height = Random.Shared.Next(1, _canvas.Height / 2);

            ConsoleColor foreground = (ConsoleColor) Random.Shared.Next(0, 16);
            ConsoleColor background = (ConsoleColor) Random.Shared.Next(0, 16);

            _canvas.CreateRectangle(left, top, width, height, ' ', foreground, background);
            _canvas.Render();
        }
    }
}
