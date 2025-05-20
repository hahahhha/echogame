using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace echogame.Models
{
    public class LevelModel
    {
        public event Action LevelChanged;
        public LevelMap LevelState {  get; private set; }

        public readonly int CellWidth;
        public readonly int CellHeight;

        public readonly string Path;

        private readonly LightersManager lightersManager;
        public Point PlayerStartPos { get; private set; }

        //public LevelModel(string[] map, int cellWidth, int cellHeight)
        //{
        //    LevelState = LevelCreator.CreateLevelByMap(
        //        map,
        //        LevelMap.DefaultCellsByChar,
        //        LevelMap.DefaultRoutsByCell,
        //        cellWidth,
        //        cellHeight
        //        );
        //    CellHeight = cellHeight;
        //    CellWidth = cellWidth;
        //}

        public LevelModel(string path, int cellWidth, int cellHeight, Point playerStartPos)
        {
            lightersManager = new LightersManager();

            LevelState = LevelCreator.CreateLevelByMapFromTxt(
                path,
                LevelMap.DefaultCellsByChar,
                LevelMap.DefaultRoutsByCell,
                cellWidth,
                cellHeight,
                lightersManager
                );
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            Path = path;
            PlayerStartPos = playerStartPos;
        }

        public void ChangeLevel(string[] map)
        {
            LevelState = LevelCreator.CreateLevelByMap(
                map,
                LevelMap.DefaultCellsByChar,
                LevelMap.DefaultRoutsByCell,
                CellWidth,
                CellHeight,
                lightersManager
                );
            LevelChanged.Invoke();
        }

        //public void ChangeLevelByTxt(string path)
        //{
        //    ChangeLevel(File.ReadLines(path).ToArray());
        //}

        public LightersManager GetLighterManager()
        {
            return lightersManager;
        }
    }
}
