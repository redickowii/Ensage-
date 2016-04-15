namespace AllinOne.ObjectManager
{
    using AllinOne.Methods;
    using AllinOne.ObjectManager.Heroes;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using System.Collections.Generic;
    using System.Linq;

    internal class MyHeroInfo
    {
        #region Fields

        private static float _attackRange;

        #endregion Fields

        #region Methods

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

        public static List<Ability> GetAbilities()
        {
            return AllyHeroes.AbilityDictionary[Var.Me.Handle].Where(x => x.CanBeCasted()).ToList();
        }

        public static List<Item> GetItems()
        {
            return AllyHeroes.ItemDictionary[Var.Me.Handle].Where(x => x.CanBeCasted()).ToList();
        }

        public static float GetProjectileSpeed(Entity unit)
        {
            return Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
                ? 800
                : UnitDatabase.GetByName(unit.Name).ProjectileSpeed;
        }

        #endregion Methods
    }
}