using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Models
{
    public class DarknessModel
    {
        public PointF Position { get; private set; }

        private readonly PlayerModel playerModel;

        public DarknessModel(PlayerModel playerModel)
        {
            this.playerModel = playerModel;
            OnPlayerPositionChanged();
            playerModel.PositionChanged += OnPlayerPositionChanged;
            this.playerModel = playerModel;
        }

        private void OnPlayerPositionChanged()
        {
            Position = playerModel.Center;
            Console.WriteLine("pos changed");
        }
    }
}
