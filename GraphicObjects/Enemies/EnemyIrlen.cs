﻿using AI2D.Engine;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyIrlen : BaseEnemy
    {
        private string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private string[] _imagePaths = {
            #region images.
            "Irle (1).png",
            "Irle (2).png",
            "Irle (3).png",
            "Irle (4).png",
            "Irle (5).png",
            "Irle (6).png",
            "Irle (7).png",
            "Irle (8).png"
            #endregion
        };

        public EnemyIrlen(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            LoadResources(_assetPath +_imagePaths[imageIndex], new System.Drawing.Size(32, 32));
        }
    }
}
