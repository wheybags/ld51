﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;

namespace ld51
{
    public enum Tool
    {
        Belt,
        Delete,
        FactorySaw,
        FactoryGlue,
        FactoryPaintRed,
        FactoryPaintGreen,
        FactoryPaintBlue,
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public unsafe class GameState
    {
        public Tilemap level {get; private set;}
        public Vector2 viewpoint = new Vector2(1, 0);

        public Tool tool = Tool.Belt;
        public Direction toolDirection = Direction.Up;

        public List<Item> items = new List<Item>();
        public Dictionary<Point, Item> itemsByPos = new Dictionary<Point, Item>();

        public List<Factory> factories = new List<Factory>();
        public Dictionary<Point, Factory> factoriesByPos = new Dictionary<Point, Factory>();


        InputHandler inputHandler = new InputHandler();
        long tick = 0;


        public GameState(Tilemap level)
        {
            this.level = level;

            addItem(new Point(0,0), new Item(new List<ItemColor>() {ItemColor.Red, ItemColor.Red, ItemColor.Red, ItemColor.Red}));
            addItem(new Point(1,0), new Item(new List<ItemColor>() {ItemColor.Green, ItemColor.Green, ItemColor.Green, ItemColor.Green}));
            addItem(new Point(2,0), new Item(new List<ItemColor>() {ItemColor.Blue, ItemColor.Blue, ItemColor.Blue, ItemColor.Blue}));
        }

        private bool isFactoryPart(Point p)
        {
            return factoriesByPos.ContainsKey(p);
        }

        public void update(long gameTimeMs)
        {
            // Console.WriteLine(gameTimeMs);
            inputHandler.update(gameTimeMs);

            Vector2 movement = new Vector2();
            if (inputHandler.currentState.getInput(Input.Left))
                movement.X++;
            if (inputHandler.currentState.getInput(Input.Right))
                movement.X--;
            if (inputHandler.currentState.getInput(Input.Up))
                movement.Y++;
            if (inputHandler.currentState.getInput(Input.Down))
                movement.Y--;

            viewpoint += movement * Render.renderScale * 0.25f;


            if (inputHandler.downThisFrame(Input.RotateTool))
            {
                switch (this.toolDirection)
                {
                    case Direction.Up:
                        this.toolDirection = Direction.Right;
                        break;
                    case Direction.Right:
                        this.toolDirection = Direction.Down;
                        break;
                    case Direction.Down:
                        this.toolDirection = Direction.Left;
                        break;
                    case Direction.Left:
                        this.toolDirection = Direction.Up;
                        break;
                }
            }

            if (inputHandler.currentState.getInput(Input.ActivateTool))
            {
                Point selectedPoint = Render.getSelectedPoint(this);

                switch (this.tool)
                {
                    case Tool.Belt:
                    {
                        if (this.level.isPointValid(selectedPoint) && !isFactoryPart(selectedPoint))
                        {
                            int newTile = 0;
                            switch (this.toolDirection)
                            {
                                case Direction.Up:
                                    newTile = Constants.beltUp;
                                    break;
                                case Direction.Right:
                                    newTile = Constants.beltRight;
                                    break;
                                case Direction.Down:
                                    newTile = Constants.beltDown;
                                    break;
                                case Direction.Left:
                                    newTile = Constants.beltLeft;
                                    break;
                            }
                            this.level.get(selectedPoint)->tileId = newTile;
                        }
                        break;
                    }

                    case Tool.FactorySaw:
                    case Tool.FactoryGlue:
                    case Tool.FactoryPaintRed:
                    case Tool.FactoryPaintGreen:
                    case Tool.FactoryPaintBlue:
                    {
                        if (this.level.isPointValid(selectedPoint) &&
                            this.level.isPointValid(selectedPoint + new Point(1,1)) &&
                            !isFactoryPart(selectedPoint) && this.level.get(selectedPoint)->tileId == Constants.floor &&
                            !isFactoryPart(selectedPoint + new Point(1,0)) && this.level.get(selectedPoint + new Point(1,0))->tileId == Constants.floor &&
                            !isFactoryPart(selectedPoint + new Point(0,1)) && this.level.get(selectedPoint + new Point(0,1))->tileId == Constants.floor &&
                            !isFactoryPart(selectedPoint + new Point(1,1)) && this.level.get(selectedPoint + new Point(1,1))->tileId == Constants.floor)
                        {
                            FactoryType type = FactoryType.Saw;
                            switch (this.tool)
                            {
                                case Tool.FactorySaw:
                                    type = FactoryType.Saw;
                                    break;
                                case Tool.FactoryGlue:
                                    type = FactoryType.Glue;
                                    break;
                                case Tool.FactoryPaintRed:
                                    type = FactoryType.PaintRed;
                                    break;
                                case Tool.FactoryPaintGreen:
                                    type = FactoryType.PaintGreen;
                                    break;
                                case Tool.FactoryPaintBlue:
                                    type = FactoryType.PaintBlue;
                                    break;
                            }
                            this.addFactory(selectedPoint, new Factory(type));
                        }
                        break;
                    }

                    case Tool.Delete:
                    {
                        if (this.level.isPointValid(selectedPoint))
                        {
                            this.factoriesByPos.TryGetValue(selectedPoint, out Factory factory);
                            if (factory != null)
                            {
                                removeFactory(factory);
                            }
                            else
                            {
                                this.level.get(selectedPoint)->tileId = Constants.floor;
                            }
                        }
                        break;
                    }
                }
            }

            if (inputHandler.downThisFrame(Input.SelectDelete))
                this.tool = Tool.Delete;
            if (inputHandler.downThisFrame(Input.SelectBelt))
                this.tool = Tool.Belt;
            if (inputHandler.downThisFrame(Input.SelectFactorySaw))
                this.tool = Tool.FactorySaw;
            if (inputHandler.downThisFrame(Input.SelectFactoryGlue))
                this.tool = Tool.FactoryGlue;
            if (inputHandler.downThisFrame(Input.SelectFactoryPaintRed))
                this.tool = Tool.FactoryPaintRed;
            if (inputHandler.downThisFrame(Input.SelectFactoryPaintGreen))
                this.tool = Tool.FactoryPaintGreen;
            if (inputHandler.downThisFrame(Input.SelectFactoryPaintBlue))
                this.tool = Tool.FactoryPaintBlue;

            this.updateBeltItems();

            this.tick++;
        }

        private void addItem(Point pos, Item item)
        {
            item.position = pos;
            item.renderPosition = new Vector2(pos.X, pos.Y);

            this.itemsByPos.Add(pos, item);
            this.items.Add(item);
        }

        private void removeItem(Item item)
        {
            this.items.Remove(item);
            this.itemsByPos.Remove(item.position);
        }

        private void addFactory(Point pos, Factory factory)
        {
            factory.topLeft = pos;
            this.factories.Add(factory);
            for (int y = 0; y < Constants.factoryDimensions.Y; y++)
            {
                for (int x = 0; x < Constants.factoryDimensions.X; x++)
                {
                    factoriesByPos[pos + new Point(x, y)] = factory;
                }
            }
        }

        private void removeFactory(Factory factory)
        {
            this.factories.Remove(factory);
            for (int y = 0; y < Constants.factoryDimensions.Y; y++)
            {
                for (int x = 0; x < Constants.factoryDimensions.X; x++)
                {
                    factoriesByPos.Remove(factory.topLeft + new Point(x, y));
                }
            }
        }

        int ticksSinceLastItemUpdate = 0;
        private void updateBeltItems()
        {
            if (ticksSinceLastItemUpdate > Constants.updatesPerSecond / Constants.itemMoveSpeedRealTilesPerSecond)
            {
                foreach(Item item in this.items)
                    recursiveUpdateItem(item);

                foreach(Factory factory in this.factories)
                    updateFactory(factory);

                ticksSinceLastItemUpdate = 0;
            }
            else
            {
                ticksSinceLastItemUpdate++;
            }

            foreach (Item item in this.items)
                item.visualUpdate(this);
        }

        private void updateFactory(Factory factory)
        {
            // suck up inputs
            for (int x = 0; x < Constants.factoryDimensions.X; x++)
            {
                Point point = new Point(factory.topLeft.X + x, factory.topLeft.Y - 1);
                FactoryBuffer factoryBuffer = factory.getInput(point);

                itemsByPos.TryGetValue(point, out Item item);
                if (item != null && factoryBuffer.items.Count < factoryBuffer.maxSize && item.lastTouchedTick == this.tick)
                {
                    this.removeItem(item);
                    factoryBuffer.items.Add(item);
                }
            }

            // craft
            switch (factory.type)
            {
                case FactoryType.Saw:
                {
                    if (factory.inputsL.items.Count > 0 && factory.outputs.items.Count <= factory.outputs.maxSize - 2)
                    {
                        Item input = factory.inputsL.items[factory.inputsL.items.Count - 1];
                        factory.inputsL.items.RemoveAt(factory.inputsL.items.Count - 1);

                        Util.ReleaseAssert(input.parts.Count > 1 && input.parts.Count <= 4);
                        switch (input.parts.Count)
                        {
                            case 1:
                                factory.outputs.items.Add(input);
                                break;
                            case 2:
                                factory.outputs.items.Add(new Item(new List<ItemColor>(){input.parts[0]}));
                                factory.outputs.items.Add(new Item(new List<ItemColor>(){input.parts[1]}));
                                break;
                            case 3:
                                factory.outputs.items.Add(new Item(new List<ItemColor>(){input.parts[0], input.parts[1]}));
                                factory.outputs.items.Add(new Item(new List<ItemColor>(){input.parts[2]}));
                                break;
                            case 4:
                                factory.outputs.items.Add(new Item(new List<ItemColor>(){input.parts[0], input.parts[1]}));
                                factory.outputs.items.Add(new Item(new List<ItemColor>(){input.parts[2], input.parts[3]}));
                                break;
                        }
                    }
                    break;
                }

                case FactoryType.Glue:
                {
                    if (factory.outputs.items.Count < factory.outputs.maxSize)
                    {
                        Item inputL = null;
                        if (factory.inputsL.items.Count > 0)
                            inputL = factory.inputsL.items[factory.inputsL.items.Count - 1];

                        Item inputR = null;
                        if (factory.inputsR.items.Count > 0)
                            inputR = factory.inputsR.items[factory.inputsR.items.Count - 1];

                        if (inputL != null && inputL.parts.Count == 4)
                        {
                            factory.outputs.items.Add(inputL);
                            factory.inputsL.items.RemoveAt(factory.inputsL.items.Count - 1);
                            inputL = null;
                        }

                        if (inputR != null && inputR.parts.Count == 4)
                        {
                            factory.outputs.items.Add(inputR);
                            factory.inputsR.items.RemoveAt(factory.inputsR.items.Count - 1);
                            inputR = null;
                        }

                        int totalCount = 0;
                        totalCount += inputL != null ? inputL.parts.Count : 0;
                        totalCount += inputR != null ? inputR.parts.Count : 0;

                        if (totalCount > 4)
                        {
                            Util.ReleaseAssert(inputR != null && inputL != null);
                            factory.outputs.items.Add(inputL);
                            factory.inputsL.items.RemoveAt(factory.inputsL.items.Count - 1);
                            factory.outputs.items.Add(inputR);
                            factory.inputsR.items.RemoveAt(factory.inputsR.items.Count - 1);
                        }
                        else if (inputL != null && inputR != null)
                        {
                            inputL.parts.AddRange(inputR.parts);
                            inputL.parts.Sort();
                            factory.outputs.items.Add(inputL);
                            factory.inputsL.items.RemoveAt(factory.inputsL.items.Count - 1);
                            factory.inputsR.items.RemoveAt(factory.inputsR.items.Count - 1);
                        }
                    }
                    break;
                }

                case FactoryType.PaintRed:
                case FactoryType.PaintGreen:
                case FactoryType.PaintBlue:
                {
                    if (factory.inputsL.items.Count > 0 && factory.outputs.items.Count < factory.outputs.maxSize)
                    {
                        Item input = factory.inputsL.items[factory.inputsL.items.Count - 1];
                        factory.inputsL.items.RemoveAt(factory.inputsL.items.Count - 1);

                        ItemColor color = ItemColor.Red;
                        switch (factory.type)
                        {
                            case FactoryType.PaintRed:
                                color = ItemColor.Red;
                                break;
                            case FactoryType.PaintGreen:
                                color = ItemColor.Green;
                                break;
                            case FactoryType.PaintBlue:
                                color = ItemColor.Blue;
                                break;
                        }

                        for (int i = 0; i < input.parts.Count; i++)
                            input.parts[i] = color;
                        factory.outputs.items.Add(input);
                    }
                    break;
                }
            }

            // dump outputs
            while (factory.outputs.items.Count > 0)
            {
                Item output = factory.outputs.items[factory.outputs.items.Count - 1];

                bool placed = false;
                for (int x = 0; x < Constants.factoryDimensions.X; x++)
                {
                    Point point = new Point(factory.topLeft.X + x, factory.topLeft.Y + Constants.factoryDimensions.Y);
                    itemsByPos.TryGetValue(point, out Item blocker);
                    if (blocker == null)
                    {
                        addItem(point, output);
                        placed = true;
                        break;
                    }
                }

                if (placed)
                    factory.outputs.items.RemoveAt(factory.outputs.items.Count - 1);
                else
                    break;
            }
        }

        private bool recursiveUpdateItem(Item item)
        {
            if (item.lastTouchedTick == this.tick)
                return false;

            item.lastTouchedTick = this.tick;

            Tile* tile = this.level.get(item.position);

            Point movement = new Point();
            switch (tile->tileId)
            {
                case Constants.beltRight:
                    movement = new Point(1, 0);
                    break;
                case Constants.beltDown:
                    movement = new Point(0, 1);
                    break;
                case Constants.beltLeft:
                    movement = new Point(-1, 0);
                    break;
                case Constants.beltUp:
                    movement = new Point(0, -1);
                    break;
            }

            if (movement == Point.Zero)
                return false;

            Point destination = item.position + movement;

            if (!this.level.isPointValid(destination))
                return false;

            this.itemsByPos.TryGetValue(destination, out Item blockedByItem);

            if (blockedByItem != null && !recursiveUpdateItem(blockedByItem))
                return false;

            Tile* destinationTile = this.level.get(destination);
            if (destinationTile->tileId != Constants.beltRight &&
                destinationTile->tileId != Constants.beltDown &&
                destinationTile->tileId != Constants.beltLeft &&
                destinationTile->tileId != Constants.beltUp)
            {
                return false;
            }

            itemsByPos.Remove(item.position);
            item.position += movement;
            itemsByPos[item.position] = item;

            return true;
        }
    }
}