namespace AllinOne.Menu
{
    using AllinOne.Variables;
    using Ensage.Common.Menu;

    internal class DevMenu
    {
        public static void Load()
        {
            MainMenu.Dev.AddItem(new MenuItem("ON/OFF", "ON/OFF").SetValue(true));
            MainMenu.Dev.AddItem(new MenuItem("1", "Show Errors ?").SetValue(false));
            MainMenu.Dev.AddItem(new MenuItem("2", "Dev Info").SetValue(false));

            MainMenu.Dev.AddItem(new MenuItem("effects", "Effects").SetValue(new StringList(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" })))
                .ValueChanged += (sender, arg) => { AllDrawing.ShowMeMore.ShowHeroEffect(arg.GetNewValue<StringList>().SelectedIndex); };

            MainMenu.Dev.AddItem(new MenuItem("x", "XXX").SetValue(new Slider(300, 0, 1920)));
            MainMenu.Dev.AddItem(new MenuItem("y", "YYY").SetValue(new Slider(300, 0, 1080)));
        }

        public static void Update()
        {
            MenuVar.OnOff = MainMenu.Dev.Item("ON/OFF").GetValue<bool>();
            MenuVar.ShowInfo = MainMenu.Dev.Item("2").GetValue<bool>();
            MenuVar.ShowErrors = MainMenu.Dev.Item("1").GetValue<bool>();
        }
    }
}