﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ld51
{
    public enum Tool
    {
        Belt,
        Delete,
        Factory,
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


        InputHandler inputHandler = new InputHandler();


        public GameState(Tilemap level)
        {
            this.level = level;

            items.Add(new Item());
        }

        private bool isFactoryPart(Point p)
        {
            Tile* tile = this.level.get(p);
            return tile->tileId == Constants.factoryTopLeft ||
                   tile->tileId == Constants.factoryTopRight ||
                   tile->tileId == Constants.factoryBottomLeft ||
                   tile->tileId == Constants.factoryBottomRight;
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

                    case Tool.Factory:
                    {
                        if (this.level.isPointValid(selectedPoint) &&
                            this.level.isPointValid(selectedPoint + new Point(1,1)) &&
                            !isFactoryPart(selectedPoint) &&
                            !isFactoryPart(selectedPoint + new Point(1,0)) &&
                            !isFactoryPart(selectedPoint + new Point(0,1)) &&
                            !isFactoryPart(selectedPoint + new Point(1,1)))
                        {
                            this.level.get(selectedPoint)->tileId = Constants.factoryTopLeft;
                            this.level.get(selectedPoint + new Point(1,0))->tileId = Constants.factoryTopRight;
                            this.level.get(selectedPoint + new Point(0,1))->tileId = Constants.factoryBottomLeft;
                            this.level.get(selectedPoint + new Point(1,1))->tileId = Constants.factoryBottomRight;
                        }
                        break;
                    }

                    case Tool.Delete:
                    {
                        if (this.level.isPointValid(selectedPoint))
                        {
                            if (isFactoryPart(selectedPoint))
                            {
                                Point topLeft = new Point();
                                switch (this.level.get(selectedPoint)->tileId)
                                {
                                    case Constants.factoryTopLeft:
                                        topLeft = selectedPoint;
                                        break;
                                    case Constants.factoryTopRight:
                                        topLeft = selectedPoint + new Point(-1, 0);
                                        break;
                                    case Constants.factoryBottomRight:
                                        topLeft = selectedPoint + new Point(-1, -1);
                                        break;
                                    case Constants.factoryBottomLeft:
                                        topLeft = selectedPoint + new Point(0, -1);
                                        break;
                                }

                                this.level.get(topLeft)->tileId = Constants.floor;
                                this.level.get(topLeft + new Point(1,0))->tileId = Constants.floor;
                                this.level.get(topLeft + new Point(0,1))->tileId = Constants.floor;
                                this.level.get(topLeft + new Point(1,1))->tileId = Constants.floor;
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
            if (inputHandler.downThisFrame(Input.SelectFactory))
                this.tool = Tool.Factory;

            foreach (Item item in this.items)
                item.update(this);
        }
    }
}