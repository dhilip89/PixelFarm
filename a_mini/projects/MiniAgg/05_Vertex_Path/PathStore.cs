//2014 BSD,WinterDev   
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
using System;
using System.Collections.Generic;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg.VertexSource
{
    //---------------------------------------------------------------path_base
    // A container to store vertices with their flags. 
    // A path consists of a number of contours separated with "move_to" 
    // commands. The path storage can keep and maintain more than one
    // path. 
    // To navigate to the beginning of a particular path, use rewind(path_id);
    // Where path_id is what start_new_path() returns. So, when you call
    // start_new_path() you need to store its return value somewhere else
    // to navigate to the path afterwards.
    //
    // See also: vertex_source concept
    //------------------------------------------------------------------------ 
    public sealed class PathStore
    {
        VertexStore myvxs;
        public PathStore()
        {
            myvxs = new VertexStore();
        } 
         
        public int Count
        {
            get { return myvxs.Count; }
        }
        public void Clear()
        {
            myvxs.Clear();
        }

        // Make path functions
        //--------------------------------------------------------------------
        public int StartNewPath()
        {
            if (!ShapePath.IsStop(myvxs.GetLastCommand()))
            {
                myvxs.AddVertex(0.0, 0.0, ShapePath.FlagsAndCommand.CommandEmpty);
            }
            return myvxs.Count;
        }
        public void Stop()
        {
            myvxs.AddVertex(0, 0, ShapePath.FlagsAndCommand.CommandEmpty);
        }
        //--------------------------------------------------------------------


        void RelToAbs(ref double x, ref double y)
        {
            if (myvxs.Count != 0)
            {
                double x2;
                double y2;
                if (ShapePath.IsVertextCommand(myvxs.GetLastVertex(out x2, out y2)))
                {
                    x += x2;
                    y += y2;
                }
            }
        }

        public void MoveTo(double x, double y)
        {
            myvxs.AddMoveTo(x, y);
        }

        public void LineTo(double x, double y)
        {
            myvxs.AddLineTo(x, y);
        }

        public void HorizontalLineTo(double x)
        {
            myvxs.AddLineTo(x, GetLastY());
        }

        public void VerticalLineTo(double y)
        {
            myvxs.AddLineTo(GetLastX(), y);
        }

        //--------------------------------------------------------------------
        /// <summary>
        /// Draws a quadratic Bezier curve from the current point to (x,y) using (xControl,yControl) as the control point.
        /// </summary>
        /// <param name="xControl"></param>
        /// <param name="yControl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3(double xControl, double yControl, double x, double y)
        {
            myvxs.AddVertexCurve3(xControl, yControl);
            myvxs.AddVertexCurve3(x, y);
        }

        /// <summary>
        /// Draws a quadratic Bezier curve from the current point to (x,y) using (xControl,yControl) as the control point.
        /// </summary>
        /// <param name="xControl"></param>
        /// <param name="yControl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3Rel(double dx_ctrl, double dy_ctrl, double dx_to, double dy_to)
        {
            RelToAbs(ref dx_ctrl, ref dy_ctrl);
            RelToAbs(ref dx_to, ref dy_to);
            myvxs.AddVertexCurve3(dx_ctrl, dy_ctrl);
            myvxs.AddVertexCurve3(dx_to, dy_to);
        }

        /// <summary>
        /// <para>Draws a quadratic Bezier curve from the current point to (x,y).</para>
        /// <para>The control point is assumed to be the reflection of the control point on the previous command relative to the current point.</para>
        /// <para>(If there is no previous command or if the previous command was not a curve, assume the control point is coincident with the current point.)</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3(double x, double y)
        {
            double x0;
            double y0;
            if (ShapePath.IsVertextCommand(myvxs.GetLastVertex(out x0, out y0)))
            {
                double x_ctrl;
                double y_ctrl;
                ShapePath.FlagsAndCommand cmd = myvxs.GetBeforeLastVetex(out x_ctrl, out y_ctrl);
                if (ShapePath.IsCurve(cmd))
                {
                    x_ctrl = x0 + x0 - x_ctrl;
                    y_ctrl = y0 + y0 - y_ctrl;
                }
                else
                {
                    x_ctrl = x0;
                    y_ctrl = y0;
                }
                Curve3(x_ctrl, y_ctrl, x, y);
            }
        }

        /// <summary>
        /// <para>Draws a quadratic Bezier curve from the current point to (x,y).</para>
        /// <para>The control point is assumed to be the reflection of the control point on the previous command relative to the current point.</para>
        /// <para>(If there is no previous command or if the previous command was not a curve, assume the control point is coincident with the current point.)</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Curve3Rel(double dx_to, double dy_to)
        {
            RelToAbs(ref dx_to, ref dy_to);
            Curve3(dx_to, dy_to);
        }

        public void Curve4(double x_ctrl1, double y_ctrl1,
                                   double x_ctrl2, double y_ctrl2,
                                   double x_to, double y_to)
        {
            myvxs.AddVertexCurve4(x_ctrl1, y_ctrl1);
            myvxs.AddVertexCurve4(x_ctrl2, y_ctrl2);
            myvxs.AddVertexCurve4(x_to, y_to);
        }

        public void Curve4Rel(double dx_ctrl1, double dy_ctrl1,
                                       double dx_ctrl2, double dy_ctrl2,
                                       double dx_to, double dy_to)
        {
            RelToAbs(ref dx_ctrl1, ref dy_ctrl1);
            RelToAbs(ref dx_ctrl2, ref dy_ctrl2);
            RelToAbs(ref dx_to, ref dy_to);

            myvxs.AddVertexCurve4(dx_ctrl1, dy_ctrl1);
            myvxs.AddVertexCurve4(dx_ctrl2, dy_ctrl2);
            myvxs.AddVertexCurve4(dx_to, dy_to);

        }
      
        //--------------------------------------------------------------------
        public void Curve4(double x_ctrl2, double y_ctrl2,
                       double x_to, double y_to)
        {
            double x0;
            double y0;
            if (ShapePath.IsVertextCommand(GetLastVertex(out x0, out y0)))
            {
                double x_ctrl1;
                double y_ctrl1;
                ShapePath.FlagsAndCommand cmd = GetBeforeLastVertex(out x_ctrl1, out y_ctrl1);
                if (ShapePath.IsCurve(cmd))
                {
                    x_ctrl1 = x0 + x0 - x_ctrl1;
                    y_ctrl1 = y0 + y0 - y_ctrl1;
                }
                else
                {
                    x_ctrl1 = x0;
                    y_ctrl1 = y0;
                }
                Curve4(x_ctrl1, y_ctrl1, x_ctrl2, y_ctrl2, x_to, y_to);
            }
        }

        public void Curve4Rel(double dx_ctrl2, double dy_ctrl2,
                                       double dx_to, double dy_to)
        {
            RelToAbs(ref dx_ctrl2, ref dy_ctrl2);
            RelToAbs(ref dx_to, ref dy_to);
            Curve4(dx_ctrl2, dy_ctrl2, dx_to, dy_to);
        }

        //=======================================================================
        //TODO: implement arc to ***
        /*
        public void arc_to(double rx, double ry,
                               double angle,
                               bool large_arc_flag,
                               bool sweep_flag,
                               double x, double y)
    {
        if(m_vertices.total_vertices() && is_vertex(m_vertices.last_command()))
        {
            double epsilon = 1e-30;
            double x0 = 0.0;
            double y0 = 0.0;
            m_vertices.last_vertex(&x0, &y0);

            rx = fabs(rx);
            ry = fabs(ry);

            // Ensure radii are valid
            //-------------------------
            if(rx < epsilon || ry < epsilon) 
            {
                line_to(x, y);
                return;
            }

            if(calc_distance(x0, y0, x, y) < epsilon)
            {
                // If the endpoints (x, y) and (x0, y0) are identical, then this
                // is equivalent to omitting the elliptical arc segment entirely.
                return;
            }
            bezier_arc_svg a(x0, y0, rx, ry, angle, large_arc_flag, sweep_flag, x, y);
            if(a.radii_ok())
            {
                join_path(a);
            }
            else
            {
                line_to(x, y);
            }
        }
        else
        {
            move_to(x, y);
        }
    }

    public void arc_rel(double rx, double ry,
                                double angle,
                                bool large_arc_flag,
                                bool sweep_flag,
                                double dx, double dy)
    {
        rel_to_abs(&dx, &dy);
        arc_to(rx, ry, angle, large_arc_flag, sweep_flag, dx, dy);
    }
     */
        //=======================================================================

        //--------------------------------------------------------------------
        public VertexStore Vxs
        {
            get { return this.myvxs; }
        }
        public VertexStoreSnap MakeVertexSnap()
        {
            return new VertexStoreSnap(this.myvxs);
        }

        ShapePath.FlagsAndCommand GetLastVertex(out double x, out double y)
        {
            return myvxs.GetLastVertex(out x, out y);
        }

        ShapePath.FlagsAndCommand GetBeforeLastVertex(out double x, out double y)
        {
            return myvxs.GetBeforeLastVetex(out x, out y);
        }

        double GetLastX()
        {
            return myvxs.GetLastX();
        }

        double GetLastY()
        {
            return myvxs.GetLastY();
        }
         

        // Flip all vertices horizontally or vertically, 
        // between x1 and x2, or between y1 and y2 respectively
        //--------------------------------------------------------------------
        public void FlipX(double x1, double x2)
        {
            int i;
            double x, y;
            int count = this.myvxs.Count;
            for (i = 0; i < count; ++i)
            {
                ShapePath.FlagsAndCommand flags = myvxs.GetVertex(i, out x, out y);
                if (ShapePath.IsVertextCommand(flags))
                {
                    myvxs.ReplaceVertex(i, x2 - x + x1, y);
                }
            }
        }

        public void FlipY(double y1, double y2)
        {
            int i;
            double x, y;
            int count = this.myvxs.Count;
            for (i = 0; i < count; ++i)
            {
                ShapePath.FlagsAndCommand flags = myvxs.GetVertex(i, out x, out y);
                if (ShapePath.IsVertextCommand(flags))
                {
                    myvxs.ReplaceVertex(i, x, y2 - y + y1);
                }
            }
        }
        public void ClosePolygon()
        {
            ClosePolygon(ShapePath.FlagsAndCommand.FlagNone);
        }
        public void ClosePolygonCCW()
        {
            ClosePolygon(ShapePath.FlagsAndCommand.FlagCCW);
        }
        void ClosePolygon(ShapePath.FlagsAndCommand flags)
        {
            var flags2 = flags | ShapePath.FlagsAndCommand.FlagClose;

            if (ShapePath.IsVertextCommand(myvxs.GetLastCommand()))
            {
                myvxs.AddVertex(0.0, 0.0, ShapePath.FlagsAndCommand.CmdEndFigure | flags2);
            }
        }

        //// Concatenate path. The path is added as is.
        public void ConcatPath(VertexStoreSnap s)
        {
            double x, y;
            ShapePath.FlagsAndCommand cmd_flags;
            var snapIter = s.GetVertexSnapIter();
            while ((cmd_flags = snapIter.GetNextVertex(out x, out y)) != ShapePath.FlagsAndCommand.CommandEmpty)
            {
                myvxs.AddVertex(x, y, cmd_flags);
            }
        }

        //--------------------------------------------------------------------
        // Join path. The path is joined with the existing one, that is, 
        // it behaves as if the pen of a plotter was always down (drawing)
        //template<class VertexSource>  
        public void JoinPath(VertexStoreSnap s)
        {
            double x, y;
            var snapIter = s.GetVertexSnapIter();
            ShapePath.FlagsAndCommand cmd = snapIter.GetNextVertex(out x, out y);
            if (cmd == ShapePath.FlagsAndCommand.CommandEmpty)
            {
                return;
            }

            if (ShapePath.IsVertextCommand(cmd))
            {
                double x0, y0;
                ShapePath.FlagsAndCommand flags0 = GetLastVertex(out x0, out y0);

                if (ShapePath.IsVertextCommand(flags0))
                {
                    if (AggMath.calc_distance(x, y, x0, y0) > AggMath.VERTEX_DISTANCE_EPSILON)
                    {
                        if (ShapePath.IsMoveTo(cmd))
                        {
                            cmd = ShapePath.FlagsAndCommand.CommandLineTo;
                        }
                        myvxs.AddVertex(x, y, cmd);
                    }
                }
                else
                {
                    if (ShapePath.IsStop(flags0))
                    {
                        cmd = ShapePath.FlagsAndCommand.CommandMoveTo;
                    }
                    else
                    {
                        if (ShapePath.IsMoveTo(cmd))
                        {
                            cmd = ShapePath.FlagsAndCommand.CommandLineTo;
                        }
                    }
                    myvxs.AddVertex(x, y, cmd);
                }
            }

            while ((cmd = snapIter.GetNextVertex(out x, out y)) != ShapePath.FlagsAndCommand.CommandEmpty)
            {
                myvxs.AddVertex(x, y, ShapePath.IsMoveTo(cmd) ? ShapePath.FlagsAndCommand.CommandLineTo : cmd);
            }

        }
            

        public void TranslateAll(double dx, double dy)
        {
            int index;
            int num_ver = myvxs.Count;
            for (index = 0; index < num_ver; index++)
            {
                double x, y;
                if (ShapePath.IsVertextCommand(myvxs.GetVertex(index, out x, out y)))
                {
                    x += dx;
                    y += dy;
                    myvxs.ReplaceVertex(index, x, y);
                }
            }
        } 

        //--------------------------------------------------------------------
        public void TransformAll(Transform.Affine trans)
        {
            int index;
            int num_ver = myvxs.Count;
            for (index = 0; index < num_ver; index++)
            {
                double x, y;
                if (ShapePath.IsVertextCommand(myvxs.GetVertex(index, out x, out y)))
                {
                    trans.Transform(ref x, ref y);
                    myvxs.ReplaceVertex(index, x, y);
                }
            }
        }

      
        //----------------------------------------------------------

        public static void UnsafeDirectSetData(
            PathStore pathStore,
            int m_allocated_vertices,
            int m_num_vertices,
            double[] m_coord_xy,
            ShapePath.FlagsAndCommand[] m_CommandAndFlags)
        {

            VertexStore.UnsafeDirectSetData(
                pathStore.myvxs,
                m_allocated_vertices,
                m_num_vertices,
                m_coord_xy,
                m_CommandAndFlags);
        }
        public static void UnsafeDirectGetData(
            PathStore pathStore,
            out int m_allocated_vertices,
            out int m_num_vertices,
            out double[] m_coord_xy,
            out ShapePath.FlagsAndCommand[] m_CommandAndFlags)
        {
            VertexStore.UnsafeDirectGetData(
                pathStore.myvxs,
                out m_allocated_vertices,
                out m_num_vertices,
                out m_coord_xy,
                out m_CommandAndFlags);
        }

    }
}