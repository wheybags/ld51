using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            for (int y = 0; y < gameState.level.h; y++)
            {
                for (int x = 0; x < gameState.level.w; x++)
                {
                    renderTileAtPixel(renderScale, gameState.level.get(x, y)->tileId, gameState.viewpoint * renderScale + new Vector2(x, y) * Constants.tileSize * renderScale);
                }
            }

            spriteBatch.End();
        }

        private static Point getCoordsInTileset(int tileId)
        {
            int tilesetWidth = Textures.tileset.Width / Constants.tileSize;
            int tileY = tileId / tilesetWidth;
            int tileX = tileId - (tileY * tilesetWidth);
            return new Point(tileX, tileY);
        }

        private static void renderTileAtPixel(int scale, int tileId, Vector2 pixelPos)
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

            spriteBatch.Draw(Textures.tileset,target, source, Color.White);
        }
    }
}