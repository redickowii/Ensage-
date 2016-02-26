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
        public class Damage
        {
            public string ClassId { get; set; }
            public int[] Dmgint { get; set; }
            public int Cooldown { get; set; }
        }

        private static readonly Menu Menu = new Menu("LastHit", "lasthit", true);

        private static Unit _creepTarget;
        private static Hero _target;
        private static Hero _me;
        private static double _aPoint;
        private static int outrange;
        private static bool isloaded;
        private static uint aRange;
        private static List<Damage> Dmg = new List<Damage>(); 

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
            Menu.AddItem(new MenuItem("usespell", "Use spell ?").SetValue(true));
            Menu.AddItem(new MenuItem("AOC", "Atteck own creeps ?").SetValue(true));
            Menu.AddItem(new MenuItem("outrange", "Bonus range").SetValue(new Slider(100, 100, 500)));
            Menu.AddToMainMenu();

            Dmg.Add(new Damage { ClassId = "CDOTA_Unit_Hero_Zuus", Dmgint = new int[4] {85, 100, 115, 145 }, Cooldown = 1750});
            Dmg.Add(new Damage { ClassId = "CDOTA_Unit_Hero_Bristleback", Dmgint = new int[4] { 20, 40, 60, 80 }, Cooldown = 3000 });
            Dmg.Add(new Damage { ClassId = "CDOTA_Unit_Hero_PhantomAssassin", Dmgint = new int[4] { 60, 100, 140, 180 }, Cooldown = 6000 });

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.Load();

        }

        #region OnGameUpdate
        private static void Drawhpbar()
         {
             var attackRange = _me.GetAttackRange();
             var enemies =
                 ObjectManager.GetEntities<Unit>()
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
                             && x.Team != _me.Team && x.Distance2D(_me) < attackRange + 500);
             foreach (var enemy in enemies.Where(x => x != null ))
            {


                var health = enemy.Health;
                var maxHealth = enemy.MaximumHealth;
                if (health == maxHealth) continue;
                var damge = (float)GetPhysDamageOnUnit(enemy, 0);
                var hpleft = health;
                var hpperc = hpleft / maxHealth;

                var dmgperc = Math.Min(damge, health) / maxHealth;
                var hbarpos = HUDInfo.GetHPbarPosition(enemy);

                Vector2 screenPos;
                var enemyPos = enemy.Position + new Vector3(0, 0, enemy.HealthBarOffset);
                if (!Drawing.WorldToScreen(enemyPos, out screenPos)) continue;

                var start = screenPos;

                hbarpos.X = start.X - (HUDInfo.GetHPBarSizeX(enemy) / 2);
                hbarpos.Y = start.Y;
                var hpvarx = hbarpos.X;
                var hpbary = hbarpos.Y;
                var a = (float)Math.Round((damge * HUDInfo.GetHPBarSizeX(enemy)) / (enemy.MaximumHealth));
                var position = hbarpos + new Vector2(hpvarx * hpperc + 10, -12);
                try
                {
                    var left = (float)Math.Round(damge / 7);
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
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!isloaded)
            {
                _me = ObjectManager.LocalHero;
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                isloaded = true;
                _target = null;
            }

            if (_me == null || !_me.IsValid)
            {
                isloaded = false;
                _me = ObjectManager.LocalHero;
                _target = null;
                return;
            }

            if (Game.IsPaused || Game.IsChatOpen)
            {
                return;
            }

            if (_target != null && (!_target.IsValid || !_target.IsVisible || !_target.IsAlive || _target.Health <= 0))
            {
                _target = null;
            }
            var canCancel = Orbwalking.CanCancelAnimation();
            var wait = false;
            var rnd = new Random();
            _aPoint = _me.SecondsPerAttack * rnd.Next(100,600);
            aRange = _me.AttackRange;
            outrange = Menu.Item("outrange").GetValue<Slider>().Value;
            if (!canCancel) return;
            if (_target != null && !_target.IsVisible && !Orbwalking.AttackOnCooldown(_target))
            {
                _target = _me.ClosestToMouseTarget(500);
            }
            else if (_target == null || !Orbwalking.AttackOnCooldown(_target))
            {
                var bestAa = _me.BestAATarget();
                if (bestAa != null)
                {
                    _target = _me.BestAATarget();
                }
            }

            if (Game.IsKeyDown(Menu.Item("harass").GetValue<KeyBind>().Key))
            {
                if ((_creepTarget == null || !_creepTarget.IsValid || !_creepTarget.IsVisible || _creepTarget.IsAlive || !Orbwalking.AttackOnCooldown(_creepTarget)) && Utils.SleepCheck("cast"))
                {
                    _creepTarget = GetLowestHpCreep(_me, null);
                    _creepTarget = KillableCreep(false, _creepTarget,ref wait);
                    if (_creepTarget != null)
                    {
                        //Game.PrintMessage(" ", MessageType.LogMessage );
                        if (Menu.Item("usespell").GetValue<bool>() && Spellkill(_me) != null && Utils.SleepCheck("wait"))
                        {
                            if (Spell(_me).CanBeCasted())
                            {
                                Spell(_me).UseAbility();
                                Spell(_me).UseAbility(Spellkill(_me));
                            }
                            Utils.Sleep(100,"cast");
                            Utils.Sleep(Dmg.Find(x => x.ClassId == _me.ClassID.ToString()).Cooldown, "wait");
                        }
                        else if (!(_creepTarget.Distance2D(_me) > _me.AttackRange) || _creepTarget == null)
                        {
                            if ((_creepTarget.Health) < GetPhysDamageOnUnit(_creepTarget, 0)*3 &&
                                     (_creepTarget.Health) >= GetPhysDamageOnUnit(_creepTarget, 0) &&
                                     _creepTarget != null &&
                                     _creepTarget.Team != _me.Team && Utils.SleepCheck("stop"))
                            {
                                Utils.Sleep(_aPoint + Game.Ping, "stop");
                                _me.Attack(_creepTarget);
                                _me.Hold();
                            }
                            else if ((_creepTarget.Health) < GetPhysDamageOnUnit(_creepTarget, 0) &&
                                     _creepTarget != null)
                            {
                                _me.Attack(_creepTarget);
                            }
                        }
                        else
                        {
                            _me.Move(_creepTarget.Position);
                        }
                    }
                    else
                    {
                        if (_target != null && !_target.IsVisible)
                        {
                            var closestToMouse = _me.ClosestToMouseTarget(500);
                            if (closestToMouse != null)
                                _target = closestToMouse;
                        }
                        else if (Menu.Item("harassheroes").GetValue<bool>())
                            _target = _me.BestAATarget();
                        else
                            _target = null; 
                        Orbwalking.Orbwalk(_target, 500);
                    }
                }
            }
            if (Game.IsKeyDown(Menu.Item("farmKey").GetValue<KeyBind>().Key))
            {
                if (_creepTarget == null || !_creepTarget.IsValid || !_creepTarget.IsVisible || !_creepTarget.IsAlive || _creepTarget.Health <= 0 || !Orbwalking.AttackOnCooldown(_creepTarget))
                {
                    _creepTarget = GetLowestHpCreep(_me, null);
                    _creepTarget = KillableCreep(true, _creepTarget, ref wait);
                }
                if (_creepTarget != null)
                {
                    Orbwalking.Orbwalk(_creepTarget, 500);
                }
            }

            if (Game.IsKeyDown(Menu.Item("combatkey").GetValue<KeyBind>().Key))
            {
                Orbwalking.Orbwalk(_target, attackmodifiers: true);
            }

            if (Game.IsKeyDown(Menu.Item("kitekey").GetValue<KeyBind>().Key))
            {
                Orbwalking.Orbwalk(
                    _target,
                    attackmodifiers: true,
                    bonusWindupMs: Menu.Item("bonusWindup").GetValue<Slider>().Value);
            }
        }
        #endregion

        #region predict
        private static Ability Spell(Hero me)
        {
            switch (me.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_Zuus:
                    return me.Spellbook.Spell1;
                case ClassID.CDOTA_Unit_Hero_Bristleback:
                    return me.Spellbook.Spell2;
                case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                    return me.Spellbook.Spell1;
            }
            return null;
        }

        private static Unit Spellkill(Hero me)
        {
            try
            {
                var attackRange = me.GetAttackRange();
                var targetlist =
                    ObjectManager.GetEntities<Unit>()
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
                                && x.Team != me.Team && x.Distance2D(me) < attackRange + outrange)
                        .OrderByDescending(creep => creep.Health);
                foreach (var target in targetlist)
                {
                    switch (me.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Zuus:
                            if (
                                Dmg.Find(x => x.ClassId == me.ClassID.ToString()).Dmgint[me.Spellbook.Spell1.Level - 1] >
                                target.Health)
                                return target;
                            break;
                        case ClassID.CDOTA_Unit_Hero_Bristleback:
                            float quillSprayDmg = 0;
                            try
                            {
                                if (
                                    target.Modifiers.Find(
                                        x =>
                                            x.Name == "modifier_bristleback_quill_spray_stack" ||
                                            x.Name == "modifier_bristleback_quill_spray").IsDebuff)
                                    quillSprayDmg =
                                        target.Modifiers.Find(
                                            x =>
                                                x.Name == "modifier_bristleback_quill_spray_stack" ||
                                                x.Name == "modifier_bristleback_quill_spray").StackCount*30 +
                                        (me.Spellbook.Spell2.Level - 1)*2;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            if (
                                (Dmg.Find(x => x.ClassId == me.ClassID.ToString()).Dmgint[me.Spellbook.Spell2.Level - 1] +
                                 quillSprayDmg)*(float) (1 - 0.06*target.Armor/(1 + 0.06*target.Armor)) > target.Health)
                                return target;
                            break;
                        case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                            float BonusDamage = 0;
                            var time = UnitDatabase.GetAttackBackswing(me) +
                                       me.Distance2D(target)/me.Spellbook.Spell1.GetProjectileSpeed();
                            if (time >= target.AttackSpeedValue)
                                BonusDamage = (float) (time*target.AttacksPerSecond*target.MinimumDamage);
                            if (
                                Dmg.Find(x => x.ClassId == me.ClassID.ToString()).Dmgint[me.Spellbook.Spell1.Level - 1] +
                                BonusDamage > target.Health)
                                return target;
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static Unit GetAllLowestHpCreep(Hero source)
        {
            try
            {
                var attackRange = source.GetAttackRange();
                var lowestHp =
                    ObjectManager.GetEntities<Unit>()
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

        public static Unit GetLowestHpCreep(Hero source, Unit markedcreep)
        {
            try
            {
                var attackRange = source.GetAttackRange();
                var lowestHp =
                    ObjectManager.GetEntities<Unit>()
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
            if (minion == null) return null;
            var missilespeed = GetProjectileSpeed(_me);
            var time = _me.IsRanged == false ? 0 : UnitDatabase.GetAttackBackswing(_me) + (_me.Distance2D(minion) / missilespeed);
            double test = 0;
            if (time >= minion.AttackSpeedValue)
            {
                test = time * minion.AttacksPerSecond * minion.MinimumDamage;
            }

            if (minion.Health < GetPhysDamageOnUnit(_creepTarget, 0)*1.5)
            {
                if (_me.CanAttack())
                    return minion;
            }
            if (islaneclear)
            {
                return minion;
            }

            if (Menu.Item("denied").GetValue<bool>())
            {
                var minion2 = GetAllLowestHpCreep(_me);
                test = time * minion2.AttacksPerSecond * minion2.MinimumDamage;
                if (minion2.Health < GetPhysDamageOnUnit(minion2, test) * 1.5 && minion2.Team == _me.Team)
                {
                    if (_me.CanAttack())
                        return minion2;
                }

            }

            if (!Menu.Item("AOC").GetValue<bool>()) return null;
            {
                var minion2 = GetAllLowestHpCreep(_me);
                test = time * minion2.AttacksPerSecond * minion2.MinimumDamage;
                if (!(minion2.Health > GetPhysDamageOnUnit(minion2, test)) ||
                    minion2.Health >= minion2.MaximumHealth/2 || minion2.Team != _me.Team) return null;
                if (_me.CanAttack())
                    return minion2;
            }
            return null;
        }

        private static double GetPhysDamageOnUnit(Unit unit, double bonusdamage)
        {
            var quelling_blade = _me.FindItem("item_quelling_blade");
            double physDamage = _me.MinimumDamage + _me.BonusDamage;
            if (quelling_blade != null)
            {
                if (_me.IsRanged)
                {
                    physDamage = _me.MinimumDamage * 1.15 + _me.BonusDamage;
                }
                else
                {
                    physDamage = _me.MinimumDamage * 1.4 + _me.BonusDamage;
                }
            }
            var damageMp = 1 - 0.06 * unit.Armor / (1 + 0.06 * Math.Abs(unit.Armor));

            var realDamage = (bonusdamage + physDamage) * damageMp;

            if (unit.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                        unit.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage / 2;
            }

            return realDamage;
        }

        public static float GetProjectileSpeed(Hero unit)
        {
            var projectileSpeed = UnitDatabase.GetByName(unit.Name).ProjectileSpeed;

            return (float)projectileSpeed;
        }
        #endregion

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            if (Menu.Item("hpleftcreep").GetValue<bool>())
            {
                Drawhpbar();
            }
            
        }
    }

}
