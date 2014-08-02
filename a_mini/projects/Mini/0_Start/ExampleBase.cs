﻿//BSD 2014, WinterDev

using System;
using System.Collections.Generic;
using MatterHackers.Agg;


namespace Mini
{


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExInfoAttribute : Attribute
    {
        public ExInfoAttribute()
        {
        }
        public ExInfoAttribute(ExampleCategory catg)
        {
            this.Category = catg;
        }
        public ExInfoAttribute(string desc)
        {
            this.Description = desc;
        }
        public ExInfoAttribute(ExampleCategory catg, string desc)
        {
            this.Category = catg;
            this.Description = desc;
        }
        public string Description { get; private set; }
        public ExampleCategory Category { get; private set; }
        public string OrderCode { get; set; }
    }

    public enum ExampleCategory
    {
        Vector,
        Bitmap
    }
    public delegate Graphics2D RequestNewGraphic2DDelegate();

    public abstract class ExampleBase
    {
        public ExampleBase()
        {
            this.Width = 800;
            this.Height = 600;
        }
        public event RequestNewGraphic2DDelegate RequestNewGfx2d;

        public abstract void Draw(MatterHackers.Agg.Graphics2D g);
        public virtual void Init() { }
        public virtual void MouseDrag(int x, int y) { }
        public virtual void MouseDown(int x, int y) { }
        public virtual void MouseUp(int x, int y) { }
        public int Width { get; set; }
        public int Height { get; set; }

        protected Graphics2D NewGraphics2D()
        {
            if (RequestNewGfx2d == null)
            {
                throw new NotSupportedException();
            }
            else
            {
                return RequestNewGfx2d();
            }
        }

    }

    public class ExConfigAttribute : Attribute
    {
        public ExConfigAttribute()
        {

        }
        public ExConfigAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }


        public int MinValue { get; set; }
        public int MaxValue { get; set; }

    }
    enum ExConfigPresentaionHint
    {
        TextBox,
        CheckBox,
        OptionBoxes,
        SlideBarDiscrete,
        SlideBarContinuous
    }




    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NoteAttribute : Attribute
    {
        public NoteAttribute(string desc)
        {
            this.Desc = desc;
        }
        public string Desc { get; set; }
    }

    class ExampleConfigDesc
    {
        System.Reflection.PropertyInfo property;
        List<ExampleConfigValue> optionFields;

        public ExampleConfigDesc(ExConfigAttribute config, System.Reflection.PropertyInfo property)
        {
            this.property = property;
            this.OriginalConfigAttribute = config;
            if (!string.IsNullOrEmpty(config.Name))
            {
                this.Name = config.Name;
            }
            else
            {
                this.Name = property.Name;
            }


            Type propType = property.PropertyType;
            if (propType == typeof(bool))
            {
                this.PresentaionHint = ExConfigPresentaionHint.CheckBox;
            }
            else if (propType.IsEnum)
            {
                this.PresentaionHint = Mini.ExConfigPresentaionHint.OptionBoxes;
                //find option
                var enumFields = propType.GetFields();

                int j = enumFields.Length;
                optionFields = new List<ExampleConfigValue>(j);
                for (int i = 0; i < j; ++i)
                {
                    var enumField = enumFields[i];
                    if (enumField.IsStatic)
                    {
                        //use field name or note that assoc with its field name

                        string fieldNameOrNote = enumField.Name;
                        var foundNotAttr = enumField.GetCustomAttributes(typeof(NoteAttribute), false);
                        if (foundNotAttr.Length > 0)
                        {
                            fieldNameOrNote = ((NoteAttribute)foundNotAttr[0]).Desc;
                        }
                        optionFields.Add(new ExampleConfigValue(property, enumField, fieldNameOrNote));
                    }
                }
            }
            else if (propType == typeof(Int32))
            {
                this.PresentaionHint = Mini.ExConfigPresentaionHint.SlideBarDiscrete;
            }
            else if (propType == typeof(double))
            {
                this.PresentaionHint = Mini.ExConfigPresentaionHint.SlideBarContinuous;
            }
            else
            {
                this.PresentaionHint = ExConfigPresentaionHint.TextBox;
            }

        }
        public string Name
        {
            get;
            private set;
        }
        public ExConfigAttribute OriginalConfigAttribute
        {
            get;
            private set;
        }
        public ExConfigPresentaionHint PresentaionHint
        {
            get;
            private set;
        }
        public void InvokeSet(object target, object value)
        {
            this.property.GetSetMethod().Invoke(target, new object[] { value });
        }
        public object InvokeGet(object target)
        {
            return this.property.GetGetMethod().Invoke(target, null);
        }
        public List<ExampleConfigValue> GetOptionFields()
        {
            return this.optionFields;
        }
    }
    class ExampleAndDesc
    {
        static Type exConfig = typeof(ExConfigAttribute);
        static Type exInfoAttrType = typeof(ExInfoAttribute);

        List<ExampleConfigDesc> configList = new List<ExampleConfigDesc>();

        public ExampleAndDesc(Type t, string name)
        {
            this.Type = t;
            this.Name = name;
            this.OrderCode = "";
            var p1 = t.GetProperties();

            ExInfoAttribute[] exInfoList = t.GetCustomAttributes(exInfoAttrType, false) as ExInfoAttribute[];
            int m = exInfoList.Length;


            if (m > 0)
            {

                for (int n = 0; n < m; ++n)
                {
                    ExInfoAttribute info = exInfoList[n];
                    if (!string.IsNullOrEmpty(info.OrderCode))
                    {
                        this.OrderCode = info.OrderCode;
                    }

                    if (!string.IsNullOrEmpty(info.Description))
                    {
                        this.Description += " " + info.Description;
                    }
                }

            }
            if (string.IsNullOrEmpty(this.Description))
            {
                this.Description = this.Name;
            }



            foreach (var property in t.GetProperties())
            {
                if (property.DeclaringType == t)
                {
                    var foundAttrs = property.GetCustomAttributes(exConfig, false);
                    if (foundAttrs.Length > 0)
                    {
                        //this is configurable attrs
                        configList.Add(new ExampleConfigDesc((ExConfigAttribute)foundAttrs[0], property));
                    }
                }

            }
        }
        public Type Type { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return this.OrderCode + " : " + this.Name;
        }
        public List<ExampleConfigDesc> GetConfigList()
        {
            return this.configList;
        }
        public string Description
        {
            get;
            private set;
        }
        public string OrderCode
        {
            get;
            set;
        }
    }
    class ExampleConfigValue
    {

        System.Reflection.FieldInfo fieldInfo;
        System.Reflection.PropertyInfo property;
        public ExampleConfigValue(System.Reflection.PropertyInfo property, System.Reflection.FieldInfo fieldInfo, string name)
        {
            this.property = property;
            this.fieldInfo = fieldInfo;
            this.Name = name;
            this.ValueAsInt32 = (int)fieldInfo.GetValue(null);

        }
        public string Name { get; set; }
        public int ValueAsInt32 { get; private set; }

        public void InvokeSet(object target)
        {

            this.property.GetSetMethod().Invoke(target, new object[] { ValueAsInt32 });
        }
    }
}