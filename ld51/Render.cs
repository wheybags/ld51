using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ld51
{
    public static unsafe class Render
    {
        private static SpriteBatch spriteBatch = new SpriteBatch(Game1.game.GraphicsDevice);
        public static int renderScale = 3;

        public static void render(long ms, GraphicsDevice GraphicsDevice, GameState gameState)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(0, 0, 0, 255));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap);

            if (gameState.target != null)
                renderGame(ms, GraphicsDevice, gameState);
            else
                renderWinScreen(ms, GraphicsDevice, gameState);

            spriteBatch.End();
        }

        private static void renderWinScreen(long ms, GraphicsDevice GraphicsDevice, GameState gameState)
        {
            GraphicsDevice.Clear(new Color(90, 105, 136, 255));
            Vector2 winPos = new Vector2(Game1.game.Window.ClientBounds.Width / 2 - Textures.hudBottom.Bounds.Width * renderScale / 2, 0);
            spriteBatch.Draw(Textures.win, winPos, null, Color.White, 0f, new Vector2(0,0), new Vector2(renderScale, renderScale), SpriteEffects.None, 1);

            renderNumber(winPos + new Vector2(85, 10) * renderScale, gameState.score);

            int[] hist = new int[10];

            int max = gameState.onlineScores.Max();
            int bucketSize = max / hist.Length;

            foreach (int score in gameState.onlineScores)
            {
                if (score > 0)
                    hist[Math.Min(score/bucketSize, hist.Length-1)]++;
            }

            // Vector2 histTopLeft = winPos + new Vector2(0, 16) * renderScale;
            // Vector2 histBottomRight = histTopLeft + new Vector2(64, 64) * renderScale;

            Rectangle r = new Rectangle((int)winPos.X - 50*renderScale, (int)winPos.Y +64*renderScale, 256*renderScale, 128*renderScale);

            spriteBatch.Draw(Textures.white, r, Color.White);

            int countMax = hist.Max();
            float s = (float)r.Height / (float)countMax;

            int off = r.Width / hist.Length;
            for (int i = 0; i < hist.Length; i++)
            {
                Point bl = new Point(r.Left + off * i + 1*renderScale, r.Bottom);
                Point tr = new Point(r.Left + off * (i+1) -1*renderScale, (int)(r.Bottom - hist[i] * s));

                Rectangle line = new Rectangle(bl.X, tr.Y, tr.X - bl.X, bl.Y - tr.Y);
                spriteBatch.Draw(Textures.white, line, Color.Black);

                int bucketMin = i * bucketSize + 1;
                int bucketMax = (i+1) * bucketSize;
                renderNumber(new Vector2(bl.X, bl.Y) + new Vector2(0, 10 * renderScale), bucketMin);
            }


            {
                float ss = (float)r.Width / (float)max;
                Rectangle line = new Rectangle((int)(r.Left + ss * gameState.score), r.Top, 1 * renderScale, r.Height);
                spriteBatch.Draw(Textures.white, line, Color.Red);
            }
        }

        private static void renderGame(long ms, GraphicsDevice GraphicsDevice, GameState gameState)
        {
            Point selectedPoint = getSelectedPoint(gameState);

            for (int y = 0; y < gameState.level.h; y++)
            {
                for (int x = 0; x < gameState.level.w; x++)
                {
                    Color color = Color.White;
                    Point currentPoint = new Point(x, y);

                    bool factorySelected = gameState.tool == Tool.FactorySaw ||
                                           gameState.tool == Tool.FactoryGlue ||
                                           gameState.tool == Tool.FactoryPaintRed ||
                                           gameState.tool == Tool.FactoryPaintGreen ||
                                           gameState.tool == Tool.FactoryPaintBlue;

                    if (currentPoint == selectedPoint ||
                        (factorySelected && (currentPoint == selectedPoint + new Point(0,1) ||
                                             currentPoint == selectedPoint + new Point(1,0) ||
                                             currentPoint == selectedPoint + new Point(1,1)))
                       )
                    {
                        color = Color.Green;
                    }

                    int tile = gameState.level.get(x, y)->tileId;
                    float rotation = 0f;
                    switch (tile)
                    {
                        case Constants.beltRight:
                            tile = Constants.beltUp;
                            rotation = 90f;
                            break;
                        case Constants.beltDown:
                            tile = Constants.beltUp;
                            rotation = 180f;
                            break;
                        case Constants.beltLeft:
                            tile = Constants.beltUp;
                            rotation = 270f;
                            break;
                    }

                    renderTileAtPixel(ms, renderScale,
                                      tile,
                                      (gameState.viewpoint + new Vector2(x, y)) * Constants.tileSize * renderScale,
                                      color,
                                      rotation);
                }
            }

            foreach (Item item in gameState.items)
            {
                Vector2 pos = (gameState.viewpoint + item.renderPosition) * Constants.tileSize * renderScale;
                renderItem(pos, item.parts);
            }

            foreach (Factory factory in gameState.factories)
            {
                Point tilesetCoords = getCoordsInTileset(Constants.factoryTopLeft);

                Rectangle sourceRect = new Rectangle()
                {
                    X = tilesetCoords.X * Constants.tileSize,
                    Y = tilesetCoords.Y * Constants.tileSize,
                    Width = Constants.tileSize * Constants.factoryDimensions.X,
                    Height = Constants.tileSize * Constants.factoryDimensions.Y,
                };

                Vector2 pos = (gameState.viewpoint + new Vector2(factory.topLeft.X, factory.topLeft.Y)) * Constants.tileSize * renderScale;
                spriteBatch.Draw(Textures.tileset.getCurrentFrame(ms),
                    pos,
                    sourceRect,
                    Color.White,
                    0,
                    Vector2.Zero,
                    new Vector2(renderScale, renderScale),
                    SpriteEffects.None,
                    0);

                int icon = 0;
                switch (factory.type)
                {
                    case FactoryType.Saw:
                        icon = Constants.factoryIconSaw;
                        break;
                    case FactoryType.Glue:
                        icon = Constants.factoryIconGlue;
                        break;
                    case FactoryType.PaintRed:
                        icon = Constants.factoryIconPaintRed;
                        break;
                    case FactoryType.PaintGreen:
                        icon = Constants.factoryIconPaintGreen;
                        break;
                    case FactoryType.PaintBlue:
                        icon = Constants.factoryIconPaintBlue;
                        break;
                }
                renderTileAtPixel(0, renderScale, icon, pos + new Vector2(8, 8) * renderScale, Color.White);
            }

            int toolTile1 = -1;
            int toolTile2 = -1;
            switch (gameState.tool)
            {
                case Tool.Belt:
                    toolTile1 = Constants.beltTool;
                    break;
                case Tool.BeltJunction:
                    toolTile1 = Constants.beltJunctionTool;
                    break;
                case Tool.Delete:
                    toolTile1 = Constants.deleteTool;
                    break;
                case Tool.FactorySaw:
                    toolTile1 = Constants.factoryTool;
                    toolTile2 = Constants.factoryIconSaw;
                    break;
                case Tool.FactoryGlue:
                    toolTile1 = Constants.factoryTool;
                    toolTile2 = Constants.factoryIconGlue;
                    break;
                case Tool.FactoryPaintRed:
                    toolTile1 = Constants.factoryTool;
                    toolTile2 = Constants.factoryIconPaintRed;
                    break;
                case Tool.FactoryPaintGreen:
                    toolTile1 = Constants.factoryTool;
                    toolTile2 = Constants.factoryIconPaintGreen;
                    break;
                case Tool.FactoryPaintBlue:
                    toolTile1 = Constants.factoryTool;
                    toolTile2 = Constants.factoryIconPaintBlue;
                    break;
            }
            Util.ReleaseAssert(toolTile1 != -1);

            float toolRot = 0;
            if (gameState.tool == Tool.Belt)
            {
                switch (gameState.toolDirection)
                {
                    case Direction.Up:
                        toolRot = 0;
                        break;
                    case Direction.Right:
                        toolRot = 90;
                        break;
                    case Direction.Down:
                        toolRot = 180;
                        break;
                    case Direction.Left:
                        toolRot = 270;
                        break;
                }
            }

            Point mousePos_ = Mouse.GetState().Position;
            Vector2 mousePos = new Vector2(mousePos_.X, mousePos_.Y);
            renderTileAtPixel(0, renderScale, toolTile1, mousePos, Color.White, toolRot);
            if (toolTile2 != -1)
                renderTileAtPixel(0, renderScale, toolTile2, mousePos + new Vector2(10, 0) * renderScale, Color.White, toolRot);

            // top hud
            {
                Vector2 hudTopPos = new Vector2(Game1.game.Window.ClientBounds.Width / 2 - Textures.hudBottom.Bounds.Width * renderScale / 2, 0);
                spriteBatch.Draw(Textures.hudTop, hudTopPos, null, Color.White, 0f, new Vector2(0,0), new Vector2(renderScale, renderScale), SpriteEffects.None, 1);

                float secondsRemaining =(((float)gameState.targetTicksRemaining) / ((float)Constants.updatesPerSecond));
                float fractPart = secondsRemaining - (long)secondsRemaining;

                if (gameState.target != null && (!gameState.started || secondsRemaining < 55 || fractPart > 0.5))
                    renderItem(hudTopPos + new Vector2(32, 11) * renderScale, gameState.target);

                renderNumber(hudTopPos + new Vector2(85, 10) * renderScale, gameState.score);

                if (!gameState.started || secondsRemaining < 55 || fractPart > 0.5)
                    renderNumber(hudTopPos + new Vector2(85, 20) * renderScale, (int)secondsRemaining);

                if (gameState.nextTarget != null)
                    renderItem(hudTopPos + new Vector2(112, 11) * renderScale, gameState.nextTarget);
            }

            // bottom hud
            {
                Vector2 hudBottomPos = new Vector2(Game1.game.Window.ClientBounds.Width / 2 - Textures.hudBottom.Bounds.Width * renderScale / 2,
                    Game1.game.Window.ClientBounds.Height - Textures.hudBottom.Bounds.Height * renderScale);
                spriteBatch.Draw(Textures.hudBottom, hudBottomPos, null, Color.White, 0f, new Vector2(0,0), new Vector2(renderScale, renderScale), SpriteEffects.None, 1);
            }

        }

        private static void renderNumber(Vector2 pos, int number)
        {
            string str = "" + number;
            foreach (char c in str)
            {
                int tile = Constants.numberZero + (c - '0');
                renderTileAtPixel(0, renderScale, tile, pos, Color.White);
                pos.X += 5 * renderScale;
            }
        }

        private static void renderItem(Vector2 pos, List<ItemColor> parts)
        {
            float off = 3 * renderScale;
            Vector2[] positions = new []
            {
                pos + new Vector2(-off,-off),
                pos + new Vector2(+off,-off),
                pos + new Vector2(-off,+off),
                pos + new Vector2(+off,+off),
            };

            Color getColor(ItemColor color)
            {
                switch (color)
                {
                    case ItemColor.Red:
                        return new Color(255,0,0);
                        break;
                    case ItemColor.Green:
                        return new Color(0,255,0);
                        break;
                    case ItemColor.Blue:
                        return new Color(0,0,255);
                        break;
                }
                throw new Exception();
            }

            renderTileAtPixel(0, renderScale, Constants.item, positions[0], getColor(parts[0]));

            if (parts.Count == 3 && parts[1] == parts[2])
            {
                renderTileAtPixel(0, renderScale, Constants.item, positions[2], getColor(parts[1]));
                renderTileAtPixel(0, renderScale, Constants.item, positions[3], getColor(parts[2]));
            }
            else
            {
                if (parts.Count >= 2)
                    renderTileAtPixel(0, renderScale, Constants.item, positions[1], getColor(parts[1]));
                if (parts.Count >= 3)
                    renderTileAtPixel(0, renderScale, Constants.item, positions[2], getColor(parts[2]));
                if (parts.Count == 4)
                    renderTileAtPixel(0, renderScale, Constants.item, positions[3], getColor(parts[3]));
            }
        }


        public static Point getSelectedPoint(GameState gameState)
        {
            Point mousePos_ = Mouse.GetState().Position;
            Vector2 mousePos = new Vector2(mousePos_.X, mousePos_.Y);

            Vector2 selectedPoint_ = mousePos / (renderScale * Constants.tileSize) - gameState.viewpoint;
            Point selectedPoint = new Point((int)selectedPoint_.X, (int)selectedPoint_.Y);

            return selectedPoint;
        }

        private static Point getCoordsInTileset(int tileId)
        {
            int tilesetWidth = Textures.tileset.Width / Constants.tileSize;
            int tileY = tileId / tilesetWidth;
            int tileX = tileId - (tileY * tilesetWidth);
            return new Point(tileX, tileY);
        }

        private static void renderTileAtPixel(long ms, int scale, int tileId, Vector2 pixelPos, Color color, float rotationDegrees = 0f)
        {
            Point tilesetCoords = getCoordsInTileset(tileId);

            Rectangle sourceRect = new Rectangle()
            {
                X = tilesetCoords.X * Constants.tileSize,
                Y = tilesetCoords.Y * Constants.tileSize,
                Width = Constants.tileSize,
                Height = Constants.tileSize,
            };
            spriteBatch.Draw(Textures.tileset.getCurrentFrame(ms),
                             pixelPos + new Vector2(Constants.tileSize/2f, Constants.tileSize/2f) * scale,
                             sourceRect,
                             color,
                             MathHelper.ToRadians(rotationDegrees),
                             new Vector2(Constants.tileSize/2f, Constants.tileSize/2f),
                             new Vector2(scale, scale), SpriteEffects.None, 0);
        }
    }
}