using ConsoleRenderer.Examples.Programs;

namespace ConsoleRenderer.Examples
{
    public class ExampleSelector
    {
        private readonly Dictionary<string, ProgramDefinition> _definitions = new Dictionary<string, ProgramDefinition>();

        public ExampleSelector()
        {
            var pong = new Pong(60);
            _definitions.Add("pong", new ProgramDefinition(pong, pong.Tick));

            var rectangles = new Rectangles();
            _definitions.Add("rectangles", new ProgramDefinition(rectangles, rectangles.Tick));

            var whiteNoise = new WhiteNoise();
            _definitions.Add("whitenoise", new ProgramDefinition(whiteNoise, whiteNoise.Tick));

            var colorNoise = new ColorNoise();
            _definitions.Add("colornoise", new ProgramDefinition(colorNoise, colorNoise.Tick));
        }

        internal ProgramDefinition GetProgramDefinition(string[] args)
        {
            if (args.Length == 0)
                return _definitions["pong"];

            if (_definitions.TryGetValue(args[0], out var programDefinition))
                return programDefinition;

            return _definitions["pong"];
        }
    }
}
