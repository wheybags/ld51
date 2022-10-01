using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;

namespace ld51
{
    public struct Tile
    {
        public int tileId;
    }

    public unsafe class Tilemap
    {
        private UnManagedArray<Tile> tiles;

        public int w { get; private set; }
        public int h { get; private set; }

        public Tilemap(string path, XmlDocument doc = null)
        {
            doc ??= Util.openXML(path);

            // I fucking hate XML.
            // And I fucking hate XML in C# even more.

            var map = Util.getUniqueByTagName(doc, "map");
            var layer = Util.getUniqueByTagName(map, "layer");
            this.w = int.Parse(layer.GetAttribute("width"));
            this.h = int.Parse(layer.GetAttribute("height"));
            var data = Util.getUniqueByTagName(layer, "data");
            Util.ReleaseAssert(data.GetAttribute("encoding") == "csv");


            tiles = new UnManagedArray<Tile>(sizeof(Tile) * this.w * this.h);

            Tile* current = tiles.get(0);
            foreach (var item in data.InnerText.Split(','))
            {
                *current = new Tile()
                {
                  tileId  = int.Parse(item.Trim()) - 1
                };
                current++;
            }
        }

        public Tile* get(int x, int y)
        {
            Util.DebugAssert(isPointValid(x, y));
            return tiles.get((y * this.w) + x);
        }
        public Tile* get(Point p) { return get(p.X, p.Y); }

        public bool isPointValid(int x, int y)
        {
            return x >= 0 && x < this.w && y >= 0 && y < this.h;
        }
        public bool isPointValid(Point p) { return isPointValid(p.X, p.Y); }

    }
}