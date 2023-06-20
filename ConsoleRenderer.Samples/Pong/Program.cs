namespace ConsoleRenderer.Samples
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var pong = new Pong(60);
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
            
            return 0;
        }
    }
}