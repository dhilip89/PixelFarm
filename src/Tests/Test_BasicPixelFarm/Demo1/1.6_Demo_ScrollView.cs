﻿//Apache2, 2014-2018, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.6 ScrollView")]
    class Demo_ScrollView : DemoBase
    {

        protected override void OnStartDemo(SampleViewport viewport)
        {
            //AddScrollView1(viewport, 0, 0);
            AddScrollView2(viewport, 10, 0);
        }

        void AddScrollView1(SampleViewport viewport, int x, int y)
        {
            var panel = new LayoutFarm.CustomWidgets.SimpleBox(200, 175);
            panel.NeedClipArea = true;
            panel.SetLocation(x + 30, y + 30);
            panel.BackColor = Color.LightGray;
            viewport.AddContent(panel);
            //-------------------------  
            {
                //vertical scrollbar
                var vscbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                vscbar.SetLocation(x + 10, y + 10);
                vscbar.MinValue = 0;
                vscbar.MaxValue = 170;
                vscbar.SmallChange = 20;
                viewport.AddContent(vscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(vscbar.SliderBox, panel);
            }
            //-------------------------  
            {
                //horizontal scrollbar
                var hscbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                hscbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                hscbar.SetLocation(x + 30, y + 10);
                hscbar.MinValue = 0;
                hscbar.MaxValue = 170;
                hscbar.SmallChange = 20;
                viewport.AddContent(hscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(hscbar.SliderBox, panel);
            }

            //add content to panel
            for (int i = 0; i < 10; ++i)
            {
                var box1 = new LayoutFarm.CustomWidgets.SimpleBox(400, 30);
                box1.HasSpecificWidth = true;
                box1.HasSpecificHeight = true;
                box1.BackColor = Color.OrangeRed;
                box1.SetLocation(i * 20, i * 40);
                panel.AddChild(box1);
            }
            //--------------------------   
            panel.PerformContentLayout();
            panel.SetViewport(0, 0);
        }
        void AddScrollView2(SampleViewport viewport, int x, int y)
        {
            var panel = new LayoutFarm.CustomWidgets.SimpleBox(800, 1000);
            panel.NeedClipArea = true;
            panel.SetLocation(x + 10, y + 30);
            panel.BackColor = Color.LightGray;
            panel.ContentLayoutKind = CustomWidgets.BoxContentLayoutKind.VerticalStack;
            viewport.AddContent(panel);
            //-------------------------  
            //load images...

            //check folder before load
            string[] fileNames = new string[0];

            if (System.IO.Directory.Exists("../../Data/imgs"))
            {
                fileNames = System.IO.Directory.GetFiles("../../Data/imgs", "0*.jpg");
            }
            //select only
            int lastY = 0;
            ImageBinder binder = viewport.GetImageBinder(fileNames[2]);

            for (int i = 0; i < fileNames.Length * 4; ++i) //5 imgs
            {
                var imgbox = new LayoutFarm.CustomWidgets.ImageBox(36, 400);
                imgbox.ImageBinder = binder;
                imgbox.BackColor = Color.OrangeRed;
                imgbox.SetLocation(0, lastY);
                imgbox.MouseUp += (s, e) =>
                {
                    if (e.Button == UIMouseButtons.Right)
                    {
                        //test remove this imgbox on right mouse click
                        panel.RemoveChild(imgbox);
                    }
                };
                lastY += imgbox.Height + 5;
                panel.AddChild(imgbox);
            }
            //--------------------------
            //panel may need more 
            panel.SetViewport(0, 0);
            //-------------------------  
            {
                //vertical scrollbar
                var vscbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                vscbar.SetLocation(x + 10, y + 10);
                vscbar.MinValue = 0;
                vscbar.MaxValue = lastY;
                vscbar.SmallChange = 20;
                viewport.AddContent(vscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(vscbar.SliderBox, panel);
            }
            //-------------------------  
            {
                //horizontal scrollbar
                var hscbar = new LayoutFarm.CustomWidgets.ScrollBar(150, 15);
                hscbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                hscbar.SetLocation(x + 30, y + 10);
                hscbar.MinValue = 0;
                hscbar.MaxValue = 170;
                hscbar.SmallChange = 20;
                viewport.AddContent(hscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(hscbar.SliderBox, panel);
            }
            panel.PerformContentLayout();
        }

    }
}