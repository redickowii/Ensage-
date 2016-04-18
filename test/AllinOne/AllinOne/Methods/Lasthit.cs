namespace AllinOne.Methods
{
    using AllinOne.Menu;
    using AllinOne.ObjectManager;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Lasthit
    {
        #region TEST

        public static int Attack(Entity unit)
        {
            var count = 0;
            try
            {
                var creeps =
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                x.Distance2D(unit) <= x.AttackRange + 100 && x.IsAttacking() && x.IsAlive &&
                                x.Handle != unit.Handle && x.Team != unit.Team)
                        .ToList();
                count += (from creep in creeps
                          let angle = creep.Rotation < 0 ? Math.Abs(creep.Rotation) : 360 - creep.Rotation
                          where Math.Abs(angle - creep.FindAngleForTurnTime(unit.Position)) <= 3
                          select creep).Count();
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("Attack Count Error");
            }
            return count;
        }

        public static void Attack_Calc()
        {
            if (!ObjectManager.GetEntities<Unit>().Any(x => x.Distance2D(Var.Me) <= 2000 && x.IsAlive && x.Health > 0))
                return;
            var creeps =
                ObjectManager.GetEntities<Unit>()
                    .Where(x => x.Distance2D(Var.Me) <= 2000 && x.IsAlive && x.Health > 0)
                    .ToList();
            foreach (var creep in creeps)
            {
                if (!Var.CreepsDic.Any(x => x.AHealth(creep)))
                {
                    Var.CreepsDic.Add(new DictionaryUnit { Unit = creep, Ht = new List<Ht>() });
                }
            }
            //if (Common.SleepCheck("Test"))
            //{
            //    foreach (var creep in Var.CreepsDic)
            //    {
            //        Console.WriteLine("Unit : {0}", creep.Unit.Handle);
            //        foreach (var ht in creep.Ht)
            //        {
            //            Console.WriteLine("Health - time : {0} - {1} - {2}", ht.Health, ht.Time, ht.ACreeps);
            //        }
            //    }
            //    Common.Sleep(2000, "Test");
            //}
            Clear();
        }

        public static double Healthpredict(Unit unit, double time)
        {
            if (Var.CreepsDic.Count != 0 && MenuVar.Test)
            {
                if (Var.CreepsDic.All(x => x.Unit.Handle != unit.Handle)) return 0;
                try
                {
                    var hta = Var.CreepsDic.First(x => x.Unit.Handle == unit.Handle).Ht.ToArray();
                    var length = hta.Length - 1;
                    if (hta.Length - hta[length].ACreeps >= 0)
                    {
                        var aCreeps = hta[length].ACreeps;

                        if (time <=
                            hta[length - aCreeps + 1].Time -
                            hta[length - aCreeps].Time)
                        {
                            return hta[length - aCreeps].Health -
                                    hta[length - aCreeps + 1].Health - 10;
                        }
                    }
                }
                catch (Exception)
                {
                    if (MenuVar.ShowErrors)
                        Console.WriteLine("Health predict Error");
                }
            }
            return 0;
        }

        private static void Clear()
        {
            if (!Common.SleepCheck("Clear")) return;
            var creeps = ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive).ToList();
            Var.CreepsDic = (from creep in creeps
                             where Var.CreepsDic.Any(x => x.Unit.Handle == creep.Handle)
                             select Var.CreepsDic.Find(x => x.Unit.Handle == creep.Handle)).ToList();
            Common.Sleep(10000, "Clear");
        }

        private static void UpdateCreeps()
        {
            try
            {
                Var.Creeps = ObjectManager.GetEntities<Unit>()
                    .Where(
                        x =>
                            (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                             x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Additive
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Barracks
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Building
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creature) && x.IsAlive && x.IsVisible
                            && x.Distance2D(Var.Me) < MenuVar.Outrange + MyHeroInfo.AttackRange()).ToList();
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("Update Creeps Error");
            }
        }

        #endregion TEST

        #region Main

        public static void Combat()
        {
            Orbwalking.Orbwalk(Var.Target, attackmodifiers: true);
        }

        public static void Drawhpbar()
        {
            try
            {
                UpdateCreeps();
                if (Var.Creeps.Count == 0) return;
                foreach (var creep in Var.Creeps)
                {
                    if ((MenuVar.Sapport && Var.Me.Team != creep.Team) ||
                        (!MenuVar.Sapport && Var.Me.Team == creep.Team))
                        continue;
                    var health = creep.Health;
                    var maxHealth = creep.MaximumHealth;
                    if (health == maxHealth) continue;
                    var damge = (float) GetDamageOnUnitForDrawhpbar(creep, 0);
                    var hpperc = health / maxHealth;

                    var hbarpos = HUDInfo.GetHPbarPosition(creep);

                    Vector2 screenPos;
                    var enemyPos = creep.Position + new Vector3(0, 0, creep.HealthBarOffset);
                    if (!Drawing.WorldToScreen(enemyPos, out screenPos)) continue;

                    var start = screenPos;

                    hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep) / 2;
                    hbarpos.Y = start.Y;
                    var hpvarx = hbarpos.X;
                    var a = (float) Math.Floor(damge * HUDInfo.GetHPBarSizeX(creep) / creep.MaximumHealth);
                    var position = hbarpos + new Vector2(hpvarx * hpperc + 10, -12);
                    if (creep.ClassID == ClassID.CDOTA_BaseNPC_Tower)
                    {
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep) * 6;
                        position = hbarpos + new Vector2(hpvarx * hpperc - 5, -1);
                    }
                    else if (creep.ClassID == ClassID.CDOTA_BaseNPC_Barracks)
                    {
                        hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep);
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep) * 6;
                        position = hbarpos + new Vector2(hpvarx * hpperc + 10, -1);
                    }
                    else if (creep.ClassID == ClassID.CDOTA_BaseNPC_Building)
                    {
                        hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep) / 2;
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep);
                        position = hbarpos + new Vector2(hpvarx * hpperc + 10, -1);
                    }

                    Drawing.DrawRect(
                        position,
                        new Vector2(a, HUDInfo.GetHpBarSizeY(creep) - 4),
                        creep.Health > damge
                            ? creep.Health > damge * 2 ? new Color(180, 205, 205, 40) : new Color(255, 0, 0, 60)
                            : new Color(127, 255, 0, 80));
                    Drawing.DrawRect(position, new Vector2(a, HUDInfo.GetHpBarSizeY(creep) - 4), Color.Black, true);

                    if (!MenuVar.Test) continue;
                    var time = Var.Me.IsRanged == false
                        ? Var.HeroAPoint / 1000 + Var.Me.GetTurnTime(Var.CreeptargetH.Position)
                        : Var.HeroAPoint / 1000 + Var.Me.GetTurnTime(creep.Position) +
                          Var.Me.Distance2D(creep) / MyHeroInfo.GetProjectileSpeed(Var.Me);
                    var damgeprediction = Healthpredict(creep, time);
                    var b = (float) Math.Round(damgeprediction * 1 * HUDInfo.GetHPBarSizeX(creep) / creep.MaximumHealth);
                    var position2 = position + new Vector2(a, 0);
                    Drawing.DrawRect(position2, new Vector2(b, HUDInfo.GetHpBarSizeY(creep) - 4), Color.YellowGreen);
                    Drawing.DrawRect(position2, new Vector2(b, HUDInfo.GetHpBarSizeY(creep) - 4), Color.Black, true);
                }
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("Draw Hpbar Error");
            }
        }

        public static void Farm()
        {
            if (!Common.SleepCheck("cast")) return;
            if (!Var.DisableAaKeyPressed)
            {
                Common.Autoattack(0);
                Var.DisableAaKeyPressed = true;
                Var.AutoAttackTypeDef = false;
            }
            Var.CreeptargetH = KillableCreep(Var.Me, false, false, 99);
            if (Var.CreeptargetH != null && Var.CreeptargetH.IsValid && Var.CreeptargetH.IsVisible && Var.CreeptargetH.IsAlive)
            {
                if (MenuVar.UseSpell && Common.SleepCheck("cooldown"))
                    SpellCast();
                Orbwalking.Orbwalk(Var.CreeptargetH);
            }
        }

        public static List<Unit> GetNearestCreep(Unit source, float range)
        {
            try
            {
                return ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Additive ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Barracks ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Building ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creature) &&
                                 x.IsAlive && x.IsVisible && x.Distance2D(source) < range)
                                 .OrderBy(creep => creep.Health)
                                 .ToList();
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("GetNearestCreep Error");
            }
            return null;
        }

        public static void Kite()
        {
            Orbwalking.Orbwalk(
                Var.Target,
                attackmodifiers: true,
                bonusWindupMs: MenuVar.BonusWindUp);
        }

        public static void LastHit()
        {
            if (!Common.SleepCheck("cast")) return;
            if (!Var.DisableAaKeyPressed)
            {
                Common.Autoattack(0);
                Var.DisableAaKeyPressed = true;
                Var.AutoAttackTypeDef = false;
            }
            Var.CreeptargetH = KillableCreep(Var.Me, false, false, 2);
            if (MenuVar.UseSpell && Common.SleepCheck("cooldown"))
                SpellCast();
            if (Var.CreeptargetH != null && Var.CreeptargetH.IsValid &&
                Var.CreeptargetH.IsVisible && Var.CreeptargetH.IsAlive)
            {
                var time = Var.Me.IsRanged == false
                    ? Var.HeroAPoint / 1000 + Var.Me.GetTurnTime(Var.CreeptargetH.Position)
                    : Var.HeroAPoint / 1000 + Var.Me.GetTurnTime(Var.CreeptargetH.Position) +
                      Var.Me.Distance2D(Var.CreeptargetH) / MyHeroInfo.GetProjectileSpeed(Var.Me);
                var getDamage = GetDamageOnUnit(Var.Me, Var.CreeptargetH, 0);
                if (Var.CreeptargetH.Distance2D(Var.Me) <= MyHeroInfo.AttackRange())
                {
                    if (Var.CreeptargetH.Health < getDamage ||
                        Var.CreeptargetH.Health < getDamage && Var.CreeptargetH.Team == Var.Me.Team &&
                        (MenuVar.Denie || MenuVar.Aoc))
                    {
                        if (!Var.Me.IsAttacking())
                            Var.Me.Attack(Var.CreeptargetH);
                    }
                    else if (Var.CreeptargetH.Health < getDamage * 2 && Var.CreeptargetH.Health >= getDamage &&
                             Var.CreeptargetH.Team != Var.Me.Team && Common.SleepCheck("stop"))
                    {
                        Var.Me.Hold();
                        Var.Me.Attack(Var.CreeptargetH);
                        Common.Sleep((float) Var.HeroAPoint / 2 + Game.Ping, "stop");
                    }
                }
                else if (Var.Me.Distance2D(Var.CreeptargetH) >= MyHeroInfo.AttackRange() && Common.SleepCheck("walk"))
                {
                    Var.Me.Move(Var.CreeptargetH.Position);
                    Common.Sleep(100 + Game.Ping, "walk");
                }
            }
            else
            {
                Var.Target = MenuVar.Harass ? Var.Me.BestAATarget() : null;
                Orbwalking.Orbwalk(Var.Target, 500);
            }
        }

        public static void SummonFarm()
        {
            if (Var.Summons.Count == 0) return;
            foreach (var summon in Var.Summons)
            {
                Var.CreeptargetS = KillableCreep(summon.Key, false, true, 99);

                if (Var.CreeptargetS != null && Var.CreeptargetS.IsValid && Var.CreeptargetS.IsVisible &&
                    Var.CreeptargetS.IsAlive && Common.SleepCheck(summon.Key.Handle + "attack"))
                {
                    summon.Key.Attack(Var.CreeptargetS);
                    Common.Sleep(summon.Key.SecondsPerAttack * 300 + Game.Ping, summon.Key.Handle + "attack");
                }
            }
        }

        public static void SummonLastHit()
        {
            if (Var.Summons.Count == 0) return;
            if (!Var.SummonsDisableAaKeyPressed)
            {
                Common.AutoattackSummons(0);
                Var.SummonsDisableAaKeyPressed = true;
                Var.SummonsAutoAttackTypeDef = false;
            }
            foreach (var summon in Var.Summons)
            {
                var attackRange = summon.Key.AttackRange;
                Var.CreeptargetS = KillableCreep(summon.Key, false, true, 3);
                if (Var.CreeptargetS != null && Var.CreeptargetS.IsValid && Var.CreeptargetS.IsVisible &&
                    Var.CreeptargetS.IsAlive &&
                    Var.CreeptargetS.Health < GetDamageOnUnit(summon.Key, Var.CreeptargetS, 0) * 3)
                {
                    var getDamage = GetDamageOnUnit(summon.Key, Var.CreeptargetS, 0);
                    if (Var.CreeptargetS.Distance2D(summon.Key) <= attackRange)
                    {
                        if (Var.CreeptargetS.Health < getDamage)
                        {
                            if (summon.Key.NetworkActivity != NetworkActivity.Attack &&
                                Common.SleepCheck(summon.Key.Handle + "attack") ||
                                !Common.SleepCheck(summon.Key.Handle + "harass"))
                                summon.Key.Attack(Var.CreeptargetS);
                            Common.Sleep(summon.Key.SecondsPerAttack * 1000 + Game.Ping, summon.Key.Handle + "attack");
                        }
                        else if (Var.CreeptargetS.Health > getDamage && Common.SleepCheck(summon.Key.Handle + "stop"))
                        {
                            summon.Key.Hold();
                            Common.Sleep(300 + Game.Ping, summon.Key.Handle + "stop");
                        }
                    }
                    else if (summon.Key.Distance2D(Var.CreeptargetS) >= attackRange &&
                                Common.SleepCheck(summon.Key.Handle + "walk"))
                    {
                        summon.Key.Move(Var.CreeptargetS.Position);
                        Common.Sleep(300 + Game.Ping, summon.Key.Handle + "walk");
                    }
                }
                else if (MenuVar.SummonsHarass && summon.Key.Distance2D(Var.Target) < 1000 &&
                            Common.SleepCheck(summon.Key.Handle + "harass"))
                {
                    summon.Key.Attack(Var.Target);
                    Common.Sleep(1000 + Game.Ping, summon.Key.Handle + "harass");
                }
            }
        }

        private static double GetDamageOnUnit(Unit unit, Unit minion, double bonusdamage)
        {
            double modif = 1;
            double magicdamage = 0;
            double physDamage = unit.MinimumDamage + unit.BonusDamage;
            if (unit.Handle == Var.Me.Handle)
            {
                var quellingBlade = unit.FindItem("item_quelling_blade");
                if (quellingBlade != null && minion.Team != unit.Team)
                {
                    if (unit.IsRanged)
                    {
                        physDamage = unit.MinimumDamage * 1.15 + unit.BonusDamage;
                    }
                    else
                    {
                        physDamage = unit.MinimumDamage * 1.4 + unit.BonusDamage;
                    }
                }
                switch (unit.ClassID)
                {
                    case ClassID.CDOTA_Unit_Hero_AntiMage:
                        if (minion.MaximumMana > 0 && minion.Mana > 0 && Var.Q.Level > 0 && minion.Team != unit.Team)
                            bonusdamage += (Var.Q.Level - 1) * 12 + 28 * 0.6;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Viper:
                        if (Var.W.Level > 0 && minion.Team != unit.Team)
                        {
                            var nethertoxindmg = Var.W.Level * 2.5;
                            //var percent = Math.Floor((double) unit.Health / unit.MaximumHealth * 100);
                            //if (percent > 80 && percent <= 100)
                            //    bonusdamage2 = nethertoxindmg * 0.5;
                            //else if (percent > 60 && percent <= 80)
                            //    bonusdamage2 = nethertoxindmg * 1;
                            //else if (percent > 40 && percent <= 60)
                            //    bonusdamage2 = nethertoxindmg * 2;
                            //else if (percent > 20 && percent <= 40)
                            //    bonusdamage2 = nethertoxindmg * 4;
                            //else if (percent > 0 && percent <= 20)
                            //    bonusdamage2 = nethertoxindmg * 8;
                        }
                        break;

                    case ClassID.CDOTA_Unit_Hero_Ursa:
                        var furymodif = 0;
                        if (unit.Modifiers.Any(x => x.Name == "modifier_ursa_fury_swipes_damage_increase"))
                            furymodif =
                                minion.Modifiers.Find(x => x.Name == "modifier_ursa_fury_swipes_damage_increase")
                                    .StackCount;
                        if (Var.E.Level > 0)
                        {
                            if (furymodif > 0)
                                bonusdamage += furymodif * ((Var.E.Level - 1) * 5 + 15);
                            else
                                bonusdamage += (Var.E.Level - 1) * 5 + 15;
                        }
                        break;

                    case ClassID.CDOTA_Unit_Hero_BountyHunter:
                        if (Var.W.Level > 0 && Var.W.AbilityState == AbilityState.Ready)
                            bonusdamage += physDamage * ((Var.W.Level - 1) * 0.25 + 0.50);
                        break;

                    case ClassID.CDOTA_Unit_Hero_Weaver:
                        if (Var.E.Level > 0 && Var.E.AbilityState == AbilityState.Ready)
                            bonusdamage += physDamage;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Kunkka:
                        if (Var.W.Level > 0 && Var.W.AbilityState == AbilityState.Ready && Var.W.IsAutoCastEnabled)
                            bonusdamage += (Var.W.Level - 1) * 15 + 25;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Juggernaut:
                        if (Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage;
                        break;

                    case ClassID.CDOTA_Unit_Hero_Brewmaster:
                        if (Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage;
                        break;

                    case ClassID.CDOTA_Unit_Hero_ChaosKnight:
                        if (Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage * ((Var.E.Level - 1) * 0.5 + 0.25);
                        break;

                    case ClassID.CDOTA_Unit_Hero_SkeletonKing:
                        if (Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage * ((Var.E.Level - 1) * 0.5 + 0.5);
                        break;

                    case ClassID.CDOTA_Unit_Hero_Life_Stealer:
                        if (Var.E.Level > 0)
                            bonusdamage += minion.Health * ((Var.E.Level - 1) * 0.01 + 0.045);
                        break;

                    case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                        if (Var.E.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage += physDamage * ((Var.E.Level - 1) * 1.1 + 1.3);
                        break;
                }
                if (unit.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload"))
                {
                    magicdamage += (Var.E.Level - 1) * 20 + 30;
                }
                if (unit.Modifiers.Any(x => x.Name == "modifier_chilling_touch"))
                {
                    magicdamage += (Var.E.Level - 1) * 20 + 50;
                }
                if (unit.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && Var.W.Level > 0 &&
                    MenuVar.UseSpell &&
                    unit.Distance2D(minion) <= MyHeroInfo.AttackRange())
                {
                    magicdamage += (Var.W.Level - 1) * 6 + 6;
                }
            }
            if (unit.Modifiers.Any(x => x.Name == "modifier_bloodseeker_bloodrage"))
            {
                modif = modif *
                        (ObjectManager.GetEntities<Hero>()
                            .First(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker)
                            .Spellbook.Spell1.Level - 1) * 0.05 + 1.25;
            }
            if (Var.Summons != null && Var.Summons.Count > 1 && Var.Summons.Any(x => x.Key.Handle != unit.Handle) &&
                Var.CreeptargetS != null)
            {
                if (Var.CreeptargetH == null ||
                    (Var.CreeptargetH.Handle != Var.CreeptargetS.Handle && Var.Me.Handle != unit.Handle))
                {
                    bonusdamage =
                        Var.Summons.Where(
                            x =>
                                x.Key.Handle != unit.Handle && x.Key.CanAttack() &&
                                (Math.Abs(x.Key.Distance2D(minion) - x.Key.AttackRange) < 100 ||
                                 Math.Abs(x.Key.Distance2D(minion) - unit.Distance2D(minion)) < 100))
                            .Aggregate(bonusdamage, (current, x) => current + x.Key.MinimumDamage + x.Key.BonusDamage);
                }
                else if (Var.CreeptargetH.Handle == Var.CreeptargetS.Handle && Var.Me.Handle != unit.Handle)
                {
                    if (Var.Summons.Any(x => x.Key.Handle != unit.Handle))
                        bonusdamage +=
                            Var.Summons.Where(
                                x =>
                                    x.Key.Handle != unit.Handle && x.Key.CanAttack() &&
                                    x.Key.Distance2D(minion) < x.Key.AttackRange)
                                .Aggregate(bonusdamage,
                                    (current, x) => current + x.Key.MinimumDamage + x.Key.BonusDamage);
                    if (Var.Me.Distance2D(minion) < Var.Me.AttackRange && Var.Me.CanAttack())
                        bonusdamage += Var.Me.MinimumDamage + Var.Me.BonusDamage;
                }
                else if (Var.Me.Handle == unit.Handle && Var.CreeptargetH.Handle == Var.CreeptargetS.Handle)
                {
                    bonusdamage +=
                        Var.Summons.Where(x => x.Key.Distance2D(minion) < x.Key.AttackRange && x.Key.CanAttack())
                            .Aggregate(bonusdamage, (current, x) => current + x.Key.MinimumDamage + x.Key.BonusDamage);
                }
            }

            var damageMp = 1 - 0.06 * minion.Armor / (1 + 0.06 * Math.Abs(minion.Armor));
            magicdamage = magicdamage * (1 - minion.MagicDamageResist);

            var realDamage = ((bonusdamage + physDamage) * damageMp + magicdamage) * modif;
            if (minion.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                minion.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }
            if (realDamage > minion.MaximumHealth)
                realDamage = minion.Health + 10;

            return realDamage;
        }

        private static double GetDamageOnUnitForDrawhpbar(Unit unit, double bonusdamage)
        {
            var quellingBlade = Var.Me.FindItem("item_quelling_blade");
            double modif = 1;
            double magicdamage = 0;
            double physDamage = Var.Me.MinimumDamage + Var.Me.BonusDamage;
            if (quellingBlade != null && unit.Team != Var.Me.Team)
            {
                if (Var.Me.IsRanged)
                {
                    physDamage = Var.Me.MinimumDamage * 1.15 + Var.Me.BonusDamage;
                }
                else
                {
                    physDamage = Var.Me.MinimumDamage * 1.4 + Var.Me.BonusDamage;
                }
            }
            double bonusdamage2 = 0;
            switch (Var.Me.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_AntiMage:
                    if (unit.MaximumMana > 0 && unit.Mana > 0 && Var.Q.Level > 0 && unit.Team != Var.Me.Team)
                        bonusdamage2 = (Var.Q.Level - 1) * 12 + 28 * 0.6;
                    break;

                case ClassID.CDOTA_Unit_Hero_Viper:
                    if (Var.W.Level > 0 && unit.Team != Var.Me.Team)
                    {
                        var nethertoxindmg = Var.W.Level * 2.5;
                        //var percent = Math.Floor((double) unit.Health / unit.MaximumHealth * 100);
                        //if (percent > 80 && percent <= 100)
                        //    bonusdamage2 = nethertoxindmg * 0.5;
                        //else if (percent > 60 && percent <= 80)
                        //    bonusdamage2 = nethertoxindmg * 1;
                        //else if (percent > 40 && percent <= 60)
                        //    bonusdamage2 = nethertoxindmg * 2;
                        //else if (percent > 20 && percent <= 40)
                        //    bonusdamage2 = nethertoxindmg * 4;
                        //else if (percent > 0 && percent <= 20)
                        //    bonusdamage2 = nethertoxindmg * 8;
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Ursa:
                    var furymodif = 0;
                    if (Var.Me.Modifiers.Any(x => x.Name == "modifier_ursa_fury_swipes_damage_increase"))
                        furymodif =
                            unit.Modifiers.Find(x => x.Name == "modifier_ursa_fury_swipes_damage_increase").StackCount;
                    if (Var.E.Level > 0)
                    {
                        if (furymodif > 0)
                            bonusdamage2 = furymodif * ((Var.E.Level - 1) * 5 + 15);
                        else
                            bonusdamage2 = (Var.E.Level - 1) * 5 + 15;
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_BountyHunter:
                    if (Var.W.Level > 0 && Var.W.AbilityState == AbilityState.Ready)
                        bonusdamage2 = physDamage * ((Var.W.Level - 1) * 0.25 + 0.50);
                    break;

                case ClassID.CDOTA_Unit_Hero_Weaver:
                    if (Var.E.Level > 0 && Var.E.AbilityState == AbilityState.Ready)
                        bonusdamage2 = physDamage;
                    break;

                case ClassID.CDOTA_Unit_Hero_Kunkka:
                    if (Var.W.Level > 0 && Var.W.AbilityState == AbilityState.Ready && Var.W.IsAutoCastEnabled)
                        bonusdamage2 = (Var.W.Level - 1) * 15 + 25;
                    break;

                case ClassID.CDOTA_Unit_Hero_Juggernaut:
                    if (Var.E.Level > 0)
                        if (Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage;
                    break;

                case ClassID.CDOTA_Unit_Hero_Brewmaster:
                    if (Var.E.Level > 0)
                        if (Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage;
                    break;

                case ClassID.CDOTA_Unit_Hero_ChaosKnight:
                    if (Var.E.Level > 0)
                        if (Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage * ((Var.E.Level - 1) * 0.5 + 0.25);
                    break;

                case ClassID.CDOTA_Unit_Hero_Clinkz:
                    if (Var.W.Level > 0 && Var.W.IsAutoCastEnabled && unit.Team != Var.Me.Team)
                        bonusdamage2 = (Var.W.Level - 1) * 10 + 30;
                    break;

                case ClassID.CDOTA_Unit_Hero_SkeletonKing:
                    if (Var.E.Level > 0)
                        if (Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage * ((Var.E.Level - 1) * 0.5 + 0.5);
                    break;

                case ClassID.CDOTA_Unit_Hero_Life_Stealer:
                    if (Var.W.Level > 0)
                        bonusdamage2 = unit.Health * ((Var.W.Level - 1) * 0.01 + 0.045);
                    break;

                case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                    if (Var.R.Level > 0)
                        if (Var.Me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage * ((Var.R.Level - 1) * 1.1 + 1.3);
                    break;
            }

            if (Var.Me.Modifiers.Any(x => x.Name == "modifier_bloodseeker_bloodrage"))
            {
                modif = modif *
                        (ObjectManager.GetEntities<Hero>()
                            .First(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker)
                            .Spellbook.Spell1.Level - 1) * 0.05 + 1.25;
            }
            if (Var.Me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload"))
            {
                magicdamage = magicdamage + (Var.E.Level - 1) * 20 + 30;
            }
            if (Var.Me.Modifiers.Any(x => x.Name == "modifier_chilling_touch"))
            {
                magicdamage = magicdamage + (Var.E.Level - 1) * 20 + 50;
            }
            if (Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && Var.W.Level > 0 && MenuVar.UseSpell &&
                Var.Me.Distance2D(unit) <= MyHeroInfo.AttackRange())
            {
                magicdamage = magicdamage + (Var.W.Level - 1) * 6 + 6;
            }

            bonusdamage = bonusdamage + bonusdamage2;
            var damageMp = 1 - 0.06 * unit.Armor / (1 + 0.06 * Math.Abs(unit.Armor));
            magicdamage = magicdamage * (1 - unit.MagicDamageResist);

            var realDamage = ((bonusdamage + physDamage) * damageMp + magicdamage) * modif;
            if (unit.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                unit.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }
            if (realDamage > unit.MaximumHealth)
                realDamage = unit.Health + 10;

            return realDamage;
        }

        private static Unit GetLowestHpCreep(Unit source, float range)
        {
            try
            {
                var lowestHp =
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Additive ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Barracks ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Building ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creature) &&
                                 x.IsAlive && x.IsVisible && x.Team != source.Team &&
                                 x.Distance2D(source) < range)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                return lowestHp;
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("GetLowestHpCreep Error");
            }
            return null;
        }

        private static Unit GetMyLowestHpCreep(Unit source, float range)
        {
            try
            {
                return ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Tower ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Additive ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Barracks ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Building ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creature) &&
                                 x.IsAlive && x.IsVisible && x.Team == source.Team &&
                                 x.Distance2D(source) < range)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
            }
            catch (Exception)
            {
                Console.WriteLine("Error GetAllLowestHpCreep");
            }
            return null;
        }

        private static Unit KillableCreep(Unit unit, bool islaneclear, bool summon, double x)
        {
            try
            {
                Unit minion, miniondenie;
                if (summon)
                {
                    minion = Var.Summons[unit].DefaultIfEmpty(null).FirstOrDefault(s => s.Team != Var.Me.Team);
                    miniondenie = Var.Summons[unit].DefaultIfEmpty(null).FirstOrDefault(s => s.Team == Var.Me.Team);
                }
                else
                {
                    minion = GetLowestHpCreep(unit, Common.GetOutRange(unit));
                    miniondenie = GetMyLowestHpCreep(unit, Common.GetOutRange(unit));
                }
                if (minion == null && miniondenie == null) return null;
                var percent = minion.Health / minion.MaximumHealth * 100;
                if ((miniondenie.Health > GetDamageOnUnit(unit, miniondenie, 0) ||
                    minion.Health < GetDamageOnUnit(unit, minion, 0) + 30) &&
                    (percent < 90 || GetDamageOnUnit(unit, minion, 0) > minion.MaximumHealth) &&
                    minion.Health < GetDamageOnUnit(unit, minion, 0) * x && !MenuVar.Sapport)
                {
                    if (unit.CanAttack())
                        return minion;
                }
                else if (islaneclear)
                {
                    return minion;
                }

                if (MenuVar.Denie && !summon || MenuVar.SummonsDenie && summon)
                {
                    if (miniondenie.Health <= GetDamageOnUnit(unit, miniondenie, 0) * x * 0.75 &&
                        miniondenie.Health <= miniondenie.MaximumHealth / 2 &&
                        miniondenie.Team == unit.Team)
                    {
                        if (unit.CanAttack())
                            return miniondenie;
                    }
                }

                if (MenuVar.Aoc && !summon || MenuVar.SummonsAoc && summon)
                {
                    if (miniondenie.Health <= miniondenie.MaximumHealth / 2 &&
                        miniondenie.Health > GetDamageOnUnit(unit, miniondenie, 0) * x * 0.75 &&
                        miniondenie.Team == unit.Team)
                        if (unit.CanAttack())
                            return miniondenie;
                }
                return null;
            }
            catch (Exception)
            {
                //
            }
            return null;
        }

        private static void SpellCast()
        {
            try
            {
                foreach (var creep in Var.Creeps.Where(x => x.Team != Var.Me.Team)
                    .OrderByDescending(creep => creep.Health))
                {
                    double damage = 0;
                    switch (Var.Me.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Zuus:
                            if (Var.Q.Level > 0 && Var.Q.CanBeCasted() && Var.Me.Distance2D(creep) > MyHeroInfo.AttackRange())
                            {
                                damage = ((Var.Q.Level - 1) * 15 + 85) * (1 - creep.MagicDamageResist);
                                if (damage > creep.Health)
                                {
                                    Var.Q.UseAbility(creep);
                                    Common.Sleep(Var.Q.GetCastPoint(Var.Q.Level) * 1000 + 50 + Game.Ping, "cast");
                                    Common.Sleep(Var.Q.GetCooldown(Var.Q.Level) * 1000 + 50 + Game.Ping, "cooldown");
                                }
                            }
                            break;

                        case ClassID.CDOTA_Unit_Hero_Bristleback:
                            if (Var.W.Level > 0 && Var.W.CanBeCasted() && Var.Me.Distance2D(creep) > MyHeroInfo.AttackRange())
                            {
                                double quillSprayDmg = 0;
                                if (creep.Modifiers.Any(
                                        x =>
                                            x.Name == "modifier_bristleback_quill_spray_stack" ||
                                            x.Name == "modifier_bristleback_quill_spray"))
                                    quillSprayDmg =
                                        creep.Modifiers.Find(
                                            x =>
                                                x.Name == "modifier_bristleback_quill_spray_stack" ||
                                                x.Name == "modifier_bristleback_quill_spray").StackCount * 30 +
                                        (Var.W.Level - 1) * 2;
                                damage = ((Var.W.Level - 1) * 20 + 20 + quillSprayDmg) *
                                         (1 - 0.06 * creep.Armor / (1 + 0.06 * creep.Armor));
                                if (damage > creep.Health && Var.W.CastRange > Var.Me.Distance2D(creep))
                                {
                                    Var.W.UseAbility();
                                    Common.Sleep(Var.W.GetCastPoint(Var.W.Level) * 1000 + 50 + Game.Ping, "cast");
                                    Common.Sleep(Var.W.GetCooldown(Var.W.Level) * 1000 + 50 + Game.Ping, "cooldown");
                                }
                            }
                            break;

                        case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                            if (Var.Q.Level > 0 && Var.Q.CanBeCasted() && Var.Me.Distance2D(creep) > MyHeroInfo.AttackRange())
                            {
                                var time = 300 + Var.Me.Distance2D(creep) / Var.Q.GetProjectileSpeed();
                                if (time < creep.SecondsPerAttack * 1000)
                                    damage = ((Var.Q.Level - 1) * 40 + 60) * (1 - 0.06 * creep.Armor / (1 + 0.06 * creep.Armor));
                                if (damage > creep.Health)
                                {
                                    Var.Q.UseAbility(creep);
                                    Common.Sleep(Var.Q.GetCastPoint(Var.Q.Level) * 1000 + 50 + Game.Ping, "cast");
                                    Common.Sleep(6 * 1000 + Game.Ping, "cooldown");
                                }
                            }
                            break;

                        case ClassID.CDOTA_Unit_Hero_Pudge:
                            if (Var.W.Level > 0)
                            {
                                if (Var.CreeptargetH != null && creep.Handle == Var.CreeptargetH.Handle &&
                                    Var.Me.Distance2D(creep) <= MyHeroInfo.AttackRange())
                                {
                                    damage = GetDamageOnUnit(Var.Me, creep, 0);
                                    if (damage > creep.Health && !Var.W.IsToggled && Var.Me.IsAttacking())
                                    {
                                        Var.W.ToggleAbility();
                                        Common.Sleep(200 + Game.Ping, "cooldown");
                                    }
                                }
                                if (Var.W.IsToggled)
                                {
                                    Var.W.ToggleAbility();
                                    Common.Sleep((float) Var.HeroAPoint + Game.Ping, "cooldown");
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }

        #endregion Main
    }
}