using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace ld51
{
    public static class Constants
    {
        public static readonly string rootPath = getRootPath();
        public const int tileSize = 16;

        public const int floor = 1;

        public const int beltUp = 0;
        public const int beltDown = 16;
        public const int beltRight = 32;
        public const int beltLeft = 48;

        public const int deleteTool = 17;
        public const int beltTool = 18;
        public const int factoryTool = 33;

        public const int factoryTopLeft = 19;
        public static Point factoryDimensions = new Point(2, 2);

        public const int item = 2;

        public const long updatesPerSecond = 60;

        public const float itemMoveSpeedRealTilesPerSecond = 2f;
        public const float itemMoveSpeedAnimateTilesPerSecond = 3f;

        private static string getRootPath() {
            string root = Assembly.GetEntryAssembly()!.Location;
            while (!Directory.Exists(root + "/gfx"))
                root = Directory.GetParent(root).FullName;
            return root;
        }
    }
}