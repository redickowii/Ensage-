namespace AllinOne.ObjectManager
{
    using System.Linq;
    using System.Collections.Generic;
    using Ensage;

    internal class Buildings
    {
        public static List<Entity> Towers;

        public static void GetBuildings()
        {
            Towers = ObjectManager.GetEntities<Entity>().Where(x => x.IsAlive && (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                x.ClassID == ClassID.CDOTA_Unit_Fountain))
            .ToList();
        }
    }
}
