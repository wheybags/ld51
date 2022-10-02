using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Xna.Framework;

namespace ld51
{
    public static class Util
    {
        public static void ReleaseAssert(bool val)
        {
            if (!val)
                throw new Exception("assertion failed");
        }

        public static void DebugAssert(bool val)
        {
#if DEBUG
            if (!val)
                throw new Exception("assertion failed");
#endif
        }

        public static long getMs(GameTime time)
        {
            return time.TotalGameTime.Ticks / TimeSpan.TicksPerMillisecond;
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, Int64 count);

        public static XmlDocument openXML(string path)
        {
            using FileStream f = File.OpenRead(Path.Combine(Constants.rootPath, path));
            XmlDocument doc = new XmlDocument();
            doc.Load(f);
            return doc;
        }

        public static XmlElement getUniqueByTagName(XmlElement e, string tagName)
        {
            var list = e.GetElementsByTagName(tagName);
            Util.ReleaseAssert(list.Count <= 1);
            if (list.Count == 0)
                return null;
            return (XmlElement)list.Item(0);
        }

        public static XmlElement getUniqueByTagName(XmlDocument e, string tagName)
        {
            var list = e.GetElementsByTagName(tagName);
            Util.ReleaseAssert(list.Count <= 1);
            if (list.Count == 0)
                return null;
            return (XmlElement)list.Item(0);
        }

        public static Direction rotateCW(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Up;
            }
            throw new Exception();
        }

        public static Point directionToVec(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return new Point(0, -1);
                case Direction.Right:
                    return new Point(1, 0);
                case Direction.Down:
                    return new Point(0, 1);
                case Direction.Left:
                    return new Point(-1, 0);
            }
            throw new Exception();
        }
    }
}