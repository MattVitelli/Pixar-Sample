//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoApp
{
    static class RandomHelper
    {
        static Random randomGen = new Random();
        public static Random RandomGen { get { return randomGen; } }
    }
}
