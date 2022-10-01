using System.IO;
using System.Reflection;

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

        private static string getRootPath() {
            string root = Assembly.GetEntryAssembly()!.Location;
            while (!Directory.Exists(root + "/gfx"))
                root = Directory.GetParent(root).FullName;
            return root;
        }
    }
}