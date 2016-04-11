namespace AllinOne.ObjectManager
{
    using AllinOne.Methods;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    class MyHeroInfo
    {
        private static float _attackRange;

        public static float AttackRange()
        {
            if (!Common.SleepCheck("MyHeroInfo.AttackRange"))
            {
                return _attackRange;
            }
            Common.Sleep(1000, "MyHeroInfo.AttackRange");

            if (Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord)
                _attackRange = Var.Q.IsToggled ? 128 : Var.Me.GetAttackRange();
            else
                _attackRange = Var.Me.GetAttackRange();
            
            return _attackRange;
        }

        public static float GetProjectileSpeed(Entity unit)
        {
            return Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
                ? 800
                : UnitDatabase.GetByName(unit.Name).ProjectileSpeed;
        }
    }
}
