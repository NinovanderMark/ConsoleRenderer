using ConsoleRenderer.Examples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class ColorNoise : BaseExampleProgram
    {
        public override void Tick()
        {
            for (int y = 0; y < Canvas.Height; y++)
            {
                for (int x = 0; x < Canvas.Width; x++)
                {
                    ConsoleColor color = (ConsoleColor)Random.Shared.Next(16);
                    Canvas.Set(x, y, ' ', color, color);
                }
            }

            Canvas.Render();
        }
    }
}
