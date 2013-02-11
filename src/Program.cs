//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------

using System;
using System.Windows.Forms;

namespace PhotoApp
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
