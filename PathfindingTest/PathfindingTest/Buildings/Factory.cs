using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Players;

namespace PathfindingTest.Buildings
{
    public class Factory : Building
    {

        public Factory(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = BuildingType.Factory;
            this.constructDuration = 8;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Factory");
        }
    }
}
