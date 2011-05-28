using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PathfindingTest;
using PathfindingTest.Players;
using PathfindingTest.UI;
using PathfindingTest.Pathfinding;
using PathfindingTest.Combat;

namespace PathfindingTest.Units
{
    public class Engineer : Unit
    {
        private Texture2D collisionRadiusTexture { get; set; }

        /// <summary>
        /// Engineer Constructor.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="cm"></param>
        /// <param name="startLocation"></param>
        /// <param name="c"></param>
        public Engineer(Player p, int x, int y) : base(p, x, y, 1f)
        {
            this.type = UnitType.Engineer;
            this.texture = Game1.GetInstance().Content.Load<Texture2D>("Units/Engineer");
            this.collisionRadiusTexture = Game1.GetInstance().Content.Load<Texture2D>("Misc/patternPreview");

            this.collisionRadius = texture.Width / 2;
        }

        /// <summary>
        /// Standard Update function.
        /// </summary>
        /// <param name="ks"></param>
        /// <param name="ms"></param>
        public override void Update(KeyboardState ks, MouseState ms)
        {
            UpdateMovement();
        }

        /// <summary>
        /// Standard Draw function.
        /// </summary>
        /// <param name="sb"></param>
        internal override void Draw(SpriteBatch sb)
        {
            //sb.Draw(this.collisionRadiusTexture,
            //    new Rectangle((int)(x - collisionRadius), (int)(y - collisionRadius), 
            //        (int)(collisionRadius * 2), (int)(collisionRadius * 2)), this.color);
            sb.Draw(this.texture, new Vector2(x - (texture.Width / 2), y - (texture.Height / 2)), this.color);

            if (this.DefineRectangle().Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                this.DrawHealthBar(sb);
            }
        }

        public override void OnAggroRecieved(AggroEvent e)
        {
            // Console.Out.WriteLine("Recieved aggro from something! D=");
        }

        public override void OnAggro(AggroEvent e)
        {
            // Console.Out.WriteLine("Aggroing something, *grins*");
        }
    }
}
