using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAInputHandler.MouseInput;

namespace XNAInterfaceComponents.Components
{
    public class XNAPanel : ParentComponent
    {
        public XNAPanel(ParentComponent parent, Rectangle bounds)
            : base(parent, bounds)
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            if (!this.visible) return;
            if (this.clearTexture == null) clearTexture = ComponentUtil.GetClearTexture2D(sb);
            // Console.Out.WriteLine("Drawing panel!");

            Color drawColor = new Color();
            if (this.isMouseOver) drawColor = this.mouseOverColor;
            else drawColor = this.backgroundColor;

            sb.Draw(clearTexture, this.GetScreenLocation(), drawColor);
            if( this.border != null ) this.border.Draw(sb);

            foreach (Component child in this.children)
            {
                child.Draw(sb);
            }
        }

        public override void Update()
        {
            foreach (Component child in this.children)
            {
                child.Update();
            }
        }

        public override void OnMouseEnter(MouseEvent m_event)
        {
            this.isMouseOver = true;
            Console.Out.WriteLine("Panel @ " + this.GetScreenLocation() + " mouse entered!");
        }

        public override void OnMouseExit(MouseEvent m_event)
        {
            this.isMouseOver = false;
            Console.Out.WriteLine("Panel @ " + this.GetScreenLocation() + " mouse exitted!");
        }
    }
}
