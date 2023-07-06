using ConsoleRenderer.Examples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class Horizontal : BaseExampleProgram
    {
        private int _x;

        public override void Tick()
        {
            Canvas
                .Clear()
                .CreateRectangle(_x, 0, 7, Canvas.Height, ' ', ConsoleColor.White, ConsoleColor.Black)
                .CreateRectangle(_x + 1, 0, 5, Canvas.Height, ' ', ConsoleColor.White, ConsoleColor.DarkGray)
                .CreateRectangle(_x + 2, 0, 3, Canvas.Height, ' ', ConsoleColor.White, ConsoleColor.Gray)
                .CreateRectangle(_x + 3, 0, 1, Canvas.Height, ' ', ConsoleColor.White, ConsoleColor.White)
                .Render();

            _x++;
            if ( _x-3 >= Canvas.Width )
                _x = -3;
        }
    }
}
