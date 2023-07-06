namespace ConsoleRenderer.Examples.Base
{
    internal abstract class BaseExampleProgram
    {
        protected ConsoleCanvas Canvas { get; set; }

        protected BaseExampleProgram() : this(new ConsoleCanvas())
        {
        }

        protected BaseExampleProgram(ConsoleCanvas canvas)
        {
            Canvas = canvas;
        }

        public abstract void Tick();

        public void SetInterlaced(bool interlaced)
        {
            Canvas.Interlaced = interlaced;
        }
    }
}
