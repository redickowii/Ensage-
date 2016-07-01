namespace AllinOne.ObjectManager
{
    using System.Linq;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.Common;
    using AllinOne.Variables;

    internal class Couriers
    {
        #region Methods

        public static List<Courier> AllyCouriers;
        public static List<Courier> EnemyCouriers;

        public static void Update()
        {
            if (Utils.SleepCheck("getCouriers"))
            {
                UpdateCouriers();
                Utils.Sleep(1000, "getCouriers");
            }
        }

        public static void UpdateCouriers()
        {
            AllyCouriers = ObjectManager.GetEntities<Courier>().Where(x => x.IsAlive && x.Team == Var.Me.Team).ToList();
            EnemyCouriers = ObjectManager.GetEntities<Courier>().Where(x => x.IsAlive && x.Team != Var.Me.Team).ToList();
        }

        #endregion Methods
    }
}
