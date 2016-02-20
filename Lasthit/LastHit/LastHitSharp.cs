namespace LastHit
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

    using SharpDX;
    using System.Collections.Generic;
    using System.Linq;

    internal class LastHitSharp
    {
        #region Declare Static Fields

        private static readonly Menu Menu = new Menu("LastHit", "lasthit", true);

        private static Unit creepTarget;
        private static Hero target;
        private static Hero me;
        private static double aPoint;
        private static int tickRate = 10, outrange;
        private static bool isloaded;
        private static uint aRange;
        private static Bool _screenSizeLoaded = false;
        private static float _screenX;
        private static readonly Dictionary<Unit, string> CreepsDictionary = new Dictionary<Unit, string>();
        private static readonly Dictionary<Unit, Team> CreepsTeamDictionary = new Dictionary<Unit, Team>();
        #endregion

        public static void Init()
        {

            Menu.AddItem(new MenuItem("combatkey", "Chase mode").SetValue(new KeyBind(32, KeyBindType.Press)));
            Menu.AddItem(new MenuItem("harass", "Lasthit mode").SetValue(new KeyBind('C', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("farmKey", "Farm mode").SetValue(new KeyBind('V', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("kitekey", "Kite mode").SetValue(new KeyBind('H', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("bonuswindup", "Bonus WindUp time on kitting").SetValue(new Slider(500, 100, 2000)).SetTooltip("Time between attacks in kitting mode"));
            Menu.AddItem(new MenuItem("hpleftcreep", "Mark hp ?").SetValue(true));
            Menu.AddItem(new MenuItem("harassheroes", "Harass in lasthit mode ?").SetValue(true));
            Menu.AddItem(new MenuItem("denied", "Deny creep ?").SetValue(true));
            Menu.AddItem(new MenuItem("AOC", "Atteck own creeps ?").SetValue(true));
            Menu.AddItem(new MenuItem("outrange", "Bonus range").SetValue(new Slider(100, 100, 500)));
            Menu.AddToMainMenu();


            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.Load();

        }

        #region OnGameUpdate
        private static void drawhpbar()
         {
             var attackRange = me.GetAttackRange();
             var enemies =
                 ObjectMgr.GetEntities<Unit>()
                     .Where(
                         x =>
                         (x.ClassID == ClassID.CDOTA_BaseNPC_Tower || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                          || x.ClassID == ClassID.CDOTA_BaseNPC_Creep
                          || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                          || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                          || x.ClassID == ClassID.CDOTA_BaseNPC_Additive
                          || x.ClassID == ClassID.CDOTA_BaseNPC_Barracks
                          || x.ClassID == ClassID.CDOTA_BaseNPC_Building
                          || x.ClassID == ClassID.CDOTA_BaseNPC_Creature) && x.IsAlive && x.IsVisible
                             && x.Team != me.Team && x.Distance2D(me) < attackRange + 500);
             foreach (var enemy in enemies.Where(x => x != null ))
            {


                var health = enemy.Health;
                var maxHealth = enemy.MaximumHealth;
                if (health != maxHealth)
                {

                    var damge = (float)GetPhysDamageOnUnit(enemy, 0);
                    var hpleft = health;
                    var hpperc = hpleft / maxHealth;

                    var dmgperc = Math.Min(damge, health) / maxHealth;
                    Vector2 hbarpos;



                    hbarpos = HUDInfo.GetHPbarPosition(enemy);

                    Vector2 screenPos;
                    var enemyPos = enemy.Position + new Vector3(0, 0, enemy.HealthBarOffset);
                    if (!Drawing.WorldToScreen(enemyPos, out screenPos)) continue;

                    var start = screenPos;


                    hbarpos.X = start.X - (HUDInfo.GetHPBarSizeX(enemy) / 2);
                    hbarpos.Y = start.Y;
                    var hpvarx = hbarpos.X;
                    var hpbary = hbarpos.Y;
                    float a = (float)Math.Round((damge * HUDInfo.GetHPBarSizeX(enemy)) / (enemy.MaximumHealth));
                    var position = hbarpos + new Vector2(hpvarx * hpperc + 10, -12);
                    try
                    {
                        float left = (float)Math.Round(damge / 7);
                        Drawing.DrawRect(
                            position,
                            new Vector2(a, (float)(HUDInfo.GetHpBarSizeY(enemy) - 4)),
                            (enemy.Health > damge) ? (enemy.Health > damge * 2) ? new Color(180, 205, 205, 40) : new Color(255, 0, 0, 60) : new Color(127, 255, 0, 80));
                        Drawing.DrawRect(position, new Vector2(a, (float)(HUDInfo.GetHpBarSizeY(enemy) - 4)), Color.Black, true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
         }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!isloaded)
            {
                me = ObjectMgr.LocalHero;
                if (!Game.IsInGame || me == null)
                {
                    return;
                }
                isloaded = true;
                target = null;
            }

            if (me == null || !me.IsValid)
            {
                isloaded = false;
                me = ObjectMgr.LocalHero;
                target = null;
                return;
            }

            if (Game.IsPaused)
            {
                return;
            }

            if (Game.IsChatOpen)
            {
                return;
            }

            if (target != null && (!target.IsValid || !target.IsVisible || !target.IsAlive || target.Health <= 0))
            {
                target = null;
            }
            var canCancel = Orbwalking.CanCancelAnimation();
            bool wait = false;

            aPoint = ((UnitDatabase.GetAttackPoint(me) * 100) / (1 + me.AttackSpeedValue)) * 1000;
            aRange = me.AttackRange;
            outrange = Menu.Item("outrange").GetValue<Slider>().Value;
           
            if (canCancel)
            {
                if (target != null && !target.IsVisible && !Orbwalking.AttackOnCooldown(target))
                {
                    target = me.ClosestToMouseTarget(500);
                }
                else if (target == null || !Orbwalking.AttackOnCooldown(target))
                {
                    var bestAa = me.BestAATarget();
                    if (bestAa != null)
                    {
                        target = me.BestAATarget();
                    }
                }

                if (Game.IsKeyDown(Menu.Item("harass").GetValue<KeyBind>().Key))
                {
                    if ((creepTarget == null || !creepTarget.IsValid || !creepTarget.IsVisible || creepTarget.Health <= 0 || !Orbwalking.AttackOnCooldown(creepTarget)))
                    {
                        creepTarget = GetLowestHPCreep(me, null);
                        creepTarget = KillableCreep(false, creepTarget,ref wait);
                        if (creepTarget != null)
                        {
                            if (creepTarget.Distance2D(me) > me.AttackRange && creepTarget != null)
                            {
                                me.Move(creepTarget.Position);
                            }
                            else if ((creepTarget.Health) < GetPhysDamageOnUnit(creepTarget, 0)*3 &&
                                     (creepTarget.Health) >= GetPhysDamageOnUnit(creepTarget, 0) && creepTarget != null &&
                                     Utils.SleepCheck("stop"))
                            {
                                Utils.Sleep(aPoint + Game.Ping, "stop");
                                me.Attack(creepTarget);
                                me.Hold();
                            }
                            else if ((creepTarget.Health) < GetPhysDamageOnUnit(creepTarget, 0) &&
                                     creepTarget != null)
                            {
                                me.Attack(creepTarget);
                            }
                        }
                        else
                        {
                            me.Move(Game.MousePosition);
                            /*if (target != null && !target.IsVisible)
                            {
                                var closestToMouse = me.ClosestToMouseTarget(500);
                                if (closestToMouse != null)
                                    target = closestToMouse;
                            }
                            else if (Menu.Item("harassheroes").GetValue<bool>())
                                target = me.BestAATarget();
                            else
                                target = null; 
                            Orbwalking.Orbwalk(target, 500);*/
                        }
                    }
                }
            }

            if (Game.IsKeyDown(Menu.Item("farmKey").GetValue<KeyBind>().Key))
            {
                if (creepTarget == null || !creepTarget.IsValid || !creepTarget.IsVisible || !creepTarget.IsAlive || creepTarget.Health <= 0 || !Orbwalking.AttackOnCooldown(creepTarget))
                {
                    creepTarget = GetLowestHPCreep(me, null);
                    creepTarget = KillableCreep(true, creepTarget, ref wait);
                }
                else if (creepTarget != null)
                {
                    Orbwalking.Orbwalk(creepTarget);
                }
            }

            if (Game.IsKeyDown(Menu.Item("combatkey").GetValue<KeyBind>().Key))
            {
                Orbwalking.Orbwalk(target, attackmodifiers: true);
            }

            if (Game.IsKeyDown(Menu.Item("kitekey").GetValue<KeyBind>().Key))
            {
                Orbwalking.Orbwalk(
                    target,
                    attackmodifiers: true,
                    bonusWindupMs: Menu.Item("bonusWindup").GetValue<Slider>().Value);
            }
        }

        #endregion

        #region predict

        public static Unit GetAllLowestHPCreep(Hero source)
        {
            try
            {
                var attackRange = source.GetAttackRange();
                var lowestHp =
                    ObjectMgr.GetEntities<Unit>()
                        .Where(
                            x =>
                            (x.ClassID == ClassID.CDOTA_BaseNPC_Tower || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Additive
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Barracks
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Building
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creature) && x.IsAlive && x.IsVisible
                                /*&& x.Team != source.Team*/ && x.Distance2D(source) < attackRange + outrange)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                return lowestHp;
            }
            catch (Exception)
            {
                //no   
            }
            return null;
        }
        public static Unit GetLowestHPCreep(Hero source, Unit markedcreep)
        {
            try
            {
                var attackRange = source.GetAttackRange();
                var lowestHp =
                    ObjectMgr.GetEntities<Unit>()
                        .Where(
                            x =>
                            (x.ClassID == ClassID.CDOTA_BaseNPC_Tower || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Additive
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Barracks
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Building
                             || x.ClassID == ClassID.CDOTA_BaseNPC_Creature) && x.IsAlive && x.IsVisible
                            && x.Team != source.Team && x.Distance2D(source) < attackRange + outrange && x != markedcreep)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                return lowestHp;
            }
            catch (Exception)
            {
                //no   
            }
            return null;
        }
        private static Unit KillableCreep(bool islaneclear, Unit minion,ref bool wait)
        {

            double test = 0;
            if (minion != null)
            {


                var missilespeed = GetProjectileSpeed(me);
                var time = me.IsRanged == false ? 0 : UnitDatabase.GetAttackBackswing(me) + (me.Distance2D(minion) / missilespeed);
                if (time >= minion.AttackSpeedValue)
                {
                    test = time * minion.AttacksPerSecond * minion.MinimumDamage;
                }

                if (minion != null && (minion.Health) < GetPhysDamageOnUnit(creepTarget, 0) * 1.5)
                {

                    if (me.CanAttack())
                    {
                        return minion;
                    }
                }

                if (Menu.Item("denied").GetValue<bool>())
                {
                    Unit minion2 = GetAllLowestHPCreep(me);
                    test = time * minion2.AttacksPerSecond * minion2.MinimumDamage;
                    if (minion2 != null && (minion2.Health) < GetPhysDamageOnUnit(minion2, test) * 1.5 && minion2.Team == me.Team)
                    {
                        if (me.CanAttack())
                            return minion2;
                    }

                }

                if (Menu.Item("AOC").GetValue<bool>())
                {
                    Unit minion2 = GetAllLowestHPCreep(me);
                    test = time * minion2.AttacksPerSecond * minion2.MinimumDamage;
                    if (minion2 != null && minion2.Health > GetPhysDamageOnUnit(minion2, test) && minion2.Health < minion2.MaximumHealth/2 && minion2.Team == me.Team)
                    {
                        if (me.CanAttack())
                            return minion2;
                    }
                }


                if (minion != null && (minion.Health) >= GetPhysDamageOnUnit(minion, test) && (minion.Health) <= (GetPhysDamageOnUnit(minion, test) + me.MinimumDamage * 2))
                {
                    {
                        return null;
                    }
                }
            }
            //Console.WriteLine("test " + test );
            return islaneclear == true ? minion : null;
        }
        private static double GetPhysDamageOnUnit(Unit unit, double bonusdamage)
        {
            Item quelling_blade = me.FindItem("item_quelling_blade");
            double PhysDamage = me.MinimumDamage + me.BonusDamage;
            if (quelling_blade != null)
            {
                if (me.IsRanged)
                {
                    PhysDamage = me.MinimumDamage * 1.15 + me.BonusDamage;
                }
                else
                {
                    PhysDamage = me.MinimumDamage * 1.4 + me.BonusDamage;

                }
            }
            double _damageMP = 1 - 0.06 * unit.Armor / (1 + 0.06 * Math.Abs(unit.Armor));

            double realDamage = (bonusdamage + PhysDamage) * _damageMP;

            if (unit.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                        unit.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }

            return realDamage;
        }
        public static float GetProjectileSpeed(Hero unit)
        {
            //Console.WriteLine(unit.AttacksPerSecond * Game.FindKeyValues(unit.Name + "/AttackRate", KeyValueSource.Hero).FloatValue / 0.01);
            //var ProjectileSpeed = Game.FindKeyValues(unit.Name + "/ProjectileSpeed", KeyValueSource.Unit).FloatValue;
            var ProjectileSpeed = UnitDatabase.GetByName(unit.Name).ProjectileSpeed;

            return (float)ProjectileSpeed;
        }

        #endregion

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            if (Menu.Item("hpleftcreep").GetValue<bool>())
            {
                drawhpbar();
            }
            
        }
    }

}
