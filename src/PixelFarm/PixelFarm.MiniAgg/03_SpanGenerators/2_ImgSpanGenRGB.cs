////BSD, 2014-2018, WinterDev
////----------------------------------------------------------------------------
//// Anti-Grain Geometry - Version 2.4
//// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
////
//// C# Port port by: Lars Brubaker
////                  larsbrubaker@gmail.com
//// Copyright (C) 2007
////
//// Permission to copy, use, modify, sell and distribute this software 
//// is granted provided this copyright notice appears in all copies. 
//// This software is provided "as is" without express or implied
//// warranty, and with no claim as to its suitability for any purpose.
////
////----------------------------------------------------------------------------
//// Contact: mcseem@antigrain.com
////          mcseemagg@yahoo.com
////          http://www.antigrain.com
////----------------------------------------------------------------------------
////
//// Adaptation for high precision colors has been sponsored by 
//// Liberty Technology Systems, Inc., visit http://lib-sys.com
////
//// Liberty Technology Systems, Inc. is the provider of
//// PostScript and PDF technology for software developers.
//// 
////----------------------------------------------------------------------------

//using System;
//using img_subpix_const = PixelFarm.Agg.ImageFilterLookUpTable.ImgSubPixConst;
//namespace PixelFarm.Agg.Imaging
//{
//#if DEBUG
//    // it should be easy to write a 90 rotating or mirroring filter too. LBB 2012/01/14
//    class dbugImgSpanGenRGB_NNStepXby1 : ImgSpanGen
//    {
//        const int BASE_SHIFT = 8;
//        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
//        const int BASE_MASK = BASE_SCALE - 1;
//        ImageReaderWriterBase srcRW;
//        public dbugImgSpanGenRGB_NNStepXby1(IImageReaderWriter src, ISpanInterpolator spanInterpolator)
//            : base(spanInterpolator)
//        {
//            this.srcRW = (ImageReaderWriterBase)src;
//            if (srcRW.BitDepth != 24)
//            {
//                throw new NotSupportedException("The source is expected to be 32 bit.");
//            }
//        }
//        public sealed override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
//        {

//            ISpanInterpolator spanInterpolator = Interpolator;
//            spanInterpolator.Begin(x + dx, y + dy, len);
//            int x_hr;
//            int y_hr;
//            spanInterpolator.GetCoord(out x_hr, out y_hr);
//            int x_lr = x_hr >> img_subpix_const.SHIFT;
//            int y_lr = y_hr >> img_subpix_const.SHIFT;
//            int bufferIndex = srcRW.GetBufferOffsetXY(x_lr, y_lr);
//            byte[] srcBuffer = srcRW.GetBuffer();
//            unsafe
//            {
//                fixed (byte* pSource = &srcBuffer[bufferIndex])
//                {
//                    int* src_ptr = (int*)pSource;
//                    do
//                    {
//                        int src_value = *src_ptr;
//                        //separate each component
//                        byte a = (byte)((src_value >> 24) & 0xff);
//                        byte r = (byte)((src_value >> 16) & 0xff);
//                        byte g = (byte)((src_value >> 8) & 0xff);
//                        byte b = (byte)((src_value) & 0xff);

//                        //TODO: review here, color from source buffer
//                        //should be in 'pre-multiplied' format.
//                        //so it should be converted to 'straight' color by call something like ..'FromPreMult()' 

//                        outputColors[startIndex++] = Drawing.Color.FromArgb(a, r, g, b);

//                        src_ptr++;//move next
//                    } while (--len != 0);
//                }
//            }


//            //ISpanInterpolator spanInterpolator = Interpolator;
//            //spanInterpolator.Begin(x + dx, y + dy, len);
//            //int x_hr;
//            //int y_hr;
//            //spanInterpolator.GetCoord(out x_hr, out y_hr);
//            //int x_lr = x_hr >> img_subpix_const.SHIFT;
//            //int y_lr = y_hr >> img_subpix_const.SHIFT;
//            //int bufferIndex = srcRW.GetBufferOffsetXY(x_lr, y_lr);
//            //byte[] srcBuff = srcRW.GetBuffer();
//            ////Drawing.Color color = Drawing.Color.White;
//            //do
//            //{
//            //    ////color.blue = srcBuff[bufferIndex++];
//            //    ////color.green = srcBuff[bufferIndex++];
//            //    ////color.red = srcBuff[bufferIndex++];

//            //    ////TODO: review, use CO (color order)
//            //    //int b = srcBuff[bufferIndex++]; //B
//            //    //int g = srcBuff[bufferIndex++]; //G
//            //    //int r = srcBuff[bufferIndex++]; //R

//            //    outputColors[startIndex++] = Drawing.Color.FromArgb(255, r, g, b);
//            //} while (--len != 0);
//        }
//    }




//    //=====================================span_image_filter_rgb_bilinear_clip
//    class dbugImgSpanGenRGB_BilinearClip : ImgSpanGen
//    {
//        Drawing.Color m_bgcolor;
//        const int BASE_SHIFT = 8;
//        const int BASE_SCALE = (int)(1 << BASE_SHIFT);
//        const int BASE_MASK = BASE_SCALE - 1;
//        ImageReaderWriterBase srcRW;
//        //--------------------------------------------------------------------
//        public dbugImgSpanGenRGB_BilinearClip(IImageReaderWriter src,
//                                          Drawing.Color back_color,
//                                          ISpanInterpolator inter)
//            : base(inter)
//        {
//            m_bgcolor = back_color;
//            srcRW = (ImageReaderWriterBase)src;
//        }

//        public Drawing.Color BackgroundColor
//        {
//            get { return this.m_bgcolor; }
//            set { this.m_bgcolor = value; }
//        }
//        public sealed override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
//        {
//            ISpanInterpolator spanInterpolator = base.Interpolator;
//            spanInterpolator.Begin(x + base.dx, y + base.dy, len);
//            int accColor0, accColor1, accColor2;
//            int sourceAlpha;
//            int back_r = m_bgcolor.red;
//            int back_g = m_bgcolor.green;
//            int back_b = m_bgcolor.blue;
//            int back_a = m_bgcolor.alpha;
//            int bufferIndex32;
//            int maxx = (int)srcRW.Width - 1;
//            int maxy = (int)srcRW.Height - 1;
//            int[] srcBuffer32 = srcRW.GetBuffer32();
//            unchecked
//            {
//                do
//                {
//                    int x_hr;
//                    int y_hr;
//                    spanInterpolator.GetCoord(out x_hr, out y_hr);
//                    x_hr -= base.dxInt;
//                    y_hr -= base.dyInt;
//                    int x_lr = x_hr >> img_subpix_const.SHIFT;
//                    int y_lr = y_hr >> img_subpix_const.SHIFT;
//                    int weight;
//                    if (x_lr >= 0 && y_lr >= 0 &&
//                       x_lr < maxx && y_lr < maxy)
//                    {
//                        accColor0 =
//                        accColor1 =
//                        accColor2 = img_subpix_const.SCALE * img_subpix_const.SCALE / 2;
//                        x_hr &= img_subpix_const.MASK;
//                        y_hr &= img_subpix_const.MASK;
//                        bufferIndex = srcRW.GetBufferOffsetXY(x_lr, y_lr);
//                        weight = ((img_subpix_const.SCALE - x_hr) *
//                                 (img_subpix_const.SCALE - y_hr));

//                        accColor0 += weight * srcBuffer[bufferIndex + CO.R];
//                        accColor1 += weight * srcBuffer[bufferIndex + CO.G];
//                        accColor2 += weight * srcBuffer[bufferIndex + CO.B];
//                        bufferIndex += 3;
//                        weight = (x_hr * (img_subpix_const.SCALE - y_hr));
//                        accColor0 += weight * srcBuffer[bufferIndex + CO.R];
//                        accColor1 += weight * srcBuffer[bufferIndex + CO.G];
//                        accColor2 += weight * srcBuffer[bufferIndex + CO.B];
//                        y_lr++;
//                        bufferIndex = srcRW.GetBufferOffsetXY(x_lr, y_lr);
//                        weight = ((img_subpix_const.SCALE - x_hr) * y_hr);
//                        accColor0 += weight * srcBuffer[bufferIndex + CO.R];
//                        accColor1 += weight * srcBuffer[bufferIndex + CO.G];
//                        accColor2 += weight * srcBuffer[bufferIndex + CO.B];
//                        bufferIndex += 3;
//                        weight = (x_hr * y_hr);
//                        accColor0 += weight * srcBuffer[bufferIndex + CO.R];
//                        accColor1 += weight * srcBuffer[bufferIndex + CO.G];
//                        accColor2 += weight * srcBuffer[bufferIndex + CO.B];
//                        accColor0 >>= img_subpix_const.SHIFT * 2;
//                        accColor1 >>= img_subpix_const.SHIFT * 2;
//                        accColor2 >>= img_subpix_const.SHIFT * 2;
//                        sourceAlpha = BASE_MASK;
//                    }
//                    else
//                    {
//                        if (x_lr < -1 || y_lr < -1 ||
//                           x_lr > maxx || y_lr > maxy)
//                        {
//                            accColor0 = back_r;
//                            accColor1 = back_g;
//                            accColor2 = back_b;
//                            sourceAlpha = back_a;
//                        }
//                        else
//                        {
//                            accColor0 =
//                            accColor1 =
//                            accColor2 = img_subpix_const.SCALE * img_subpix_const.SCALE / 2;
//                            sourceAlpha = img_subpix_const.SCALE * img_subpix_const.SCALE / 2;
//                            x_hr &= img_subpix_const.MASK;
//                            y_hr &= img_subpix_const.MASK;
//                            weight = ((img_subpix_const.SCALE - x_hr) * (img_subpix_const.SCALE - y_hr));
//                            if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
//                            {
//                                BlendInFilterPixel(ref accColor0, ref accColor1, ref accColor2, ref sourceAlpha,
//                                   srcRW.GetBuffer(),
//                                   srcRW.GetBufferOffsetXY(x_lr, y_lr),
//                                   weight);
//                            }
//                            else
//                            {
//                                accColor0 += back_r * weight;
//                                accColor1 += back_g * weight;
//                                accColor2 += back_b * weight;
//                                sourceAlpha += back_a * weight;
//                            }
//                            x_lr++;
//                            weight = (x_hr * (img_subpix_const.SCALE - y_hr));
//                            if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
//                            {
//                                BlendInFilterPixel(ref accColor0, ref accColor1, ref accColor2, ref sourceAlpha,
//                                   srcRW.GetBuffer(),
//                                   srcRW.GetBufferOffsetXY(x_lr, y_lr),
//                                   weight);
//                            }
//                            else
//                            {
//                                accColor0 += back_r * weight;
//                                accColor1 += back_g * weight;
//                                accColor2 += back_b * weight;
//                                sourceAlpha += back_a * weight;
//                            }

//                            x_lr--;
//                            y_lr++;
//                            weight = ((img_subpix_const.SCALE - x_hr) * y_hr);
//                            if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
//                            {
//                                BlendInFilterPixel(ref accColor0, ref accColor1, ref accColor2, ref sourceAlpha,
//                                    srcRW.GetBuffer(),
//                                    srcRW.GetBufferOffsetXY(x_lr, y_lr),
//                                    weight);
//                            }
//                            else
//                            {
//                                accColor0 += back_r * weight;
//                                accColor1 += back_g * weight;
//                                accColor2 += back_b * weight;
//                                sourceAlpha += back_a * weight;
//                            }

//                            x_lr++;
//                            weight = (x_hr * y_hr);
//                            if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
//                            {
//                                BlendInFilterPixel(ref accColor0, ref accColor1, ref accColor2, ref sourceAlpha,
//                                 srcRW.GetBuffer(),
//                                 srcRW.GetBufferOffsetXY(x_lr, y_lr),
//                                 weight);
//                            }
//                            else
//                            {
//                                accColor0 += back_r * weight;
//                                accColor1 += back_g * weight;
//                                accColor2 += back_b * weight;
//                                sourceAlpha += back_a * weight;
//                            }
//                            accColor0 >>= img_subpix_const.SHIFT * 2;
//                            accColor1 >>= img_subpix_const.SHIFT * 2;
//                            accColor2 >>= img_subpix_const.SHIFT * 2;
//                            sourceAlpha >>= img_subpix_const.SHIFT * 2;
//                        }
//                    }

//                    //outputColors[startIndex].red = (byte)accColor0;
//                    //outputColors[startIndex].green = (byte)accColor1;
//                    //outputColors[startIndex].blue = (byte)accColor2;
//                    //outputColors[startIndex].alpha = (byte)sourceAlpha;

//                    outputColors[startIndex] = PixelFarm.Drawing.Color.FromArgb(
//                    (byte)sourceAlpha,
//                    (byte)accColor0,
//                    (byte)accColor1,
//                    (byte)accColor2
//                    );

//                    startIndex++;
//                    spanInterpolator.Next();
//                } while (--len != 0);
//            }
//        }

//        static void BlendInFilterPixel(ref int accColor0, ref int accColor1, ref int accColor2, ref int sourceAlpha,
//             byte[] srcBuffer, int bufferIndex, int weight)
//        {
//            unchecked
//            {
//                accColor0 += weight * srcBuffer[bufferIndex + CO.R];
//                accColor1 += weight * srcBuffer[bufferIndex + CO.G];
//                accColor2 += weight * srcBuffer[bufferIndex + CO.B];
//                sourceAlpha += weight * BASE_MASK;
//            }
//        }
//    }
//#endif
//}
