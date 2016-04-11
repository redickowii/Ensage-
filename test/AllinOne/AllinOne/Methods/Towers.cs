namespace AllinOne.Methods
{
    using System.Collections.Generic;
    using System.Linq;

    using AllinOne.Variables;

    using Ensage;

    using SharpDX;

    internal class Towers
    {

        public static void TowerDestroyed()
        {
            foreach (var x in Var.OwnTowerRange.Values.ToList())
            {
                x.Dispose();
            }
            foreach (var x in Var.EnemyTowerRange.Values.ToList())
            {
                x.Dispose();
            }
            Var.OwnTowerRange.Clear();
            Var.EnemyTowerRange.Clear();
            Var.TowerLoad = false;
        }

        public static void UpdateDicTowers(List<Entity> buildings)
        {
            foreach (var building in buildings)
            {
                if (Var.OwnTowerRange.Any(x => x.Key == "" + building.Handle + "1") ||
                    Var.OwnTowerRange.Any(x => x.Key == "" + building.Handle + "2") ||
                    Var.EnemyTowerRange.Any(x => x.Key == "" + building.Handle + "1") ||
                    Var.EnemyTowerRange.Any(x => x.Key == "" + building.Handle + "2")
                    )
                    continue;
                ParticleEffect particleEffect;
                switch (building.ClassID)
                {
                    case ClassID.CDOTA_BaseNPC_Tower:
                        particleEffect = new ParticleEffect(
                            @"particles\ui_mouseactions\drag_selected_ring.vpcf", building);
                        particleEffect.SetControlPoint(1, new Vector3(30, 144, 255));
                        particleEffect.SetControlPoint(2, new Vector3(900 + Var.Me.HullRadius + 50, 255, 0));
                        particleEffect.SetControlPoint(3, new Vector3(10, 0, 0));
                        if (building.Team == Var.Me.Team)
                        {
                            Var.OwnTowerRange.Add("" + building.Handle + "1", particleEffect);
                        }
                        else
                        {
                            Var.EnemyTowerRange.Add("" + building.Handle + "1", particleEffect);
                        }

                        particleEffect = new ParticleEffect(
                            @"particles\ui_mouseactions\drag_selected_ring.vpcf", building);
                        particleEffect.SetControlPoint(1, new Vector3(178, 34, 34));
                        particleEffect.SetControlPoint(2, new Vector3(850 + Var.Me.HullRadius + 50, 255, 0));
                        particleEffect.SetControlPoint(3, new Vector3(10, 0, 0));
                        if (building.Team == Var.Me.Team)
                        {
                            Var.OwnTowerRange.Add("" + building.Handle + "2", particleEffect);
                        }
                        else
                        {
                            Var.EnemyTowerRange.Add("" + building.Handle + "2", particleEffect);
                        }
                        break;
                    case ClassID.CDOTA_Unit_Fountain:
                        particleEffect = new ParticleEffect(
                            @"particles\ui_mouseactions\drag_selected_ring.vpcf", building);
                        particleEffect.SetControlPoint(1, new Vector3(30, 144, 255));
                        particleEffect.SetControlPoint(2, new Vector3(1200 + Var.Me.HullRadius + 50, 255, 0));
                        particleEffect.SetControlPoint(3, new Vector3(10, 0, 0));
                        if (building.Team == Var.Me.Team)
                        {
                            Var.OwnTowerRange.Add("" + building.Handle + "1", particleEffect);
                        }
                        else
                        {
                            Var.EnemyTowerRange.Add("" + building.Handle + "1", particleEffect);
                        }

                        particleEffect = new ParticleEffect(
                            @"particles\ui_mouseactions\drag_selected_ring.vpcf", building);
                        particleEffect.SetControlPoint(1, new Vector3(178, 34, 34));
                        particleEffect.SetControlPoint(2, new Vector3(1350 + Var.Me.HullRadius + 50, 255, 0));
                        particleEffect.SetControlPoint(3, new Vector3(10, 0, 0));
                        if (building.Team == Var.Me.Team)
                        {
                            Var.OwnTowerRange.Add("" + building.Handle + "2", particleEffect);
                        }
                        else
                        {
                            Var.EnemyTowerRange.Add("" + building.Handle + "2", particleEffect);
                        }
                        break;
                }
            }
        }

    }
}
