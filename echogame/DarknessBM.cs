using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using echogame.Models;
using echogame.Views;
using echogame.Controls;


namespace echogame
{
    public class DarknessBM
    {
        public DarknessBMModel Model { get; private set; }
        public DarknessBMView View { get; private set; }
        public DarknessBMController Controller { get; private set; }

        public DarknessBM(DarknessBMModel model, DarknessBMView view, DarknessBMController controller)
        {
            Model = model;
            View = view;
            Controller = controller;
        }
    }
}
