using System;
using Microsoft.Xna.Framework;

namespace ld51
{
    public class GameState
    {
        public Tilemap level {get; private set;}
        public Vector2 viewpoint {get; private set;}

        InputHandler inputHandler = new InputHandler();


        public GameState(Tilemap level)
        {
            this.level = level;
        }

        public void update(long gameTimeMs)
        {
            Console.WriteLine(gameTimeMs);
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

            viewpoint += movement * Render.renderScale * 1.5f;
        }
    }
}