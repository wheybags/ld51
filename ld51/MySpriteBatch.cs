using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ld51
{
    public struct FloatRectangle
    {
        public float x, y, w, h;

        public FloatRectangle(float x, float y, float w, float h) { this.x = x; this.y = y; this.w = w; this.h = h; }
        public FloatRectangle(Rectangle r) { x = r.X; y = r.Y; w = r.Width; h = r.Height; }
    }

    public class MySpriteBatch : SpriteBatch
    {
        public MySpriteBatch(GraphicsDevice graphicsDevice) : base(graphicsDevice) {}


        // I JUST WANT TO USE FLOATING POINT RECTS IS THAT SO MUCH TO ASK
        // copy pasted from disassembly of SpriteBatch::Draw(), then bastardised to get access to private members
        public void Draw(
            Texture2D texture,
            FloatRectangle destinationRectangle,
            FloatRectangle sourceRectangle,
            Color color)
        {
            //this.CheckValid(texture);
            object batcher = typeof(SpriteBatch).GetField("_batcher", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);

            object batchItem = batcher.GetType().GetMethod("CreateBatchItem").Invoke(batcher, Array.Empty<object>());

            SpriteSortMode sortMode = (SpriteSortMode)typeof(SpriteBatch).GetField("_sortMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
            //int textureSortingKey = (int)typeof(Texture).GetField("_sortingKey").GetValue(texture); // can't get this working for whatever reason, meh, I don't use it

            batchItem.GetType().GetField("Texture").SetValue(batchItem, texture);


            batchItem.GetType().GetField("SortKey").SetValue(batchItem, 0f);
            //batchItem.SortKey = sortMode == SpriteSortMode.Texture ? (float) textureSortingKey : 0.0f;

            float TexelWidth = 1f / texture.Width;
            float TexelHeight = 1f / texture.Height;


            Vector2 _texCoordTL = new Vector2();
            Vector2 _texCoordBR = new Vector2();
            _texCoordTL.X = sourceRectangle.x * TexelWidth;
            _texCoordTL.Y = sourceRectangle.y * TexelHeight;
            _texCoordBR.X = (sourceRectangle.x + sourceRectangle.w) * TexelWidth;
            _texCoordBR.Y = (sourceRectangle.y + sourceRectangle.h) * TexelHeight;


            foreach (var method in batchItem.GetType().GetMethods())
            {
                if (method.Name != "Set" || method.GetParameters().Length != 8)
                    continue;

                object[] p = new object[] { (float)destinationRectangle.x, (float)destinationRectangle.y, (float)destinationRectangle.w, (float)destinationRectangle.h, color, _texCoordTL, _texCoordBR, 0.0f };
                method.Invoke(batchItem, p);
                break;
            }

            typeof(SpriteBatch).GetMethod("FlushIfNeeded", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, Array.Empty<object>());
        }
    }
}