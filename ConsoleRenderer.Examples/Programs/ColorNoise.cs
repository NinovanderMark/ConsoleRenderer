using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class ColorNoise
    {
        private readonly ConsoleCanvas _canvas;

        internal ColorNoise()
        {
            _canvas = new ConsoleCanvas();
        }

        public void Tick()
        {
            for (int y = 0; y < _canvas.Height; y++)
            {
                for (int x = 0; x < _canvas.Width; x++)
                {
                    ConsoleColor color = (ConsoleColor)Random.Shared.Next(16);
                    _canvas.Set(x, y, ' ', color, color);
                }
            }

            _canvas.Render();
        }
    }
}
