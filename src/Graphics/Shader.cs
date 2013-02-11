//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoApp.Graphics
{
    public class Shader
    {
        // Shader Types
        PixelShader ps;
        VertexShader vs;

        // Compiler-Related Members
        bool compiled = false;
        string errorMessage = null;

        // Our graphics device
        GraphicsDevice GraphicsDevice;

        //-----------------------------------------------------------------
        // CompileFromFiles(GraphicsDevice device, string psFileName, 
        // string vsFileName)
        // Compiles shaders from a given filename
        //-----------------------------------------------------------------
        public void CompileFromFiles(GraphicsDevice device, string psFileName, string vsFileName)
        {
            this.GraphicsDevice = device;
            ShaderProfile psProf = GraphicsDevice.GraphicsDeviceCapabilities.MaxPixelShaderProfile;
            ShaderProfile vsProf = GraphicsDevice.GraphicsDeviceCapabilities.MaxVertexShaderProfile;
            CompiledShader psShader = ShaderCompiler.CompileFromFile(psFileName, null, null, CompilerOptions.PackMatrixRowMajor, "main", psProf, TargetPlatform.Windows);
            CompiledShader vsShader = ShaderCompiler.CompileFromFile(vsFileName, null, null, CompilerOptions.PackMatrixRowMajor, "main", vsProf, TargetPlatform.Windows);
            errorMessage = null;
            if (vsShader.ErrorsAndWarnings.Length > 1)
                errorMessage = "Vertex Shader: " + vsShader.ErrorsAndWarnings;
            if (psShader.ErrorsAndWarnings.Length > 1)
            {
                if (errorMessage == null)
                    errorMessage = "Pixel Shader: " + psShader.ErrorsAndWarnings;
                else
                    errorMessage = errorMessage + "\n Pixel Shader: " + psShader.ErrorsAndWarnings;
                Console.WriteLine(errorMessage);
            }
            // Did we successfully compile?
            if (psShader.Success && vsShader.Success)
            {
                ps = new PixelShader(GraphicsDevice, psShader.GetShaderCode());
                vs = new VertexShader(GraphicsDevice, vsShader.GetShaderCode());
                compiled = true;
            }
        }

        //-----------------------------------------------------------------
        // SetupShader()
        // Sets the graphic device's vertex and pixel shaders 
        // to the shaders stored in this class
        //-----------------------------------------------------------------
        public void SetupShader()
        {
            // If our shaders aren't compiled, we need to exit early
            if (!compiled)
            {
                if (errorMessage != null)
                    Console.WriteLine(errorMessage);
                return;
            }
            // Set our shaders on the device
            GraphicsDevice.PixelShader = ps;
            GraphicsDevice.VertexShader = vs;
        }
    }
}
