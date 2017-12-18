﻿//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.ES20;
namespace OpenTkEssTest
{
    [Info(OrderCode = "111")]
    [Info("T111_FrameBuffer")]
    public class T111_FrameBuffer : DemoBase
    {
        GLRenderSurface _glsf;
        GLCanvasPainter painter;
        FrameBuffer frameBuffer;
        bool isInit;
        protected override void OnGLContextReady(GLRenderSurface canvasGL, GLCanvasPainter painter)
        {
            this._glsf = canvasGL;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            int max = Math.Max(this.Width, this.Height);
            frameBuffer = _glsf.CreateFrameBuffer(max, max);
        }
        protected override void DemoClosing()
        {
            _glsf.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsf.SmoothMode = CanvasSmoothMode.Smooth;
            _glsf.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsf.Clear(PixelFarm.Drawing.Color.White);
            _glsf.ClearColorBuffer();
            //-------------------------------
            if (!isInit)
            {
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                //------------------------------------------------------------------------------------
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer.FrameBufferId);
                //--------
                //do draw to frame buffer here
                GL.ClearColor(OpenTK.Graphics.Color4.Red);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                //------------------------------------------------------------------------------------



                //------------------------------------------------------------------------------------ 
                GL.BindTexture(TextureTarget.Texture2D, frameBuffer.TextureId);
                GL.GenerateMipmap(TextureTarget.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                //------------------------------------------------------------------------------------

                GLBitmap bmp = new GLBitmap(frameBuffer.TextureId, frameBuffer.Width, frameBuffer.Height);
                bmp.IsBigEndianPixel = true;
                _glsf.DrawImage(bmp, 15, 300);
            }
            else
            {
                _glsf.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

