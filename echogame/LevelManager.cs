using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public class LevelManager
    {
        public Level CurrentLevel { get; private set; }
        private List<Level> levels;
        private Dictionary<int, Level> levelsByNumber;

        // num = 0 - для глобал левела
        public LevelManager(List<Level> levels)
        {
            levelsByNumber = levels.ToDictionary(l => l.Number, l => l);
            this.levels = levels;
            CurrentLevel = levels[0];
        }

        public Level GetLevelByNum(int num)
        {
            return levelsByNumber[num];
        }

        public void ChangeLevelByNum(int levelNum)
        {
            CurrentLevel = levelsByNumber[levelNum];
            CurrentLevel.Model.ChangeLevelByTxt(levelsByNumber[levelNum].Model.Path);
        }

        public void Draw(Graphics g)
        {
            CurrentLevel.View.Draw(g);
        }
    }
}
