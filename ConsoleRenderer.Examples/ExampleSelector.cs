using ConsoleRenderer.Examples.Base;
using ConsoleRenderer.Examples.Programs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleRenderer.Examples
{
    public class ExampleSelector
    {
        private readonly Dictionary<string, BaseExampleProgram> _definitions = new()
        {
            { "pong", new Pong(60) },
            { "rectangles", new Rectangles() },
            { "whitenoise", new WhiteNoise() },
            { "colornoise", new ColorNoise() },
            { "horizontal", new Horizontal() },
            { "vertical", new Vertical() }
        };

        internal BaseExampleProgram? GetExampleProgram(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the name of an example you want to start, and optionally provide 'interlaced' as second argument to run in interlaced mode");
                Console.WriteLine("Available examples: " + string.Join(", ", _definitions.Keys));
                return null;
            }

            var program = GetProgram(args[0]);
            if (program == null)
                return null;

            if ( args.Any(a => a.ToLower() == "interlaced"))
                program.SetInterlaced(true);

            return program;            
        }

        private BaseExampleProgram? GetProgram(string programName)
        {
            if (_definitions.TryGetValue(programName, out var programDefinition))
                return programDefinition;

            Console.WriteLine($"Unable to find example with name '{programName}'");
            Console.WriteLine("Available examples: " + string.Join(", ", _definitions.Keys));
            return null;
        }
    }
}
