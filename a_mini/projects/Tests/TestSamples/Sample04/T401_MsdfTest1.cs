﻿//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "401")]
    [Info("T401_MsdfTest1")]
    public class T401_MsdfTest1 : DemoBase
    {
        GLRenderSurface canvas2d;
        bool resInit;
        GLBitmap msdf_bmp;
        GLBitmap sdf_bmp;
        protected override void OnGLContextReady(GLRenderSurface canvasGL, GLCanvasPainter painter)
        {
            this.canvas2d = canvasGL;

        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            if (!resInit)
            {

                msdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\msdf_75.png");
                sdf_bmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\sdf_75.png");
                resInit = true;
            }
            canvas2d.Clear(PixelFarm.Drawing.Color.White);

            canvas2d.DrawImageWithMsdf(msdf_bmp, 0, 400, 6);
            canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 500, 0.5f);
            canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 520, 0.4f);
            canvas2d.DrawImageWithMsdf(msdf_bmp, 100, 550, 0.3f);
            canvas2d.DrawImage(msdf_bmp, 150, 400);

            canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 200, 400, 6);
            canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 500, 0.5f);
            canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 520, 0.4f);
            canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 300, 550, 0.3f);

            //
            canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 400, 6);
            canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 500, 0.5f);
            canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 520, 0.4f);
            canvas2d.DrawImageWithMsdf(sdf_bmp, 400, 550, 0.3f);
            canvas2d.DrawImage(sdf_bmp, 400, 300);

            SwapBuffers();
        }
    }
}

