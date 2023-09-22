using ConsoleRenderer.Examples.Base;
using System;

namespace ConsoleRenderer.Examples.Programs
{
    internal class BouncingText : BaseExampleProgram
    {
        private readonly double _framerate;

        private int _x;
        private int _y;
        private int _xVel;
        private int _yVel;
        private string _text = "Hello Console";

        DateTime _previousFrame;

        public BouncingText(int framesPerSecond)
        {
            _xVel = 1;
            _yVel = 1;

            _previousFrame = DateTime.Now;
            _framerate = 1000d / framesPerSecond;
        }

        public override void Tick()
        {
            Canvas.Clear();
            Canvas.CreateBorder();

            var currentTime = DateTime.Now;
            if ((currentTime - _previousFrame).TotalMilliseconds >= _framerate)
            {
                _x += _xVel;
                _y += _yVel;
                if (_x < 1 || _x + 4 >= Canvas.Width)
                {
                    _xVel = 0 - _xVel;
                    _x += _xVel * 2;
                }

                if (_y < 1 || _y + 1 >= Canvas.Height)
                {
                    _yVel = 0 - _yVel;
                    _y += _yVel * 2;
                }

                _previousFrame = currentTime;
            }

            Canvas.Text(_x, _y, _text, true);
            Canvas.Render();
        }
    }
}
