using ConsoleRenderer.Examples.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples
{
    internal static class ExampleSelector
    {
        internal static ProgramDefinition GetProgramDefinition(string[] args)
        {
            if (args.Length == 0)
                return PongDefinition();

            switch (args[0].ToLower())
            {
                case "rectangles":
                    return RectanglesDefinition();

                case "noise":
                    return WhiteNoiseDefinition();

                default:
                    return PongDefinition();
            }
        }

        internal static ProgramDefinition PongDefinition()
        {
            var pong = new Pong(60);
            return new ProgramDefinition(pong, pong.Tick);
        }

        internal static ProgramDefinition RectanglesDefinition()
        {
            var rectangle = new Rectangles();
            return new ProgramDefinition(rectangle, rectangle.Tick);
        }

        internal static ProgramDefinition WhiteNoiseDefinition()
        {
            var noise = new WhiteNoise();
            return new ProgramDefinition(noise, noise.Tick);
        }
    }
}
