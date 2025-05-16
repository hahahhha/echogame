using echogame.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public class LevelMap
    {
        /// <summary>
        /// Point - левый верхний угол текстуры, Image - сама текстура
        /// </summary>

        // для отрисовки
        public Dictionary<Point, Image> ImagesToDrawAtPoints { get; private set; }

        public static readonly Dictionary<char, LevelCell> DefaultCellsByChar =
            new Dictionary<char, LevelCell>()
            {
                {'#', LevelCell.Wall },
                {' ', LevelCell.Empty }
            };

        public static readonly Dictionary<LevelCell, string> DefaultRoutsByCell =
            new Dictionary<LevelCell, string>()
            {
                {LevelCell.Empty, "../../Textures/empty_low.png" },
                {LevelCell.Wall, "../../Textures/wall_low.png" }
            };

        private int cellWidth;
        private int cellHeight;

        private int cellWidthAmount;
        private int cellHeightAmount;

        // полезно для других классов
        public readonly Dictionary<Point, LevelCell> CellMap; // (0,0), (0, 50), ...
        public readonly List<Point> WallsPositions = new List<Point>();

        public readonly List<SolidWall> WallsGrid = new List<SolidWall>();

        public LevelMap(Dictionary<Point, LevelCell> cellMap, Dictionary<LevelCell, string> routByCell,
            int cellWidth, int cellHeight, int cellWidthAmount, int cellHeightAmount)
        {
            ImagesToDrawAtPoints = new Dictionary<Point, Image>();
            CellMap = cellMap;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.cellWidthAmount = cellWidthAmount;
            this.cellHeightAmount = cellHeightAmount;
            foreach (var point in cellMap.Keys)
            {
                ImagesToDrawAtPoints[point] = Image.FromFile(routByCell[cellMap[point]]);

                if (cellMap[point] == LevelCell.Wall)
                {
                    WallsPositions.Add(point);
                }
            }

            WallsGrid = GetSolidWalls();
        }

        public List<SolidWall> GetSolidWalls()
        {
            List<SolidWall> solidWalls = new List<SolidWall>();

            var checkedPositions = new HashSet<Point>();


            foreach (var wallPos in WallsPositions)
            {
                if (checkedPositions.Contains(wallPos)) continue;
                var horizontalGroup = new List<Point>();
                horizontalGroup.Add(wallPos);
                checkedPositions.Add(wallPos);
                foreach (var nextWallPos in WallsPositions)
                {
                    if (checkedPositions.Contains(nextWallPos)) continue;
                    if (Math.Abs(horizontalGroup[horizontalGroup.Count - 1].X - nextWallPos.X) <= cellWidth &&
                        horizontalGroup[horizontalGroup.Count - 1].Y == nextWallPos.Y)
                    {
                        horizontalGroup.Add(nextWallPos);
                        checkedPositions.Add(nextWallPos);
                    }
                }
                // есть горизонтальная стена
                if (horizontalGroup.Count > 1)
                {
                    var minX = horizontalGroup.Min(p => p.X);
                    var maxX = horizontalGroup.Max(p => p.X);
                    var minY = horizontalGroup.Min(p => p.Y);
                    var maxY = horizontalGroup.Max(p => p.Y);
                    solidWalls.Add(new SolidWall(new Size(maxX - minX + cellWidth, maxY - minY + cellHeight), new Point(minX, minY)));
                }
                else
                {
                    // горизонтальная стена не нашлась, ищу вертикальную
                    var verticalGroup = new List<Point> { wallPos };
                    foreach (var nextWallPos in WallsPositions)
                    {
                        if (checkedPositions.Contains(nextWallPos)) continue;
                        if (verticalGroup[verticalGroup.Count - 1].X == nextWallPos.X &&
                            Math.Abs(verticalGroup[verticalGroup.Count - 1].Y - nextWallPos.Y) <= cellHeight)
                        {
                            verticalGroup.Add(nextWallPos);
                            checkedPositions.Add(nextWallPos);
                        }
                    }
                    var minX = verticalGroup.Min(p => p.X);
                    var maxX = verticalGroup.Max(p => p.X);
                    var minY = verticalGroup.Min(p => p.Y);
                    var maxY = verticalGroup.Max(p => p.Y);
                    solidWalls.Add(new SolidWall(new Size(maxX - minX + cellWidth, maxY - minY + cellHeight), new Point(minX, minY)));
                }
            }

            return solidWalls;
        }
    }
}
