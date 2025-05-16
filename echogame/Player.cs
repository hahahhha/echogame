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
    public class Player
    {
        public PlayerModel Model { get; private set; }
        public PlayerView View { get; private set; }
        public PlayerController Controller { get; private set; }

        public Player(PlayerModel model, PlayerView view, PlayerController controller)
        {
            Model = model;
            View = view;
            Controller = controller;
        }
    }
}
