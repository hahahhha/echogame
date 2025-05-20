using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using echogame.Models;
using echogame.Views;
namespace echogame
{
    public class LightersManager
    {
        private List<Lighter> lighters = new List<Lighter>();
        private int lightRadius = 75;
        private readonly string pathToTexture = "../../Textures/flashlight.png";

        public LightersManager()
        {
        }

        public void CreateLighter(Point position)
        {
            // по хорошему сайз бы поменять на размер клетки.....
            var model = new LighterModel(lightRadius, position, new Size(50, 50));
            var view = new LighterView(model, pathToTexture);
            var lighter = new Lighter(model, view);
            lighters.Add(lighter);
        }

        public void DrawAllLighters(Graphics g)
        {
            foreach (var lighter in lighters.Where(l => l.Model.IsAlive))
            {
                lighter.View.Draw(g);
            }
        }

        public void RemoveByPoint(Point pos)
        {
            var lighterToRemove = lighters.Where(l => l.Model.Position == pos).FirstOrDefault();
            if (lighterToRemove == null) throw new ArgumentException("Не существует фонаря по заданной точке");
            lighters.Remove(lighterToRemove);
        }

        public void RemoveLighter(Lighter lighter)
        {
            lighters.Remove(lighter);
        }

        public List<Lighter> GetActiveLighters()
        {
            return lighters.Where(l => l.Model.IsAlive).ToList();
        }
    }
}
