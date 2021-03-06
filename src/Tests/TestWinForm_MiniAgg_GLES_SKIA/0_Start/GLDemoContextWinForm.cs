﻿//BSD, 2014-2018, WinterDev

using System;
using PixelFarm.DrawingGL;

namespace Mini
{
    class GLDemoContextWinForm
    {
        //this context is for WinForm

        DemoBase demobase;
        OpenTK.MyGLControl glControl;
        IntPtr hh1;
        GLRenderSurface _glsx;
        GLPainter canvasPainter;

        public void LoadGLControl(OpenTK.MyGLControl glControl)
        {
            //----------------------
            this.glControl = glControl;
            glControl.SetGLPaintHandler(HandleGLPaint);
            hh1 = glControl.Handle; //ensure that contrl handler is created
            glControl.MakeCurrent();
        }
        public void LoadSample(DemoBase demobase)
        {
            this.demobase = demobase;
            //1.
            //note:when we init,
            //no glcanvas/ painter are created
            demobase.Init();
            //-----------------------------------------------
            //2. check if demo will create canvas/painter context
            //or let this GLDemoContext create for it

            hh1 = glControl.Handle; //ensure that contrl handler is created
            glControl.MakeCurrent();

            demobase.BuildCustomDemoGLContext(out this._glsx, out this.canvasPainter);
            //
            if (this._glsx == null)
            {
                //if demo not create canvas and painter
                //the we create for it
                int max = Math.Max(glControl.Width, glControl.Height);
                _glsx = PixelFarm.Drawing.GLES2.GLES2Platform.CreateGLRenderSurface(max, max, glControl.Width, glControl.Height);
                _glsx.SmoothMode = SmoothMode.Smooth;//set anti-alias  
                canvasPainter = new GLPainter(_glsx);
                //create text printer for opengl 
                //----------------------
                //1. win gdi based
                //var printer = new WinGdiFontPrinter(_glsx, glControl.Width, glControl.Height);
                //canvasPainter.TextPrinter = printer;
                //----------------------
                //2. raw vxs
                //var openFontStore = new Typography.TextServices.OpenFontStore();
                //var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter,openFontStore);
                //canvasPainter.TextPrinter = printer;
                //----------------------
                //3. agg texture based font texture

                //var printer = new AggTextSpanPrinter(canvasPainter, glControl.Width, 30);
                //canvasPainter.TextPrinter = printer;
                //----------------------
                //4. texture atlas based font texture 
                //------------
                //resolve request font 
                var printer = new GLBitmapGlyphTextPrinter(canvasPainter, PixelFarm.Drawing.GLES2.GLES2Platform.TextService);
                canvasPainter.TextPrinter = printer;

                //var openFontStore = new Typography.TextServices.OpenFontStore();
                //var printer = new GLBmpGlyphTextPrinter(canvasPainter, openFontStore);
                //canvasPainter.TextPrinter = printer;
            }

            //-----------------------------------------------
            demobase.SetEssentialGLHandlers(
                () => this.glControl.SwapBuffers(),
                () => this.glControl.GetEglDisplay(),
                () => this.glControl.GetEglSurface()
            );
            //-----------------------------------------------
            this.glControl.SetGLPaintHandler((s, e) =>
            {
                demobase.InvokeGLPaint();
            });
            DemoBase.InvokeGLContextReady(demobase, this._glsx, this.canvasPainter);
            DemoBase.InvokePainterReady(demobase, this.canvasPainter);
        }
        void HandleGLPaint(object sender, System.EventArgs e)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Black;
            _glsx.ClearColorBuffer();
            //example
            canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            canvasPainter.FillRect(20, 20, 150, 150);
            //load bmp image 
            //------------------------------------------------------------------------- 
            if (demobase != null)
            {
                demobase.Draw(canvasPainter);
            }
            glControl.SwapBuffers();
        }
        public void CloseDemo()
        {
            demobase.CloseDemo();
        }

    }

}