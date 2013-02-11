namespace PhotoApp
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.parameterBox = new System.Windows.Forms.GroupBox();
            this.drawImageCheckBox = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numColorsParameter = new PhotoApp.UI.GenericParameter();
            this.colorSmoothParameter = new PhotoApp.UI.GenericParameter();
            this.sharpnessParameter = new PhotoApp.UI.GenericParameter();
            this.RotoControl = new PhotoApp.RotoControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.parameterBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.parameterBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.RotoControl);
            this.splitContainer1.Size = new System.Drawing.Size(1056, 677);
            this.splitContainer1.SplitterDistance = 233;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // parameterBox
            // 
            this.parameterBox.Controls.Add(this.drawImageCheckBox);
            this.parameterBox.Controls.Add(this.numColorsParameter);
            this.parameterBox.Controls.Add(this.colorSmoothParameter);
            this.parameterBox.Controls.Add(this.sharpnessParameter);
            this.parameterBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterBox.Location = new System.Drawing.Point(0, 0);
            this.parameterBox.Name = "parameterBox";
            this.parameterBox.Size = new System.Drawing.Size(233, 677);
            this.parameterBox.TabIndex = 0;
            this.parameterBox.TabStop = false;
            this.parameterBox.Text = "Visuals";
            // 
            // drawImageCheckBox
            // 
            this.drawImageCheckBox.AutoSize = true;
            this.drawImageCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.drawImageCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drawImageCheckBox.Location = new System.Drawing.Point(3, 225);
            this.drawImageCheckBox.Name = "drawImageCheckBox";
            this.drawImageCheckBox.Size = new System.Drawing.Size(227, 21);
            this.drawImageCheckBox.TabIndex = 3;
            this.drawImageCheckBox.Text = "Show Original Image";
            this.drawImageCheckBox.UseVisualStyleBackColor = true;
            this.drawImageCheckBox.CheckedChanged += new System.EventHandler(this.drawImageCheckBox_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1056, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(164, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // numColorsParameter
            // 
            this.numColorsParameter.Dock = System.Windows.Forms.DockStyle.Top;
            this.numColorsParameter.Location = new System.Drawing.Point(3, 156);
            this.numColorsParameter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numColorsParameter.Maximum = 256F;
            this.numColorsParameter.Minimum = 8F;
            this.numColorsParameter.Name = "numColorsParameter";
            this.numColorsParameter.NumSteps = 252;
            this.numColorsParameter.ParameterName = "Number of Colors";
            this.numColorsParameter.Size = new System.Drawing.Size(227, 69);
            this.numColorsParameter.TabIndex = 2;
            this.numColorsParameter.Value = 39.49207F;
            this.numColorsParameter.ValueAsInt = 32;
            this.numColorsParameter.ValueChanged += new System.EventHandler(this.numColorsParameter_ValueChanged);
            // 
            // colorSmoothParameter
            // 
            this.colorSmoothParameter.Dock = System.Windows.Forms.DockStyle.Top;
            this.colorSmoothParameter.Location = new System.Drawing.Point(3, 87);
            this.colorSmoothParameter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.colorSmoothParameter.Maximum = 16F;
            this.colorSmoothParameter.Minimum = 0F;
            this.colorSmoothParameter.Name = "colorSmoothParameter";
            this.colorSmoothParameter.NumSteps = 16;
            this.colorSmoothParameter.ParameterName = "Color Smoothness";
            this.colorSmoothParameter.Size = new System.Drawing.Size(227, 69);
            this.colorSmoothParameter.TabIndex = 1;
            this.colorSmoothParameter.Value = 8F;
            this.colorSmoothParameter.ValueAsInt = 8;
            this.colorSmoothParameter.ValueChanged += new System.EventHandler(this.colorSmoothParameter_ValueChanged);
            // 
            // sharpnessParameter
            // 
            this.sharpnessParameter.Dock = System.Windows.Forms.DockStyle.Top;
            this.sharpnessParameter.Location = new System.Drawing.Point(3, 18);
            this.sharpnessParameter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sharpnessParameter.Maximum = 20F;
            this.sharpnessParameter.Minimum = 0.1F;
            this.sharpnessParameter.Name = "sharpnessParameter";
            this.sharpnessParameter.NumSteps = 100;
            this.sharpnessParameter.ParameterName = "Outline Sharpness";
            this.sharpnessParameter.Size = new System.Drawing.Size(227, 69);
            this.sharpnessParameter.TabIndex = 0;
            this.sharpnessParameter.Value = 12.239F;
            this.sharpnessParameter.ValueAsInt = 61;
            this.sharpnessParameter.ValueChanged += new System.EventHandler(this.sharpnessParameter_ValueChanged);
            // 
            // RotoControl
            // 
            this.RotoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RotoControl.Location = new System.Drawing.Point(0, 0);
            this.RotoControl.Margin = new System.Windows.Forms.Padding(4);
            this.RotoControl.Name = "RotoControl";
            this.RotoControl.Size = new System.Drawing.Size(818, 677);
            this.RotoControl.TabIndex = 0;
            this.RotoControl.Text = "roto-photo";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 705);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Roto-Photo by Matt Vitelli";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.parameterBox.ResumeLayout(false);
            this.parameterBox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private RotoControl RotoControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox parameterBox;
        private PhotoApp.UI.GenericParameter sharpnessParameter;
        private PhotoApp.UI.GenericParameter colorSmoothParameter;
        private PhotoApp.UI.GenericParameter numColorsParameter;
        private System.Windows.Forms.CheckBox drawImageCheckBox;
    }
}

