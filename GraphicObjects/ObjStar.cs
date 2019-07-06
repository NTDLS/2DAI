﻿using AI2D.Engine;
using System.Linq;

namespace AI2D.GraphicObjects
{
    public class ObjStar : BaseGraphicObject
    {
        private string _assetStarPath = @"..\..\Assets\Graphics\Star\";
        private string[] _assetStarFiles = {
            #region images.
            "Star 1.png",
            "Star 2.png",
            "Star 3.png",
            "Star 4.png",
            #endregion
        };

        public ObjStar(Core core)
            : base(core)
        {
            int _explosionImageIndex = Utility.RandomNumber(0, _assetStarFiles.Count());
            LoadResources(_assetStarPath + _assetStarFiles[_explosionImageIndex]);

            //LoadResources(@"..\..\Assets\Graphics\Star\Star 1.png");

            X = Utility.Random.Next(0, core.Display.VisibleSize.Width);
            Y = Utility.Random.Next(0, core.Display.VisibleSize.Height);
        }
    }
}