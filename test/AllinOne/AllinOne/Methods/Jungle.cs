namespace AllinOne.Methods
{
    using AllinOne.Menu;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Jungle
    {
        #region Fields

        private static Unit _closestNeutral;

        #endregion Fields

        #region Methods

        public static void GetClosestCamp(List<Unit> h)
        {
            foreach (var baseNpcCreep in h)
            {
                var num1 = Var.Camps.Count(x => x.Stacked && x.State == 0);
                var num3 = h.Count;
                var num2 = Var.Camps.Count(x => x.Stacked && x.State == 0 && x.Unit == null);
                try
                {
                    var bid = 0;
                    var bwaitPosition = new Vector3();
                    var cont = true;
                    foreach (
                        var camp in
                            Var.Camps.Where(x => x.Stacked)
                                .Where(x => x.Unit != null).Where(x => x.Unit.Handle == baseNpcCreep.Handle))
                    {
                        if (camp.State != 0)
                            cont = false;
                        bid = camp.Id;
                        bwaitPosition = camp.WaitPosition;
                    }
                    if (!cont) continue;
                    if (num1 == num2)
                    {
                        Var.Camps.First(x => GetClosestCampu(baseNpcCreep, 1).Id == x.Id).Unit = baseNpcCreep;
                    }
                    else if (num2 > 0 && num1 - num2 < num3)
                    {
                        if (!Unittrue(baseNpcCreep)) continue;
                        Var.Camps.First(x => GetClosestCampu(baseNpcCreep, 1).Id == x.Id).Unit = baseNpcCreep;
                    }
                    else if (num1 <= num3 && num2 == 0 && GetClosestCampu(baseNpcCreep, 2).Id != 0)
                    {
                        var id = GetClosestCampu(baseNpcCreep, 2).Id;
                        var unit = GetClosestCampu(baseNpcCreep, 2).Unit;

                        var waitPosition = GetClosestCampu(baseNpcCreep, 2).WaitPosition;
                        if (baseNpcCreep.Distance2D(waitPosition) < unit.Distance2D(waitPosition) &&
                            baseNpcCreep.Distance2D(bwaitPosition) > baseNpcCreep.Distance2D(waitPosition)
                            )
                        {
                            Var.Camps.First(x => x.Id == id).Unit = baseNpcCreep;
                            Var.Camps.First(x => x.Id == bid).Unit = unit;
                        }
                    }
                    else if (num1 - num2 == num3 && num2 > 0 && GetClosestCampu(baseNpcCreep, 3).Id != 0)
                    {
                        var id = GetClosestCampu(baseNpcCreep, 3).Id;
                        var unit = GetClosestCampu(baseNpcCreep, 3).Unit;
                        var waitPosition = GetClosestCampu(baseNpcCreep, 3).WaitPosition;
                        if ((baseNpcCreep.Distance2D(waitPosition) < unit.Distance2D(waitPosition) &&
                             baseNpcCreep.Distance2D(bwaitPosition) > baseNpcCreep.Distance2D(waitPosition)) ||
                            unit == null)
                        {
                            Var.Camps.First(x => x.Id == id).Unit = baseNpcCreep;
                            Var.Camps.First(x => x.Id == bid).Unit = unit;
                        }
                    }
                }
                catch (Exception)
                {
                    if (MenuVar.ShowErrors)
                        Common.PrintError("Error GetClosestCamp");
                }
            }
        }

        public static void Stack()
        {
            Clear();
            if (!Var.Camps.Any(x => x.Stacked && x.Unit != null)) return;
            foreach (var camp in Var.Camps.Where(x => x.Stacked && x.Unit != null))
            {
                var unit = camp.Unit;
                if (!Utils.SleepCheck("JungleStack." + unit.Handle)) continue;
                var time =
                    (int) (camp.Starttime - unit.Distance2D(camp.WaitPosition) / unit.MovementSpeed - 5 + Game.Ping / 1000);
                switch (camp.State)
                {
                    case 0:
                        if (Var.Seconds < time) continue;
                        unit.Move(camp.WaitPosition);
                        camp.State = 1;
                        Utils.Sleep(500, "JungleStack." + unit.Handle);
                        break;

                    case 1:
                        var creepscount = CreepCount(unit, 800);
                        if (creepscount == 0 && unit.Distance2D(camp.WaitPosition) < 10)
                        {
                            camp.Empty = true;
                            camp.Unit.Move(camp.WaitPositionN);
                            camp.Unit = null;
                            camp.State = 5;
                            continue;
                        }
                        if (Var.Seconds < time) continue;
                        if (unit.Distance2D(camp.WaitPosition) < 10)
                            camp.State = 2;
                        unit.Move(camp.WaitPosition);
                        Utils.Sleep(500, "JungleStack." + unit.Handle);
                        break;

                    case 2:
                        if (Var.Seconds >= camp.Starttime - 5)
                        {
                            _closestNeutral = GetNearestCreepToPull(unit, 800);
                            creepscount = CreepCount(unit, 800);
                            var creeps = creepscount >= 6 ? creepscount * 75 / 1000 : 0;
                            float distance = 0;
                            if (unit.AttackRange < unit.Distance2D(_closestNeutral))
                            {
                                distance = (unit.Distance2D(_closestNeutral) - unit.AttackRange) / unit.MovementSpeed;
                            }
                            camp.AttackTime =
                                (int)
                                    Math.Round(camp.Starttime - creeps - distance -
                                               (unit.IsRanged ? unit.SecondsPerAttack - unit.BaseAttackTime / 3 : 0));
                            camp.State = 3;
                            unit.Move(PositionCalc(unit, _closestNeutral));
                        }
                        Utils.Sleep(500, "JungleStack." + unit.Handle);
                        break;

                    case 3:
                        if (Var.Seconds >= camp.AttackTime)
                        {
                            _closestNeutral = GetNearestCreepToPull(unit, 800);
                            float distance = 0;
                            if (unit.AttackRange < unit.Distance2D(_closestNeutral))
                            {
                                distance = (unit.Distance2D(_closestNeutral) - unit.AttackRange) / unit.MovementSpeed;
                            }
                            var tWait = (distance + unit.SecondsPerAttack - unit.BaseAttackTime / 3) * 1000 + Game.Ping;
                            unit.Attack(_closestNeutral);
                            Utils.Sleep(tWait, "JungleStack.Wait" + unit.Handle);
                            camp.State = 4;
                        }
                        break;

                    case 4:
                        if (unit.Distance2D(_closestNeutral) <= 150 || Utils.SleepCheck("JungleStack.Wait" + unit.Handle))
                        {
                            unit.Stop();
                            unit.Move(camp.StackPosition);
                            camp.State = 6;
                        }
                        break;

                    case 6:
                        if (Var.Seconds == 0)
                        {
                            unit.Move(camp.WaitPositionN);
                            camp.State = 0;
                        }
                        Utils.Sleep(1000, "JungleStack." + unit.Handle);
                        break;
                }
            }
        }

        private static void Clear()
        {
            foreach (var camp in Var.Camps)
            {
                if (Var.Seconds == 1)
                {
                    camp.Empty = false;
                    camp.Unit = null;
                    camp.State = 0;
                }
                if (Var.Seconds > 47 && camp.Unit == null) camp.State = 5;
                if (camp.Unit == null) continue;
                if (camp.Unit.IsAlive) continue;
                camp.Unit = null;
                camp.State = 0;
            }
        }

        private static int CreepCount(Unit h, float radius)
        {
            try
            {
                return
                    Ensage.ObjectManager.GetEntities<Unit>()
                        .Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && h.Distance2D(x) <= radius)
                        .ToList().Count;
            }
            catch (Exception)
            {
                //
            }
            return 0;
        }

        private static JungleCamps GetClosestCampu(Unit h, int n)
        {
            JungleCamps[] closest =
            {
                new JungleCamps {WaitPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), Id = 0}
            };
            foreach (var camp in Var.Camps.Where(x => x.State == 0 && x.Stacked))
            {
                if (h.Distance2D(closest[0].WaitPosition) > h.Distance2D(camp.WaitPosition))
                {
                    switch (n)
                    {
                        case 1:
                            if (camp.Unit == null)
                            {
                                closest[0] = camp;
                            }
                            break;

                        case 2:

                            if (camp.Unit != null)
                            {
                                if (camp.Unit.Handle != h.Handle)
                                {
                                    closest[0] = camp;
                                }
                            }

                            break;

                        case 3:
                            closest[0] = camp;
                            break;
                    }
                }
            }
            return closest[0];
        }

        private static JungleCamps GetFurtherCamp(Hero h)
        {
            JungleCamps[] further = { new JungleCamps { WaitPosition = h.Position, Id = 0 } };
            foreach (var camp in Var.Camps)
            {
                if (camp.Empty || !camp.Stacked || camp.Unit != null) continue;
                if (h.Distance2D(camp.WaitPosition) > h.Distance2D(further[0].WaitPosition))
                    further[0] = camp;
            }
            return further[0];
        }

        private static Unit GetNearestCreepToPull(Unit h, float radius)
        {
            var neutrals =
                Ensage.ObjectManager.GetEntities<Unit>()
                    .Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && h.Distance2D(x) <= radius)
                    .ToList();
            Unit bestCreep = null;
            var bestDistance = float.MaxValue;
            foreach (var neutral in neutrals.Where(neutral => h.Distance2D(neutral) < bestDistance))
            {
                bestDistance = h.Distance2D(neutral);
                bestCreep = neutral;
            }
            return bestCreep;
        }

        private static Vector3 PositionCalc(Unit me, Unit target)
        {
            var l = (me.Distance2D(target) - me.Distance2D(target) + 2) / (me.Distance2D(target) - 2);
            var posA = me.Position;
            var posB = target.Position;
            var x = (posA.X + l * posB.X) / (1 + l);
            var y = (posA.Y + l * posB.Y) / (1 + l);
            return new Vector3((int) x, (int) y, posA.Z);
        }

        private static bool Unittrue(Unit h)
        {
            return Var.Camps.Where(camp => camp.Unit != null).All(camp => camp.Unit.Handle != h.Handle);
        }

        #endregion Methods
    }
}