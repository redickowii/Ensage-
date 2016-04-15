namespace AllinOne.ObjectManager
{
    using Ensage;
    using System.Collections.Generic;
    using System.Linq;

    internal class Buildings
    {
        #region Fields

        public static List<Entity> Towers;

        #endregion Fields

        #region Methods

        public static void GetBuildings()
        {
            Towers = ObjectManager.GetEntities<Entity>().Where(x => x.IsAlive && (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                x.ClassID == ClassID.CDOTA_Unit_Fountain))
            .ToList();

            foreach (var tower in Towers)
            {
                Methods.Towers.TowerRange.Add(tower, new List<ParticleEffect>());
            }
        }

        #endregion Methods
    }
}