using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAInterfaceComponents.AbstractComponents
{
    public abstract class ChildComponent : Component
    {
        public static SpriteFont DEFAULT_FONT { get; set; }
        public static Color DEFAULT_FONT_COLOR { get; set; }
        public String text { get; set; }
        public Boolean enabled { get; set; }
        public SpriteFont font { get; set; }
        public Color fontColor { get; set; }

        static ChildComponent()
        {
            DEFAULT_FONT_COLOR = Color.Black;
        }

        public ChildComponent(ParentComponent parent, Rectangle bounds)
            : base(bounds)
        {
            this.parent = parent;
            this.enabled = true;
            this.backgroundColor = Color.Red;
            this.font = DEFAULT_FONT;
            this.fontColor = DEFAULT_FONT_COLOR;
        }
    }
}
