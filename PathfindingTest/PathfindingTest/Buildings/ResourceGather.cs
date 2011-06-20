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
using PathfindingTest.Multiplayer.Data;
using SocketLibrary.Protocol;

namespace PathfindingTest.Buildings
{
    public class ResourceGather : Building
    {

        public ResourceGather(Player p, Color c)
            : base(p)
        {
            this.c = c;
            this.constructC = new Color(this.c.R, this.c.G, this.c.B, 0);
            this.type = Type.Resources;
            this.constructDuration = 3;

            this.maxHealth = 1000f;
            this.currentHealth = 0f;

            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Buildings/Resources");

            if (Game1.GetInstance().IsMultiplayerGame())
            {
                Boolean isLocal = this.p == Game1.CURRENT_PLAYER;
                this.multiplayerData = new BuildingMultiplayerData(this, isLocal);
                if (isLocal)
                {
                    this.multiplayerData.RequestServerID(BuildingHeaders.TYPE_RESOURCES_GATHER);
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