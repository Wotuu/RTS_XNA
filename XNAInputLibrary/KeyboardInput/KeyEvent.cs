﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace XNAInputLibrary.KeyboardInput
{
    public class KeyEvent
    {
        public Keys key { get; set; }
        public Type type { get; set; }
        public Modifier[] modifiers { get; set; }

        public enum Modifier
        {
            ALT, 
            SHIFT,
            CTRL
        }

        public enum Type
        {
            Pressed,
            Typed,
            Released
        }

        public KeyEvent(Keys key, Type type)
        {
            this.key = key;
            this.type = type;
        }
    }
}
