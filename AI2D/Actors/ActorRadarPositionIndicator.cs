﻿using AI2D.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Actors
{
    public class ActorRadarPositionIndicator: ActorBase
    {
        private const string _assetPath = @"..\..\..\Assets\Graphics\";
        private readonly string _assetFile = "Radar Indicator 8.png";

        public ActorRadarPositionIndicator(Core core)
            : base(core)
        {
            Initialize(_assetPath + _assetFile, new Size(8, 8));

            X = 0;
            Y = 0;
        }
    }
}
