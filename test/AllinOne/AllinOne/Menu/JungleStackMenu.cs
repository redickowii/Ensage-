namespace AllinOne.Menu
{
    using Ensage.Common.Menu;

    internal class JungleStackMenu
    {
        #region Methods

        public static void Load()
        {
            //MainMenu.MenuStack.AddItem(new MenuItem("FPS", "More Fps").SetValue(true));
            MainMenu.MenuStack.AddItem(new MenuItem("stack", "Stack").SetValue(new KeyBind('F', KeyBindType.Toggle)));
            MainMenu.MenuStack.AddItem(new MenuItem("drawline", "Draw line?").SetValue(false));
        }

        public static void Update()
        {
            MenuVar.StackKey = MainMenu.MenuStack.Item("stack").GetValue<KeyBind>().Active;
            MenuVar.DrawStackLine = MainMenu.MenuStack.Item("drawline").GetValue<bool>();
            //MenuVar.MoreFps = MainMenu.MenuStack.Item("FPS").GetValue<bool>();
        }

        #endregion Methods
    }
}