using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SocketLibrary.Protocol;
using PathfindingTest.Multiplayer.Data;

namespace PathfindingTest.Buildings
{
    public class Fortress : Building
    {

        public Fortress(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Fortress;
            this.constructDuration = 15;

            this.maxHealth = 5000f;
            this.currentHealth = 0f;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Fortress");

            if (Game1.GetInstance().IsMultiplayerGame())
            {
                Boolean isLocal = this.p == Game1.CURRENT_PLAYER;
                this.multiplayerData = new BuildingMultiplayerData(this, isLocal);
                if (isLocal)
                {
                    this.multiplayerData.RequestServerID(BuildingHeaders.TYPE_FORTRESS);
                }
            }
        }

        public override void Update(KeyboardState ks, MouseState ms)
        {
            DefaultUpdate(ks, ms);
        }

        internal override void Draw(SpriteBatch sb)
        {
            DefaultDraw(sb);
        }
    }
}
