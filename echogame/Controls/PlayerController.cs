using echogame.Models;
using echogame.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace echogame.Controls
{
    public class PlayerController
    {
        // control знает про model и view
        private readonly PlayerModel playerModel;
        private readonly PlayerView playerView;

        public PlayerController(PlayerModel playerModel, PlayerView playerView)
        {
            this.playerModel = playerModel;
            this.playerView = playerView;
        }

        public void HandleInput(Keys pressedKey)
        {
            var deltaX = 0;
            var deltaY = 0;

            switch (pressedKey)
            {
                case Keys.W:
                    deltaY = -playerModel.SpeedY; 
                    break;
                case Keys.A:
                    deltaX = -playerModel.SpeedX; 
                    break;
                case Keys.S:
                    deltaY = playerModel.SpeedX; 
                    break;
                case Keys.D:
                    deltaX = playerModel.SpeedX; 
                    break;
            }
            if (playerModel.IsStepAvialable(new Point(
                playerModel.Position.X + deltaX, playerModel.Position.Y + deltaY)))
            {
                playerModel.Move(deltaX, deltaY);
            }
        }
    }
}
