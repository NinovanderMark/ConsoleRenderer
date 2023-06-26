namespace ConsoleRenderer.Examples
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            long count = 0;

            ConsoleColor foreground = Console.ForegroundColor;
            ConsoleColor background = Console.BackgroundColor;

            // Having the cursor visible causes some unsightly artifacts, which we may want to get rid of
            Console.CursorVisible = false;

            // Get the program we want to run
            var program = new ExampleSelector().GetProgramDefinition(args);

            while (true)
            {
                if (Console.KeyAvailable)
                    break;

                program.Tick.Invoke();
                count++;
            }

            // Reset the Console fields to what they were prior
            Console.CursorVisible = true;
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            timer.Stop();
            Console.WriteLine();
            Console.WriteLine($"Rendered {count} times in {timer.ElapsedMilliseconds}ms ({count / (timer.ElapsedMilliseconds / 1000f):0.00} fps)");

            return 0;
        }
    }
}