using echogame.Models;
using echogame.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public class Level
    {        
        public int Number { get; private set; }
        public LevelModel Model { get; private set; }
        public LevelView View { get; private set; }

        public Level(LevelModel model, LevelView view, int num)
        {
            Model = model;
            View = view;
            Number = num;
        }
    }
}
