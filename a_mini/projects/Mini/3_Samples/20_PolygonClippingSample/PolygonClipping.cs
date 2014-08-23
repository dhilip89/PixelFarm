﻿//2014 BSD, WinterDev
//MatterHackers

using System;
using System.Collections.Generic;

using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;

using MatterHackers.VectorMath;
using MatterHackers.Agg.Transform;

using Mini;
using ClipperLib;

namespace MatterHackers.Agg.Sample_PolygonClipping
{

    public enum OperationOption
    {
        None,
        OR,
        AND,
        XOR,
        [Note("A-B")]
        A_B,
        [Note("B-A")]
        B_A,
    }

    public enum PolygonExampleSet
    {
        [Note("Two Simple Paths")]
        TwoSimplePaths,
        [Note("Closed Stroke")]
        CloseStroke,
        [Note("Great Britain and Arrows")]
        GBAndArrow,
        [Note("Great Britain and Spiral")]
        GBAndSpiral,
        [Note("Spiral and Glyph")]
        SprialAndGlyph
    }


    [Info(OrderCode = "20")]
    public class PolygonClippingDemo : DemoBase
    {
        PathStorage CombinePaths(IVertexSource a, IVertexSource b, ClipType clipType)
        {
            List<List<IntPoint>> aPolys = CreatePolygons(a);
            List<List<IntPoint>> bPolys = CreatePolygons(b);

            Clipper clipper = new Clipper();

            clipper.AddPaths(aPolys, PolyType.ptSubject, true);
            clipper.AddPaths(bPolys, PolyType.ptClip, true);

            List<List<IntPoint>> intersectedPolys = new List<List<IntPoint>>();
            clipper.Execute(clipType, intersectedPolys);

            PathStorage output = new PathStorage();

            foreach (List<IntPoint> polygon in intersectedPolys)
            {
                bool first = true;
                foreach (IntPoint point in polygon)
                {
                    if (first)
                    {
                        output.Add(point.X / 1000.0, point.Y / 1000.0, ShapePath.FlagsAndCommand.CommandMoveTo);
                        first = false;
                    }
                    else
                    {
                        output.Add(point.X / 1000.0, point.Y / 1000.0, ShapePath.FlagsAndCommand.CommandLineTo);
                    }
                }

                output.ClosePolygon();
            }

            output.Add(0, 0, ShapePath.FlagsAndCommand.CommandStop);

            return output;
        }

        private static List<List<IntPoint>> CreatePolygons(IVertexSource a)
        {
            List<List<IntPoint>> allPolys = new List<List<IntPoint>>();
            List<IntPoint> currentPoly = null;
            VertexData last = new VertexData();
            VertexData first = new VertexData();
            bool addedFirst = false;
            foreach (VertexData vertexData in a.Vertices())
            {
                if (vertexData.IsLineTo)
                {
                    if (!addedFirst)
                    {
                        currentPoly.Add(new IntPoint((long)(last.position.x * 1000), (long)(last.position.y * 1000)));
                        addedFirst = true;
                        first = last;
                    }
                    currentPoly.Add(new IntPoint((long)(vertexData.position.x * 1000), (long)(vertexData.position.y * 1000)));
                    last = vertexData;
                }
                else
                {
                    addedFirst = false;
                    currentPoly = new List<IntPoint>();
                    allPolys.Add(currentPoly);
                    if (vertexData.IsMoveTo)
                    {
                        last = vertexData;
                    }
                    else
                    {
                        last = first;
                    }
                }
            }

            return allPolys;
        }

        double m_x;
        double m_y;
        ColorRGBA BackgroundColor;


        public PolygonClippingDemo()
        {
            BackgroundColor = ColorRGBA.White;
            this.Width = 800;
            this.Height = 600;
        }
        [DemoConfig]
        public PolygonExampleSet PolygonSet
        {
            get;
            set;
        }
        [DemoConfig]
        public OperationOption OpOption
        {
            get;
            set;
        }

        public override void Draw(Graphics2D g)
        {
            if (BackgroundColor.Alpha0To255 > 0)
            {
                g.FillRectangle(new RectangleDouble(0, 0, this.Width, Height), BackgroundColor);
            }
            render_gpc(g);
        }

        void render_gpc(Graphics2D graphics2D)
        {

            switch (this.PolygonSet)
            {
                case PolygonExampleSet.TwoSimplePaths:
                    {
                        //------------------------------------
                        // Two simple paths
                        //
                        PathStorage ps1 = new PathStorage();
                        PathStorage ps2 = new PathStorage();

                        double x = m_x - Width / 2 + 100;
                        double y = m_y - Height / 2 + 100;
                        ps1.MoveTo(x + 140, y + 145);
                        ps1.LineTo(x + 225, y + 44);
                        ps1.LineTo(x + 296, y + 219);
                        ps1.ClosePolygon();

                        ps1.LineTo(x + 226, y + 289);
                        ps1.LineTo(x + 82, y + 292);

                        ps1.MoveTo(x + 220, y + 222);
                        ps1.LineTo(x + 363, y + 249);
                        ps1.LineTo(x + 265, y + 331);

                        ps1.MoveTo(x + 242, y + 243);
                        ps1.LineTo(x + 268, y + 309);
                        ps1.LineTo(x + 325, y + 261);

                        ps1.MoveTo(x + 259, y + 259);
                        ps1.LineTo(x + 273, y + 288);
                        ps1.LineTo(x + 298, y + 266);

                        ps2.MoveTo(100 + 32, 100 + 77);
                        ps2.LineTo(100 + 473, 100 + 263);
                        ps2.LineTo(100 + 351, 100 + 290);
                        ps2.LineTo(100 + 354, 100 + 374);

                        graphics2D.Render(ps1, new ColorRGBAf(0, 0, 0, 0.1).GetAsRGBA_Bytes());
                        graphics2D.Render(ps2, new ColorRGBAf(0, 0.6, 0, 0.1).GetAsRGBA_Bytes());

                        CreateAndRenderCombined(graphics2D, ps1, ps2);
                    }
                    break;

                case PolygonExampleSet.CloseStroke:
                    {
                        //------------------------------------
                        // Closed stroke
                        //
                        PathStorage ps1 = new PathStorage();
                        PathStorage ps2 = new PathStorage();
                        Stroke stroke = new Stroke(ps2);
                        stroke.width(10.0);

                        double x = m_x - Width / 2 + 100;
                        double y = m_y - Height / 2 + 100;
                        ps1.MoveTo(x + 140, y + 145);
                        ps1.LineTo(x + 225, y + 44);
                        ps1.LineTo(x + 296, y + 219);
                        ps1.ClosePolygon();

                        ps1.LineTo(x + 226, y + 289);
                        ps1.LineTo(x + 82, y + 292);

                        ps1.MoveTo(x + 220 - 50, y + 222);
                        ps1.LineTo(x + 265 - 50, y + 331);
                        ps1.LineTo(x + 363 - 50, y + 249);
                        ps1.close_polygon(ShapePath.FlagsAndCommand.FlagCCW);

                        ps2.MoveTo(100 + 32, 100 + 77);
                        ps2.LineTo(100 + 473, 100 + 263);
                        ps2.LineTo(100 + 351, 100 + 290);
                        ps2.LineTo(100 + 354, 100 + 374);
                        ps2.ClosePolygon();

                        graphics2D.Render(ps1, new ColorRGBAf(0, 0, 0, 0.1).GetAsRGBA_Bytes());
                        graphics2D.Render(stroke, new ColorRGBAf(0, 0.6, 0, 0.1).GetAsRGBA_Bytes());

                        CreateAndRenderCombined(graphics2D, ps1, stroke);
                    }
                    break;


                case PolygonExampleSet.GBAndArrow:
                    {
                        //------------------------------------
                        // Great Britain and Arrows
                        //
                        PathStorage gb_poly = new PathStorage();
                        PathStorage arrows = new PathStorage();
                        MatterHackers.Agg.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);

                        make_arrows(arrows);

                        Affine mtx1 = Affine.NewIdentity();
                        Affine mtx2 = Affine.NewIdentity();
                        mtx1 *= Affine.NewTranslation(-1150, -1150);
                        mtx1 *= Affine.NewScaling(2.0);

                        mtx2 = mtx1;
                        mtx2 *= Affine.NewTranslation(m_x - Width / 2, m_y - Height / 2);

                        VertexSourceApplyTransform trans_gb_poly = new VertexSourceApplyTransform(gb_poly, mtx1);
                        VertexSourceApplyTransform trans_arrows = new VertexSourceApplyTransform(arrows, mtx2);

                        graphics2D.Render(trans_gb_poly, new ColorRGBAf(0.5, 0.5, 0, 0.1).GetAsRGBA_Bytes());

                        Stroke stroke_gb_poly = new Stroke(trans_gb_poly);
                        stroke_gb_poly.Width = 0.1;
                        graphics2D.Render(stroke_gb_poly, new ColorRGBAf(0, 0, 0).GetAsRGBA_Bytes());

                        graphics2D.Render(trans_arrows, new ColorRGBAf(0.0, 0.5, 0.5, 0.1).GetAsRGBA_Bytes());

                        CreateAndRenderCombined(graphics2D, trans_gb_poly, trans_arrows);
                    }
                    break;

                case PolygonExampleSet.GBAndSpiral:
                    {
                        //------------------------------------
                        // Great Britain and a Spiral
                        //
                        spiral sp = new spiral(m_x, m_y, 10, 150, 30, 0.0);
                        Stroke stroke = new Stroke(sp);
                        stroke.width(15.0);

                        PathStorage gb_poly = new PathStorage();
                        MatterHackers.Agg.Sample_PolygonClipping.GreatBritanPathStorage.Make(gb_poly);

                        Affine mtx = Affine.NewIdentity(); ;
                        mtx *= Affine.NewTranslation(-1150, -1150);
                        mtx *= Affine.NewScaling(2.0);

                        VertexSourceApplyTransform trans_gb_poly = new VertexSourceApplyTransform(gb_poly, mtx);

                        graphics2D.Render(trans_gb_poly, new ColorRGBAf(0.5, 0.5, 0, 0.1).GetAsRGBA_Bytes());

                        Stroke stroke_gb_poly = new Stroke(trans_gb_poly);
                        stroke_gb_poly.width(0.1);
                        graphics2D.Render(stroke_gb_poly, new ColorRGBAf(0, 0, 0).GetAsRGBA_Bytes());

                        graphics2D.Render(stroke, new ColorRGBAf(0.0, 0.5, 0.5, 0.1).GetAsRGBA_Bytes());

                        CreateAndRenderCombined(graphics2D, trans_gb_poly, stroke);
                    }
                    break;

                case PolygonExampleSet.SprialAndGlyph:
                    {
                        //------------------------------------
                        // Spiral and glyph
                        //
                        spiral sp = new spiral(m_x, m_y, 10, 150, 30, 0.0);
                        Stroke stroke = new Stroke(sp);
                        stroke.width(15.0);

                        PathStorage glyph = new PathStorage();
                        glyph.MoveTo(28.47, 6.45);
                        glyph.curve3(21.58, 1.12, 19.82, 0.29);
                        glyph.curve3(17.19, -0.93, 14.21, -0.93);
                        glyph.curve3(9.57, -0.93, 6.57, 2.25);
                        glyph.curve3(3.56, 5.42, 3.56, 10.60);
                        glyph.curve3(3.56, 13.87, 5.03, 16.26);
                        glyph.curve3(7.03, 19.58, 11.99, 22.51);
                        glyph.curve3(16.94, 25.44, 28.47, 29.64);
                        glyph.LineTo(28.47, 31.40);
                        glyph.curve3(28.47, 38.09, 26.34, 40.58);
                        glyph.curve3(24.22, 43.07, 20.17, 43.07);
                        glyph.curve3(17.09, 43.07, 15.28, 41.41);
                        glyph.curve3(13.43, 39.75, 13.43, 37.60);
                        glyph.LineTo(13.53, 34.77);
                        glyph.curve3(13.53, 32.52, 12.38, 31.30);
                        glyph.curve3(11.23, 30.08, 9.38, 30.08);
                        glyph.curve3(7.57, 30.08, 6.42, 31.35);
                        glyph.curve3(5.27, 32.62, 5.27, 34.81);
                        glyph.curve3(5.27, 39.01, 9.57, 42.53);
                        glyph.curve3(13.87, 46.04, 21.63, 46.04);
                        glyph.curve3(27.59, 46.04, 31.40, 44.04);
                        glyph.curve3(34.28, 42.53, 35.64, 39.31);
                        glyph.curve3(36.52, 37.21, 36.52, 30.71);
                        glyph.LineTo(36.52, 15.53);
                        glyph.curve3(36.52, 9.13, 36.77, 7.69);
                        glyph.curve3(37.01, 6.25, 37.57, 5.76);
                        glyph.curve3(38.13, 5.27, 38.87, 5.27);
                        glyph.curve3(39.65, 5.27, 40.23, 5.62);
                        glyph.curve3(41.26, 6.25, 44.19, 9.18);
                        glyph.LineTo(44.19, 6.45);
                        glyph.curve3(38.72, -0.88, 33.74, -0.88);
                        glyph.curve3(31.35, -0.88, 29.93, 0.78);
                        glyph.curve3(28.52, 2.44, 28.47, 6.45);
                        glyph.ClosePolygon();

                        glyph.MoveTo(28.47, 9.62);
                        glyph.LineTo(28.47, 26.66);
                        glyph.curve3(21.09, 23.73, 18.95, 22.51);
                        glyph.curve3(15.09, 20.36, 13.43, 18.02);
                        glyph.curve3(11.77, 15.67, 11.77, 12.89);
                        glyph.curve3(11.77, 9.38, 13.87, 7.06);
                        glyph.curve3(15.97, 4.74, 18.70, 4.74);
                        glyph.curve3(22.41, 4.74, 28.47, 9.62);
                        glyph.ClosePolygon();

                        Affine mtx = Affine.NewIdentity();
                        mtx *= Affine.NewScaling(4.0);
                        mtx *= Affine.NewTranslation(220, 200);
                        VertexSourceApplyTransform trans = new VertexSourceApplyTransform(glyph, mtx);
                        FlattenCurves curve = new FlattenCurves(trans);

                        CreateAndRenderCombined(graphics2D, stroke, curve);

                        graphics2D.Render(stroke, new ColorRGBAf(0, 0, 0, 0.1).GetAsRGBA_Bytes());

                        graphics2D.Render(curve, new ColorRGBAf(0, 0.6, 0, 0.1).GetAsRGBA_Bytes());
                    }
                    break;
            }
        }

        private void CreateAndRenderCombined(Graphics2D graphics2D, IVertexSource ps1, IVertexSource ps2)
        {
            PathStorage combined = null;

            switch (this.OpOption)
            {
                case OperationOption.OR:
                    combined = CombinePaths(ps1, ps2, ClipType.ctUnion);
                    break;
                case OperationOption.AND:
                    combined = CombinePaths(ps1, ps2, ClipType.ctIntersection);
                    break;
                case OperationOption.XOR:
                    combined = CombinePaths(ps1, ps2, ClipType.ctXor);
                    break;
                case OperationOption.A_B:
                    combined = CombinePaths(ps1, ps2, ClipType.ctDifference);
                    break;
                case OperationOption.B_A:
                    combined = CombinePaths(ps2, ps1, ClipType.ctDifference);
                    break;
            }

            if (combined != null)
            {
                graphics2D.Render(combined, new ColorRGBAf(0.5, 0.0, 0, 0.5).GetAsRGBA_Bytes());
            }
        }

        public override void MouseDrag(int x, int y)
        {
            m_x = x;
            m_y = y;
        }
        public override void MouseDown(int x, int y,bool isRightoy)
        {
            m_x = x;
            m_y = y;
        }
        public override void MouseUp(int x, int y)
        {
            m_x = x;
            m_y = y;
        }
        void make_arrows(PathStorage ps)
        {
            ps.Clear();
            ps.MoveTo(1330.599999999999909, 1282.399999999999864);
            ps.LineTo(1377.400000000000091, 1282.399999999999864);
            ps.LineTo(1361.799999999999955, 1298.000000000000000);
            ps.LineTo(1393.000000000000000, 1313.599999999999909);
            ps.LineTo(1361.799999999999955, 1344.799999999999955);
            ps.LineTo(1346.200000000000045, 1313.599999999999909);
            ps.LineTo(1330.599999999999909, 1329.200000000000045);
            ps.ClosePolygon();

            ps.MoveTo(1330.599999999999909, 1266.799999999999955);
            ps.LineTo(1377.400000000000091, 1266.799999999999955);
            ps.LineTo(1361.799999999999955, 1251.200000000000045);
            ps.LineTo(1393.000000000000000, 1235.599999999999909);
            ps.LineTo(1361.799999999999955, 1204.399999999999864);
            ps.LineTo(1346.200000000000045, 1235.599999999999909);
            ps.LineTo(1330.599999999999909, 1220.000000000000000);
            ps.ClosePolygon();

            ps.MoveTo(1315.000000000000000, 1282.399999999999864);
            ps.LineTo(1315.000000000000000, 1329.200000000000045);
            ps.LineTo(1299.400000000000091, 1313.599999999999909);
            ps.LineTo(1283.799999999999955, 1344.799999999999955);
            ps.LineTo(1252.599999999999909, 1313.599999999999909);
            ps.LineTo(1283.799999999999955, 1298.000000000000000);
            ps.LineTo(1268.200000000000045, 1282.399999999999864);
            ps.ClosePolygon();

            ps.MoveTo(1268.200000000000045, 1266.799999999999955);
            ps.LineTo(1315.000000000000000, 1266.799999999999955);
            ps.LineTo(1315.000000000000000, 1220.000000000000000);
            ps.LineTo(1299.400000000000091, 1235.599999999999909);
            ps.LineTo(1283.799999999999955, 1204.399999999999864);
            ps.LineTo(1252.599999999999909, 1235.599999999999909);
            ps.LineTo(1283.799999999999955, 1251.200000000000045);
            ps.ClosePolygon();
        }
    }



    public class spiral : IVertexSource
    {
        double m_x;
        double m_y;
        double m_r1;
        double m_r2;
        double m_step;
        double m_start_angle;

        double m_angle;
        double m_curr_r;
        double m_da;
        double m_dr;
        bool m_start;

        public spiral(double x, double y, double r1, double r2, double step, double start_angle = 0)
        {
            m_x = x;
            m_y = y;
            m_r1 = r1;
            m_r2 = r2;
            m_step = step;
            m_start_angle = start_angle;
            m_angle = start_angle;
            m_da = AggBasics.deg2rad(4.0);
            m_dr = m_step / 90.0;
        }

        public IEnumerable<VertexData> Vertices()
        {
            throw new NotImplementedException();
        }

        public void rewind(int index)
        {
            m_angle = m_start_angle;
            m_curr_r = m_r1;
            m_start = true;
        }

        public ShapePath.FlagsAndCommand vertex(out double x, out double y)
        {
            x = 0;
            y = 0;
            if (m_curr_r > m_r2)
            {
                return ShapePath.FlagsAndCommand.CommandStop;
            }

            x = m_x + Math.Cos(m_angle) * m_curr_r;
            y = m_y + Math.Sin(m_angle) * m_curr_r;
            m_curr_r += m_dr;
            m_angle += m_da;
            if (m_start)
            {
                m_start = false;
                return ShapePath.FlagsAndCommand.CommandMoveTo;
            }
            return ShapePath.FlagsAndCommand.CommandLineTo;
        }
    }

    class conv_poly_counter
    {
        int m_contours;
        int m_points;

        conv_poly_counter(IVertexSource src)
        {
            m_contours = 0;
            m_points = 0;

            foreach (VertexData vertexData in src.Vertices())
            {
                if (ShapePath.is_vertex(vertexData.command))
                {
                    ++m_points;
                }

                if (ShapePath.is_move_to(vertexData.command))
                {
                    ++m_contours;
                }
            }
        }
    }
}
