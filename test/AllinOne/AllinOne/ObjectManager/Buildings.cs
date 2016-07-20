using AllinOne.Variables;

namespace AllinOne.ObjectManager
{
    using Ensage;
    using System.Collections.Generic;
    using System.Linq;

    internal class Buildings
    {
        #region Fields

        public static List<Unit> Towers;

        public static Unit AllyFountain;

        public static Unit EnemyFountain;

        #endregion Fields

        #region Methods

        public static void GetBuildings()
        {
            Towers = ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive && (x.ClassID == ClassID.CDOTA_BaseNPC_Tower)).ToList();

            if (ObjectManager.GetEntities<Unit>().Any(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team == Var.Me.Team))
                AllyFountain = ObjectManager.GetEntities<Unit>().First(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team == Var.Me.Team);

            if (ObjectManager.GetEntities<Unit>().Any(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team != Var.Me.Team))
                EnemyFountain = ObjectManager.GetEntities<Unit>().First(x => x.ClassID == ClassID.CDOTA_Unit_Fountain && x.Team != Var.Me.Team);

            //foreach (var tower in Towers)
            //{
            //    Methods.Towers.TowerRange.Add(tower, new List<ParticleEffect>());
            //}
        }

        #endregion Methods
    }
}