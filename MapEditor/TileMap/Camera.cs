using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MapEditor.TileMap
{
    public class Camera
    {
        public Vector2 Position;

        public Camera()
        {
            this.Position = new Vector2();
        }

        public void LockToVector(Vector2 position)
        {
            Position.X = position.X + Engine.TileWidth
                         - (Engine.ViewPortWidth / 2);
            Position.Y = position.Y + Engine.TileHeight
                         - (Engine.ViewPortHeight / 2);
        }

        public void LockCamera()
        {
            Position.X = MathHelper.Clamp(
                Position.X,
                0,
                TileMap.WidthInPixels - Engine.ViewPortWidth);
            Position.Y = MathHelper.Clamp(
                Position.Y,
                0,
                TileMap.HeightInPixels - Engine.ViewPortHeight);
        }
    }
}
