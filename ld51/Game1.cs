using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ld51
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;

        public static Game1 game;

        GameState gameState;

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public Game1()
        {
#if DEBUG
            AllocConsole();
#endif
            graphics = new GraphicsDeviceManager(this);
            game = this;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            if (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height >= 2160)
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
                Render.renderScale = 3;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
                Render.renderScale = 2;
            }
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Textures.loadTextures();
            gameState = new GameState(new Tilemap("level/level.tmx"));
        }

        private long lastUpdate = -999;
        protected override void Update(GameTime gameTime)
        {
            long gameTimeMs = Util.getMs(gameTime);

            long updateInterval = 1000 / Constants.updatesPerSecond;

            if (gameTimeMs > lastUpdate + updateInterval)
            {
                gameState.update(gameTimeMs);
                lastUpdate = gameTimeMs;
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            long gameTimeMs = Util.getMs(gameTime);
            Render.render(gameTimeMs, GraphicsDevice, gameState);

            base.Draw(gameTime);
        }
    }
}
