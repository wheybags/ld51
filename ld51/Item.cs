using Microsoft.Xna.Framework;

namespace ld51
{
    public unsafe class Item
    {
        public Point position;
        public Vector2 renderPosition;
        public long lastTouchedTick = 0;

        public Item(Point position)
        {
            this.position = position;
            this.renderPosition = new Vector2(position.X, position.Y);
        }

        int ticksSinceLastUpdate = 0;
        public void visualUpdate(GameState gameState)
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
    }
}