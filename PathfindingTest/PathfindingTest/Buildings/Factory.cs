using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Players;
using Microsoft.Xna.Framework.Input;
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;

namespace PathfindingTest.Buildings
{
    public class Factory : Building
    {

        public Factory(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Factory;
            this.constructDuration = 8;

            this.maxHealth = 3000f;
            this.currentHealth = 0f;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Factory");

            if (Game1.GetInstance().IsMultiplayerGame())
            {
                Boolean isLocal = this.p == Game1.CURRENT_PLAYER;
                this.multiplayerData = new BuildingMultiplayerData(this, isLocal);
                if (isLocal)
                {
                    this.multiplayerData.RequestServerID(BuildingHeaders.TYPE_FACTORY);
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
