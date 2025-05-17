using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public static class LevelCreator
    {
        /// <summary>
        /// Просто создание уровня
        /// </summary>
        /// 
        /// <param name="cellsByMapChars">Словарь, показывающий соответствие символа к ячейке уровня</param>
        /// <param name="isGameLevel">Параметр для меню</param>
        /// <returns></returns>
        /// 
        public static LevelMap CreateLevelByMap(string[] map,
            Dictionary<char, LevelCell> cellsByMapChars,
            Dictionary<LevelCell, string> routsByCells,
            int cellWidth,
            int cellHeight,
            bool isGameLevel = true)
        {
            var currentX = 0;
            var currentY = 0;
            var cellMap = new Dictionary<Point, LevelCell>();
            foreach (var row in map)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    cellMap[new Point(currentX, currentY)] = cellsByMapChars[row[i]];
                    currentX += cellWidth;
                }
                currentY += cellHeight;
                currentX = 0;
            }
            return new LevelMap(cellMap, routsByCells, cellWidth, cellHeight, map.Select(s => s.Length).Max(), map.Length);
        }

        public static LevelMap CreateLevelByMapFromTxt(
            string path,
            Dictionary<char, LevelCell> cellsByMapChars,
            Dictionary<LevelCell, string> routsByCells,
            int cellWidth,
            int cellHeight,
            bool isGameLevel = true)
        {
            return CreateLevelByMap(
                File.ReadAllLines(path).ToArray(),
                cellsByMapChars,
                routsByCells,
                cellWidth,
                cellHeight,
                isGameLevel);
        }
    }
}
