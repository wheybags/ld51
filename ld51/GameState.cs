using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ld51
{
    public enum Tool
    {
        Belt,
        Delete,
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
                if (this.level.isPointValid(selectedPoint))
                {
                    switch (this.tool)
                    {
                        case Tool.Belt:
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
                            break;
                        }
                    }
                }
            }

            foreach (Item item in this.items)
                item.update(this);
        }
    }
}