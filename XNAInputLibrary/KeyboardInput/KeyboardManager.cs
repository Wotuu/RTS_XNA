using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using XNAInputLibrary.KeyboardInput;


public delegate void OnKeyPressed(KeyEvent e);
public delegate void OnKeyTyped(KeyEvent e);
public delegate void OnKeyReleased(KeyEvent e);

namespace XNAInputLibrary.KeyboardInput
{
    public class KeyboardManager
    {
        public static KeyboardManager instance = null;
        public OnKeyPressed keyPressedListeners { get; set; }
        public OnKeyTyped keyTypedListeners { get; set; }
        public OnKeyReleased keyReleasedListeners { get; set; }

        private long firstPressedKeyTicks { get; set; }
        private Keys firstPressedTypeableKey { get; set; }
        public int keyTypedLagMS { get; set; }

        private Keys[] previousFramePressedKeys { get; set; }



        private KeyboardManager()
        {
            keyTypedLagMS = 500;
        }

        private Boolean IsTypeable(Keys key)
        {
            if (key.ToString().Length == 1) return true;
            if (key.ToString().Contains("Left") ||
                key.ToString().Contains("Right") ||
                key.ToString().Contains("Lock") ||
                key.ToString().Contains("Tab") ||
                key.ToString().Contains("Back") ||
                key.ToString().Contains("Enter") ||
                key.ToString().Contains("Up") ||
                key.ToString().Contains("Down") ||
                key.ToString().Contains("Del") ||
                key.ToString().Contains("Esc") ||
                key.ToString().Contains("F1") ||
                key.ToString().Contains("F2") ||
                key.ToString().Contains("F3") ||
                key.ToString().Contains("F4") ||
                key.ToString().Contains("F5") ||
                key.ToString().Contains("F6") ||
                key.ToString().Contains("F7") ||
                key.ToString().Contains("F8") ||
                key.ToString().Contains("F9") ||
                key.ToString().Contains("F10") ||
                key.ToString().Contains("F11") ||
                key.ToString().Contains("F12"))
            {
                return false;
            }
            return true;
        }

        public void Update(KeyboardState state)
        {
            LinkedList<Keys> releasedKeys = this.GetReleasedKeys(state.GetPressedKeys());
            foreach (Keys key in releasedKeys)
            {
                if (this.keyReleasedListeners != null)
                {
                    keyReleasedListeners(new KeyEvent(key, KeyEvent.Type.Released));
                }
                if (state.GetPressedKeys().Length == 0)
                {
                    Console.Out.WriteLine("Reset first typed key!");
                    this.firstPressedTypeableKey = new Keys();
                    this.firstPressedKeyTicks = 0;
                }
            }


            foreach (Keys key in state.GetPressedKeys())
            {
                if (this.keyPressedListeners != null
                    && !this.previousFramePressedKeys.Contains(key))
                {
                    keyPressedListeners(new KeyEvent(key, KeyEvent.Type.Pressed));
                }

                if ( firstPressedTypeableKey.ToString() == "None" )
                {
                    firstPressedTypeableKey = key;
                    firstPressedKeyTicks = System.DateTime.UtcNow.Ticks;
                }

                if (key == firstPressedTypeableKey)
                {
                    if (System.DateTime.UtcNow.Ticks > ((firstPressedKeyTicks + (keyTypedLagMS * 10000))))
                    {
                        keyTypedListeners(new KeyEvent(key, KeyEvent.Type.Typed));
                    }
                }
            }
            /*
            foreach (Keys key in state.GetPressedKeys())
            {
                if (lastPressedKey == key)
                {
                    keyLastPressedTicks[index] = System.DateTime.UtcNow.Ticks;
                    keyIsHeld[index] = true;

                    if (this.keyPressedListeners != null)
                    {
                        // Only for characters
                        if (key.ToString().Length == 1) keyPressedListeners(new KeyEvent(key, KeyEvent.Type.Pressed));
                        else  {

                        }
                    }
                    // Console.Out.WriteLine("Key pressed: " + key.ToString());
                }
                else
                {
                    // Console.Out.WriteLine(System.DateTime.UtcNow.Ticks + ">" + ((keyLastPressedTicks[index] + (keyTypedLagMS * 10000))));
                    if (System.DateTime.UtcNow.Ticks > ((keyLastPressedTicks[index] + (keyTypedLagMS * 10000))))
                    {
                        if (this.keyTypedListeners != null)
                        {
                            // Only for characters
                            if( key.ToString().Length == 1 ) keyTypedListeners(new KeyEvent(key, KeyEvent.Type.Typed));
                            else
                            {

                            }
                        }
                        //Console.Out.WriteLine("Key typed: " + key.ToString());
                    }
                }
            }*/
            this.previousFramePressedKeys = state.GetPressedKeys();
        }

        /// <summary>
        /// Gets the released keys since last frame
        /// </summary>
        /// <param name="newKeys"></param>
        /// <returns></returns>
        private LinkedList<Keys> GetReleasedKeys(Keys[] newKeys)
        {
            LinkedList<Keys> result = new LinkedList<Keys>();
            if (this.previousFramePressedKeys == null) return result;
            for (int i = 0; i < this.previousFramePressedKeys.Length; i++)
            {
                Keys currentKey = this.previousFramePressedKeys[i];
                result.AddLast(currentKey);
                for (int j = 0; j < newKeys.Length; j++)
                {
                    if (currentKey == newKeys[j])
                    {
                        result.Remove(currentKey);
                        break;
                    }
                }
            }
            return result;
        }

        public static KeyboardManager GetInstance()
        {
            if (instance == null) instance = new KeyboardManager();
            return instance;
        }
    }
}
