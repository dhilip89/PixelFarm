﻿//BSD, 2014-2018, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------



using PixelFarm.Agg.Imaging;
namespace PixelFarm.Agg
{
    public enum BlurMethod
    {
        StackBlur,
        RecursiveBlur,
        ChannelBlur
    }

    class FilterMan
    {
        StackBlur stackBlur;
        RecursiveBlur m_recursive_blur;

        ShapenFilterPdn pdnSharpen;

        public void DoStackBlur(ImageReaderWriterBase readerWriter, int radius)
        {
            if (stackBlur == null)
            {
                stackBlur = new StackBlur();
            }
            stackBlur.Blur(readerWriter, radius, radius);
        }
        public void DoRecursiveBlur(ImageReaderWriterBase readerWriter, int radius)
        {
            if (m_recursive_blur == null)
            {
                m_recursive_blur = new RecursiveBlur(new RecursiveBlurCalcRGB());
            }
            m_recursive_blur.Blur(readerWriter, radius);
        }
        public void DoSharpen(ImageReaderWriterBase readerWriter, int radius)
        {
            if (pdnSharpen == null)
            {
                pdnSharpen = new ShapenFilterPdn();
            }
            pdnSharpen.Sharpen(readerWriter, radius);
        }

    }
}