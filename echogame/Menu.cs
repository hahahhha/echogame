using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using echogame.Views;
using echogame.Models;

namespace echogame
{
    public class Menu
    {
        public MenuModel Model { get; private set; }
        public MenuView View { get; private set; }

        public Menu(MenuModel model, MenuView view)
        {
            Model = model;
            View = view;
        }
    }
}
