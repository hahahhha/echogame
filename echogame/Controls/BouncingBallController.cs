using echogame.Models;
using echogame.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Controls
{
    public class BouncingBallController
    {
        private readonly BouncingBallModel model;

        public BouncingBallController(BouncingBallModel model)
        {
            this.model = model;
        }

        public void Update(float deltaTime)
        {
            model.Update(deltaTime);
        }
    }
}
