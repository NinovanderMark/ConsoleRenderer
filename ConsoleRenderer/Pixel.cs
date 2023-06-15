using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    public struct Pixel
    {
        public char Character;
        public ConsoleColor Foreground;
        public ConsoleColor Background;

        public static bool operator ==(Pixel p1, Pixel p2)
        {
            return p1.Character == p2.Character && 
                p1.Foreground == p2.Foreground &&
                p1.Background == p2.Background;
        }

        public static bool operator !=(Pixel p1, Pixel p2)
        {
            return p1.Character != p2.Character || 
                p1.Foreground != p2.Foreground ||
                p1.Background != p2.Background;
        }

        public override bool Equals(object? obj)
        {
            if ( obj == null) 
                return false;

            if (obj is Pixel pixel)
                return pixel == this;
            
            return false;
        }
    }
}
