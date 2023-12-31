﻿using ConsoleRenderer.Examples.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.Examples.Programs
{
    internal class Pong : BaseExampleProgram
    {
        private readonly double _framerate;

        private int _x;
        private int _y;
        private int _xVel;
        private int _yVel;

        DateTime _previousFrame;

        public Pong(int framesPerSecond)
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

                if (_x < 1)
                {
                    _x = 1;
                    _xVel = 1;
                }
                else if (_x + 1 >= Canvas.Width)
                {
                    _x = Canvas.Width - 2;
                    _xVel = -1;
                }

                if (_y < 1)
                {
                    _y = 1;
                    _yVel = 1;
                }
                else if (_y + 1 >= Canvas.Height)
                {
                    _y = Canvas.Height - 2;
                    _yVel = -1;
                }

                _previousFrame = currentTime;
            }

            Canvas.Set(_x, _y, ConsoleColor.Blue);
            Canvas.Render();
        }
    }
}
