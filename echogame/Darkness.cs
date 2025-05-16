using echogame.Controls;
using echogame.Models;
using echogame.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public class Darkness
    {
        public DarknessModel Model { get; private set; }
        public DarknessView View { get; private set; }
        public DarknessController Controller { get; private set; }

        public Darkness(DarknessModel model, DarknessView view, DarknessController ctrl)
        {
            Model = model;
            View = view;
            Controller = ctrl;
        }

    }
}
