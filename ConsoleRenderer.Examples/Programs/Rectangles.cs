using ConsoleRenderer.Examples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class Rectangles : BaseExampleProgram
    {
        public override void Tick()
        {
            int left = Random.Shared.Next(0, Canvas.Width - 1);
            int top = Random.Shared.Next(0, Canvas.Height - 1);
            int width = Random.Shared.Next(1, Canvas.Width / 2);
            int height = Random.Shared.Next(1, Canvas.Height / 2);

            ConsoleColor foreground = (ConsoleColor) Random.Shared.Next(0, 16);
            ConsoleColor background = (ConsoleColor) Random.Shared.Next(0, 16);

            Canvas.CreateRectangle(left, top, width, height, ' ', foreground, background);
            Canvas.Render();
        }
    }
}
