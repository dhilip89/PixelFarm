﻿//BSD, 2014-2018, WinterDev
using System;
using PixelFarm.Drawing;
namespace PixelFarm.Drawing.Effects
{
    public class ImgFilterStackBlur : ImageFilter
    {
        public override ImageFilterName Name
        {
            get
            {
                return ImageFilterName.StackBlur;
            }
        }
    }
    public class ImgFilterRecursiveBlur : ImageFilter
    {
        public override ImageFilterName Name
        {
            get
            {
                return ImageFilterName.RecursiveBlur;
            }
        }
    }
    public class ImgFilterSharpen : ImageFilter
    {
        public override ImageFilterName Name
        {
            get
            {
                return ImageFilterName.Sharpen;
            }
        }

    }
}