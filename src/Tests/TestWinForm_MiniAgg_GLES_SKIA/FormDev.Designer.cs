﻿namespace Mini
{
    partial class FormDev
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.chkGdiAntiAlias = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.cmdTestNativeLib = new System.Windows.Forms.Button();
            this.cmdSignedDistance = new System.Windows.Forms.Button();
            this.cmdTestColorBlend = new System.Windows.Forms.Button();
            this.lstBackEndRenderer = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(234, 433);
            this.listBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(252, 413);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 32);
            this.button1.TabIndex = 1;
            this.button1.Text = "Create-Save-Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(417, 413);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "Test Filter";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(417, 375);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 32);
            this.button3.TabIndex = 3;
            this.button3.Text = "test font path";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(252, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(429, 317);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(252, 375);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(150, 32);
            this.button4.TabIndex = 5;
            this.button4.Text = "TestBz";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(573, 375);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(150, 32);
            this.button5.TabIndex = 6;
            this.button5.Text = "TestGdiPlus";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // chkGdiAntiAlias
            // 
            this.chkGdiAntiAlias.AutoSize = true;
            this.chkGdiAntiAlias.Location = new System.Drawing.Point(596, 14);
            this.chkGdiAntiAlias.Name = "chkGdiAntiAlias";
            this.chkGdiAntiAlias.Size = new System.Drawing.Size(85, 17);
            this.chkGdiAntiAlias.TabIndex = 8;
            this.chkGdiAntiAlias.Text = "Gdi AntiAlias";
            this.chkGdiAntiAlias.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(573, 413);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(150, 32);
            this.button6.TabIndex = 9;
            this.button6.Text = "TestGLES2";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(252, 451);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(150, 32);
            this.button7.TabIndex = 11;
            this.button7.Text = "Create-Save-Load";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // cmdTestNativeLib
            // 
            this.cmdTestNativeLib.Location = new System.Drawing.Point(252, 489);
            this.cmdTestNativeLib.Name = "cmdTestNativeLib";
            this.cmdTestNativeLib.Size = new System.Drawing.Size(150, 32);
            this.cmdTestNativeLib.TabIndex = 12;
            this.cmdTestNativeLib.Text = "Test FtLib";
            this.cmdTestNativeLib.UseVisualStyleBackColor = true;
            this.cmdTestNativeLib.Click += new System.EventHandler(this.cmdTestNativeLib_Click);
            // 
            // cmdSignedDistance
            // 
            this.cmdSignedDistance.Location = new System.Drawing.Point(252, 527);
            this.cmdSignedDistance.Name = "cmdSignedDistance";
            this.cmdSignedDistance.Size = new System.Drawing.Size(150, 32);
            this.cmdSignedDistance.TabIndex = 14;
            this.cmdSignedDistance.Text = "Test SignedDistance";
            this.cmdSignedDistance.UseVisualStyleBackColor = true;
            this.cmdSignedDistance.Click += new System.EventHandler(this.cmdSignedDistance_Click);
            // 
            // cmdTestColorBlend
            // 
            this.cmdTestColorBlend.Location = new System.Drawing.Point(417, 527);
            this.cmdTestColorBlend.Name = "cmdTestColorBlend";
            this.cmdTestColorBlend.Size = new System.Drawing.Size(150, 32);
            this.cmdTestColorBlend.TabIndex = 15;
            this.cmdTestColorBlend.Text = "Test Color Blend";
            this.cmdTestColorBlend.UseVisualStyleBackColor = true;
            this.cmdTestColorBlend.Click += new System.EventHandler(this.cmdTestColorBlend_Click);
            // 
            // lstBackEndRenderer
            // 
            this.lstBackEndRenderer.FormattingEnabled = true;
            this.lstBackEndRenderer.Location = new System.Drawing.Point(12, 455);
            this.lstBackEndRenderer.Name = "lstBackEndRenderer";
            this.lstBackEndRenderer.Size = new System.Drawing.Size(234, 121);
            this.lstBackEndRenderer.TabIndex = 16;
            // 
            // FormDev
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 588);
            this.Controls.Add(this.lstBackEndRenderer);
            this.Controls.Add(this.cmdTestColorBlend);
            this.Controls.Add(this.cmdSignedDistance);
            this.Controls.Add(this.cmdTestNativeLib);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.chkGdiAntiAlias);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Name = "FormDev";
            this.Text = "DevForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox chkGdiAntiAlias;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button cmdTestNativeLib;
        private System.Windows.Forms.Button cmdSignedDistance;
        private System.Windows.Forms.Button cmdTestColorBlend;
        private System.Windows.Forms.ListBox lstBackEndRenderer;
    }
}