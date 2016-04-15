namespace AllinOne.Methods
{
    using AllinOne.Menu;
    using AllinOne.ObjectManager;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Towers
    {
        #region Fields

        public static bool TowerLoad;
        public static Dictionary<Entity, List<ParticleEffect>> TowerRange;

        #endregion Fields

        #region Methods

        public static void Load()
        {
            if (MenuVar.EnemiesTowers)
            {
                Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team != Var.Me.Team).ToList(), 0,
                    1350 + Var.Me.HullRadius + 50, ClassID.CDOTA_Unit_Fountain, new Vector3(178, 34, 34));
                Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team != Var.Me.Team).ToList(), 0,
                    850 + Var.Me.HullRadius + 50, ClassID.CDOTA_BaseNPC_Tower, new Vector3(178, 34, 34));
                if (MenuVar.TrueSight)
                {
                    Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team != Var.Me.Team).ToList(), 1,
                        1200 + Var.Me.HullRadius + 50, ClassID.CDOTA_Unit_Fountain, new Vector3(30, 144, 255));
                    Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team != Var.Me.Team).ToList(), 1,
                        900 + Var.Me.HullRadius + 50, ClassID.CDOTA_BaseNPC_Tower, new Vector3(30, 144, 255));
                }
                else
                {
                    Towers.TowerDisposeTruesight(Var.Me.GetEnemyTeam());
                }
            }
            else
            {
                Towers.TowerDisposeEffects(Var.Me.GetEnemyTeam());
            }
            if (MenuVar.OwnTowers)
            {
                Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team == Var.Me.Team).ToList(), 0,
                    1350 + Var.Me.HullRadius + 50, ClassID.CDOTA_Unit_Fountain, new Vector3(178, 34, 34));

                Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team == Var.Me.Team).ToList(), 0,
                    850 + Var.Me.HullRadius + 50, ClassID.CDOTA_BaseNPC_Tower, new Vector3(178, 34, 34));
                if (MenuVar.TrueSight)
                {
                    Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team == Var.Me.Team).ToList(), 1,
                        1200 + Var.Me.HullRadius + 50, ClassID.CDOTA_Unit_Fountain, new Vector3(30, 144, 255));

                    Towers.DrawTowerRange(Buildings.Towers.Where(x => x.Team == Var.Me.Team).ToList(), 1,
                        900 + Var.Me.HullRadius + 50, ClassID.CDOTA_BaseNPC_Tower, new Vector3(30, 144, 255));
                }
                else
                {
                    Towers.TowerDisposeTruesight(Var.Me.Team);
                }
            }
            else
            {
                Towers.TowerDisposeEffects(Var.Me.Team);
            }
        }

        public static void TowerDestroyed()
        {
            var temp = TowerRange.Where(x => !x.Key.IsAlive).ToList();
            foreach (var x in temp)
            {
                x.Value.ForEach(y => y.Dispose());
                TowerRange.Remove(x.Key);
            }
        }

        private static void DrawTowerRange(List<Entity> buildings, int index, float range, ClassID classId, Vector3 colour)
        {
            foreach (var building in buildings.Where(x => x.ClassID == classId))
            {
                if ((TowerRange[building].Count >= 2 || !MenuVar.TrueSight) &&
                    (TowerRange[building].Count >= 1 || MenuVar.TrueSight))
                    continue;
                if (TowerRange[building].All(x => Math.Abs(x.GetControlPoint(2).X - range) > 0)
                    || TowerRange[building].Count == 0)
                {
                    ParticleEffect particleEffect = building.AddParticleEffect(
                        @"particles\ui_mouseactions\drag_selected_ring.vpcf");
                    particleEffect.SetControlPoint(1, colour);
                    particleEffect.SetControlPoint(2, new Vector3(range, 255, 0));
                    particleEffect.SetControlPoint(3, new Vector3(10, 0, 0));
                    TowerRange[building].Add(particleEffect);
                }
            }
        }

        private static void TowerDisposeEffects(Team team)
        {
            foreach (var x in TowerRange.Where(x => x.Key.Team == team).ToList())
            {
                x.Value.ForEach(y => y.Dispose());
                x.Value.Clear();
            }
        }

        private static void TowerDisposeTruesight(Team team)
        {
            if (TowerRange.First(x => x.Key.Team == team).Value.Count < 2) return;
            foreach (var towerRange in TowerRange.Where(x => x.Key.Team == team).ToList())
            {
                towerRange.Value[1].Dispose();
                towerRange.Value.RemoveAt(1);
            }
        }

        #endregion Methods
    }
}