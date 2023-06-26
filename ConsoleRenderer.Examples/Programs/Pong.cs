using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    public class Pong
    {
        private readonly ConsoleCanvas _canvas;
        private readonly double _framerate;

        private int _x;
        private int _y;
        private int _xVel;
        private int _yVel;

        DateTime _previousFrame;

        public Pong(int framesPerSecond)
        {
            _canvas = new ConsoleCanvas();
            _xVel = 1;
            _yVel = 1;

            _previousFrame = DateTime.Now;
            _framerate = 1000d / framesPerSecond;
        }

        public void Tick()
        {
            _canvas.Clear();
            _canvas.CreateBorder();

            var currentTime = DateTime.Now;
            if ((currentTime - _previousFrame).TotalMilliseconds >= _framerate)
            {
                _x += _xVel;
                _y += _yVel;
                if (_x < 1 || _x + 1 >= _canvas.Width)
                {
                    _xVel = 0 - _xVel;
                    _x += _xVel * 2;
                }

                if (_y < 1 || _y + 1 >= _canvas.Height)
                {
                    _yVel = 0 - _yVel;
                    _y += _yVel * 2;
                }

                _previousFrame = currentTime;
            }

            _canvas.Set(_x, _y, ConsoleColor.Blue);
            _canvas.Render();
        }
    }
}
