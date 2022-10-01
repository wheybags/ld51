using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ld51
{
    public class Factory
    {
        public Point topLeft;
        public List<Item> inputs = new List<Item>();
        public List<Item> outputs = new List<Item>();

        public Factory(Point topLeft)
        {
            this.topLeft = topLeft;
        }
    }
}