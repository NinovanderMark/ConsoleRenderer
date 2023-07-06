using ConsoleRenderer.Examples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class WhiteNoise : BaseExampleProgram
    {
        public override void Tick()
        {
            for (int y = 0; y < Canvas.Height; y++)
            {
                for (int x = 0; x < Canvas.Width; x++)
                {
                    ConsoleColor color = Random.Shared.NextSingle() < 0.5 ? ConsoleColor.White : ConsoleColor.Black;
                    Canvas.Set(x, y, ' ', color, color);
                }
            }

            Canvas.Render();
        }
    }
}
