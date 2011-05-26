using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using XNAInterfaceComponents.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using XNAInterfaceComponents.AbstractComponents;
using XNAInputHandler.MouseInput;

namespace XNAInterfaceComponents.Managers
{
    public class ComponentManager : Drawable, MouseClickListener, MouseMotionListener
    {
        public LinkedList<ParentComponent> componentList = new LinkedList<ParentComponent>();
        private static ComponentManager instance = null;

        /// <summary>
        /// Draws all panels
        /// </summary>
        /// <param name="sb">The spritebatch to draw on.</param>
        public void Draw(SpriteBatch sb)
        {
            foreach (ParentComponent c in componentList)
            {
                c.Draw(sb);
            }
        }

        /// <summary>
        /// Updates all the panels.
        /// </summary>
        public void Update()
        {
            foreach (ParentComponent c in componentList)
            {
                c.Update();
            }
        }



        private ComponentManager()
        {
            MouseManager.GetInstance().mouseClickedListeners += this.OnMouseClick;
            MouseManager.GetInstance().mouseReleasedListeners += this.OnMouseRelease;
            MouseManager.GetInstance().mouseMotionListeners += this.OnMouseMotion;
            MouseManager.GetInstance().mouseDragListeners += this.OnMouseDrag;
        }

        public static ComponentManager GetInstance()
        {
            if (instance == null) instance = new ComponentManager();
            return instance;
        }

        public void OnMouseClick(MouseEvent m_event)
        {
            foreach (ParentComponent pc in this.componentList)
            {
                pc.RequestFocusAt(m_event.location);
            }
        }

        public void OnMouseRelease(MouseEvent m_event)
        {

        }

        private Component previousMouseOver = null;
        public void OnMouseMotion(MouseEvent e)
        {
            foreach (ParentComponent pc in this.componentList)
            {
                Component mouseOver = pc.GetComponentAt(e.location);
                 
                if( previousMouseOver != null && previousMouseOver != mouseOver ) FireMouseExitEvents(e);   
                if (mouseOver != null)
                {
                    if (!mouseOver.isMouseOver)
                    {
                        ((MouseOverable)mouseOver).OnMouseEnter(e);
                    }
                    previousMouseOver = mouseOver;
                }

            }
        }

        /// <summary>
        /// Fire mouse exit events on those that currently have mouse over.
        /// </summary>
        /// <param name="e">The event to pass</param>
        private void FireMouseExitEvents(MouseEvent e)
        {
            for (int i = 0; i < this.componentList.Count; i++)
            {
                ParentComponent pc = this.componentList.ElementAt(i);
                if (pc.isMouseOver) ((MouseOverable)pc).OnMouseExit(e);
                for (int j = 0; j < pc.ChildCount(); j++)
                {
                    Component c = pc.ChildAt(j);
                    if (c.isMouseOver) ((MouseOverable)c).OnMouseExit(e);
                }
            }
        }

        public void OnMouseDrag(MouseEvent e)
        {
            // throw new NotImplementedException();
        }
    }
}
