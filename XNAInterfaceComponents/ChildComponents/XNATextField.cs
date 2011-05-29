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
using Microsoft.Xna.Framework.Input;

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

        /// <summary>
        /// Gets the display text
        /// </summary>
        /// <returns>The display text</returns>
        public String GetDisplayText()
        {
            Char[] array = this.text.ToArray();

            float currentStringWidth = 0;
            float textWidth = this.font.MeasureString(this.text).X;
            float viewportX = this.bounds.Width - this.padding.left - this.padding.right;

            if (viewportX > textWidth) return this.text;

            String result = "";
            for (int i = Math.Max(this.caret.index, array.Length) - 1; i >= 0; i--)
            {
                Char currentChar = array[i];

                currentStringWidth += this.font.MeasureString(currentChar + "").X;
                if (currentStringWidth < viewportX)
                {
                    result += currentChar + "";
                }
            } 

            char[] charArray = result.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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
            String displayText = GetDisplayText();
            Vector2 fontDimensions = this.font.MeasureString(displayText);
            float drawY = drawRect.Y + (this.bounds.Height / 2) - (fontDimensions.Y / 2);

            sb.DrawString(font, displayText,
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

        #region Insert & Delete character
        /// <summary>
        /// Inserts a string at the current caret position.
        /// </summary>
        /// <param name="s">The string to add</param>
        private void InsertStringAtCaret(String s)
        {
            String newString = "";
            Char[] array = this.text.ToCharArray();
            for (int i = 0; i < this.caret.index; i++)
            {
                newString += "" + array[i];
            }
            newString += s;
            for (int i = this.caret.index; i < array.Length; i++)
            {
                newString += "" + array[i];
            }
            this.caret.index += s.Length;
            this.text = newString;
        }

        /// <summary>
        /// Deletes a character, and places the caret before or after the deleted character
        /// </summary>
        /// <param name="index">The index to remove</param>
        /// <param name="isBackspace"></param>
        private void DeleteCharacterAt(int index, Boolean isBackspace)
        {
            String newString = "";
            Char[] array = this.text.ToCharArray();

            if (isBackspace)
            {
                for (int i = 0; i < index - 1; i++)
                {
                    newString += "" + array[i];
                }
                for (int i = index; i < array.Length; i++)
                {
                    newString += "" + array[i];
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    newString += "" + array[i];
                }
                for (int i = index + 1; i < array.Length; i++)
                {
                    newString += "" + array[i];
                }
            }
            this.text = newString;
            if (isBackspace) this.caret.index = index - 1;
            else this.caret.index = index;
        }
#endregion

        private void ProcessKey(KeyEvent e)
        {
            if (this.isFocussed)
            {
                String keyString = e.key.ToString();
                if (keyString.Equals("Left"))
                {
                    if (this.caret.index > 0) this.caret.index--;
                }
                else if (keyString.Equals("Right"))
                {
                    if (this.caret.index < this.text.Length) this.caret.index++;
                }
                else if (keyString.Contains("Back"))
                {
                    if (this.caret.index > 0) DeleteCharacterAt(this.caret.index, true);
                }
                else if (keyString.Contains("Delete"))
                {
                    if (this.caret.index < this.text.Length) DeleteCharacterAt(this.caret.index, false);
                }
                else if (keyString.Equals("Enter"))
                {
                    this.OnFocusLost();
                }
                else if( keyString.Equals("F1") ||
                    keyString.Equals("F2") ||
                    keyString.Equals("F3") ||
                    keyString.Equals("F4") ||
                    keyString.Equals("F5") ||
                    keyString.Equals("F6") ||
                    keyString.Equals("F7") ||
                    keyString.Equals("F8") ||
                    keyString.Equals("F9") ||
                    keyString.Equals("F10") ||
                    keyString.Equals("F11") ||
                    keyString.Equals("F12"))
                {
                    // Do nothing
                }
                else
                {
                    String typedChar = "";
                    #region Convert keyString to a typedChar
                    if (keyString.Equals("Space")) typedChar = " ";
                    else if (keyString.Equals("OemQuestion"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "?";
                        else typedChar = "/";
                    }
                    else if (keyString.Equals("OemPeriod")) 
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = ">";
                        else typedChar = ".";
                    }
                    else if (keyString.Equals("OemSemicolon"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = ":";
                        else typedChar = ";";
                    }
                    else if (keyString.Equals("OemQuotes"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "\"";
                        else typedChar = "'";
                    } 
                    else if (keyString.Equals("OemComma"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "<";
                        else typedChar = ",";
                    }
                    else if (keyString.Equals("OemOpenBrackets"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "{";
                        else typedChar = "[";
                    }
                    else if (keyString.Equals("OemCloseBrackets"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "}";
                        else typedChar = "]";
                    } 
                    else if (keyString.Equals("OemPipe"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "|";
                        else typedChar = "\\";
                    }
                    else if (keyString.Equals("OemMinus"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "_";
                        else typedChar = "-";
                    }
                    else if (keyString.Equals("OemPlus"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "+";
                        else typedChar = "=";
                    }
                    else if (keyString.Equals("OemTilde"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "~";
                        else typedChar = "`";
                    }
                    else if (keyString.Equals("D0"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = ")";
                        else typedChar = "0";
                    }
                    else if (keyString.Equals("D1"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "!";
                        else typedChar = "1";
                    }
                    else if (keyString.Equals("D2"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "@";
                        else typedChar = "2";
                    }
                    else if (keyString.Equals("D3"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "#";
                        else typedChar = "3";
                    }
                    else if (keyString.Equals("D4"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "$";
                        else typedChar = "4";
                    }
                    else if (keyString.Equals("D5"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "%";
                        else typedChar = "5";
                    }
                    else if (keyString.Equals("D6"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "^";
                        else typedChar = "6";
                    }
                    else if (keyString.Equals("D7"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "&";
                        else typedChar = "7";
                    }
                    else if (keyString.Equals("D8"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "*";
                        else typedChar = "8";
                    }
                    else if (keyString.Equals("D9"))
                    {
                        if (e.modifiers.Contains(KeyEvent.Modifier.SHIFT)) typedChar = "(";
                        else typedChar = "9";
                    }
                    else
                    {
                        if (keyString.Length == 1)
                        {
                            if( !e.modifiers.Contains(KeyEvent.Modifier.SHIFT) )
                            keyString = keyString.ToLower();
                        }
                        typedChar = keyString;
                    }
                    #endregion


                    this.InsertStringAtCaret(typedChar);
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
