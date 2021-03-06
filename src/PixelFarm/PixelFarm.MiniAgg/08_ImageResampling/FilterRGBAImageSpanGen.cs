﻿//BSD, 2014-2018, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
#define USE_UNSAFE_CODE

using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
using img_filter_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgFilterConst;
namespace PixelFarm.Agg.Imaging
{
    //==============================================span_image_resample_rgba
    public class FilterRGBAImageSpanGen : FilterImageSpanGenerator
    {
        const int BASE_MASK = 255;
        const int DOWN_SCALE_SHIFT = (int)ImageFilterLookUpTable.ImgFilterConst.SHIFT;
        //--------------------------------------------------------------------
        public FilterRGBAImageSpanGen(IImageReaderWriter src,
                            ISpanInterpolator inter,
                            ImageFilterLookUpTable filter) :
            base(src, inter, filter)
        {
            if (src.GetRecieveBlender().NumPixelBits != 32)
            {
                throw new System.FormatException("You have to use a rgba blender with span_image_resample_rgba");
            }
        }

        //--------------------------------------------------------------------
        public override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = base.Interpolator;
            spanInterpolator.Begin(x + base.dx, y + base.dy, len);
            int c0, c1, c2, c3;
            int[] weightArray = FilterLookup.WeightArray;
            int diameter = base.FilterLookup.Diameter;
            int filter_scale = diameter << img_subpix_const.SHIFT;
            int[] weight_array = weightArray;
            do
            {
                int rx;
                int ry;
                spanInterpolator.GetCoord(out x, out y);
                spanInterpolator.GetLocalScale(out rx, out ry);
                AdjustScale(ref rx, ref ry);
                int rx_inv = img_subpix_const.SCALE * img_subpix_const.SCALE / rx;
                int ry_inv = img_subpix_const.SCALE * img_subpix_const.SCALE / ry;
                int radius_x = (diameter * rx) >> 1;
                int radius_y = (diameter * ry) >> 1;
                int len_x_lr =
                    (diameter * rx + img_subpix_const.MASK) >>
                         img_subpix_const.SHIFT;
                x += base.dxInt - radius_x;
                y += base.dyInt - radius_y;
                c0 = c1 = c2 = c3 = img_filter_const.SCALE / 2;
                int y_lr = y >> img_subpix_const.SHIFT;
                int y_hr = ((img_subpix_const.MASK - (y & img_subpix_const.MASK)) *
                               ry_inv) >> img_subpix_const.SHIFT;
                int total_weight = 0;
                int x_lr = x >> img_subpix_const.SHIFT;
                int x_hr = ((img_subpix_const.MASK - (x & img_subpix_const.MASK)) *
                               rx_inv) >> img_subpix_const.SHIFT;
                int x_hr2 = x_hr;
                int sourceIndex;
                byte[] buff = BaseGetSpan(x_lr, y_lr, len_x_lr, out sourceIndex);
                for (; ; )
                {
                    int weight_y = weight_array[y_hr];
                    x_hr = x_hr2;
                    for (; ; )
                    {
                        int weight = (weight_y * weight_array[x_hr] +
                                      img_filter_const.SCALE / 2) >>
                                      DOWN_SCALE_SHIFT;
                        c0 += buff[sourceIndex + CO.R] * weight;
                        c1 += buff[sourceIndex + CO.G] * weight;
                        c2 += buff[sourceIndex + CO.B] * weight;
                        c3 += buff[sourceIndex + CO.A] * weight;
                        total_weight += weight;
                        x_hr += rx_inv;
                        if (x_hr >= filter_scale) break;
                        buff = BaseNextX(out sourceIndex);
                    }
                    y_hr += ry_inv;
                    if (y_hr >= filter_scale)
                    {
                        break;
                    }

                    buff = BaseNextY(out sourceIndex);
                }

                c0 /= total_weight;
                c1 /= total_weight;
                c2 /= total_weight;
                c3 /= total_weight;
                //clamps..
                if (c0 < 0) { c0 = 0; }
                else if (c0 > BASE_MASK) { c0 = BASE_MASK; }

                if (c1 < 0) { c1 = 0; }
                else if (c1 > BASE_MASK) { c1 = BASE_MASK; }

                if (c2 < 0) { c2 = 0; }
                else if (c2 > BASE_MASK) { c2 = BASE_MASK; }

                if (c3 < 0) { c3 = 0; }
                else if (c3 > BASE_MASK) { c3 = BASE_MASK; }


                //outputColors[startIndex].red = (byte)c0;
                //outputColors[startIndex].green = (byte)c1;
                //outputColors[startIndex].blue = (byte)c2;
                //outputColors[startIndex].alpha = (byte)c3;

                outputColors[startIndex] = Drawing.Color.FromArgb((byte)c3, (byte)c0, (byte)c1, (byte)c2);

                startIndex++;
                spanInterpolator.Next();
            } while (--len != 0);
        }
        /*
                    ISpanInterpolator spanInterpolator = base.interpolator();
                    spanInterpolator.begin(x + base.filter_dx_dbl(), y + base.filter_dy_dbl(), len);

                    int* fg = stackalloc int[4];

                    byte* fg_ptr;
                    fixed (int* pWeightArray = filter().weight_array())
                    {
                        int diameter = (int)base.filter().diameter();
                        int filter_scale = diameter << (int)img_subpix_const.image_subpixel_shift;

                        int* weight_array = pWeightArray;

                        do
                        {
                            int rx;
                            int ry;
                            int rx_inv = (int)img_subpix_const.image_subpixel_scale;
                            int ry_inv = (int)img_subpix_const.image_subpixel_scale;
                            spanInterpolator.coordinates(out x, out y);
                            spanInterpolator.local_scale(out rx, out ry);
                            base.adjust_scale(ref rx, ref ry);

                            rx_inv = (int)img_subpix_const.image_subpixel_scale * (int)img_subpix_const.image_subpixel_scale / rx;
                            ry_inv = (int)img_subpix_const.image_subpixel_scale * (int)img_subpix_const.image_subpixel_scale / ry;

                            int radius_x = (diameter * rx) >> 1;
                            int radius_y = (diameter * ry) >> 1;
                            int len_x_lr =
                                (diameter * rx + (int)img_subpix_const.image_subpixel_mask) >>
                                    (int)(int)img_subpix_const.image_subpixel_shift;

                            x += base.filter_dx_int() - radius_x;
                            y += base.filter_dy_int() - radius_y;

                            fg[0] = fg[1] = fg[2] = fg[3] = (int)image_filter_scale_e.image_filter_scale / 2;

                            int y_lr = y >> (int)(int)img_subpix_const.image_subpixel_shift;
                            int y_hr = (((int)img_subpix_const.image_subpixel_mask - (y & (int)img_subpix_const.image_subpixel_mask)) * 
                                           ry_inv) >>
                                               (int)(int)img_subpix_const.image_subpixel_shift;
                            int total_weight = 0;
                            int x_lr = x >> (int)(int)img_subpix_const.image_subpixel_shift;
                            int x_hr = (((int)img_subpix_const.image_subpixel_mask - (x & (int)img_subpix_const.image_subpixel_mask)) * 
                                           rx_inv) >>
                                               (int)(int)img_subpix_const.image_subpixel_shift;
                            int x_hr2 = x_hr;
                            fg_ptr = base.source().span(x_lr, y_lr, (int)len_x_lr);

                            for(;;)
                            {
                                int weight_y = weight_array[y_hr];
                                x_hr = x_hr2;
                                for(;;)
                                {
                                    int weight = (weight_y * weight_array[x_hr] +
                                                 (int)image_filter_scale_e.image_filter_scale / 2) >> 
                                                 downscale_shift;
                                    fg[0] += *fg_ptr++ * weight;
                                    fg[1] += *fg_ptr++ * weight;
                                    fg[2] += *fg_ptr++ * weight;
                                    fg[3] += *fg_ptr++ * weight;
                                    total_weight += weight;
                                    x_hr  += rx_inv;
                                    if(x_hr >= filter_scale) break;
                                    fg_ptr = base.source().next_x();
                                }
                                y_hr += ry_inv;
                                if (y_hr >= filter_scale)
                                {
                                    break;
                                }

                                fg_ptr = base.source().next_y();
                            }

                            fg[0] /= total_weight;
                            fg[1] /= total_weight;
                            fg[2] /= total_weight;
                            fg[3] /= total_weight;

                            if(fg[0] < 0) fg[0] = 0;
                            if(fg[1] < 0) fg[1] = 0;
                            if(fg[2] < 0) fg[2] = 0;
                            if(fg[3] < 0) fg[3] = 0;

                            if(fg[0] > fg[0]) fg[0] = fg[0];
                            if(fg[1] > fg[1]) fg[1] = fg[1];
                            if(fg[2] > fg[2]) fg[2] = fg[2];
                            if (fg[3] > base_mask) fg[3] = base_mask;

                            span->R_Byte = (byte)fg[ImageBuffer.OrderR];
                            span->G_Byte = (byte)fg[ImageBuffer.OrderG];
                            span->B_Byte = (byte)fg[ImageBuffer.OrderB];
                            span->A_Byte = (byte)fg[ImageBuffer.OrderA];

                            ++span;
                            interpolator().Next();
                        } while(--len != 0);
                    }
                                                              */
    }
}