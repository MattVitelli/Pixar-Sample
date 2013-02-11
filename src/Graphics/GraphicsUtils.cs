//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoApp.Rendering;
namespace PhotoApp
{
    public static class GraphicsUtils
    {
        public static Quad quad = new Quad();

        //-----------------------------------------------------------------
        // SetSamplerState(GraphicsDevice device, int samplerIndex, 
        // TextureFilter filter, TextureAddressMode addressMode)
        // Sets the sampler state at a given index. Used to 
        // control mip-mapping and texture filters, as well as 
        // the address mode (wrap vs clamp, etc)
        //-----------------------------------------------------------------
        public static void SetSamplerState(GraphicsDevice device, int samplerIndex, TextureFilter filter, TextureAddressMode addressMode)
        {
            device.SamplerStates[samplerIndex].AddressU = addressMode;
            device.SamplerStates[samplerIndex].AddressV = addressMode;
            device.SamplerStates[samplerIndex].MagFilter = filter;
            device.SamplerStates[samplerIndex].MinFilter = filter;
            device.SamplerStates[samplerIndex].MipFilter = filter;
        }

        //-----------------------------------------------------------------
        // SetTextureState(GraphicsDevice device, int index, 
        // Texture2D srcTexture)
        // Sets the texture at a given index. Also sets up inverse
        // resolution shader constants for use in shaders.
        //-----------------------------------------------------------------
        public static void SetTextureState(GraphicsDevice device, int index, Texture2D srcTexture)
        {
            SetSamplerState(device, 0, TextureFilter.Linear, TextureAddressMode.Wrap);
            device.Textures[0] = srcTexture;
            Vector2 invRes = Vector2.One / new Vector2(srcTexture.Width, srcTexture.Height);
            device.SetVertexShaderConstant(0, invRes);
            device.SetPixelShaderConstant(0, invRes);
        }
    }
}
