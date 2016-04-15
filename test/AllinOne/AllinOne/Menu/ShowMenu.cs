namespace AllinOne.Menu
{
    using Ensage.Common.Menu;

    internal class ShowMenu
    {
        #region Fields

        private static readonly StringList Effects = new StringList(new[] { "Smoke", "Shadow Amulet", "Diffusal", "All" });

        private static readonly Slider MiniSlider = new Slider(25, 5, 40);

        #endregion Fields

        #region Methods

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

            subMenu = new Menu("Tower Range", "towerrange", false);
            subMenu.AddItem(new MenuItem("owntowers", "My Towers").SetValue(false).SetTooltip("Show your tower range."));
            subMenu.AddItem(new MenuItem("enemytowers", "Enemies Towers").SetValue(false).SetTooltip("Show the enemies towers range."));
            subMenu.AddItem(new MenuItem("truesight", "Truesight").SetValue(false));
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

            MenuVar.OwnTowers = MainMenu.ShowMeMore.Item("owntowers").GetValue<bool>();
            MenuVar.EnemiesTowers = MainMenu.ShowMeMore.Item("enemytowers").GetValue<bool>();
            MenuVar.TrueSight = MainMenu.ShowMeMore.Item("truesight").GetValue<bool>();
        }

        #endregion Methods
    }
}