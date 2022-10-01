using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace ld51
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public Game1()
        {
#if DEBUG
            AllocConsole();
#endif
            graphics = new GraphicsDeviceManager(this);
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

        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            long gameTimeMs = Util.getMs(gameTime);


            base.Draw(gameTime);
        }
    }
}
