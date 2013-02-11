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

        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            RotoControl.LoadImage();
        }

        private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            RotoControl.SaveImage();
        }
    }
}
