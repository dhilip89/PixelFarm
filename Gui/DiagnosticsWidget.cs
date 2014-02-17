﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatterHackers.Agg.UI
{
    public class DiagnosticWidget : SystemWindow
    {
        internal class WidgetInList
        {
            internal GuiWidget sizeLable;
            internal GuiWidget boundsLable;

            internal WidgetInList(GuiWidget sizeLable, GuiWidget boundsLable)
            {
                this.sizeLable = sizeLable;
                this.boundsLable = boundsLable;
            }
        }

        GuiWidget topLevelWindow;
        FlowLayoutWidget topToBottomTotal;

        Dictionary<GuiWidget, WidgetInList> widgetRefList = new Dictionary<GuiWidget, WidgetInList>();

        public DiagnosticWidget(GuiWidget topLevelWindow)
            : base(300, 600)
        {
            Title = "Widget Diagnostics";

            this.topLevelWindow = topLevelWindow;
            BackgroundColor = RGBA_Bytes.White;
            topLevelWindow.MouseMove += new MouseEventHandler(topLevelWindow_MouseMove);

            ShowAsSystemWindow();
        }

        public override void OnClosed(EventArgs e)
        {
            topLevelWindow.MouseMove -= new MouseEventHandler(topLevelWindow_MouseMove);
            foreach (KeyValuePair<GuiWidget, WidgetInList> keyValue in widgetRefList)
            {
                keyValue.Key.PositionChanged -= new EventHandler(updateWidgetInfo);
                keyValue.Key.BoundsChanged -= new EventHandler(updateWidgetInfo);
            }
            base.OnClosed(e);
        }

        void AddInfoRecursive(GuiWidget widgetToAddInfoAbout, FlowLayoutWidget layoutToAddInfoTo, int level = 0)
        {
            FlowLayoutWidget indented = new FlowLayoutWidget();
            indented.AddChild(new LineWidget(15, 5));

            FlowLayoutWidget widgetInfo = new FlowLayoutWidget(FlowDirection.TopToBottom);

            string info = widgetToAddInfoAbout.GetType().ToString();
            if (widgetToAddInfoAbout.Name != null && widgetToAddInfoAbout.Name != "")
            {
                info += " " + widgetToAddInfoAbout.Name;
            }
            else if (widgetToAddInfoAbout.Text != null && widgetToAddInfoAbout.Text != "")
            {
                info += " " + widgetToAddInfoAbout.Text;
            }

            widgetInfo.AddChild(new TextWidget(info));
            
            TextWidget sizeAndPositon = new TextWidget(string.Format("  Size {0}, Position {1}", widgetToAddInfoAbout.LocalBounds, widgetToAddInfoAbout.OriginRelativeParent), pointSize: 8, textColor: RGBA_Bytes.Red);
            sizeAndPositon.AutoExpandBoundsToText = true;
            widgetInfo.AddChild(sizeAndPositon);

            TextWidget boundsText = new TextWidget(string.Format("  Bounds {0}", widgetToAddInfoAbout.BoundsRelativeToParent), pointSize: 8, textColor: RGBA_Bytes.Red);
            boundsText.AutoExpandBoundsToText = true;
            widgetInfo.AddChild(boundsText);

            if (!widgetRefList.ContainsKey(widgetToAddInfoAbout))
            {
                widgetRefList.Add(widgetToAddInfoAbout, new WidgetInList(sizeAndPositon, boundsText));
                widgetToAddInfoAbout.PositionChanged += new EventHandler(updateWidgetInfo);
                widgetToAddInfoAbout.BoundsChanged += new EventHandler(updateWidgetInfo);
            }

            FlowLayoutWidget childrenWidgetInfo = new FlowLayoutWidget(FlowDirection.TopToBottom);

            indented.AddChild(childrenWidgetInfo);

            widgetInfo.AddChild(indented);

            foreach (GuiWidget child in widgetToAddInfoAbout.Children)
            {
                AddInfoRecursive(child, childrenWidgetInfo, level + 1);
            }

            layoutToAddInfoTo.AddChild(widgetInfo);
        }

        void updateWidgetInfo(object sender, EventArgs e)
        {
            GuiWidget widgetToAddInfoAbout = sender as GuiWidget;
            if (widgetToAddInfoAbout != null)
            {
                widgetRefList[widgetToAddInfoAbout].sizeLable.Text = string.Format("  Size {0}, Position {1}", widgetToAddInfoAbout.LocalBounds, widgetToAddInfoAbout.OriginRelativeParent);
                widgetRefList[widgetToAddInfoAbout].boundsLable.Text = string.Format("  Bounds {0}", widgetToAddInfoAbout.BoundsRelativeToParent);
            }
        }

        int count = 0;
        void topLevelWindow_MouseMove(object sender, MouseEventArgs mouseEvent)
        {
            count++;
            if (count == 20)
            {
                RemoveAllChildren();

                ScrollableWidget allContainer = new ScrollableWidget(true);
                topToBottomTotal = new FlowLayoutWidget(FlowDirection.TopToBottom);

                topToBottomTotal.SuspendLayout();
                GuiWidget.DefaultEnforceIntegerBounds = true;
                AddInfoRecursive(topLevelWindow, topToBottomTotal);
                GuiWidget.DefaultEnforceIntegerBounds = false;
                topToBottomTotal.ResumeLayout();
                topToBottomTotal.PerformLayout();
                allContainer.AddChild(topToBottomTotal);

                AddChild(allContainer);
                allContainer.AnchorAll();
            }
        }
    }

    public class LineWidget : GuiWidget
    {
        public LineWidget(double width, double height)
            : base(width, height)
        {
            VAnchor = UI.VAnchor.ParentBottomTop;
            VAnchor = UI.VAnchor.FitToChildren;
            HAnchor = UI.HAnchor.FitToChildren;
        }

        public override void OnDraw(Graphics2D graphics2D)
        {
            graphics2D.Line(Width / 2, 0, Width / 2, Height, RGBA_Bytes.Black);
            base.OnDraw(graphics2D);
        }
    }
}