using AllinOne.Variables;

namespace AllinOne.Menu
{
    using Ensage.Common.Menu;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class OverlayMenu
    {
        private static Slider Height = new Slider(4, 3, 25);

        public static void Load()
        {
            var subMenu = new Menu("Ally top overlay", "allytopoverlay", false);
            var subsubMenu = new Menu("Health", "HealthA", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayallyhp", "Show Ally top Hp?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OHHA", "Height").SetValue(Height));
            subMenu.AddSubMenu(subsubMenu);
            subsubMenu = new Menu("Mana", "ManaA", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayallymp", "Show Ally top Mana?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OMHA", "Height").SetValue(Height));
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
            subMenu.AddSubMenu(subsubMenu);
            subsubMenu = new Menu("Mana", "ManaE", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayenemymp", "Show Enemy top Mana?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OMHE", "Height").SetValue(Height));
            subMenu.AddSubMenu(subsubMenu);
            subsubMenu = new Menu("Ultimate", "UltimateE", false);
            subsubMenu.AddItem(new MenuItem("showtopoverlayenemyultline", "Show Enemy top Ult Line?").SetValue(false));
            subsubMenu.AddItem(new MenuItem("OUHE", "Height").SetValue(Height));
            subsubMenu.AddItem(new MenuItem("showtopoverlayenemyulttext", "Show Enemy top Ult Text?").SetValue(false));
            subMenu.AddSubMenu(subsubMenu);

            subMenu.AddItem(new MenuItem("showtopoverlayenemy", "Show Enemy top Owerlay?").SetValue(false));
            MainMenu.Overlay.AddSubMenu(subMenu);

            subMenu = new Menu("Runes", "runes", false);
            subMenu.AddItem(new MenuItem("showrunesmimimap", "Show Runes on mimimap?").SetValue(true).SetTooltip("Show Rune icon on minimap."));
            subMenu.AddItem(new MenuItem("showruneschat", "Show Runes in chat?").SetValue(false).SetTooltip("Show Rune in chat."));
            MainMenu.Overlay.AddSubMenu(subMenu);

            subMenu = new Menu("Tower Range", "towerrange", false);
            subMenu.AddItem(new MenuItem("owntowers", "My Towers").SetValue(false).SetTooltip("Show your tower range."));
            subMenu.AddItem(new MenuItem("enemytowers", "Enemies Towers").SetValue(false).SetTooltip("Show the enemies towers range."));
            MainMenu.Overlay.AddSubMenu(subMenu);
        }

        public static void Update()
        {
            MenuVar.OwnTowers = MainMenu.Overlay.Item("owntowers").GetValue<bool>();
            MenuVar.EnemiesTowers = MainMenu.Overlay.Item("enemytowers").GetValue<bool>();

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

            MenuVar.HealthHeightAlly = MainMenu.Overlay.Item("OHHA").GetValue<Slider>().Value;
            MenuVar.ManaHeightAlly = MainMenu.Overlay.Item("OMHA").GetValue<Slider>().Value;
            MenuVar.UltimateHeightAlly = MainMenu.Overlay.Item("OUHA").GetValue<Slider>().Value;
            MenuVar.HealthHeightEnemy = MainMenu.Overlay.Item("OHHE").GetValue<Slider>().Value;
            MenuVar.ManaHeightEnemy = MainMenu.Overlay.Item("OMHE").GetValue<Slider>().Value;
            MenuVar.UltimateHeightEnemy = MainMenu.Overlay.Item("OUHE").GetValue<Slider>().Value;
        }
    }
}