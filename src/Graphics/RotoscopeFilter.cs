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
    public class RotoscopeFilter
    {
        // The graphics device
        GraphicsDevice device;

        // Shaders
        Shader basic2DShader;
        Shader downsampleShader;
        Shader edgeDetectShader;
        Shader initKMeansShader;
        Shader kmeansShader;
        Shader medianShader;

        
        // Render Targets and Textures
        RenderTarget2D[] downsamples;
        DepthStencilBuffer db;
        Texture2D kmeansImage;
        RenderTarget2D kmeansTarget;
        RenderTarget2D kmeansTargetSmall;
        RenderTarget2D finalTarget;
        RenderTarget2D outlineTarget;

        // Parameters
        float edgeSharpness = 4.8f;
        int numKMeansMedianPasses = 8;
        int numClusters = 64;
        const int MAX_KMEANS_ITERATIONS = 800;
        const int MIN_KMEANS_ITERATIONS = 100;
        const double KMEANS_TOLERANCE = 0.000001;
        const int NUM_DOWNSAMPLES = 3;
        const int MAX_CLUSTERS_PER_PASS = 128;
        Vector3[] clusters;

        public RotoscopeFilter(GraphicsDevice device)
        {
            this.device = device;
            LoadShaders();
        }

        //-----------------------------------------------------------------
        // roundToPower2(int v)
        // Returns the nearest power of two rounded down from the input 
        // number
        //-----------------------------------------------------------------
        int roundToPower2(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v >> 1;
        }

        //-----------------------------------------------------------------
        // LoadTextures(int width, int height)
        // Allocates various buffers used by the rotoscoper
        // These include the kmeans buffers, downsample buffers, outline 
        // buffer, and the final image buffer
        //-----------------------------------------------------------------
        void LoadTextures(int width, int height)
        {
            db = new DepthStencilBuffer(device, width, height, device.DepthStencilBuffer.Format);
            kmeansTarget = new RenderTarget2D(device, width, height, 1, SurfaceFormat.Vector2);
            kmeansImage = new Texture2D(device, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            finalTarget = new RenderTarget2D(device, width, height, 1, SurfaceFormat.Color);
            outlineTarget = new RenderTarget2D(device, width, height, 1, SurfaceFormat.Color);

            int downWidth = roundToPower2(width);
            int downHeight = roundToPower2(height);
            downsamples = new RenderTarget2D[NUM_DOWNSAMPLES];
            for (int i = 0; i < NUM_DOWNSAMPLES; i++)
            {
                downsamples[i] = new RenderTarget2D(device, downWidth / (1 << i), downHeight / (1 << i), 1, SurfaceFormat.Color);
            }
            
            int kmeansSmallWidth = downsamples[NUM_DOWNSAMPLES - 1].Width;
            int kmeansSmallHeight = downsamples[NUM_DOWNSAMPLES - 1].Height;
            kmeansTargetSmall = new RenderTarget2D(device, kmeansSmallWidth, kmeansSmallHeight, 1, SurfaceFormat.Vector2);
        }

        //-----------------------------------------------------------------
        // LoadShaders()
        // Load shaders used by the rotoscoper
        //-----------------------------------------------------------------
        void LoadShaders()
        {
            basic2DShader = new Shader();
            basic2DShader.CompileFromFiles(device, "Shaders/BasicP.hlsl", "Shaders/BasicV.hlsl");

            kmeansShader = new Shader();
            kmeansShader.CompileFromFiles(device, "Shaders/KMeansP.hlsl", "Shaders/BasicV.hlsl");

            initKMeansShader = new Shader();
            initKMeansShader.CompileFromFiles(device, "Shaders/InitKMeansP.hlsl", "Shaders/BasicV.hlsl");

            downsampleShader = new Shader();
            downsampleShader.CompileFromFiles(device, "Shaders/HalfP.hlsl", "Shaders/BasicV.hlsl");

            medianShader = new Shader();
            medianShader.CompileFromFiles(device, "Shaders/MedianP.hlsl", "Shaders/BasicV.hlsl");

            edgeDetectShader = new Shader();
            edgeDetectShader.CompileFromFiles(device, "Shaders/OutlinesP.hlsl", "Shaders/BasicV.hlsl");
        }

        //-----------------------------------------------------------------
        // SetNumClusters(int width, int height)
        // Sets the desired number of clusters used by the K-Means 
        // algorithm.
        //-----------------------------------------------------------------
        public void SetNumClusters(int numClusters)
        {
            this.numClusters = Math.Max(numClusters, 4);
        }

        //-----------------------------------------------------------------
        // SetEdgeSharpness(float value)
        // Sets the "sharpness" of outlines. Used by OutlineP.hlsl in
        // the shaders directory.
        //-----------------------------------------------------------------
        public void SetEdgeSharpness(float value)
        {
            edgeSharpness = value;
        }

        //-----------------------------------------------------------------
        // SetNumKMeansMedianPasses(int numPasses)
        // Sets the number of median filter passes used before the K-Means
        // algorithm is run. This roughly corresponds to a value of 
        // smoothness within an image.
        //-----------------------------------------------------------------
        public void SetNumKMeansMedianPasses(int numPasses)
        {
            numKMeansMedianPasses = numPasses;
        }


        //-----------------------------------------------------------------
        // GetFrame()
        // Returns the final rotoscoped image.
        // Must be called after ProcessFrame()
        //-----------------------------------------------------------------
        public Texture2D GetFrame()
        {
            return finalTarget.GetTexture();
        }

        //-----------------------------------------------------------------
        // ProcessFrame(Texture2D srcTexture, bool recomputeKMeans)
        // Performs the task of rotoscoping an image
        // It allocates data needed for the rotoscoper if necessary, and 
        // can optionally recompute the clusters estimated by K-Means.
        // The resulting image from this call can be accessed via a called to GetFrame()
        //-----------------------------------------------------------------
        public void ProcessFrame(Texture2D srcTexture, bool recomputeKMeans)
        {
            // Have we allocated data for our image? If not, do so now!
            if (finalTarget == null || srcTexture.Width != finalTarget.Width || srcTexture.Height != finalTarget.Height)
                LoadTextures(srcTexture.Width, srcTexture.Height);

            // Initialize some state
            device.RenderState.DepthBufferEnable = false;
            device.RenderState.DepthBufferWriteEnable = false;
            device.RenderState.CullMode = CullMode.None;

            // First, we downsample
            DownsampleImage(srcTexture);
            if (recomputeKMeans)
            {
                //Next, we train our clusters on a low-resolution image
                ComputeKMeans(downsamples[NUM_DOWNSAMPLES - 1].GetTexture());
                CreateKMeansMap(srcTexture);
            }
            // Finally, we extract the outlines in the image
            ExtractOutlines(srcTexture);


            // At this stage, we have a K-Means map and an outline map
            // We will blend them together to create the final, cartoonified image

            // Pop the old depth stencil buffer and replace it with our own
            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;

            //Render to the final texture 
            device.SetRenderTarget(0, finalTarget);

            // Render the kmeans image
            GraphicsUtils.SetSamplerState(device, 0, TextureFilter.Linear, TextureAddressMode.Wrap);
            device.Textures[0] = kmeansImage;
            basic2DShader.SetupShader();
            GraphicsUtils.quad.Render(device);


            // Render the outlines using multiplicative blending, set the resulting alpha to be one 
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SeparateAlphaBlendEnabled = true;
            device.RenderState.AlphaDestinationBlend = Blend.One;
            device.RenderState.AlphaSourceBlend = Blend.One;
            device.RenderState.SourceBlend = Blend.DestinationColor;
            device.RenderState.DestinationBlend = Blend.Zero;
            GraphicsUtils.SetTextureState(device, 0, outlineTarget.GetTexture());
            GraphicsUtils.quad.Render(device);
            device.RenderState.AlphaBlendEnable = false;

            device.SetRenderTarget(0, null);

            // Restore the original depth stencil buffer
            device.DepthStencilBuffer = old;
        }

        //-----------------------------------------------------------------
        // hue2rgb(float p, float q, float t)
        // Returns a value between 0 and 1 based on parameters p, q, and t
        // Used by hsl2Rgb
        //-----------------------------------------------------------------
        float hue2rgb(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6) return p + (q - p) * 6 * t;
            if (t < 1 / 2) return q;
            if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
            return p;
        }

        //-----------------------------------------------------------------
        // hsl2Rgb(Vector3 hsl)
        // Given a color in HSL/HSV space, returns a color in RGB space.
        //-----------------------------------------------------------------
        Vector3 hsl2Rgb(Vector3 hsl)
        {
            Vector3 color;
            if (hsl.Y == 0)
                color = Vector3.One;
            else
            {
                float q = hsl.Z < 0.5 ? hsl.Z * (1 + hsl.Y) : hsl.Z + hsl.Y - hsl.Z * hsl.Y;
                float p = 2 * hsl.Z - q;
                color.X = hue2rgb(p, q, hsl.X + 1 / 3);
                color.Y = hue2rgb(p, q, hsl.X);
                color.Z = hue2rgb(p, q, hsl.X - 1 / 3);
            }
            return color;
        }


        //-----------------------------------------------------------------
        // InitClusters()
        // Initializes clusters used by K-Means.
        // Clusters are uniformly distributed along a cylindrical 
        // color space (HSL) and are then converted to RGB
        // There's no mathematical basis for why this is "better", but
        // in practice it turns out to provide consistently better results
        // than other cluster initialization methods
        //-----------------------------------------------------------------
        void InitClusters()
        {
            clusters = new Vector3[numClusters];
            int numHues = (int)Math.Floor(Math.Pow(numClusters, 1.0 / 3.0));
            int numSats = numHues;
            int numLights = numSats;
            int index = 0;
            for (int i = 0; i < numHues; i++)
            {
                for (int j = 0; j < numSats; j++)
                {
                    for (int k = 0; k < numLights; k++)
                    {
                        Vector3 hsl = new Vector3(i, j, k) / new Vector3(numHues, numSats, numLights);
                        clusters[index] = hsl2Rgb(hsl);
                        index++;
                    }
                }
            }
        }

        //-----------------------------------------------------------------
        // UpdateCentroids(ref Color[] imageData, ref Vector2[] kmeansData)
        // Updates the centroids for the current K-Means iteration.
        // This is the CPU intensive part of the K-Means algorithm.
        //-----------------------------------------------------------------
        void UpdateCentroids(ref Color[] imageData, ref Vector2[] kmeansData)
        {
            for (int i = 0; i < numClusters; i++)
                clusters[i] = Vector3.Zero;

            // Gather frequency data on clusters
            // and add the cluster vectors
            int[] clusterWeights = new int[numClusters];
            for (int i = 0; i < imageData.Length; i++)
            {
                int clusterIndex = (int)kmeansData[i].X;
                clusters[clusterIndex] += imageData[i].ToVector3();
                clusterWeights[clusterIndex]++;
            }

            // We have frequency information now, so it's time to average our clusters
            for (int i = 0; i < numClusters; i++)
            {
                if (clusterWeights[i] > 0)
                    clusters[i] /= (float)clusterWeights[i];
                else
                {
                    // If we have zero weights, initialize this cluster randomly
                    clusters[i] = imageData[RandomHelper.RandomGen.Next(i, imageData.Length)].ToVector3();
                }
            }
        }

        //-----------------------------------------------------------------
        // ComputeNearestClusters(RenderTarget2D kTarget)
        // Performs the nearest cluster computation across
        // all points in the image. This is accelerated
        // by performing the process on the GPU.
        //-----------------------------------------------------------------
        void ComputeNearestClusters(RenderTarget2D kTarget)
        {
            device.SetRenderTarget(0, kTarget);
            initKMeansShader.SetupShader();
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);

            kmeansShader.SetupShader();

            // We have a fixed number of clusters that can be sent to a shader
            // Most hardware supports up to 128, so we can perform up to 
            // 128 cluster computations in each pass. As a result,
            // we have to break up our clusters into bundles and send the bundles
            // to the GPU
            Vector3[] tempClusters = new Vector3[MAX_CLUSTERS_PER_PASS];
            for (int i = 0; i < numClusters; i += MAX_CLUSTERS_PER_PASS)
            {
                int clusterLength = Math.Min(numClusters - i, MAX_CLUSTERS_PER_PASS);
                // Copy our current batch of clusters into a bundle
                Array.Copy(clusters, i, tempClusters, 0, clusterLength);
                // Set some constants used by the shader
                device.SetPixelShaderConstant(0, Vector4.One * i);
                device.SetPixelShaderConstant(1, clusterLength * Vector4.One);
                device.SetPixelShaderConstant(2, tempClusters);
                // Setup the texture
                device.Textures[1] = kTarget.GetTexture();

                // Finally, render to the buffer
                device.SetRenderTarget(0, kTarget);
                GraphicsUtils.quad.Render(device);
                device.SetRenderTarget(0, null);
            }
        }

        //-----------------------------------------------------------------
        // CreateKMeansMap(Texture2D srcTexture)
        // Creates the KMeans map used in rendering
        // This uses the clusters estimated by the downsampled image
        // to extrapolate on the colors in the original high-resolution 
        // image.
        //-----------------------------------------------------------------
        void CreateKMeansMap(Texture2D srcTexture)
        {
            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;

            device.SetRenderTarget(0, kmeansTarget);
            initKMeansShader.SetupShader();
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);

            device.Textures[0] = downsamples[0].GetTexture();
            device.SetVertexShaderConstant(0, Vector2.One / new Vector2(downsamples[0].Width, downsamples[0].Height));
            kmeansShader.SetupShader();
            ComputeNearestClusters(kmeansTarget);
 
            device.DepthStencilBuffer = old;

            Vector2[] kmeansData = new Vector2[srcTexture.Width * srcTexture.Height];
            kmeansTarget.GetTexture().GetData<Vector2>(kmeansData);

            Color[] kmeansPixels = new Color[kmeansData.Length];
            for (int i = 0; i < kmeansPixels.Length; i++)
            {
                int clusterIndex = (int)kmeansData[i].X;
                kmeansPixels[i] = new Color(clusters[clusterIndex]);
            }
            kmeansImage.SetData<Color>(kmeansPixels);

        }

        //-----------------------------------------------------------------
        // ComputeKMeans(Texture2D srcTexture)
        // Performs the entire K-Means algorithm on a given source texture.
        // This handles everything from K-Means convergence criteria to
        // recomputing clusters
        //-----------------------------------------------------------------
        void ComputeKMeans(Texture2D srcTexture)
        {
            // First, we initialize our clusters
            InitClusters();

            // Next, we setup the source texture
            GraphicsUtils.SetSamplerState(device, 0, TextureFilter.Linear, TextureAddressMode.Wrap);
            device.Textures[0] = srcTexture;
            device.SetVertexShaderConstant(0, Vector2.One / new Vector2(srcTexture.Width, srcTexture.Height));

            // Setup our depth buffer
            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;

            int imageSize = srcTexture.Width * srcTexture.Height;
            Color[] imageData = new Color[imageSize];
            srcTexture.GetData<Color>(imageData);
            Vector2[] kmeansData = new Vector2[imageSize];
            Vector3[] oldClusters = new Vector3[numClusters];

            // Perform K-Means
            for (int i = 0; i < MAX_KMEANS_ITERATIONS; i++)
            {
                // Make a copy of our clusters
                Array.Copy(clusters, oldClusters, numClusters);
                
                // Compute how close our data is to each cluster
                ComputeNearestClusters(kmeansTargetSmall);
                kmeansTargetSmall.GetTexture().GetData<Vector2>(kmeansData);

                // Update the centroid estimates
                UpdateCentroids(ref imageData, ref kmeansData);

                double length = 0;
                for (int j = 0; j < clusters.Length; j++)
                    length += Vector3.DistanceSquared(clusters[j], oldClusters[j]);

                if (length < KMEANS_TOLERANCE && i > MIN_KMEANS_ITERATIONS)
                    break;
            }

            // Reset the original depth buffer
            device.DepthStencilBuffer = old;
        }

        //-----------------------------------------------------------------
        // MedianFilter(Texture2D srcTexture)
        // Performs 5x5 median filtering on a given render target
        //-----------------------------------------------------------------
        void MedianFilter(RenderTarget2D srcTexture)
        {
            medianShader.SetupShader();
            device.Textures[0] = srcTexture.GetTexture();
            Vector2 invRes = Vector2.One / new Vector2(srcTexture.Width, srcTexture.Height);
            device.SetVertexShaderConstant(0, invRes);
            device.SetPixelShaderConstant(0, invRes);
            device.SetRenderTarget(0, srcTexture);
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);
        }

        //-----------------------------------------------------------------
        // ExtractOutlines(Texture2D srcTexture)
        // Extracts outlines from a given image
        //-----------------------------------------------------------------
        void ExtractOutlines(Texture2D srcTexture)
        {
            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;

            GraphicsUtils.SetTextureState(device, 0, srcTexture);
            device.Textures[0] = srcTexture;
            device.SetPixelShaderConstant(1, edgeSharpness * Vector4.One);
            edgeDetectShader.SetupShader();

            device.SetRenderTarget(0, outlineTarget);
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);

            device.DepthStencilBuffer = old;
        }

        //-----------------------------------------------------------------
        // DownsampleImage(Texture2D srcTexture)
        // Downsamples a given image to be used later by the K-Means
        // algorithm. We also perform median filtering.
        //-----------------------------------------------------------------
        void DownsampleImage(Texture2D srcTexture)
        {
            GraphicsUtils.SetSamplerState(device, 0, TextureFilter.Linear, TextureAddressMode.Wrap);
            device.Textures[0] = srcTexture;
            device.SetVertexShaderConstant(0, Vector2.One / new Vector2(srcTexture.Width, srcTexture.Height));
            

            // Setup depth buffer
            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;

            //Render the image into our first buffer
            device.SetRenderTarget(0, downsamples[0]);
            basic2DShader.SetupShader();
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);

            // Perform a bit of median filtering to smooth out high frequency details
            for (int i = 0; i < numKMeansMedianPasses; i++)
                MedianFilter(downsamples[0]);

            // Iteratively down-sample our image
            downsampleShader.SetupShader();
            for (int i = 1; i < NUM_DOWNSAMPLES; i++)
            {
                Texture2D prevTex = downsamples[i - 1].GetTexture();
                device.Textures[0] = prevTex;
                Vector2 invRes = Vector2.One / new Vector2(prevTex.Width, prevTex.Height);
                device.SetVertexShaderConstant(0, invRes);
                device.SetPixelShaderConstant(0, invRes);

                device.SetRenderTarget(0, downsamples[i]);
                GraphicsUtils.quad.Render(device);
                device.SetRenderTarget(0, null);
            }
   
            // Reset depth buffer
            device.DepthStencilBuffer = old;
        }
    }
}
