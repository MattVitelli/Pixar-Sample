using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhotoApp.Graphics;
namespace PhotoApp
{
    class RotoControl : GraphicsDeviceControl
    {
        VertexDeclaration vertexDeclaration;
        Stopwatch timer;

        Shader basic2D;

        Texture2D frame = null;
        RotoscopeFilter filter;
        bool imageLoaded = false;
        bool imageProcessed = false;

        public void LoadImage()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Title = "Find a photo";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    frame = Texture2D.FromFile(GraphicsDevice, dlg.FileName);
                    this.Width = frame.Width;
                    this.Height = frame.Height;
                    imageLoaded = true;
                    imageProcessed = false;
                }
                catch
                {
                    frame.Dispose();
                    imageLoaded = false;
                }
            }
        }

        public void SaveImage()
        {
            if (!imageLoaded)
                return;
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Title = "Save a photo";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filter.GetFrame().Save(dlg.FileName, ImageFileFormat.Png);
            }
        }

        protected override void OnDeviceReset()
        {
            imageProcessed = false;
            base.OnDeviceReset();
        }
        
        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Create our vertex declaration.
            vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);

            // Start the animation timer.
            timer = Stopwatch.StartNew();

            //Create our filter
            filter = new RotoscopeFilter(GraphicsDevice);

            //Create a basic shader
            basic2D = new Shader();
            basic2D.CompileFromFiles(GraphicsDevice, "Shaders/BasicP.hlsl", "Shaders/BasicV.hlsl");

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }


        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set renderstates.
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.DepthBufferEnable = false;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            // Set vertex declaration
            GraphicsDevice.VertexDeclaration = vertexDeclaration;

            //Finally, draw the rotoscoped image!
            if (imageLoaded)
            {
                if (!imageProcessed)
                {
                    filter.ProcessFrame(frame);
                    imageProcessed = true;
                }

                basic2D.SetupShader();
                GraphicsUtils.SetTextureState(GraphicsDevice, 0, filter.GetFrame());
                GraphicsUtils.quad.Render(GraphicsDevice);
            }
        }
    }
}
