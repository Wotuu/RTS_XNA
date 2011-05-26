using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Players;
using Microsoft.Xna.Framework;

namespace PathfindingTest.Buildings
{
    public class Fortress : Building
    {

        public Fortress(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = BuildingType.Fortress;
            this.constructDuration = 15;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Fortress");
        }
    }
}
