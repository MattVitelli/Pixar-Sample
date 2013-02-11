using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoApp.Graphics
{
    public class Shader
    {
        string name;
        public string Name { get { return name; } }

        PixelShader ps;
        VertexShader vs;
        bool compiled = false;

        string errorMessage = null;

        GraphicsDevice GraphicsDevice;

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
            if (psShader.Success && vsShader.Success)
            {
                ps = new PixelShader(GraphicsDevice, psShader.GetShaderCode());
                vs = new VertexShader(GraphicsDevice, vsShader.GetShaderCode());
                compiled = true;
            }
        }

        public void SetupShader()
        {
            if (!compiled)
            {
                if (errorMessage != null)
                    Console.WriteLine(errorMessage);
                return;
            }
            GraphicsDevice.PixelShader = ps;
            GraphicsDevice.VertexShader = vs;
        }
    }
}
