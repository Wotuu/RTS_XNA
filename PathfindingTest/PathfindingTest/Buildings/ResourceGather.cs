using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PathfindingTest.Players;

namespace PathfindingTest.Buildings
{
    public class ResourceGather : Building
    {

        public ResourceGather(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = BuildingType.Resources;
            this.constructDuration = 3;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Resources");
        }
    }
}