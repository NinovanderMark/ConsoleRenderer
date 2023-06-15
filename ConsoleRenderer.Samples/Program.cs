namespace ConsoleRenderer.Samples
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1 || args[0] == "pong")
            {
                int fps = 60;
                if (args.Length > 1)
                    fps = int.Parse(args[1]);

                Pong(fps);
                return 0;
            }

            Console.WriteLine($"Unknown sample provided {args[0]}");
            return 0;
        }

        static void Pong(int fps)
        {
            var pong = new Pong(fps);
            var timer = System.Diagnostics.Stopwatch.StartNew();
            long count = 0;

            ConsoleColor foreground = Console.ForegroundColor;
            ConsoleColor background = Console.BackgroundColor;

            // Having the cursor visible causes some unsightly artifacts, which we may want to get rid of
            Console.CursorVisible = false;

            while (true)
            {
                if (Console.KeyAvailable)
                    break;

                pong.Tick();
                count++;
            }

            // Reset the Console fields to what they were prior
            Console.CursorVisible = true;
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            timer.Stop();
            Console.WriteLine();
            Console.WriteLine($"Rendered {count} times in {timer.ElapsedMilliseconds}ms ({count / (timer.ElapsedMilliseconds / 1000f):0.00} fps)");
        }
    }
}