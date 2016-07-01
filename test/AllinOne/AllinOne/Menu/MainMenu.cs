namespace AllinOne.Menu
{
    using Ensage.Common.Menu;

    using SharpDX;

    internal class MainMenu
    {
        #region Fields

        public static Menu Dev;
        public static Menu Menu;
        public static Menu MenuLastHit;
        public static Menu MenuSettings;
        public static Menu MenuStack;
        public static Menu Overlay;
        public static Menu ShowMeMore;
        public static Menu Courier;

        #endregion Fields

        #region Methods

        public static void Load()
        {
            Menu = new Menu("AllinOne", "AllinOne", true);
            MenuLastHit = new Menu(" LastHit", "lasthit", false, "item_quelling_blade", true);
            MenuStack = new Menu(" Jungle Stack ", "stack", false, "item_helm_of_the_dominator", true);
            Courier = new Menu(" Courier ", "courier", false, "item_courier", true);
            Overlay = new Menu(" Overlay ", "overlay", false, "item_ward_observer", true);
            ShowMeMore = new Menu(" ShowMeMore ", "showmemore", false, "item_gem", true);
            MenuSettings = new Menu(" Settings ", "settings", false, "item_refresher", true);
            Dev = new Menu(" Dev Menu ", "dev", false, "doom_bringer_doom", true);

            OverlayMenu.Load();
            ShowMenu.Load();
            CourierMenu.Load();
            JungleStackMenu.Load();
            LastHitMenu.Load();
            SettingsMenu.Load();
            DevMenu.Load();

            Menu.AddSubMenu(Overlay);
            Menu.AddSubMenu(ShowMeMore);
            Menu.AddSubMenu(Courier);
            Menu.AddSubMenu(MenuLastHit);
            Menu.AddSubMenu(MenuStack);
            Menu.AddSubMenu(MenuSettings);
            Menu.AddSubMenu(Dev);
            Menu.Color = Color.Green;
            Menu.AddToMainMenu();
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

        public static void Update()
        {
            OverlayMenu.Update();
            ShowMenu.Update();
            CourierMenu.Update();
            JungleStackMenu.Update();
            LastHitMenu.Update();
            SettingsMenu.Update();
            DevMenu.Update();
        }

        #endregion Methods
    }

    internal class MenuVar
    {
        #region Lasthit

        public static bool Aoc;
        public static int AutoAttackMode;
        public static int BonusWindUp;
        public static uint CombatKey;
        public static bool Denie;
        public static uint FarmKey;
        public static bool Harass;
        public static uint KiteKey;
        public static bool LastHitEnable;
        public static uint LastHitKey;
        public static int Outrange;
        public static bool Sapport;
        public static bool ShowHp;
        public static bool SummonsAoc;
        public static bool SummonsAutoFarm;
        public static bool SummonsAutoLasthit;
        public static bool SummonsDenie;
        public static bool SummonsEnable;
        public static bool SummonsHarass;
        public static bool Test;
        public static bool UseSpell;

        #endregion Lasthit

        #region Jungle

        public static bool DrawStackLine;
        public static bool MoreFps;
        public static bool StackKey;

        #endregion Jungle

        #region Overlay

        public static int HealthHeightAlly;
        public static int HealthHeightEnemy;
        public static int ManaHeightAlly;
        public static int ManaHeightEnemy;
        public static int OverlayAlpha;
        public static int OverlayHealthAllyBlue;
        public static int OverlayHealthAllyGreen;
        public static int OverlayHealthAllyRed;
        public static int OverlayHealthEnemyBlue;
        public static int OverlayHealthEnemyGreen;
        public static int OverlayHealthEnemyRed;
        public static int OverlayManaAllyBlue;
        public static int OverlayManaAllyGreen;
        public static int OverlayManaAllyRed;
        public static int OverlayManaEnemyBlue;
        public static int OverlayManaEnemyGreen;
        public static int OverlayManaEnemyRed;
        public static int RuneScale;
        public static bool ShowRunesChat;
        public static bool ShowRunesMinimap;
        public static bool ShowTopOverlayAlly;
        public static bool ShowTopOverlayAllyHp;
        public static bool ShowTopOverlayAllyMp;
        public static bool ShowTopOverlayAllyUltLine;
        public static bool ShowTopOverlayAllyUltText;
        public static bool ShowTopOverlayEnemy;
        public static bool ShowTopOverlayEnemyHp;
        public static bool ShowTopOverlayEnemyMp;
        public static bool ShowTopOverlayEnemyUltLine;
        public static bool ShowTopOverlayEnemyUltText;
        public static int UltimateHeightAlly;
        public static int UltimateHeightEnemy;

        #endregion Overlay

        #region ShowMeMore

        public static bool EnemiesTowers;
        public static int HeroEffectMenu;
        public static int IllusionsEffectMenu;
        public static bool Maphack;
        public static int MiniScale;
        public static bool OwnTowers;
        public static bool ShowIllusions;
        public static bool ShowLastPos;
        public static bool ShowLastPosMini;
        public static bool ShowRoshanTimer;
        public static MenuItem TestEffectMenu;
        public static bool TrueSight;
        public static bool VisiblebyEnemy;

        #endregion ShowMeMore

        #region Settings

        public static MenuItem CameraDistance;
        public static bool DodgeEnable;
        public static int DodgeFrequency;
        public static MenuItem ShowAttackRange;
        public static MenuItem ShowExpRange;

        #endregion Settings

        #region Dev

        public static bool OnOff;
        public static bool ShowErrors;
        public static bool ShowInfo;

        #endregion Dev

        #region Courier

        public static bool CouAvoidEnemy;
        public static int CouAvoidEnemyRange;
        public static bool CouBurst;
        public static bool CouAbuse;
        public static uint CouAbuseKey;
        public static bool CouForced;
        public static bool CouLock;
        public static int CouCd;

        #endregion Courier
    }
}