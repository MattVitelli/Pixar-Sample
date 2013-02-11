//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------

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
        // Graphics-related members
        VertexDeclaration vertexDeclaration;
        Shader basic2D;
        Texture2D frame = null;
        RotoscopeFilter filter;

        // State parameters
        bool imageLoaded = false;
        bool imageProcessed = false;
        bool drawOriginalImage = false;

        //-----------------------------------------------------------------
        // LoadImage(string filename)
        // Loads an image from a given filename.
        // Returns true on success and false on failure
        //-----------------------------------------------------------------
        public bool LoadImage(string filename)
        {
            try
            {
                frame = Texture2D.FromFile(GraphicsDevice, filename);
                this.Width = frame.Width;
                this.Height = frame.Height;
                imageLoaded = true;
                imageProcessed = false;
            }
            catch
            {
                if (frame != null)
                {
                    frame.Dispose();
                    frame = null;
                }
                imageLoaded = false;
            }
            return imageLoaded;
        }

        //-----------------------------------------------------------------
        // SaveImage(string filename)
        // Saves an image to the given filename in the .png format
        //-----------------------------------------------------------------
        public void SaveImage(string filename)
        {
            if (!imageLoaded)
            {
                MessageBox.Show("Error! Please load and process an image first!");
                return;
            }

            filter.GetFrame().Save(filename + ".png", ImageFileFormat.Png);
        }

        //-----------------------------------------------------------------
        // SetEdgeSharpness(float value)
        // Updates the filter's sharpness value
        //-----------------------------------------------------------------
        public void SetEdgeSharpness(float value)
        {
            filter.SetEdgeSharpness(value);
        }

        //-----------------------------------------------------------------
        // SetNumMedianPasses(int value)
        // Sets the number of median filter passes that will be
        // performed by the rotoscoper
        //-----------------------------------------------------------------
        public void SetNumMedianPasses(int value)
        {
            filter.SetNumKMeansMedianPasses(value);
            imageProcessed = false;
        }

        //-----------------------------------------------------------------
        // SetNumberOfColors(int value)
        // Sets the number of clusters used by the K-Means algorithm
        //-----------------------------------------------------------------
        public void SetNumberOfColors(int value)
        {
            filter.SetNumClusters(value);
            imageProcessed = false;
        }

        //-----------------------------------------------------------------
        // SetDrawMode(bool mode)
        // Sets the draw mode. A value of true means that we draw the 
        // original image. A value of false means we draw the rotoscoped
        // image. (Default value is false)
        //-----------------------------------------------------------------
        public void SetDrawMode(bool mode)
        {
            drawOriginalImage = mode;
        }

        //-----------------------------------------------------------------
        // OnDeviceReset()
        // Handles the case when the graphics device is reset
        // This can happen when the user resizes the window
        //-----------------------------------------------------------------
        protected override void OnDeviceReset()
        {
            imageProcessed = false;
            base.OnDeviceReset();
        }

        //-----------------------------------------------------------------
        // Initialize()
        // Called when the graphics device is finally initialized
        // This function is primarily used to load resources
        //-----------------------------------------------------------------
        protected override void Initialize()
        {
            // Create our vertex declaration.
            vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);

            // Create our filter
            filter = new RotoscopeFilter(GraphicsDevice);

            // Create a basic shader
            basic2D = new Shader();
            basic2D.CompileFromFiles(GraphicsDevice, "Shaders/BasicP.hlsl", "Shaders/BasicV.hlsl");

            // Load up a test image
            LoadImage("Examples/uma.jpg");

            // Hook the idle event to constantly redraw our image.
            Application.Idle += delegate { Invalidate(); };
        }


        //-----------------------------------------------------------------
        // Draw()
        // Handles a render call. This is where the bulk of the 
        // application's work is performed, including creating the 
        // rotoscoped image
        //-----------------------------------------------------------------
        protected override void Draw()
        {
            // Clear the back-buffer
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set renderstates.
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.DepthBufferEnable = false;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            // Set vertex declaration
            GraphicsDevice.VertexDeclaration = vertexDeclaration;

            // Finally, draw the rotoscoped image!
            if (imageLoaded)
            {
                // Update the frame, recomputing the entire image if necessary
                filter.ProcessFrame(frame, !imageProcessed);
                imageProcessed = true;

                basic2D.SetupShader();
                // Pick the image to draw
                Texture2D image = (drawOriginalImage) ? frame : filter.GetFrame();
                // Setup the texture states
                GraphicsUtils.SetTextureState(GraphicsDevice, 0, image);
                GraphicsUtils.quad.Render(GraphicsDevice);
            }
        }
    }
}
