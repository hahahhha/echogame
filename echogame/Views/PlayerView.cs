using echogame.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Views
{
    public class PlayerView
    {
        // view знает только model
        private readonly PlayerModel playerModel;
        private Image playerTexture;

        public PlayerView(PlayerModel model)
        {
            playerModel = model;
            playerModel.PositionChanged += OnPositionChanged;
            // playerModel.PlayerDied += OnPlayerDied;

            playerTexture = Image.FromFile(@"../../Textures/player_low.png");
        }

        void OnPositionChanged()
        {

        }

        void OnPlayerDied()
        {

        }

        public void Draw(Graphics g)
        {
            g.DrawImage(playerTexture, 
                playerModel.Position.X,
                playerModel.Position.Y,
                playerModel.PlayerSize.Width, 
                playerModel.PlayerSize.Height);
        }
    }
}
