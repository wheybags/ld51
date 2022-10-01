using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

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
            base.Initialize();

            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
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

            long updateInterval = 1000 / 60;

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
            Render.render(GraphicsDevice, gameState);

            base.Draw(gameTime);
        }
    }
}
