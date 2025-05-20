using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace echogame.Models
{
    public class LighterModel
    {
        public Size Size { get; private set; }
        public int LightRadius { get; private set; }
        public Point Position { get; private set; }
        public Point Center => new Point(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
        public Rectangle Collider { get; private set; }
        public bool IsAlive { get; private set; }

        public LighterModel(int radius, Point position, Size size)
        {
            LightRadius = radius;
            Position = position;
            IsAlive = true;
            Size = size;
            Collider = new Rectangle(position, size);
        }

        public void Die()
        {
            IsAlive = false;
        }

        public void MakeUnactive()
        {
            IsAlive = false;
        }
    }
}
