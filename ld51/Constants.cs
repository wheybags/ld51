using System.IO;
using System.Reflection;

namespace ld51
{
    public static class Constants
    {
        public static readonly string rootPath = getRootPath();
        public const int tileSize = 16;

        private static string getRootPath() {
            string root = Assembly.GetEntryAssembly()!.Location;
            while (!Directory.Exists(root + "/gfx"))
                root = Directory.GetParent(root).FullName;
            return root;
        }
    }
}