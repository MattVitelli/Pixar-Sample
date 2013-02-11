using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace PhotoApp.Rendering
{
    public class Quad
    {
        VertexPositionTexture[] verts = null;
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

        public void SetTexCoords(Vector2 ll, Vector2 lr, Vector2 ul, Vector2 ur)
        {
            verts[0].TextureCoordinate = ur;
            verts[1].TextureCoordinate = ul;
            verts[2].TextureCoordinate = ll;
            verts[3].TextureCoordinate = lr;
        }

        public void SetTexCoords(Vector2 min, Vector2 max)
        {
            verts[0].TextureCoordinate = max;
            verts[1].TextureCoordinate = new Vector2(min.X, max.Y);
            verts[2].TextureCoordinate = min;
            verts[3].TextureCoordinate = new Vector2(max.X, min.Y);
        }

        public void SetPositions(Vector2 ll, Vector2 lr, Vector2 ul, Vector2 ur)
        {
            verts[0].Position.X = lr.X;
            verts[0].Position.Y = lr.Y;

            verts[1].Position.X = ll.X;
            verts[1].Position.Y = ll.Y;

            verts[2].Position.X = ul.X;
            verts[2].Position.Y = ul.Y;

            verts[3].Position.X = ur.X;
            verts[3].Position.Y = ur.Y;
        }

        public void SetPositions(Vector2 min, Vector2 max)
        {
            verts[0].Position.X = max.X;
            verts[0].Position.Y = min.Y;

            verts[1].Position.X = min.X;
            verts[1].Position.Y = min.Y;

            verts[2].Position.X = min.X;
            verts[2].Position.Y = max.Y;

            verts[3].Position.X = max.X;
            verts[3].Position.Y = max.Y;
        }

        public void Render(GraphicsDevice device)
        {
            device.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    }
}
