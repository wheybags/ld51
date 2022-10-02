using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ld51
{
    public enum FactoryType
    {
        Saw,
        Glue,
        PaintRed,
        PaintGreen,
        PaintBlue,
    }

    public class FactoryBuffer
    {
        public List<Item> items;
        public int maxSize;
    }

    public class Factory
    {
        public Point topLeft;
        public FactoryBuffer inputsL;
        public FactoryBuffer inputsR;
        public FactoryBuffer outputs;
        public FactoryType type;

        public Factory(FactoryType type)
        {
            this.type = type;
            switch (this.type)
            {
                case FactoryType.PaintRed:
                case FactoryType.PaintGreen:
                case FactoryType.PaintBlue:
                case FactoryType.Saw:
                {
                    this.inputsL = new FactoryBuffer()
                    {
                        items = new List<Item>(),
                        maxSize = 2,
                    };
                    this.inputsR = inputsL;
                    break;
                }
                case FactoryType.Glue:
                {
                    this.inputsL = new FactoryBuffer()
                    {
                        items = new List<Item>(),
                        maxSize = 1,
                    };
                    this.inputsR = new FactoryBuffer()
                    {
                        items = new List<Item>(),
                        maxSize = 1,
                    };
                    break;
                }
            }

            this.outputs = new FactoryBuffer()
            {
                items = new List<Item>(),
                maxSize = 2,
            };
        }

        public FactoryBuffer getInput(Point p)
        {
            if (p == this.topLeft + new Point(0, -1))
                return this.inputsL;
            if (p == this.topLeft + new Point(1, -1))
                return this.inputsR;
            Util.ReleaseAssert(false);
            return null;
        }
    }
}