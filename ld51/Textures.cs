using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace ld51
{
    public static class Textures
    {
        public static Animation tileset;
        public static Texture2D hudBottom;
        public static Texture2D hudTop;
        public static Texture2D win;
        public static Texture2D white;

        public static void loadTextures()
        {
            tileset = new Animation("gfx/tileset", (long)(1000 * 0.15f));
            hudBottom = loadTexture("gfx/hud-bottom.png");
            hudTop = loadTexture("gfx/hud-top.png");
            win = loadTexture("gfx/win.png");
            white = loadTexture("gfx/white.png");
        }

        private static Texture2D loadTexture(string path) => Texture2D.FromFile(Game1.game.GraphicsDevice, Path.Combine(Constants.rootPath, path));
    }
}