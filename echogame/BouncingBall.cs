using echogame.Controls;
using echogame.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Models
{
    public class BouncingBall
    {
        public readonly BouncingBallModel Model;
        public readonly BouncingBallView View;
        public readonly BouncingBallController Controller;

        public BouncingBall(BouncingBallModel model, BouncingBallView view, BouncingBallController controller)
        {
            this.Model = model;
            this.View = view;
            this.Controller = controller;
        }
    }
}
