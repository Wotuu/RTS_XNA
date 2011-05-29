using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;

namespace XNAInterfaceComponents.Misc
{
    public class Caret : Drawable
    {
        public int width { get; set; }
        public int index { get; set; }
        public int blinkTicks { get; set; }
        public XNATextField parent { get; set; }
        public Color color { get; set; }

        private Boolean visible { get; set; }

        private long previousBlinkTicks { get; set; }

        public Caret(XNATextField parent)
        {
            this.parent = parent;
            previousBlinkTicks = System.DateTime.UtcNow.Ticks;
            blinkTicks = 1000000;
            this.color = Color.Black;
            this.width = 1;

            this.visible = true;
        }

        public void Update()
        {
            if (!this.parent.isFocussed) return;
            // Console.Out.WriteLine(System.DateTime.UtcNow.Millisecond + " > " + (previousBlinkTicks + blinkTicks));
            if (System.DateTime.UtcNow.Ticks > (previousBlinkTicks + blinkTicks))
            {
                this.visible = !this.visible;
                previousBlinkTicks = System.DateTime.UtcNow.Ticks;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (this.visible && this.parent.isFocussed )
            {
                Rectangle drawLocation = parent.GetScreenLocation();
                String toMeasure = "";
                Char[] array = parent.GetDisplayText().ToArray();
                for( int i = 0; i < this.index && i < array.Length; i++ ){
                    toMeasure += "" + array[i];
                }
                float offsetX = parent.font.MeasureString(toMeasure).X;
                if (offsetX == 0) offsetX = 1;
                ComponentUtil.DrawLine(sb,
                    new Point(drawLocation.X + parent.padding.left + (int)(offsetX), drawLocation.Y + parent.padding.top),
                    new Point(drawLocation.X + parent.padding.left + (int)(offsetX), drawLocation.Y - /*( * 2)*/parent.padding.top + drawLocation.Height),
                    this.color, width);
            }
        }
    }
}
