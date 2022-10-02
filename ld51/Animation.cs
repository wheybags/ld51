using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ld51
{
    public class Animation
    {
        readonly List<Texture2D> frames;
        private long frameIntervalMs;

        public int Width => frames[0].Width;
        public int Height => frames[0].Height;

        public Animation(string basePath, long frameIntervalMs = 500)
        {
            frames = new List<Texture2D>();
            for (int i = 1;; i++)
            {
                string framePath = Path.Combine(Constants.rootPath, basePath + i + ".png");
                if (!File.Exists(framePath))
                    break;

                frames.Add(Texture2D.FromFile(Game1.game.GraphicsDevice, framePath));
            }
            Util.ReleaseAssert(frames.Count > 0);
            this.frameIntervalMs = frameIntervalMs;
        }

        public Texture2D getCurrentFrame(long gameTimeMs)
        {
            long totalAnimationLength = frameIntervalMs * frames.Count;
            return frames[(int)((gameTimeMs % totalAnimationLength) / frameIntervalMs)];
        }
    }
}