using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    public class Horizontal
    {
        private readonly ConsoleCanvas _canvas;
        private int _x;

        public Horizontal()
        {
            _canvas = new ConsoleCanvas();
        }

        public void Tick()
        {
            _canvas.CreateRectangle(_x, 0, 7, _canvas.Height, ' ', ConsoleColor.White, ConsoleColor.Black)
                .CreateRectangle(_x + 1, 0, 5, _canvas.Height, ' ', ConsoleColor.White, ConsoleColor.DarkGray)
                .CreateRectangle(_x + 2, 0, 3, _canvas.Height, ' ', ConsoleColor.White, ConsoleColor.Gray)
                .CreateRectangle(_x + 3, 0, 1, _canvas.Height, ' ', ConsoleColor.White, ConsoleColor.White)
                .Render();

            _x++;
            if ( _x-3 >= _canvas.Width )
                _x = -3;
        }
    }
}
