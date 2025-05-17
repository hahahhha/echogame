using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using echogame.Models;
using echogame.Views;

namespace echogame
{
    public class Lighter
    {
        public LighterModel Model { get; private set; }
        public LighterView View { get; private set; }
        public Lighter(LighterModel model, LighterView view)
        {
            Model = model;
            View = view;
        }
    }
}