using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace ld51
{
    public static class Textures
    {
        public static Texture2D tileset;

        public static void loadTextures()
        {
            tileset = loadTexture("gfx/tileset.png");
        }

        private static Texture2D loadTexture(string path) => Texture2D.FromFile(Game1.game.GraphicsDevice, Path.Combine(Constants.rootPath, path));
    }
}