using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Interfaces;
using XNAInterfaceComponents.AbstractComponents;
using Microsoft.Xna.Framework.Graphics;
using XNAInputHandler.MouseInput;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.Misc;
using XNAInputLibrary.KeyboardInput;

namespace XNAInterfaceComponents.ChildComponents
{
    public class XNATextField : ChildComponent, Focusable, KeyboardListener
    {
        public Caret caret { get; set; }


        public XNATextField(ParentComponent parent, Rectangle bounds)
            : base(parent, bounds)
        {
            this.caret = new Caret(this);
            KeyboardManager.GetInstance().keyPressedListeners += this.OnKeyPressed;
            KeyboardManager.GetInstance().keyTypedListeners += this.OnKeyTyped;
            KeyboardManager.GetInstance().keyReleasedListeners += this.OnKeyReleased;
        }

        public override void Draw(SpriteBatch sb)
        {
            // Get a clear texture if there aint any yet.
            if (this.clearTexture == null) clearTexture = ComponentUtil.GetClearTexture2D(sb);
            // Return if this component has no parent, or if it isn't visible
            if (this.parent == null || this.visible == false) return;

            // Determine the drawcolor
            Color drawColor = new Color();
            if (this.isMouseOver) drawColor = this.mouseOverColor;
            else drawColor = this.backgroundColor;

            // Get the location on the screen on which to draw this button.
            Rectangle drawRect = this.GetScreenLocation();
            // Draw the button
            sb.Draw(clearTexture, drawRect, drawColor);

            // Draw the caret
            this.caret.Draw(sb);

            // Draw the text
            Vector2 fontDimensions = this.font.MeasureString(this.text);
            float drawY = drawRect.Y + (this.bounds.Height / 2) - (fontDimensions.Y / 2);

            sb.DrawString(font, this.text,
                new Vector2(drawRect.X + this.padding.left,
                    drawY), this.fontColor);

        }

        public override void Update()
        {
            this.caret.Update();
        }

        public override void OnMouseEnter(MouseEvent e)
        {

        }

        public override void OnMouseExit(MouseEvent e)
        {

        }

        public void OnFocusReceived()
        {
            this.isFocussed = true;
            Console.Out.WriteLine("TextField received focus");
        }

        public void OnFocusLost()
        {
            this.isFocussed = false;
            Console.Out.WriteLine("TextField lost focus");
        }

        private void ProcessKey(KeyEvent e)
        {
            if (this.isFocussed)
            {
                if (e.key.ToString().Contains("Back"))
                {
                    String newString = "";
                    Char[] array = this.text.ToCharArray();
                    if (this.caret.index > 0)
                    {
                        this.caret.index--;
                        for (int i = 0; i < this.caret.index; i++)
                        {
                            newString += "" + array[i];
                        }
                        for (int i = this.caret.index + 1; i < array.Length; i++)
                        {
                            newString += "" + array[i];
                        }
                        this.text = newString;
                    }
                } else {
                    String typedChar = "";
                    if (e.key.ToString().Contains("Space")) typedChar = " ";
                    else if (e.key.ToString().Contains("Ques")) typedChar = "?";
                    else typedChar = e.key.ToString();


                    this.text = this.text + typedChar;
                    this.caret.index = this.text.Length;
                }
            }
        }

        public void OnKeyPressed(KeyEvent e)
        {
            ProcessKey(e);
            Console.Out.WriteLine("Pressed: " + e.key.ToString());
        }

        public void OnKeyTyped(KeyEvent e)
        {
            ProcessKey(e);
            Console.Out.WriteLine("Typed: " + e.key.ToString());
        }

        public void OnKeyReleased(KeyEvent e)
        {
            Console.Out.WriteLine("Released: " + e.key.ToString());
            // throw new NotImplementedException();
        }
    }
}
