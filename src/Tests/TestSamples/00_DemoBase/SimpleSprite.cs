﻿//BSD, 2014-2018, WinterDev

/*
Copyright (c) 2013, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
using PixelFarm.Agg; 
namespace Mini
{
    public abstract class SimpleSprite
    {
        protected double angle = 0;
        protected double spriteScale = 1.0;
        protected double skewX = 0;
        protected double skewY = 0;
        bool isFreeze;
        public int Width { get; set; }
        public int Height { get; set; }

        public void Freeze()
        {
            this.isFreeze = true;
        }
        public bool IsFreezed
        {
            get
            {
                return this.isFreeze;
            }
        }

        public virtual void OnDraw(PixelFarm.Drawing.Painter p)
        {
        }
        protected void UpdateTransform(double width, double height, double x, double y)
        {
            x -= width / 2;
            y -= height / 2;
            angle = Math.Atan2(y, x);
            spriteScale = Math.Sqrt(y * y + x * x) / 100.0;
        }

        public virtual bool Move(int mouseX, int mouseY)
        {
            double x = mouseX;
            double y = mouseY;
            int width = (int)Width;
            int height = (int)Height;
            UpdateTransform(width, height, x, y);
            return true;
        }
    }
}