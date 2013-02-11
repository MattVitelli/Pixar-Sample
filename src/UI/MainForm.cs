//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------
using System.Windows.Forms;
using System.Collections.Generic;

namespace PhotoApp
{
    using GdiColor = System.Drawing.Color;
    using XnaColor = Microsoft.Xna.Framework.Graphics.Color;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------------
        // openToolStripMenuItem_Click(object sender, System.EventArgs e)
        // Callback when user clicks the Open button.
        // It handles loading a file from the user's computer
        //-----------------------------------------------------------------
        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Title = "Find a photo";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RotoControl.LoadImage(dlg.FileName);
            }
        }

        //-----------------------------------------------------------------
        // saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        // Callback when user clicks the Save button.
        // It handles saving a file to the user's computer
        //-----------------------------------------------------------------
        private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Title = "Save a photo";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RotoControl.SaveImage(dlg.FileName);
            }
        }

        //-----------------------------------------------------------------
        // sharpnessParameter_ValueChanged(object sender, 
        // System.EventArgs e)
        // Callback when user modifies the sharpness parameter
        // It makes sure the rotoscoper updates the outlines
        //-----------------------------------------------------------------
        private void sharpnessParameter_ValueChanged(object sender, System.EventArgs e)
        {
            RotoControl.SetEdgeSharpness(sharpnessParameter.Value);
        }

        //-----------------------------------------------------------------
        // colorSmoothParameter_ValueChanged(object sender, 
        // System.EventArgs e)
        // Callback when user modifies the color smoothness parameter
        // It makes sure the rotoscoper updates the K-Means clusters 
        // and performs the appropriate level of median filtering
        //-----------------------------------------------------------------
        private void colorSmoothParameter_ValueChanged(object sender, System.EventArgs e)
        {
            RotoControl.SetNumMedianPasses(colorSmoothParameter.ValueAsInt);
        }

        //-----------------------------------------------------------------
        // numColorsParameter_ValueChanged(object sender, 
        // System.EventArgs e)
        // Callback when user modifies the number of colors parameter
        // It makes sure the rotoscoper updates the K-Means clusters 
        //-----------------------------------------------------------------
        private void numColorsParameter_ValueChanged(object sender, System.EventArgs e)
        {
            RotoControl.SetNumberOfColors(numColorsParameter.ValueAsInt);
        }

        //-----------------------------------------------------------------
        // drawImageCheckBox_CheckedChanged(object sender, 
        // System.EventArgs e)
        // Callback when user clicks Draw Original Image checkbox
        // Handles correct rendering of original image vs. rotoscoped image
        //-----------------------------------------------------------------
        private void drawImageCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            RotoControl.SetDrawMode(drawImageCheckBox.Checked);
        }

        //-----------------------------------------------------------------
        // exitToolStripMenuItem_Click(object sender, 
        // System.EventArgs e)
        // Callback when user clicks Exit button.
        // Kills the application with extreme prejudice
        //-----------------------------------------------------------------
        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        //-----------------------------------------------------------------
        // MainForm_Load(object sender, System.EventArgs e)
        // Callback when form finishes loading.
        // Updates parameters of rotoscope control
        //-----------------------------------------------------------------
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            // Used to set proper state in rotoscoper control
            RotoControl.SetEdgeSharpness(sharpnessParameter.Value);
            RotoControl.SetNumMedianPasses(colorSmoothParameter.ValueAsInt);
            RotoControl.SetNumberOfColors(numColorsParameter.ValueAsInt);
        }
    }
}
