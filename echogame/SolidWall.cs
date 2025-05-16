using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public class SolidWall
    {
        public Size Size { get; private set; }
        public Point Position { get; private set; }

        public SolidWall(Size size, Point position)
        {
            Size = size;
            Position = position;
        }
    }
}
