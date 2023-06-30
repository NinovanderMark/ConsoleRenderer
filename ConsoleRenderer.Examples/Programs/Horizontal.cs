using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    public class Vertical
    {
        private readonly ConsoleCanvas _canvas;
        private int _y;

        public Vertical()
        {
            _canvas = new ConsoleCanvas();
        }

        public void Tick()
        {
            _canvas.CreateRectangle(0, _y, _canvas.Width, 7, ' ', ConsoleColor.White, ConsoleColor.Black)
                .CreateRectangle(0, _y + 1, _canvas.Width, 5, ' ', ConsoleColor.White, ConsoleColor.DarkGray)
                .CreateRectangle(0, _y + 2, _canvas.Width, 3, ' ', ConsoleColor.White, ConsoleColor.Gray)
                .CreateRectangle(0, _y + 3, _canvas.Width, 1, ' ', ConsoleColor.White, ConsoleColor.White)
                .Render();

            _y++;
            if ( _y-3 >= _canvas.Width )
                _y = -3;
        }
    }
}
