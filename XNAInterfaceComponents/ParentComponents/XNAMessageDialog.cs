using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.Misc;
using Microsoft.Xna.Framework.Graphics;

namespace XNAInterfaceComponents.ParentComponents
{
    public class XNAMessageDialog : XNAPanel
    {
        private XNALabel label { get; set; }
        private String message { get; set; }

        public static int CLIENT_WINDOW_WIDTH { get; set; }
        public static int CLIENT_WINDOW_HEIGHT { get; set; }


        public XNAButton button1 { get; set; }
        public XNAButton button2 { get; set; }
        public XNAButton button3 { get; set; }
        private int buttonWidth { get; set; }
        private int buttonSpacing { get; set; }


        public Padding padding { get; set; }
        public SpriteFont font { get; set; }
        public DialogType type { get; set; }

        public enum DialogType
        {
            OK,
            YES_CANCEL,
            YES_NO,
            YES_NO_CANCEL
        }

        /// <summary>
        /// Re-does the layout for this Message Dialog. Should be called when the user specifies a custom font
        /// rather than the the default font, or when the client window size changes.
        /// </summary>
        public void DoLayout()
        {
            Vector2 messageDimensions = this.font.MeasureString(message);
            int windowHeight = (int)messageDimensions.Y + this.padding.top + this.padding.bottom + 50;

            this.bounds = new Rectangle((CLIENT_WINDOW_WIDTH / 2) - 200,
                (CLIENT_WINDOW_HEIGHT / 2) - (windowHeight / 2),
                400, windowHeight);

            this.label = new XNALabel(this, new Rectangle(
                0,
                this.padding.top,
                this.bounds.Width,
                (int)messageDimensions.Y), message);
            this.label.textAlign = XNALabel.TextAlign.CENTER;
            this.label.border = null;


            if (type == DialogType.OK)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth / 2), this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "OK");
                this.button1.onClickListeners += this.Dispose;
            }
            else if (type == DialogType.YES_CANCEL)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth) - (this.buttonSpacing / 2),
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "Yes");
                this.button1.onClickListeners += this.Dispose;

                this.button2 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) + (this.buttonSpacing / 2),
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "Cancel");
                this.button2.onClickListeners += this.Dispose;
            }
            else if (type == DialogType.YES_NO)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth) - (this.buttonSpacing / 2),
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "Yes");
                this.button1.onClickListeners += this.Dispose;

                this.button2 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) + (this.buttonSpacing / 2),
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "No");
                this.button2.onClickListeners += this.Dispose;
            }
            else if (type == DialogType.YES_NO_CANCEL)
            {
                this.button1 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth / 2) - this.buttonWidth - this.buttonSpacing,
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "Yes");
                this.button1.onClickListeners += this.Dispose;

                this.button2 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) - (this.buttonWidth / 2), 
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "No");
                this.button2.onClickListeners += this.Dispose;

                this.button3 = new XNAButton(this, new Rectangle(
                    (this.bounds.Width / 2) + (this.buttonWidth / 2) + this.buttonSpacing,
                    this.label.bounds.Bottom + 10,
                    this.buttonWidth, 40), "Cancel");
                this.button3.onClickListeners += this.Dispose;
            }
        }

        private XNAMessageDialog(String message, DialogType type)
            : base(null,
                new Rectangle())
        {
            this.message = message;
            this.font = ChildComponent.DEFAULT_FONT;
            this.padding = new Padding(5, 5, 5, 5);
            this.type = type;
            this.border = new Border(this, 3, Color.Black);
            this.buttonWidth = 100;
            this.buttonSpacing = 30;

            this.DoLayout();
        }

        /// <summary>
        /// Every button calls this, as in .. on click, remove!
        /// </summary>
        /// <param name="source">The button that clicked it (irrelevant schmuck)</param>
        private void Dispose(XNAButton source)
        {
            this.Dispose();
        }

        /// <summary>
        /// Disposes the message dialog. Does the same as calling Unload();.
        /// </summary>
        public void Dispose()
        {
            this.Unload();
        }

        /// <summary>
        /// Creates a new message dialog. Note that you have to add your button listeners to this pane for it to do something.
        /// </summary>
        /// <param name="game">The current game.</param>
        /// <param name="message">The message of the pane.</param>
        /// <param name="type">The type of the pane</param>
        /// <returns></returns>
        public static XNAMessageDialog CreateDialog(String message, DialogType type)
        {
            return new XNAMessageDialog(message, type);
        }
    }
}
