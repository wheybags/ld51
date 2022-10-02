using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace ld51
{
    public static class Textures
    {
        public static Texture2D tileset;
        public static Texture2D hudBottom;
        public static Texture2D hudTop;

        public static void loadTextures()
        {
            tileset = loadTexture("gfx/tileset.png");
            hudBottom = loadTexture("gfx/hud-bottom.png");
            hudTop = loadTexture("gfx/hud-top.png");
        }

        private static Texture2D loadTexture(string path) => Texture2D.FromFile(Game1.game.GraphicsDevice, Path.Combine(Constants.rootPath, path));
    }
}