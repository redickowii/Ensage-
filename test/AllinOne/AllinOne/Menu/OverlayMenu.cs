namespace AllinOne.Menu
{
    using Ensage.Common.Menu;
    using SharpDX;

    internal class OverlayMenu
    {
        #region Fields

        private static readonly Slider Alpha = new Slider(200, 0, 255);
        private static readonly Slider Colour = new Slider(0, 0, 255);
        private static readonly Slider ColourHealthDef = new Slider(102, 0, 255);
        private static readonly Slider ColourManaDef = new Slider(255, 0, 255);
        private static readonly Slider Height = new Slider(4, 3, 25);
        private static readonly Slider RuneScale = new Slider(25, 5, 40);

        #endregion Fields

        #region Methods

        public static void Load()
        {
            var subMenu = new Menu("Ally top overlay", "allytopoverlay", false);
            var subsubMenu = new Menu("Health", "HealthA", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayallyhp", "Show Ally top Hp?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OHHA", "Height").SetValue(Height));
            var menu = new MenuItem("OHACR", "Red").SetValue(Colour);
            menu.FontColor = new ColorBGRA(255, 50, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OHACG", "Green").SetValue(ColourHealthDef);
            menu.FontColor = new ColorBGRA(50, 255, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OHACB", "Blue").SetValue(Colour);
            menu.FontColor = new ColorBGRA(100, 100, 255, 200);
            subsubMenu.AddItem(menu);
            subMenu.AddSubMenu(subsubMenu);
            subsubMenu = new Menu("Mana", "ManaA", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayallymp", "Show Ally top Mana?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OMHA", "Height").SetValue(Height));
            menu = new MenuItem("OMACR", "Red").SetValue(Colour);
            menu.FontColor = new ColorBGRA(255, 50, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OMACG", "Green").SetValue(Colour);
            menu.FontColor = new ColorBGRA(50, 255, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OMACB", "Blue").SetValue(ColourManaDef);
            menu.FontColor = new ColorBGRA(100, 100, 255, 200);
            subsubMenu.AddItem(menu);
            subMenu.AddSubMenu(subsubMenu);
            subsubMenu = new Menu("Ultimate", "UltimateA", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayallyultline", "Show Ally top Ult Line?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OUHA", "Height").SetValue(Height));
            subsubMenu.AddItem(new MenuItem("showtopoverlayallyulttext", "Show Ally top Ult Text?").SetValue(false));
            subMenu.AddSubMenu(subsubMenu);
            subMenu.AddItem(new MenuItem("showtopoverlayally", "Show Ally top Owerlay?").SetValue(false));
            MainMenu.Overlay.AddSubMenu(subMenu);

            subMenu = new Menu("Enemy top overlay", "enemytopoverlay", false);
            subsubMenu = new Menu("Health", "HealthE", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayenemyhp", "Show Enemy top Hp?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OHHE", "Height").SetValue(Height));
            menu = new MenuItem("OHECR", "Red").SetValue(Colour);
            menu.FontColor = new ColorBGRA(255, 50, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OHECG", "Green").SetValue(ColourHealthDef);
            menu.FontColor = new ColorBGRA(50, 255, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OHECB", "Blue").SetValue(Colour);
            menu.FontColor = new ColorBGRA(100, 100, 255, 200);
            subsubMenu.AddItem(menu);
            subMenu.AddSubMenu(subsubMenu);
            subsubMenu = new Menu("Mana", "ManaE", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayenemymp", "Show Enemy top Mana?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OMHE", "Height").SetValue(Height));
            menu = new MenuItem("OMECR", "Red").SetValue(Colour);
            menu.FontColor = new ColorBGRA(255, 50, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OMECG", "Green").SetValue(Colour);
            menu.FontColor = new ColorBGRA(50, 255, 50, 200);
            subsubMenu.AddItem(menu);
            menu = new MenuItem("OMECB", "Blue").SetValue(ColourManaDef);
            menu.FontColor = new ColorBGRA(100, 100, 255, 200);
            subsubMenu.AddItem(menu);
            subMenu.AddSubMenu(subsubMenu);
            subsubMenu = new Menu("Ultimate", "UltimateE", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayenemyultline", "Show Enemy top Ult Line?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OUHE", "Height").SetValue(Height));
            subsubMenu.AddItem(new MenuItem("showtopoverlayenemyulttext", "Show Enemy top Ult Text?").SetValue(false));
            subMenu.AddSubMenu(subsubMenu);

            subMenu.AddItem(new MenuItem("showtopoverlayenemy", "Show enemy top owerlay?").SetValue(false));
            MainMenu.Overlay.AddSubMenu(subMenu);

            subMenu = new Menu("Runes", "runes", false);
            subMenu.AddItem(new MenuItem("showrunesmimimap", "Show runes on mimimap?").SetValue(true).SetTooltip("Show rune icon on minimap."));
            subMenu.AddItem(new MenuItem("showruneschat", "Show runes in chat?").SetValue(false).SetTooltip("Show rune in chat."));
            subMenu.AddItem(new MenuItem("runescale", "Rune scale").SetValue(RuneScale));
            MainMenu.Overlay.AddSubMenu(subMenu);

            MainMenu.Overlay.AddItem(new MenuItem("OHAAlpha", "Alpha").SetValue(Alpha));
        }

        public static void Update()
        {
            MenuVar.ShowTopOverlayAllyHp = MainMenu.Overlay.Item("showtopoverlayallyhp").GetValue<bool>();
            MenuVar.ShowTopOverlayAllyMp = MainMenu.Overlay.Item("showtopoverlayallymp").GetValue<bool>();
            MenuVar.ShowTopOverlayAllyUltLine = MainMenu.Overlay.Item("showtopoverlayallyultline").GetValue<bool>();
            MenuVar.ShowTopOverlayAllyUltText = MainMenu.Overlay.Item("showtopoverlayallyulttext").GetValue<bool>();
            MenuVar.ShowTopOverlayAlly = MainMenu.Overlay.Item("showtopoverlayally").GetValue<bool>();

            MenuVar.ShowTopOverlayEnemyHp = MainMenu.Overlay.Item("showtopoverlayenemyhp").GetValue<bool>();
            MenuVar.ShowTopOverlayEnemyMp = MainMenu.Overlay.Item("showtopoverlayenemymp").GetValue<bool>();
            MenuVar.ShowTopOverlayEnemyUltLine = MainMenu.Overlay.Item("showtopoverlayenemyultline").GetValue<bool>();
            MenuVar.ShowTopOverlayEnemyUltText = MainMenu.Overlay.Item("showtopoverlayenemyulttext").GetValue<bool>();
            MenuVar.ShowTopOverlayEnemy = MainMenu.Overlay.Item("showtopoverlayenemy").GetValue<bool>();

            MenuVar.ShowRunesMinimap = MainMenu.Overlay.Item("showrunesmimimap").GetValue<bool>();
            MenuVar.ShowRunesChat = MainMenu.Overlay.Item("showruneschat").GetValue<bool>();
            MenuVar.RuneScale = MainMenu.Overlay.Item("runescale").GetValue<Slider>().Value;

            MenuVar.HealthHeightAlly = MainMenu.Overlay.Item("OHHA").GetValue<Slider>().Value;
            MenuVar.ManaHeightAlly = MainMenu.Overlay.Item("OMHA").GetValue<Slider>().Value;
            MenuVar.UltimateHeightAlly = MainMenu.Overlay.Item("OUHA").GetValue<Slider>().Value;
            MenuVar.HealthHeightEnemy = MainMenu.Overlay.Item("OHHE").GetValue<Slider>().Value;
            MenuVar.ManaHeightEnemy = MainMenu.Overlay.Item("OMHE").GetValue<Slider>().Value;
            MenuVar.UltimateHeightEnemy = MainMenu.Overlay.Item("OUHE").GetValue<Slider>().Value;

            MenuVar.OverlayHealthAllyRed = MainMenu.Overlay.Item("OHACR").GetValue<Slider>().Value;
            MenuVar.OverlayHealthAllyGreen = MainMenu.Overlay.Item("OHACG").GetValue<Slider>().Value;
            MenuVar.OverlayHealthAllyBlue = MainMenu.Overlay.Item("OHACB").GetValue<Slider>().Value;

            MenuVar.OverlayManaAllyRed = MainMenu.Overlay.Item("OMACR").GetValue<Slider>().Value;
            MenuVar.OverlayManaAllyGreen = MainMenu.Overlay.Item("OMACG").GetValue<Slider>().Value;
            MenuVar.OverlayManaAllyBlue = MainMenu.Overlay.Item("OMACB").GetValue<Slider>().Value;

            MenuVar.OverlayHealthEnemyRed = MainMenu.Overlay.Item("OHECR").GetValue<Slider>().Value;
            MenuVar.OverlayHealthEnemyGreen = MainMenu.Overlay.Item("OHECG").GetValue<Slider>().Value;
            MenuVar.OverlayHealthEnemyBlue = MainMenu.Overlay.Item("OHECB").GetValue<Slider>().Value;

            MenuVar.OverlayManaEnemyRed = MainMenu.Overlay.Item("OMECR").GetValue<Slider>().Value;
            MenuVar.OverlayManaEnemyGreen = MainMenu.Overlay.Item("OMECG").GetValue<Slider>().Value;
            MenuVar.OverlayManaEnemyBlue = MainMenu.Overlay.Item("OMECB").GetValue<Slider>().Value;

            MenuVar.OverlayAlpha = MainMenu.Overlay.Item("OHAAlpha").GetValue<Slider>().Value;
        }

        #endregion Methods
    }
}