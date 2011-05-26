using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;

namespace XNAInterfaceComponents.AbstractComponents
{
    public abstract class ChildComponent : Component
    {
        public String text { get; set; }
        public Boolean enabled { get; set; }

        public ChildComponent(ParentComponent parent, Rectangle bounds)
            : base(bounds)
        {
            this.parent = parent;
            this.enabled = true;
            this.backgroundColor = Color.Red;
        }
    }
}
