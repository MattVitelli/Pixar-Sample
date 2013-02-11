//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PhotoApp.UI
{
    public partial class GenericParameter : UserControl
    {
        public event EventHandler ValueChanged;

        float minimum = 0;
        float maximum = 1;

        public GenericParameter()
        {
            InitializeComponent();

            trackBar.ValueChanged += new EventHandler(OnValueChanged);
        }

        public string ParameterName
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public float Value
        {
            get 
            {
                float interp = (float)trackBar.Value / (float)trackBar.Maximum;
                float val = interp * maximum + (1.0f - interp) * minimum;
                return val; 
            }
            set 
            {
                int val = (int)((float)NumSteps * (value - minimum) / (maximum - minimum));
                trackBar.Value = val; 
            }
        }

        public int ValueAsInt
        {
            get { return trackBar.Value; }
            set { trackBar.Value = value; }
        }

        public int NumSteps
        {
            get { return trackBar.Maximum; }
            set { trackBar.Maximum = value; trackBar.Minimum = 0; }
        }

        public float Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        public float Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (!DesignMode && ValueChanged != null)
                this.Invoke(ValueChanged);
        }

        private void GenericParameter_SizeChanged(object sender, EventArgs e)
        {
            trackBar.Width = this.Width;
        }
    }
}
