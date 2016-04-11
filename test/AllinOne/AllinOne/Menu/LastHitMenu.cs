namespace AllinOne.Menu
{
    using AllinOne.Variables;
    using Ensage.Common.Menu;

    internal class LastHitMenu
    {
        private static readonly StringList Attack = new StringList(new[] { "Never", "Standart", "Always" }, 1);
        private static readonly Slider BonusWindUpSlider = new Slider(500, 100, 2000);
        private static readonly Slider BonusRange = new Slider(100, 100, 500);

        public static void Load()
        {
            MainMenu.MenuLastHit.AddItem(new MenuItem("enableLasthit", "Enable").SetValue(false));
            MainMenu.MenuLastHit.AddItem(new MenuItem("autoAttackMode", "Auto Attack").SetValue(Attack));
            MainMenu.MenuLastHit.AddItem(new MenuItem("bonuswindup", "Bonus WindUp time on kitting").SetValue(BonusWindUpSlider));
            MainMenu.MenuLastHit.AddItem(new MenuItem("hpleftcreep", "Mark hp ?").SetValue(true));
            MainMenu.MenuLastHit.AddItem(new MenuItem("sapp", "Support").SetValue(false));
            MainMenu.MenuLastHit.AddItem(new MenuItem("usespell", "Use spell ?").SetValue(true));
            MainMenu.MenuLastHit.AddItem(new MenuItem("harassheroes", "Harass in lasthit mode ?").SetValue(true));
            MainMenu.MenuLastHit.AddItem(new MenuItem("denied", "Deny creep ?").SetValue(true));
            MainMenu.MenuLastHit.AddItem(new MenuItem("AOC", "Atteck own creeps ?").SetValue(false));
            MainMenu.MenuLastHit.AddItem(new MenuItem("test", "Test Attack_Calc").SetValue(false));
            MainMenu.MenuLastHit.AddItem(new MenuItem("outrange", "Bonus range").SetValue(BonusRange));

            var subMenu = new Menu("Summons", "Summons", false);
            subMenu.AddItem(new MenuItem("enable", "Enable").SetValue(false).SetTooltip("Test!"));
            subMenu.AddItem(new MenuItem("harassheroes_sub", "Harass in lasthit mode ?").SetValue(false));
            subMenu.AddItem(new MenuItem("denied_sub", "Deny creep ?").SetValue(false));
            subMenu.AddItem(new MenuItem("AOC_sub", "Atteck own creeps ?").SetValue(false));
            subMenu.AddItem(new MenuItem("autoD", "Auto lasthit").SetValue(false).SetTooltip("Dont work properly!!!"));
            subMenu.AddItem(new MenuItem("autoF", "Auto farm").SetValue(false).SetTooltip("Dont work properly!!!"));
            MainMenu.MenuLastHit.AddSubMenu(subMenu);

            subMenu = new Menu("Keys", " Keys", false);
            subMenu.AddItem(new MenuItem("combatkey", "Chase mode").SetValue(new KeyBind(32, KeyBindType.Press)));
            subMenu.AddItem(new MenuItem("lasthit", "Lasthit mode").SetValue(new KeyBind('C', KeyBindType.Press)));
            subMenu.AddItem(new MenuItem("farmKey", "Farm mode").SetValue(new KeyBind('V', KeyBindType.Press)));
            subMenu.AddItem(new MenuItem("kitekey", "Kite mode").SetValue(new KeyBind('H', KeyBindType.Press)));
            MainMenu.MenuLastHit.AddSubMenu(subMenu);
        }

        public static void Update()
        {
            MenuVar.LastHitEnable = MainMenu.MenuLastHit.Item("enableLasthit").GetValue<bool>();
            if (MenuVar.LastHitEnable)
            {
                MenuVar.ShowHp = MainMenu.MenuLastHit.Item("hpleftcreep").GetValue<bool>();
                MenuVar.Sapport = MainMenu.MenuLastHit.Item("sapp").GetValue<bool>();
                MenuVar.UseSpell = MainMenu.MenuLastHit.Item("usespell").GetValue<bool>();
                MenuVar.Harass = MainMenu.MenuLastHit.Item("harassheroes").GetValue<bool>();
                MenuVar.Denie = MainMenu.MenuLastHit.Item("denied").GetValue<bool>();
                MenuVar.Aoc = MainMenu.MenuLastHit.Item("AOC").GetValue<bool>();
                MenuVar.Test = MainMenu.MenuLastHit.Item("test").GetValue<bool>();
                MenuVar.SummonsEnable = MainMenu.MenuLastHit.Item("enable").GetValue<bool>();
                if (MenuVar.SummonsEnable)
                {
                    MenuVar.SummonsHarass = MainMenu.MenuLastHit.Item("harassheroes_sub").GetValue<bool>();
                    MenuVar.SummonsDenie = MainMenu.MenuLastHit.Item("denied_sub").GetValue<bool>();
                    MenuVar.SummonsAoc = MainMenu.MenuLastHit.Item("AOC_sub").GetValue<bool>();
                    MenuVar.SummonsAutoLasthit = MainMenu.MenuLastHit.Item("autoD").GetValue<bool>();
                    MenuVar.SummonsAutoFarm = MainMenu.MenuLastHit.Item("autoF").GetValue<bool>();
                }
                MenuVar.LastHitKey = MainMenu.MenuLastHit.Item("lasthit").GetValue<KeyBind>().Key;
                MenuVar.FarmKey = MainMenu.MenuLastHit.Item("farmKey").GetValue<KeyBind>().Key;
                MenuVar.CombatKey = MainMenu.MenuLastHit.Item("combatkey").GetValue<KeyBind>().Key;
                MenuVar.KiteKey = MainMenu.MenuLastHit.Item("kitekey").GetValue<KeyBind>().Key;

                MenuVar.AutoAttackMode =
                    MainMenu.MenuLastHit.Item("autoAttackMode").GetValue<StringList>().SelectedIndex;
                MenuVar.Outrange = MainMenu.MenuLastHit.Item("outrange").GetValue<Slider>().Value;
                MenuVar.BonusWindUp = MainMenu.MenuLastHit.Item("bonuswindup").GetValue<Slider>().Value;
            }
        }
    }
}