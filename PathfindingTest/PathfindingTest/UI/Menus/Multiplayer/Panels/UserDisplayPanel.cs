using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using SocketLibrary.Users;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.ParentComponents;
using XNAInterfaceComponents.AbstractComponents;

namespace PathfindingTest.UI.Menus.Multiplayer.Panels
{
    public class UserDisplayPanel : XNAPanel
    {
        public int index { get; set; }
        public User user { get; set; }
        public static int componentHeight = 40;
        public static int componentSpacing = 10;

        public XNACheckBox readyCheckBox { get; set; }
        public XNALabel usernameLbl { get; set; }
        public XNALabel teamLbl { get; set; }
        public XNADropdown teamDropdown { get; set; }
        public XNADropdown colorDropdown { get; set; }
        public XNAButton kickBtn { get; set; }


        public UserDisplayPanel(ParentComponent parent, User user, int index) :
            base(parent, new Rectangle())
        {
            this.user = user;
            this.UpdateBounds(index);
            

            readyCheckBox = new XNACheckBox(this, new Rectangle(10, 10, 20, 20), "");

            usernameLbl = new XNALabel(this, new Rectangle(40, 5, 230, 30), user.username);
            usernameLbl.font = MenuManager.BIG_TEXTFIELD_FONT;

            teamDropdown = new XNADropdown(this, new Rectangle(280, 5, 50, 30));
            teamDropdown.dropdownLineSpace = 15;
            teamDropdown.arrowSize = 8;
            for (int i = 0; i < 8; i++) teamDropdown.AddOption(i + "");

            colorDropdown = new XNADropdown(this, new Rectangle(340, 5, 50, 30));
            colorDropdown.dropdownLineSpace = 15;
            colorDropdown.arrowSize = 8;
            Color[] colors = new Color[] { Color.Black, Color.Blue, Color.Green, Color.Purple,  
                Color.Red, Color.Pink, Color.Yellow, Color.Orange };
            for (int i = 0; i < colors.Length; i++)
            {
                colorDropdown.AddOption("");
                colorDropdown.SetBackgroundColor(i + 1, colors[i]);
            }

            kickBtn = new XNAButton(this, new Rectangle(400, 5, 75, 30), "Kick");
        }

        /// <summary>
        /// Sets the index of this panel, and updates it's location
        /// </summary>
        /// <param name="index"></param>
        public void UpdateBounds(int index)
        {
            this.index = index;
            this.bounds = new Rectangle(5, 5 + (componentHeight + componentSpacing) * this.index,
                   480,
                   componentHeight);
        }
    }
}
