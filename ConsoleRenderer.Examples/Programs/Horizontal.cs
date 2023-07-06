using ConsoleRenderer.Examples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class Vertical : BaseExampleProgram
    {
        private int _y;

        public override void Tick()
        {
            Canvas
                .CreateRectangle(0, _y, Canvas.Width, 7, ' ', ConsoleColor.White, ConsoleColor.Black)
                .CreateRectangle(0, _y + 1, Canvas.Width, 5, ' ', ConsoleColor.White, ConsoleColor.DarkGray)
                .CreateRectangle(0, _y + 2, Canvas.Width, 3, ' ', ConsoleColor.White, ConsoleColor.Gray)
                .CreateRectangle(0, _y + 3, Canvas.Width, 1, ' ', ConsoleColor.White, ConsoleColor.White)
                .Render();

            _y++;
            if ( _y-3 >= Canvas.Width )
                _y = -3;
        }
    }
}
