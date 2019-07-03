﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Types
{
    public class RectangleD
    {
        public Double X { get; set; }
        public Double Y { get; set; }
        public Double Width { get; set; }
        public Double Height { get; set; }

        public RectangleD()
        {
        }

        public RectangleD(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

    }
}
