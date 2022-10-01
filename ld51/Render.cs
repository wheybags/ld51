﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ld51
{
    public static unsafe class Render
    {
        private static SpriteBatch spriteBatch = new SpriteBatch(Game1.game.GraphicsDevice);
        public static int renderScale = 3;

        public static void render(GraphicsDevice GraphicsDevice, GameState gameState)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(0, 0, 0, 255));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap);

            Point selectedPoint = getSelectedPoint(gameState);

            for (int y = 0; y < gameState.level.h; y++)
            {
                for (int x = 0; x < gameState.level.w; x++)
                {
                    Color color = Color.White;
                    Point currentPoint = new Point(x, y);
                    if (currentPoint == selectedPoint ||
                        (gameState.tool == Tool.Factory && (currentPoint == selectedPoint + new Point(0,1) ||
                                                            currentPoint == selectedPoint + new Point(1,0) ||
                                                            currentPoint == selectedPoint + new Point(1,1)))
                       )
                    {
                        color = Color.Green;
                    }

                    renderTileAtPixel(renderScale,
                                      gameState.level.get(x, y)->tileId,
                                      (gameState.viewpoint + new Vector2(x, y)) * Constants.tileSize * renderScale,
                                      color);
                }
            }

            foreach (Item item in gameState.items)
            {
                renderTileAtPixel(renderScale,
                                  Constants.item,
                                  (gameState.viewpoint + item.renderPosition) * Constants.tileSize * renderScale,
                                  Color.White);
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

                spriteBatch.Draw(Textures.tileset,
                    (gameState.viewpoint + new Vector2(factory.topLeft.X, factory.topLeft.Y)) * Constants.tileSize * renderScale,
                    sourceRect,
                    Color.White,
                    0,
                    Vector2.Zero,
                    new Vector2(renderScale, renderScale),
                    SpriteEffects.None,
                    0);
            }

            int toolTile = -1;
            switch (gameState.tool)
            {
                case Tool.Belt:
                    toolTile = Constants.beltTool;
                    break;
                case Tool.Delete:
                    toolTile = Constants.deleteTool;
                    break;
                case Tool.Factory:
                    toolTile = Constants.factoryTool;
                    break;
            }
            Util.ReleaseAssert(toolTile != -1);

            float toolRot = 0;
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

            Point mousePos_ = Mouse.GetState().Position;
            Vector2 mousePos = new Vector2(mousePos_.X, mousePos_.Y);
            renderTileAtPixel(renderScale, toolTile, mousePos, Color.White, toolRot);

            spriteBatch.End();
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

        private static void renderTileAtPixel(int scale, int tileId, Vector2 pixelPos, Color color, float rotationDegrees = 0f)
        {
            Point tilesetCoords = getCoordsInTileset(tileId);

            Rectangle sourceRect = new Rectangle()
            {
                X = tilesetCoords.X * Constants.tileSize,
                Y = tilesetCoords.Y * Constants.tileSize,
                Width = Constants.tileSize,
                Height = Constants.tileSize,
            };
            spriteBatch.Draw(Textures.tileset,
                             pixelPos + new Vector2(Constants.tileSize/2f, Constants.tileSize/2f) * scale,
                             sourceRect,
                             color,
                             MathHelper.ToRadians(rotationDegrees),
                             new Vector2(Constants.tileSize/2f, Constants.tileSize/2f),
                             new Vector2(scale, scale), SpriteEffects.None, 0);
        }
    }
}