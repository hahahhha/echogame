using echogame.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public class LevelManager
    {
        public int TakenLightersAmount { get; private set; } = 0;
        private int TotalLightersAmount;
        public Level CurrentLevel { get; private set; }

        private Dictionary<int, Level> levelsByNumber;

        public event Action LevelNumChanged;
        public event Action LevelChanged;
        public event Action TakenLightersAmountChanged;

        public LevelManager(List<Level> levels)
        {
            levelsByNumber = levels.ToDictionary(l => l.Number, l => l);
            CurrentLevel = levels[0];
            TotalLightersAmount = CurrentLevel.Model.GetLighterManager().GetActiveLighters().Count;
            LevelChanged?.Invoke();
        }

        public Level GetLevelByNum(int num)
        {
            return levelsByNumber[num];
        }

        public void ChangeLevelByNum(int levelNum)
        {
            CurrentLevel = levelsByNumber[levelNum];
            LevelChanged?.Invoke();
            LevelNumChanged?.Invoke();
        }

        public LightersManager GetCurrentLevelLightersMng()
        {
            return CurrentLevel.Model.GetLighterManager();
        }

        public LightersManager GetLevelLighterManager(int num)
        {
            return GetLevelByNum(num).Model.GetLighterManager();
        }

        public void Draw(Graphics g)
        {
            CurrentLevel.View.Draw(g);
            CurrentLevel.Model.GetLighterManager().DrawAllLighters(g);
        }

        public void OnPlayerPositionChanged(Rectangle playerRect)
        {
            var lighters = GetCurrentLevelLightersMng().GetActiveLighters();
            foreach (var lighter in lighters)
            {
                if (lighter.Model.Collider.IntersectsWith(playerRect) && lighter.Model.IsAlive)
                {
                    lighter.Model.MakeUnactive();
                    LevelChanged?.Invoke();
                    TakenLightersAmount++;
                    TakenLightersAmountChanged?.Invoke();
                }
                if (TakenLightersAmount == TotalLightersAmount)
                {
                    if (levelsByNumber.ContainsKey(CurrentLevel.Number + 1))
                    {
                        ChangeLevelByNum(CurrentLevel.Number + 1);
                        TotalLightersAmount = CurrentLevel.Model.GetLighterManager().GetActiveLighters().Count;
                        TakenLightersAmount = 0;
                    }
                }
            }
        }

        public bool IsPlayerStepAvialable(Point nextPoint, Size size)
        {
            var nextPointRect = new Rectangle(nextPoint, size);
            var currentLevel = CurrentLevel;
            foreach (var wallPosition in currentLevel.Model.LevelState.WallsPositions)
            {
                var wallCell = new Rectangle(wallPosition, new Size(currentLevel.Model.CellWidth, currentLevel.Model.CellHeight));
                if (wallCell.IntersectsWith(nextPointRect))
                    return false;
            }
            return true;
        }
    }
}
