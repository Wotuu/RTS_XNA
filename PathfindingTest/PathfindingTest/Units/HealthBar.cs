using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PathfindingTest.Units
{
    public class HealthBar
    {
        public int percentage { get; set; }
        public Unit unit { get; set; }

        private Texture2D texture { get; set; }

        private static Color BORDER_COLOR = new Color(0, 0, 0, 255),
            BACKGROUND_COLOR = new Color(255, 0, 0, 255),
            FOREGROUND_COLOR = new Color(0, 255, 0, 255);

        public HealthBar(Unit unit)
        {
            texture = Game1.GetInstance().Content.Load<Texture2D>("Misc/solid");
            this.unit = unit;
        }


        internal void Draw(SpriteBatch sb){

            int w = unit.texture.Width;
            int h = 5;
            int x = (int)unit.x - (w / 2);
            int y = (int)unit.y - (unit.texture.Height / 2) - h;
            int innerWidth = (int)((w / 100.0) * percentage);

            sb.Draw(texture, new Rectangle(x, y, w + 2, h), BORDER_COLOR);
            sb.Draw(texture, new Rectangle(x + 1, y + 1, w, h - 2), BACKGROUND_COLOR);
            sb.Draw(texture, new Rectangle(x + 1, y + 1, innerWidth, h - 2), FOREGROUND_COLOR);
        }
    }
}
