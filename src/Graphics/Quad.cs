//-----------------------------------------------------------------
// Roto-Photo
// Rotoscoping software written by Matt Vitelli
// Copyright (C) Matt Vitelli 2013
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace PhotoApp.Rendering
{
    public class Quad
    {
        // Vertices
        VertexPositionTexture[] verts = null;
        // Indices
        short[] ib = null;

        public Quad()
        {
            verts = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(1,-1,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,-1,0),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(-1,1,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(1,1,0),
                                new Vector2(1,0))
                        };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        //-----------------------------------------------------------------
        // Render(GraphicsDevice device)
        // Renders a screen-aligned quad
        //-----------------------------------------------------------------
        public void Render(GraphicsDevice device)
        {
            device.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    }
}
