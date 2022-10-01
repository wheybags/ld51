using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ld51
{
    public static unsafe class Render
    {
        private static MySpriteBatch spriteBatch = new MySpriteBatch(Game1.game.GraphicsDevice);
        public static int renderScale = 3;

        public static void render(GraphicsDevice GraphicsDevice, GameState gameState)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(0, 0, 0, 255));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap);


            Point mousePos_ = Mouse.GetState().Position;
            Vector2 mousePos = new Vector2(mousePos_.X, mousePos_.Y);

            Vector2 selectedPoint_ = mousePos / (renderScale * Constants.tileSize) - gameState.viewpoint;
            Point selectedPoint = new Point((int)selectedPoint_.X, (int)selectedPoint_.Y);

            Console.WriteLine(selectedPoint);

            for (int y = 0; y < gameState.level.h; y++)
            {
                for (int x = 0; x < gameState.level.w; x++)
                {
                    renderTileAtPixel(renderScale,
                                      gameState.level.get(x, y)->tileId,
                                      (gameState.viewpoint + new Vector2(x, y)) * Constants.tileSize * renderScale,
                                      new Point(x, y) == selectedPoint ? Color.Green : Color.White);
                }
            }

            int toolTile = -1;
            switch (gameState.tool)
            {
                case Tool.Belt:
                    toolTile = Constants.beltTool;
                    break;
                case Tool.Delete:
                    toolTile = Constants.beltTool;
                    break;
            }
            Util.ReleaseAssert(toolTile != -1);
            renderTileAtPixel(renderScale, toolTile, mousePos, Color.White);

            spriteBatch.End();
        }

        private static Point getCoordsInTileset(int tileId)
        {
            int tilesetWidth = Textures.tileset.Width / Constants.tileSize;
            int tileY = tileId / tilesetWidth;
            int tileX = tileId - (tileY * tilesetWidth);
            return new Point(tileX, tileY);
        }

        private static void renderTileAtPixel(int scale, int tileId, Vector2 pixelPos, Color color)
        {
            Point tilesetCoords = getCoordsInTileset(tileId);

            FloatRectangle target = new FloatRectangle()
            {
                x = pixelPos.X,
                y = pixelPos.Y,
                w = Constants.tileSize * scale,
                h = Constants.tileSize * scale,
            };

            FloatRectangle source = new FloatRectangle()
            {
                x = tilesetCoords.X * Constants.tileSize,
                y = tilesetCoords.Y * Constants.tileSize,
                w = Constants.tileSize,
                h = Constants.tileSize,
            };

            spriteBatch.Draw(Textures.tileset,target, source, color);
        }
    }
}