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
        public const int spawner = 4;
        public const int goal = 3;
        public const int bin = 2;

        public const int beltUp = 0;
        public const int beltDown = 16;
        public const int beltRight = 32;
        public const int beltLeft = 48;
        public const int beltJunction = 64;

        public const int deleteTool = 17;
        public const int beltTool = 18;
        public const int beltJunctionTool = 34;
        public const int factoryTool = 33;

        public const int factoryTopLeft = 19;
        public static Point factoryDimensions = new Point(2, 2);

        public const int item = 52;

        public const int numberZero = 96;

        public const int factoryIconSaw = 21;
        public const int factoryIconGlue = 85;
        public const int factoryIconPaintRed = 37;
        public const int factoryIconPaintGreen = 53;
        public const int factoryIconPaintBlue = 69;

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