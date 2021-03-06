﻿//MIT, 2016-2018, WinterDev
using System;
using System.Collections.Generic;

using System.Drawing;
using System.IO;
using System.Windows.Forms;

using PixelFarm.Agg;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

using Typography.OpenFont;
using Typography.Rendering;
using Typography.Contours;
using Typography.TextLayout;


namespace SampleWinForms
{
    public partial class Form1 : Form
    {
        Graphics g;
        AggPainter painter;
        AggRenderSurface _aggsx;
        ActualImage destImg;
        Bitmap winBmp;



        DevTextPrinterBase selectedTextPrinter = null;
        VxsTextPrinter _devVxsTextPrinter = null;

        UI.DebugGlyphVisualizer debugGlyphVisualizer = new UI.DebugGlyphVisualizer();
        TypographyTest.BasicFontOptions _basicOptions;
        TypographyTest.GlyphRenderOptions _glyphRenderOptions;
        TypographyTest.ContourAnalysisOptions _contourAnalysisOpts;

        public Form1()
        {
            InitializeComponent();

            //
            _basicOptions = openFontOptions1.Options;
            _basicOptions.TypefaceChanged += (s, e) =>
            {
                if (_devVxsTextPrinter != null)
                {
                    _devVxsTextPrinter.Typeface = e.SelectedTypeface;
                }
            };

            _basicOptions.UpdateRenderOutput += (s, e) => UpdateRenderOutput();
            //
            _glyphRenderOptions = glyphRenderOptionsUserControl1.Options;
            _glyphRenderOptions.UpdateRenderOutput += (s, e) => UpdateRenderOutput();
            //
            _contourAnalysisOpts = glyphContourAnalysisOptionsUserControl1.Options;
            _contourAnalysisOpts.UpdateRenderOutput += (s, e) => UpdateRenderOutput();


            this.Load += new EventHandler(Form1_Load);
            txtInputChar.TextChanged += (s, e) => UpdateRenderOutput();


            //----------------
            //string inputstr = "ก้า";
            //string inputstr = "น้ำน้ำ";
            //string inputstr = "example";
            //string inputstr = "lllll";
            //string inputstr = "e";
            //string inputstr = "T";
            //string inputstr = "u";
            //string inputstr = "t";
            //string inputstr = "2";
            //string inputstr = "3";
            //string inputstr = "o";
            //string inputstr = "l";
            //string inputstr = "k";
            //string inputstr = "8";
            //string inputstr = "#";
            //string inputstr = "a";
            //string inputstr = "e";
            //string inputstr = "l";
            //string inputstr = "t";
            //string inputstr = "i";
            string inputstr = "ma";
            //string inputstr = "po";
            //string inputstr = "Å";
            //string inputstr = "fi";
            //string inputstr = "ก่นกิ่น";
            //string inputstr = "ญญู";
            //string inputstr = "ป่า"; //for gpos test 
            //string inputstr = "快速上手";
            //string inputstr = "啊";

            //----------------
            this.txtInputChar.Text = inputstr;
            _readyToRender = true;
        }




        void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Render with PixelFarm";
        }
        bool _readyToRender;
        void UpdateRenderOutput()
        {
            if (!_readyToRender) return;
            //
            if (g == null)
            {
                destImg = new ActualImage(800, 600, PixelFormat.ARGB32);
                _aggsx = new AggRenderSurface(destImg); //no platform
                painter = new AggPainter(_aggsx);
                winBmp = new Bitmap(destImg.Width, destImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                g = this.CreateGraphics();

                painter.CurrentFont = new PixelFarm.Drawing.RequestFont("tahoma", 14);

                _devVxsTextPrinter = new VxsTextPrinter(painter, _basicOptions.OpenFontStore);
                _devVxsTextPrinter.TargetCanvasPainter = painter;
                _devVxsTextPrinter.ScriptLang = _basicOptions.ScriptLang;
                _devVxsTextPrinter.PositionTechnique = Typography.TextLayout.PositionTechnique.OpenFont;
                //_devGdiTextPrinter.TargetGraphics = g;
            }

            if (string.IsNullOrEmpty(this.txtInputChar.Text))
            {
                return;
            }

            //test option use be used with lcd subpixel rendering.
            //this demonstrate how we shift a pixel for subpixel rendering tech
            _devVxsTextPrinter.UseWithLcdSubPixelRenderingTechnique = _contourAnalysisOpts.SetupPrinterLayoutForLcdSubPix;




            //1. read typeface from font file 
            TypographyTest.RenderChoice renderChoice = _basicOptions.RenderChoice;
            switch (renderChoice)
            {

                case TypographyTest.RenderChoice.RenderWithGdiPlusPath:
                    //not render in this example
                    //see more at ...
                    break;
                case TypographyTest.RenderChoice.RenderWithTextPrinterAndMiniAgg:
                    {
                        //clear previous draw
                        painter.Clear(PixelFarm.Drawing.Color.White);
                        painter.UseSubPixelLcdEffect = _contourAnalysisOpts.LcdTechnique;
                        painter.FillColor = PixelFarm.Drawing.Color.Black;

                        selectedTextPrinter = _devVxsTextPrinter;
                        selectedTextPrinter.Typeface = _basicOptions.Typeface;
                        selectedTextPrinter.FontSizeInPoints = _basicOptions.FontSizeInPoints;
                        selectedTextPrinter.ScriptLang = _basicOptions.ScriptLang;
                        selectedTextPrinter.PositionTechnique = _basicOptions.PositionTech;

                        selectedTextPrinter.HintTechnique = _glyphRenderOptions.HintTechnique;
                        selectedTextPrinter.EnableLigature = _glyphRenderOptions.EnableLigature;


                        glyphTextureBitmapGenUserControl1.SelectedTypeface = _basicOptions.Typeface;
                        glyphTextureBitmapGenUserControl1.FontSizeInPoints = _basicOptions.FontSizeInPoints;
                        glyphTextureBitmapGenUserControl1.SelectedScriptLangs.Clear();
                        glyphTextureBitmapGenUserControl1.SelectedScriptLangs.Add(_basicOptions.ScriptLang);

                        //test print 3 lines
#if DEBUG
                        GlyphDynamicOutline.dbugTestNewGridFitting = _contourAnalysisOpts.EnableGridFit;
                        GlyphDynamicOutline.dbugActualPosToConsole = _contourAnalysisOpts.WriteFitOutputToConsole;
                        GlyphDynamicOutline.dbugUseHorizontalFitValue = _contourAnalysisOpts.UseHorizontalFitAlignment;
#endif

                        char[] printTextBuffer = this.txtInputChar.Text.ToCharArray();
                        float x_pos = 0, y_pos = 200;
                        float lineSpacingPx = selectedTextPrinter.FontLineSpacingPx;
                        for (int i = 0; i < 1; ++i)
                        {
                            selectedTextPrinter.DrawString(printTextBuffer, x_pos, y_pos);
                            y_pos -= lineSpacingPx;
                        }


                        //copy from Agg's memory buffer to gdi 
                        PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(destImg, winBmp);
                        g.Clear(System.Drawing.Color.White);
                        g.DrawImage(winBmp, new System.Drawing.Point(10, 0));

                    }
                    break;

                //==============================================
                //render 1 glyph for debug and test
                case TypographyTest.RenderChoice.RenderWithMsdfGen:
                case TypographyTest.RenderChoice.RenderWithSdfGen:
                    {
                        char testChar = this.txtInputChar.Text[0];
                        Typeface typeFace = _basicOptions.Typeface;
                        RenderWithMsdfImg(typeFace, testChar, _basicOptions.FontSizeInPoints);

                    }
                    break;
                case TypographyTest.RenderChoice.RenderWithMiniAgg_SingleGlyph:
                    {
                        selectedTextPrinter = _devVxsTextPrinter;
                        //for test only 1 char 
                        RenderSingleCharWithMiniAgg(
                             _basicOptions.Typeface,
                            this.txtInputChar.Text[0],
                            _basicOptions.FontSizeInPoints);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        void RenderSingleCharWithMiniAgg(Typeface typeface, char testChar, float sizeInPoint)
        {

            //---------------
            //set up vinfo
            UI.DebugGlyphVisualizerInfoView vinfo = debugGlyphVisualizer.VisualizeInfoView;

            if (vinfo == null)
            {
                vinfo = new UI.DebugGlyphVisualizerInfoView();
                vinfo.SetTreeView(glyphContourAnalysisOptionsUserControl1.DebugTreeView);
                vinfo.SetFlushOutputHander(() =>
                {
                    painter.SetOrigin(0, 0);
                    //6. use this util to copy image from Agg actual image to System.Drawing.Bitmap
                    PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(destImg, winBmp);
                    //--------------- 
                    //7. just render our bitmap
                    g.Clear(System.Drawing.Color.White);
                    g.DrawImage(winBmp, new System.Drawing.Point(30, 100));

                });
                debugGlyphVisualizer.VisualizeInfoView = vinfo;
            }

            //---------------
            //we use the debugGlyphVisualize the render it
            this.debugGlyphVisualizer.SetFont(typeface, sizeInPoint);
            debugGlyphVisualizer.CanvasPainter = painter;
            debugGlyphVisualizer.UseLcdTechnique = _contourAnalysisOpts.LcdTechnique;
            debugGlyphVisualizer.FillBackGround = _glyphRenderOptions.FillBackground;
            debugGlyphVisualizer.DrawBorder = _glyphRenderOptions.DrawBorder;

            debugGlyphVisualizer.ShowTess = _contourAnalysisOpts.ShowTess;
            debugGlyphVisualizer.WalkTrianglesAndEdges = _contourAnalysisOpts.ShowTriangle;
            debugGlyphVisualizer.DrawEndLineHub = _contourAnalysisOpts.DrawLineHubConn;
            debugGlyphVisualizer.DrawPerpendicularLine = _contourAnalysisOpts.DrawPerpendicularLine;
            debugGlyphVisualizer.WalkCentroidBone = _contourAnalysisOpts.DrawCentroidBone;
            debugGlyphVisualizer.WalkGlyphBone = _contourAnalysisOpts.DrawGlyphBone;

            debugGlyphVisualizer.GlyphEdgeOffset = _contourAnalysisOpts.EdgeOffset;

            debugGlyphVisualizer.DrawDynamicOutline = _contourAnalysisOpts.DynamicOutline;
            debugGlyphVisualizer.DrawRegenerateOutline = _contourAnalysisOpts.DrawRegenerationOutline;
            debugGlyphVisualizer.DrawGlyphPoint = _contourAnalysisOpts.DrawGlyphPoint;

#if DEBUG
            GlyphDynamicOutline.dbugTestNewGridFitting = _contourAnalysisOpts.EnableGridFit;
            GlyphDynamicOutline.dbugActualPosToConsole = _contourAnalysisOpts.WriteFitOutputToConsole;
            GlyphDynamicOutline.dbugUseHorizontalFitValue = _contourAnalysisOpts.UseHorizontalFitAlignment;
#endif


            //------------------------------------------------------

            debugGlyphVisualizer.RenderChar(testChar, _glyphRenderOptions.HintTechnique);
            //---------------------------------------------------- 

            //--------------------------
            if (_contourAnalysisOpts.ShowGrid)
            {
                //render grid
                RenderGrids(800, 600, _gridSize, painter);
            }
            painter.SetOrigin(0, 0);
            //6. use this util to copy image from Agg actual image to System.Drawing.Bitmap
            PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(destImg, winBmp);
            //--------------- 
            //7. just render our bitmap
            g.Clear(System.Drawing.Color.White);
            g.DrawImage(winBmp, new System.Drawing.Point(30, 100));
            //g.DrawRectangle(Pens.White, new System.Drawing.Rectangle(30, 20, winBmp.Width, winBmp.Height));
        }

        void RenderWithMsdfImg(Typeface typeface, char testChar, float sizeInPoint)
        {
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            //p.UseSubPixelRendering = chkLcdTechnique.Checked;
            painter.Clear(PixelFarm.Drawing.Color.White);
            //----------------------------------------------------
            var builder = new GlyphPathBuilder(typeface);
            builder.SetHintTechnique(_glyphRenderOptions.HintTechnique);

            //----------------------------------------------------
            builder.Build(testChar, sizeInPoint);
            //----------------------------------------------------
            var glyphToContour = new GlyphContourBuilder();
            var msdfGenPars = new MsdfGenParams();

            builder.ReadShapes(glyphToContour);
            //glyphToContour.Read(builder.GetOutputPoints(), builder.GetOutputContours());
            MsdfGenParams genParams = new MsdfGenParams();
            GlyphImage glyphImg = MsdfGlyphGen.CreateMsdfImage(glyphToContour, genParams);

            var actualImg = ActualImage.CreateFromBuffer(glyphImg.Width, glyphImg.Height, PixelFormat.ARGB32, glyphImg.GetImageBuffer());
            painter.DrawImage(actualImg, 0, 0);

            //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //{
            //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\a001_xn2_" + n + ".png");
            //}

            if (_contourAnalysisOpts.ShowGrid)
            {
                //render grid
                RenderGrids(800, 600, _gridSize, painter);
            }

            //6. use this util to copy image from Agg actual image to System.Drawing.Bitmap
            PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(destImg, winBmp);
            //--------------- 
            //7. just render our bitmap
            g.Clear(System.Drawing.Color.White);
            g.DrawImage(winBmp, new System.Drawing.Point(30, 20));
        }

        void RenderGrids(int width, int height, int sqSize, PixelFarm.Drawing.Painter p)
        {
            //render grid 
            p.FillColor = PixelFarm.Drawing.Color.Gray;

            float pointW = (sqSize >= 100) ? 2 : 1;

            for (int y = 0; y < height;)
            {
                for (int x = 0; x < width;)
                {
                    p.FillRect(x, y, pointW, pointW);
                    x += sqSize;
                }
                y += sqSize;
            }
        }






        VertexStorePool _vxsPool2 = new VertexStorePool();
        int _gridSize = 5;//default 


        static List<char> CreateCharList(UnicodeRangeInfo[] unicodeRanges)
        {
            List<char> outputChars = new List<char>();
            foreach (UnicodeRangeInfo r in unicodeRanges)
            {
                int startAt = r.StartAt;
                int endAt = r.EndAt;
                for (int i = startAt; i <= endAt; ++i)
                {
                    outputChars.Add((char)i);
                }
            }

            return outputChars;
        }
        private void cmdBuildMsdfTexture_Click(object sender, EventArgs e)
        {

            ////UnicodeRangeInfo latin1 = UnicodeLangBits.BasicLatin.ToUnicodeRangeInfo();
            ////UnicodeRangeInfo latinSupplement = UnicodeLangBits.Latin1Supplement.ToUnicodeRangeInfo(); 
            ////List<char> unicodeRanges = CreateCharList(new[] {
            ////        UnicodeLangBits.BasicLatin.ToUnicodeRangeInfo(),
            ////        UnicodeLangBits.Thai.ToUnicodeRangeInfo()
            ////    }
            ////);

            ////samples...
            ////1. create texture from specific glyph index range
            //string sampleFontFile = _basicOptions.Typeface.Filename;
            ////string sampleFontFile = "../../../TestFonts/tahoma.ttf";
            ////CreateSampleMsdfTextureFont(
            ////    sampleFontFile,
            ////    18,
            ////    0,
            ////    255,
            ////    "d:\\WImageTest\\sample_msdf.png");
            ////---------------------------------------------------------
            ////2. for debug, create from some unicode chars
            ////
            //string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-*/?=(){}[]%@#^$&|.";
            //CreateSampleTextureFontFromInputChars(
            //   TextureKind.Msdf,
            //   sampleFontFile,
            //   18,
            //   chars.ToCharArray(), //eg. ABCD
            //   "d:\\WImageTest\\glyph_gen\\sample_msdf1_" + Path.GetFileNameWithoutExtension(sampleFontFile) + ".png");

            ////CreateSampleTextureFontFromScriptLangs(
            ////   TextureKind.Msdf,
            ////   sampleFontFile,
            ////   18,
            ////   new[] { ScriptLangs.Latin, ScriptLangs.Thai },
            ////   "d:\\WImageTest\\sample_msdf2_" + Path.GetFileNameWithoutExtension(sampleFontFile) + ".png");

            ////---------------------------------------------------------
            //////3.
            ////GlyphTranslatorToContour tx = new GlyphTranslatorToContour();
            ////tx.BeginRead(1);
            //////tx.MoveTo(10, 10);
            //////tx.LineTo(25, 25);
            //////tx.LineTo(15, 10);
            ////tx.MoveTo(3.84f, 0);
            ////tx.LineTo(1.64f, 0);
            ////tx.LineTo(1.64f, 18.23f);
            ////tx.LineTo(3.84f, 18.23f);
            ////tx.CloseContour();
            ////tx.EndRead();
            //////
            ////CreateSampleMsdfImg(tx, "d:\\WImageTest\\tx_contour2.bmp");

        }



        //static ushort[] GetUniqueGlyphIndexList(List<ushort> inputGlyphIndexList)
        //{
        //    Dictionary<ushort, bool> uniqueGlyphIndices = new Dictionary<ushort, bool>(inputGlyphIndexList.Count);
        //    foreach (ushort glyphIndex in inputGlyphIndexList)
        //    {
        //        if (!uniqueGlyphIndices.ContainsKey(glyphIndex))
        //        {
        //            uniqueGlyphIndices.Add(glyphIndex, true);
        //        }
        //    }
        //    //
        //    ushort[] uniqueGlyphIndexArray = new ushort[uniqueGlyphIndices.Count];
        //    int i = 0;
        //    foreach (ushort glyphIndex in uniqueGlyphIndices.Keys)
        //    {
        //        uniqueGlyphIndexArray[i] = glyphIndex;
        //        i++;
        //    }
        //    return uniqueGlyphIndexArray;
        //}

        //static void CreateSampleTextureFontFromScriptLangs(
        //    TextureKind textureKind,
        //    string fontfile, float sizeInPoint,
        //    ScriptLang[] scLangs, string outputFile)
        //{

        //    //sample
        //    var reader = new OpenFontReader();
        //    //we need to collect a substituion glyph so we use glyph layout to collect
        //    //associate glyph index 
        //    using (var fs = new FileStream(fontfile, FileMode.Open))
        //    {
        //        //1. read typeface from font file
        //        Typeface typeface = reader.Read(fs);
        //        //2. find associated glyph index base on input script langs
        //        List<ushort> outputGlyphIndexList = new List<ushort>();
        //        //
        //        foreach (ScriptLang scLang in scLangs)
        //        {
        //            typeface.CollectAllAssociateGlyphIndex(outputGlyphIndexList, scLang);
        //        }
        //        //
        //        ushort[] allAssocGlyphIndices = GetUniqueGlyphIndexList(outputGlyphIndexList);
        //        CreateSampleTextureFontFromGlyphIndices(textureKind, typeface, sizeInPoint, allAssocGlyphIndices, outputFile);
        //    }
        //}
        //static void CreateSampleTextureFontFromGlyphIndices(
        //   TextureKind textureKind,
        //   string fontfile, float sizeInPoint,
        //   ushort[] glyphIndices, string outputFile)
        //{
        //    //sample
        //    var reader = new OpenFontReader();
        //    //we need to collect a substituion glyph so we use glyph layout to collect
        //    //associate glyph index 
        //    using (var fs = new FileStream(fontfile, FileMode.Open))
        //    {
        //        //1. read typeface from font file
        //        Typeface typeface = reader.Read(fs);
        //        //2.
        //        CreateSampleTextureFontFromGlyphIndices(textureKind, typeface, sizeInPoint, glyphIndices, outputFile);
        //    }
        //}
        //static void CreateSampleTextureFontFromInputChars(
        //    TextureKind textureKind,
        //    string fontfile, float sizeInPoint,
        //    char[] chars, string outputFile)
        //{
        //    //sample
        //    var reader = new OpenFontReader();
        //    //we need to collect a substituion glyph so we use glyph layout to collect
        //    //associate glyph index 
        //    using (var fs = new FileStream(fontfile, FileMode.Open))
        //    {
        //        //1. read typeface from font file
        //        Typeface typeface = reader.Read(fs);
        //        //2.

        //        //convert input chars into glyphIndex
        //        ushort[] glyphIndices = new ushort[chars.Length];
        //        int i = 0;
        //        foreach (char ch in chars)
        //        {
        //            glyphIndices[i] = typeface.LookupIndex(ch);
        //            i++;
        //        }

        //        CreateSampleTextureFontFromGlyphIndices(textureKind, typeface, sizeInPoint, glyphIndices, outputFile);
        //    }

        //}

        //static void CreateSampleTextureFontFromGlyphIndices(
        //  TextureKind textureKind,
        //  Typeface typeface, float sizeInPoint,
        //  ushort[] glyphIndices, string outputFile)
        //{

        //    //sample: create sample msdf texture 
        //    //-------------------------------------------------------------
        //    var builder = new GlyphPathBuilder(typeface);
        //    builder.UseTrueTypeInstructions = true;
        //    //-------------------------------------------------------------
        //    var atlasBuilder = new SimpleFontAtlasBuilder();
        //    MsdfGenParams msdfGenParams = new MsdfGenParams();
        //    //
        //    AggGlyphTextureGen aggTextureGen = new AggGlyphTextureGen();
        //    float pxscale = typeface.CalculateScaleToPixelFromPointSize(sizeInPoint);
        //    int j = glyphIndices.Length;
        //    for (int i = 0; i < j; ++i)
        //    {
        //        //build glyph
        //        ushort gindex = glyphIndices[i];
        //        builder.BuildFromGlyphIndex(gindex, -1);
        //        GlyphImage glyphImg = null;
        //        if (textureKind == TextureKind.Msdf)
        //        {
        //            var glyphToContour = new GlyphContourBuilder();
        //            //glyphToContour.Read(builder.GetOutputPoints(), builder.GetOutputContours());
        //            builder.ReadShapes(glyphToContour);
        //            msdfGenParams.shapeScale = 1f / 64;
        //            glyphImg = MsdfGlyphGen.CreateMsdfImage(glyphToContour, msdfGenParams);
        //        }
        //        else
        //        {
        //            //create alpha channel texture                      
        //            glyphImg = aggTextureGen.CreateGlyphImage(builder, pxscale);
        //        }
        //        atlasBuilder.AddGlyph(gindex, glyphImg);
        //        int w = glyphImg.Width;
        //        int h = glyphImg.Height;

        //        using (Bitmap bmp = new Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //        {
        //            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
        //            int[] imgBuffer = glyphImg.GetImageBuffer();
        //            System.Runtime.InteropServices.Marshal.Copy(imgBuffer, 0, bmpdata.Scan0, imgBuffer.Length);
        //            bmp.UnlockBits(bmpdata);
        //            bmp.Save("d:\\WImageTest\\glyph_gen\\a001_alpha_" + ((int)gindex) + ".png");
        //        }
        //    }

        //    var glyphImg2 = atlasBuilder.BuildSingleImage();
        //    using (Bitmap bmp = new Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //    {
        //        var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
        //            System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
        //        int[] intBuffer = glyphImg2.GetImageBuffer();

        //        System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
        //        bmp.UnlockBits(bmpdata);
        //        bmp.Save(outputFile);
        //    }
        //    atlasBuilder.SaveFontInfo(outputFile + ".xml");

        //}

        static void CreateSampleMsdfImg(GlyphContourBuilder tx, string outputFile)
        {
            //sample

            MsdfGenParams msdfGenParams = new MsdfGenParams();
            GlyphImage glyphImg = MsdfGlyphGen.CreateMsdfImage(tx, msdfGenParams);
            int w = glyphImg.Width;
            int h = glyphImg.Height;
            using (Bitmap bmp = new Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                int[] imgBuffer = glyphImg.GetImageBuffer();
                System.Runtime.InteropServices.Marshal.Copy(imgBuffer, 0, bmpdata.Scan0, imgBuffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save(outputFile);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ////create a simple stencil texture font
            //string sampleFontFile = _basicOptions.Typeface.Filename;
            ////string sampleFontFile = "../../../TestFonts/tahoma.ttf";
            //string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-*/?=(){}[]%@#^$&|.";
            //CreateSampleTextureFontFromInputChars(
            //   TextureKind.StencilGreyScale,
            //   sampleFontFile,
            //   18,
            //   chars.ToCharArray(), //eg. ABCD
            //   "d:\\WImageTest\\sample_stencil1_" + System.IO.Path.GetFileNameWithoutExtension(sampleFontFile) + ".png");
            ////
            //CreateSampleTextureFontFromScriptLangs(
            //  TextureKind.StencilGreyScale,
            //  sampleFontFile,
            //  18,
            //  new[] { ScriptLangs.Latin },
            //  "d:\\WImageTest\\sample_stencil2_" + System.IO.Path.GetFileNameWithoutExtension(sampleFontFile) + ".png");

        }


        private void button1_Click(object sender, EventArgs e)
        {
            UpdateRenderOutput();
        }

        private void glyphTextureBitmapGenUserControl1_Load(object sender, EventArgs e)
        {

        }


        //static void CreateSampleMsdfTextureFont(string fontfile, float sizeInPoint, ushort startGlyphIndex, ushort endGlyphIndex, string outputFile)
        //{
        //    //sample
        //    var reader = new OpenFontReader();

        //    using (var fs = new FileStream(fontfile, FileMode.Open))
        //    {
        //        //1. read typeface from font file
        //        Typeface typeface = reader.Read(fs);
        //        //sample: create sample msdf texture 
        //        //-------------------------------------------------------------
        //        var builder = new GlyphPathBuilder(typeface);
        //        //builder.UseTrueTypeInterpreter = this.chkTrueTypeHint.Checked;
        //        //builder.UseVerticalHinting = this.chkVerticalHinting.Checked;
        //        //-------------------------------------------------------------
        //        var atlasBuilder = new SimpleFontAtlasBuilder();


        //        for (ushort gindex = startGlyphIndex; gindex <= endGlyphIndex; ++gindex)
        //        {
        //            //build glyph
        //            builder.BuildFromGlyphIndex(gindex, sizeInPoint);

        //            var glyphToContour = new GlyphContourBuilder();
        //            //glyphToContour.Read(builder.GetOutputPoints(), builder.GetOutputContours());
        //            var genParams = new MsdfGenParams();
        //            builder.ReadShapes(glyphToContour);
        //            genParams.shapeScale = 1f / 64; //we scale later (as original C++ code use 1/64)
        //            GlyphImage glyphImg = MsdfGlyphGen.CreateMsdfImage(glyphToContour, genParams);
        //            atlasBuilder.AddGlyph(gindex, glyphImg);

        //            //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //            //{
        //            //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
        //            //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
        //            //    bmp.UnlockBits(bmpdata);
        //            //    bmp.Save("d:\\WImageTest\\a001_xn2_" + n + ".png");
        //            //}
        //        }

        //        var glyphImg2 = atlasBuilder.BuildSingleImage();
        //        using (Bitmap bmp = new Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //        {
        //            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
        //                System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
        //            int[] intBuffer = glyphImg2.GetImageBuffer();

        //            System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
        //            bmp.UnlockBits(bmpdata);
        //            bmp.Save("d:\\WImageTest\\a_total.png");
        //        }
        //        atlasBuilder.SaveFontInfo("d:\\WImageTest\\a_info.xml");
        //    }
        //}


    }
}