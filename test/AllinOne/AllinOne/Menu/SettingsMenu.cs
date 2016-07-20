namespace AllinOne.Menu
{
    using AllinOne.CameraDistance;
    using AllinOne.Variables;
    using Ensage.Common.Menu;

    internal class SettingsMenu
    {
        #region Fields

        private static readonly Slider CameraDistance = new Slider(1500, 1134, 2000);
        private static readonly Slider Frequency = new Slider(50, 10, 1000);

        #endregion Fields

        #region Methods

        public static void Load()
        {
            var subMenu = new Menu("Auto use items", "autouseitems", false);
            var percent = new Menu("% Menu", "percentMenu", false);
            var itemConfig = new Menu("Items", "Items", false);
            subMenu.AddSubMenu(percent);
            subMenu.AddSubMenu(itemConfig);
            subMenu.AddItem(new MenuItem("midasAll", "Midas All").SetValue(true).SetTooltip("false = only creeps 5 lvl and > 950 HP."));
            itemConfig.AddItem(new MenuItem("item_config", "itemuse").SetValue(new AbilityToggler(Allitems.list_of_items)));
              
            percent.AddItem(new MenuItem("stickPs", "Stick % HP").SetValue(new Slider(10, 1, 100)));
            percent.AddItem(new MenuItem("cheesePs", "Cheese % HP").SetValue(new Slider(10, 1, 100)));
            percent.AddItem(new MenuItem("arcaneBootsPs", "Arcane Boots % MP").SetValue(new Slider(30, 1, 100)));
            MainMenu.MenuSettings.AddSubMenu(subMenu);


            MainMenu.MenuSettings.AddItem(new MenuItem("dodge", "Dodge ?").SetValue(true));
            MainMenu.MenuSettings.AddItem(new MenuItem("dodgefrequency", "Dodge Frequency").SetValue(Frequency));
            MainMenu.MenuSettings.AddItem(new MenuItem("showatkrange", "Show attack range ?").SetValue(false));
            MainMenu.MenuSettings.AddItem(
                new MenuItem("expRange", "Exp Range").SetValue(false).SetTooltip("Show your experience range."));

            MainMenu.MenuSettings.AddItem(new MenuItem("cameradistance", "Camera Distance").SetValue(CameraDistance))
                .ValueChanged += (sender, arg) => { Zoom.Slider_ValueChanged(arg); };
        }

        public static void Update()
        {
            MenuVar.DodgeEnable = MainMenu.MenuSettings.Item("dodge").GetValue<bool>();
            MenuVar.DodgeFrequency = MainMenu.MenuSettings.Item("dodgefrequency").GetValue<Slider>().Value;

            MenuVar.PercentStickUse = ((double)MainMenu.MenuSettings.Item("stickPs").GetValue<Slider>().Value / 100);
            MenuVar.PercentCheeseUse = ((double)MainMenu.MenuSettings.Item("cheesePs").GetValue<Slider>().Value / 100);
            MenuVar.PercentArcaneUse = ((double)MainMenu.MenuSettings.Item("arcaneBootsPs").GetValue<Slider>().Value / 100);
            MenuVar.MidasAllUse = MainMenu.MenuSettings.Item("midasAll").GetValue<bool>();

            MenuVar.ItemBottleUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_bottle");
            MenuVar.ItemPhaseBootsUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_phase_boots");
            MenuVar.ItemArcaneBootsUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_arcane_boots");
            MenuVar.ItemSphereUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_sphere");
            MenuVar.ItemCheeseUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_cheese");
            MenuVar.ItemMagicStickUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_magic_stick");
            MenuVar.ItemMagicWandUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_magic_wand");
            MenuVar.ItemDustUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_dust");
            MenuVar.ItemHandOfMidasUse = MainMenu.MenuSettings.Item("item_config").GetValue<AbilityToggler>().IsEnabled("item_hand_of_midas");
        }

        #endregion Methods
    }
}