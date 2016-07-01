namespace AllinOne.Menu
{
    using Ensage.Common.Menu;

    internal class CourierMenu
    {
        public static void Load()
        {
            var subMenu = new Menu("AvoidEnemy", "subMenuAvoidEnemy");
            subMenu.AddItem(new MenuItem("avoidEnemy.avoidEnemy", "Enable Avoid Enemy").SetValue(true));
            subMenu.AddItem(new MenuItem("avoidEnemy.range", "Range").SetValue(new Slider(700, 100, 1000)));
            MainMenu.Courier.AddSubMenu(subMenu);

            MainMenu.Courier.AddItem(new MenuItem("burst", "Auto burst").SetValue(true));
            MainMenu.Courier.AddItem(new MenuItem("abuse", "Bottle Abuse").SetValue(new KeyBind('U', KeyBindType.Toggle, false)));
            MainMenu.Courier.AddItem(new MenuItem("forced", "Anti Reuse deliver").SetValue(new KeyBind('Y', KeyBindType.Toggle, false)));
            MainMenu.Courier.AddItem(new MenuItem("lock", "Lock at fountain").SetValue(new KeyBind('I', KeyBindType.Toggle, false)));
            MainMenu.Courier.AddItem(new MenuItem("cd", "Rate").SetValue(new Slider(150, 50, 500)));

        }

        public static void Update()
        {
            MenuVar.CouAvoidEnemy = MainMenu.Courier.Item("avoidEnemy.avoidEnemy").GetValue<bool>();
            MenuVar.CouAvoidEnemyRange = MainMenu.Courier.Item("avoidEnemy.range").GetValue<Slider>().Value;
            MenuVar.CouBurst = MainMenu.Courier.Item("burst").GetValue<bool>();
            MenuVar.CouAbuse = MainMenu.Courier.Item("abuse").GetValue<KeyBind>().Active;
            MenuVar.CouAbuseKey = MainMenu.Courier.Item("abuse").GetValue<KeyBind>().Key;
            MenuVar.CouForced = MainMenu.Courier.Item("forced").GetValue<KeyBind>().Active;
            MenuVar.CouLock = MainMenu.Courier.Item("lock").GetValue<KeyBind>().Active;
            MenuVar.CouCd = MainMenu.Courier.Item("cd").GetValue<Slider>().Value;
        }
    }
}
