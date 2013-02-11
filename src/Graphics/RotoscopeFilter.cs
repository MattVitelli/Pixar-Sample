using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoApp.Graphics
{
    public class RotoscopeFilter
    {
        GraphicsDevice device;

        //Shaders
        Shader basic2DShader;
        Shader downsampleShader;
        Shader edgeDetectShader;
        Shader initKMeansShader;
        Shader kmeansShader;
        Shader medianShader;

        
        //Render Targets
        RenderTarget2D[] downsamples;
        DepthStencilBuffer db;
        Texture2D kmeansImage;
        RenderTarget2D kmeansTarget;
        RenderTarget2D finalTarget;
        RenderTarget2D outlineTarget;

        //Parameters
        const float EDGE_THRESHOLD = 0.0075f;
        int numClusters = 64;
        int numKMeansIterations = 800;
        int NUM_DOWNSAMPLES = 3;
        int clustersPerPass = 128;
        Vector3[] clusters;

        public RotoscopeFilter(GraphicsDevice device)
        {
            this.device = device;
            LoadShaders();
        }

        //Returns the nearest power of two rounded down from the input number
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
        }

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

        #region KMeans Code
        float hue2rgb(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6) return p + (q - p) * 6 * t;
            if (t < 1 / 2) return q;
            if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
            return p;
        }

        Vector3 hslToRgb(Vector3 hsl)
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


        //Used to initialize the K-Means clusters
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
                        clusters[index] = hslToRgb(hsl);
                        index++;
                    }
                }
            }
        }

        void UpdateCentroids(ref Color[] imageData, ref Vector2[] kmeansData)
        {
            for (int i = 0; i < numClusters; i++)
                clusters[i] = Vector3.Zero;

            int[] clusterWeights = new int[numClusters];
            for (int i = 0; i < imageData.Length; i++)
            {
                int clusterIndex = (int)kmeansData[i].X;
                clusters[clusterIndex] += imageData[i].ToVector3();
                clusterWeights[clusterIndex]++;
            }
            for (int i = 0; i < numClusters; i++)
            {
                if (clusterWeights[i] > 0)
                    clusters[i] /= (float)clusterWeights[i];
                else
                    clusters[i] = imageData[RandomHelper.RandomGen.Next(i, imageData.Length)].ToVector3();
            }
        }

        void ComputeNearestClusters(RenderTarget2D kTarget)
        {
            device.SetRenderTarget(0, kTarget);
            initKMeansShader.SetupShader();
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);

            kmeansShader.SetupShader();
            Vector3[] tempClusters = new Vector3[clustersPerPass];
            for (int i = 0; i < numClusters; i += clustersPerPass)
            {
                int clusterLength = Math.Min(numClusters - i, clustersPerPass);
                Array.Copy(clusters, i, tempClusters, 0, clusterLength);
                device.SetPixelShaderConstant(0, Vector4.One * i);
                device.SetPixelShaderConstant(1, tempClusters);
                device.Textures[1] = kTarget.GetTexture();
                device.SetRenderTarget(0, kTarget);
                GraphicsUtils.quad.Render(device);
                device.SetRenderTarget(0, null);
            }
        }

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

            for (int i = 0; i < 4; i++)
                MedianFilter(kmeansTarget);

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

        void ComputeKMeans(Texture2D srcTexture)
        {
            InitClusters();
            GraphicsUtils.SetSamplerState(device, 0, TextureFilter.Linear, TextureAddressMode.Wrap);
            device.Textures[0] = srcTexture;
            device.SetVertexShaderConstant(0, Vector2.One / new Vector2(srcTexture.Width, srcTexture.Height));

            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;
            RenderTarget2D kmeansTargetSmall = new RenderTarget2D(device, srcTexture.Width, srcTexture.Height, 1, SurfaceFormat.Vector2);

            int imageSize = srcTexture.Width * srcTexture.Height;
            Color[] imageData = new Color[imageSize];
            srcTexture.GetData<Color>(imageData);
            Vector2[] kmeansData = new Vector2[imageSize];
            Vector3[] oldClusters = new Vector3[numClusters];
            for (int i = 0; i < numKMeansIterations; i++)
            {
                Array.Copy(clusters, oldClusters, numClusters);
                ComputeNearestClusters(kmeansTargetSmall);
                kmeansTargetSmall.GetTexture().GetData<Vector2>(kmeansData);
                UpdateCentroids(ref imageData, ref kmeansData);
                double length = 0;
                for (int j = 0; j < clusters.Length; j++)
                    length += Vector3.DistanceSquared(clusters[j], oldClusters[j]);

                if (length < 0.000001 && i > 100)
                    break;
            }
            kmeansTargetSmall.Dispose();
            device.DepthStencilBuffer = old;
        }
        #endregion

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

        void ExtractOutlines(Texture2D srcTexture)
        {
            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;

            GraphicsUtils.SetTextureState(device, 0, srcTexture);
            device.Textures[0] = srcTexture;
            device.SetPixelShaderConstant(1, EDGE_THRESHOLD * Vector4.One);
            edgeDetectShader.SetupShader();

            device.SetRenderTarget(0, outlineTarget);
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);

            device.DepthStencilBuffer = old;
        }

        void DownsampleImage(Texture2D srcTexture)
        {
            GraphicsUtils.SetSamplerState(device, 0, TextureFilter.Linear, TextureAddressMode.Wrap);
            device.Textures[0] = srcTexture;
            device.SetVertexShaderConstant(0, Vector2.One / new Vector2(srcTexture.Width, srcTexture.Height));
            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;

            device.SetRenderTarget(0, downsamples[0]);
            basic2DShader.SetupShader();
            GraphicsUtils.quad.Render(device);
            device.SetRenderTarget(0, null);

            //Perform a bit of median filtering to smooth out high frequency details
            for (int i = 0; i < 8; i++)
                MedianFilter(downsamples[0]);

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
   
            device.DepthStencilBuffer = old;
        }

        public Texture2D GetFrame()
        {
            return finalTarget.GetTexture();
        }

        public void ProcessFrame(Texture2D srcTexture)
        {
            if (finalTarget == null || srcTexture.Width != finalTarget.Width || srcTexture.Height != finalTarget.Height)
                LoadTextures(srcTexture.Width, srcTexture.Height);

            device.RenderState.DepthBufferEnable = false;
            device.RenderState.DepthBufferWriteEnable = false;
            device.RenderState.CullMode = CullMode.None;

            DownsampleImage(srcTexture);
            ComputeKMeans(downsamples[NUM_DOWNSAMPLES - 1].GetTexture());
            CreateKMeansMap(srcTexture);
            ExtractOutlines(srcTexture);

            DepthStencilBuffer old = device.DepthStencilBuffer;
            device.DepthStencilBuffer = db;
            device.SetRenderTarget(0, finalTarget);
            GraphicsUtils.SetSamplerState(device, 0, TextureFilter.Linear, TextureAddressMode.Wrap);
            device.Textures[0] = kmeansImage;
            basic2DShader.SetupShader();
            GraphicsUtils.quad.Render(device);

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.DestinationColor;
            device.RenderState.DestinationBlend = Blend.Zero;
            GraphicsUtils.SetTextureState(device, 0, outlineTarget.GetTexture());
            GraphicsUtils.quad.Render(device);
            device.RenderState.AlphaBlendEnable = false;

            device.SetRenderTarget(0, null);
            device.DepthStencilBuffer = old;
        }
    }
}
