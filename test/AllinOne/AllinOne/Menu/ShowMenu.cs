namespace AllinOne.Menu
{
    using Ensage.Common.Menu;

    internal class ShowMenu
    {
        private static readonly StringList Effects = new StringList(new[] { "Smoke", "Shadow Amulet", "Diffusal", "All" });

        private static readonly Slider MiniSlider = new Slider(25, 5, 40);

        public static void Load()
        {
            MainMenu.ShowMeMore.AddItem(new MenuItem("visible", "Visible").SetValue(true));
            MainMenu.ShowMeMore.AddItem(new MenuItem("rosh", "Show Roshan timer?").SetValue(true));
            MainMenu.ShowMeMore.AddItem(new MenuItem("showlastpos", "Show last position?").SetValue(true));
            MainMenu.ShowMeMore.AddItem(new MenuItem("showlastposmini", "Show Last pos on Mmap?").SetValue(true));
            MainMenu.ShowMeMore.AddItem(new MenuItem("scalemini", "Minimap icon scale").SetValue(MiniSlider));

            var subMenu = new Menu("Maphack", "Maphack", false);
            subMenu.AddItem(new MenuItem("Maphack", "Maphack").SetValue(false));

            MainMenu.ShowMeMore.AddSubMenu(subMenu);

            subMenu = new Menu("Illusions", "Illusions", false);
            subMenu.AddItem(new MenuItem("showillusions", "Show illusions?").SetValue(true));
            subMenu.AddItem(new MenuItem("illusionseffect", "Illusion effect").SetValue(Effects))
                .ValueChanged += (sender, arg) => { AllDrawing.ShowMeMore.ShowIllusion(arg.GetNewValue<StringList>().SelectedIndex); };
            subMenu.AddItem(new MenuItem("heroeffects", "Hero effects").SetValue(new StringList(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" })))
                .ValueChanged += (sender, arg) => { AllDrawing.ShowMeMore.ShowHeroEffect(arg.GetNewValue<StringList>().SelectedIndex); };
            MainMenu.ShowMeMore.AddSubMenu(subMenu);
        }

        public static void Update()
        {
            MenuVar.Maphack = MainMenu.ShowMeMore.Item("Maphack").GetValue<bool>();
            MenuVar.ShowIllusions = MainMenu.ShowMeMore.Item("showillusions").GetValue<bool>();
            MenuVar.IllusionsEffectMenu =
                MainMenu.ShowMeMore.Item("illusionseffect").GetValue<StringList>().SelectedIndex;
            MenuVar.HeroEffectMenu = MainMenu.ShowMeMore.Item("heroeffects").GetValue<StringList>().SelectedIndex;
            MenuVar.ShowLastPos = MainMenu.ShowMeMore.Item("showlastpos").GetValue<bool>();
            MenuVar.ShowLastPosMini = MainMenu.ShowMeMore.Item("showlastposmini").GetValue<bool>();
            MenuVar.ShowRoshanTimer = MainMenu.ShowMeMore.Item("rosh").GetValue<bool>();
            MenuVar.VisiblebyEnemy = MainMenu.ShowMeMore.Item("visible").GetValue<bool>();
            MenuVar.MiniScale = MainMenu.ShowMeMore.Item("scalemini").GetValue<Slider>().Value;
        }
    }
}