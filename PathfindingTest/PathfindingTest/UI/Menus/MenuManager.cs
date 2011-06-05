using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Managers;

namespace PathfindingTest.UI.Menus
{
    public class MenuManager
    {
        private MainMenu mainMenu;

        private static MenuManager instance;

        public enum Menu
        {
            MainMenu,
            OptionsMenu,
            MultiplayerMenu,
            NoMenu
        }

        public void ShowMenu(Menu menu)
        {
            if (menu == Menu.MainMenu)
            {
                if (this.mainMenu != null) ComponentManager.GetInstance().componentList.Remove(this.mainMenu);
                mainMenu = new MainMenu();
            }
            else if (menu == Menu.OptionsMenu)
            {

            }
            else if (menu == Menu.MultiplayerMenu)
            {

            }
            else if (menu == Menu.NoMenu)
            {
                if (this.mainMenu != null) this.mainMenu.Unload(); 
            }
        }

        private MenuManager()
        {

        }

        public static MenuManager GetInstance()
        {
            if (instance == null) instance = new MenuManager();
            return instance;
        }
    }
}
