namespace AllinOne.Menu
{
    using AllinOne.CameraDistance;
    using AllinOne.Variables;
    using Ensage.Common.Menu;

    using SharpDX;

    internal class MenuVar
    {
        #region Lasthit

        public static bool LastHitEnable;
        public static int AutoAttackMode;
        public static bool ShowHp;
        public static bool Sapport;
        public static bool UseSpell;
        public static bool Harass;
        public static bool Denie;
        public static bool Aoc;
        public static int Outrange;
        public static int BonusWindUp;

        public static bool Test;

        public static bool SummonsEnable;
        public static bool SummonsHarass;
        public static bool SummonsDenie;
        public static bool SummonsAoc;
        public static bool SummonsAutoLasthit;
        public static bool SummonsAutoFarm;

        public static uint LastHitKey;
        public static uint FarmKey;
        public static uint CombatKey;
        public static uint KiteKey;

        #endregion Lasthit

        #region Jungle

        public static bool StackKey;
        public static bool MoreFps;
        public static bool DrawStackLine;

        #endregion Jungle

        #region Overlay

        public static bool ShowTopOverlayAllyHp;
        public static bool ShowTopOverlayAllyMp;
        public static bool ShowTopOverlayAllyUltLine;
        public static bool ShowTopOverlayAllyUltText;
        public static bool ShowTopOverlayAlly;

        public static bool ShowTopOverlayEnemyHp;
        public static bool ShowTopOverlayEnemyMp;
        public static bool ShowTopOverlayEnemyUltLine;
        public static bool ShowTopOverlayEnemyUltText;
        public static bool ShowTopOverlayEnemy;

        public static bool ShowRunesMinimap;
        public static bool ShowRunesChat;

        public static bool OwnTowers;
        public static bool EnemiesTowers;

        public static int HealthHeightAlly;
        public static int ManaHeightAlly;
        public static int UltimateHeightAlly;
        public static int HealthHeightEnemy;
        public static int ManaHeightEnemy;
        public static int UltimateHeightEnemy;

        #endregion Overlay

        #region ShowMeMore

        public static bool ShowIllusions;
        public static int IllusionsEffectMenu;
        public static int HeroEffectMenu;
        public static MenuItem TestEffectMenu;
        public static bool Maphack;
        public static bool ShowLastPos;
        public static bool ShowLastPosMini;
        public static bool ShowRoshanTimer;
        public static bool VisiblebyEnemy;

        #endregion ShowMeMore

        #region Settings

        public static MenuItem ShowExpRange;
        public static MenuItem ShowAttackRange;
        public static MenuItem CameraDistance;
        public static bool DodgeEnable;
        public static int DodgeFrequency;

        #endregion Settings

        #region Dev

        public static bool OnOff;
        public static bool ShowErrors;
        public static bool ShowInfo;

        #endregion Dev
    }

    internal class MainMenu
    {
        public static Menu Menu;
        public static Menu MenuLastHit;
        public static Menu MenuStack;
        public static Menu Overlay;
        public static Menu MenuSettings;
        public static Menu ShowMeMore;
        public static Menu Dev;

        public static void Load()
        {
            Menu = new Menu("AllinOne", "AllinOne", true);
            MenuLastHit = new Menu(" LastHit", "lasthit", false, "item_quelling_blade", true);
            MenuStack = new Menu(" Jungle Stack ", "stack", false, "item_helm_of_the_dominator", true);
            Overlay = new Menu(" Overlay ", "overlay", false, "item_ward_observer", true);
            ShowMeMore = new Menu(" ShowMeMore ", "showmemore", false, "item_gem", true);
            MenuSettings = new Menu(" Settings ", "settings", false, "item_refresher", true);
            Dev = new Menu(" Dev Menu ", "dev", false, "doom_bringer_doom", true);

            OverlayMenu.Load();

            ShowMenu.Load();

            JungleStackMenu.Load();

            LastHitMenu.Load();

            SettingsMenu.Load();

            DevMenu.Load();

            Menu.AddSubMenu(Overlay);
            Menu.AddSubMenu(ShowMeMore);
            Menu.AddSubMenu(MenuLastHit);
            Menu.AddSubMenu(MenuStack);
            Menu.AddSubMenu(MenuSettings);
            Menu.AddSubMenu(Dev);
            Menu.Color = Color.Green;
            Menu.AddToMainMenu();
        }

        public static void Update()
        {
            OverlayMenu.Update();
            ShowMenu.Update();
            JungleStackMenu.Update();
            LastHitMenu.Update();
            SettingsMenu.Update();
            DevMenu.Update();
        }

        public static void UnLoad()
        {
            if (Menu == null)
            {
                return;
            }

            Menu.RemoveFromMainMenu();
            Menu = null;
        }
    }
}