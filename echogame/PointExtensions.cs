using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public static class PointExtensions
    {
        public static double DistTo(this PointF point, PointF another)
        {
            return Math.Sqrt(
                Math.Pow(point.X - another.X, 2) +
                Math.Pow(point.Y - another.Y, 2)
                );
        }
    }
}
