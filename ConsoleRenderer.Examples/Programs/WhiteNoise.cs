using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class WhiteNoise
    {
        private readonly ConsoleCanvas _canvas;

        internal WhiteNoise()
        {
            _canvas = new ConsoleCanvas();
        }

        public void Tick()
        {
            for (int y = 0; y < _canvas.Height; y++)
            {
                for (int x = 0; x < _canvas.Width; x++)
                {
                    ConsoleColor color = Random.Shared.NextSingle() < 0.5 ? ConsoleColor.White : ConsoleColor.Black;
                    _canvas.Set(x, y, ' ', color, color);
                }
            }

            _canvas.Render();
        }
    }
}
