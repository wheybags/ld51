using Microsoft.Xna.Framework;

namespace ld51
{
    public unsafe class Item
    {
        public Point position;
        public Vector2 renderPosition;

        int ticksSinceLastUpdate = 0;
        public void update(GameState gameState)
        {
            // update render pos
            {
                Vector2 pos = new Vector2(this.position.X, this.position.Y);

                if (renderPosition != pos)
                {
                    float tilesPerTick = Constants.itemMoveSpeedAnimateTilesPerSecond / ((float)Constants.updatesPerSecond);
                    Vector2 originalMovement = pos - renderPosition;

                    if (tilesPerTick < originalMovement.Length())
                    {
                        Vector2 movement = originalMovement;
                        movement.Normalize();
                        movement *= tilesPerTick;
                        renderPosition += movement;
                    }
                    else
                    {
                        renderPosition = pos;
                    }
                }
            }

            if (ticksSinceLastUpdate > Constants.updatesPerSecond / Constants.itemMoveSpeedRealTilesPerSecond)
            {
                Tile* tile = gameState.level.get(this.position);

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

                if (movement != Point.Zero)
                {
                    position += movement;
                    ticksSinceLastUpdate = 0;
                }
            }
            else
            {
                ticksSinceLastUpdate++;
            }
        }
    }
}