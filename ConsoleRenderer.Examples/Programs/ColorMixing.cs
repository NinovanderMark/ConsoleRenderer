using ConsoleRenderer.Examples.Base;
using System;

namespace ConsoleRenderer.Examples.Programs
{
    internal class ColorMixing : BaseExampleProgram
    {
        public override void Tick()
        {
            for (int col = 0; col < 16; col++)
            {
                Canvas.Set(0, col, '█', (ConsoleColor)col, ConsoleColor.Black);
            }

            for (int fg = 0; fg < 16; fg++)
            {
                for (int bg = 0; bg < 16; bg++)
                {
                    Canvas.Set(1+bg, fg, '▓', (ConsoleColor)fg, (ConsoleColor)bg);
                }
            }

            for (int fg = 0; fg < 16; fg++)
            {
                for (int bg = 0; bg < 16; bg++)
                {
                    Canvas.Set(17 + bg, fg, '▒', (ConsoleColor)fg, (ConsoleColor)bg);
                }
            }

            for (int fg = 0; fg < 16; fg++)
            {
                for (int bg = 0; bg < 16; bg++)
                {
                    Canvas.Set(33 + bg, fg, '░', (ConsoleColor)fg, (ConsoleColor)bg);
                }
            }

            Canvas.Render();
        }
    }
}
