using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;

namespace LastHit
{
    internal class LastHit
    {
        #region Declare Static Fields

        public class DictionaryUnit
        {
            public Unit Unit { get; set; }

            public List<Ht> Ht { get; set; }

            public bool AHealth(Entity unit)
            {
                if (unit.Handle != Unit.Handle) return false;
                if (Ht.Any(x => x.Health - unit.Health < 10)) return true;
                Ht.Add(new Ht {Health = unit.Health, Time = Game.GameTime, ACreeps = Attack(unit)});
                return true;
            }
        }

        public class Ht
        {
            public float Health { get; set; }
            public float Time { get; set; }
            public int ACreeps { get; set; }
        }

        private static readonly Menu Menu = new Menu("LastHit", "lasthit", true, "item_quelling_blade", true);

        private static Unit _creepTarget, _creepTargetS;
        private static List<Unit> _summonsUnits;
        private static Hero _target;
        private static Hero _me;
        private static Ability _q, _w, _e, _r;
        private static double _aPoint;
        private static int _outrange, _autoAttack = 1, _stringList = 0, _i;
        private static bool _isloaded, _check = true, _checkAA = false;
        private static List<DictionaryUnit> _creeps = new List<DictionaryUnit>();
        private static float _lastRange, _ias, _attackRange;
        private static ParticleEffect _rangeDisplay;

        #endregion

        public static void Init()
        {
            Menu.AddItem(
                new MenuItem("autoAttackMode", "Auto Attack").SetValue(
                    new StringList(new[] {"Standart", "Always", "Never"})));
            Menu.AddItem(new MenuItem("bonuswindup", "Bonus WindUp time on kitting").SetValue(new Slider(500, 100, 2000)));
            Menu.AddItem(new MenuItem("hpleftcreep", "Mark hp ?").SetValue(true));
            Menu.AddItem(new MenuItem("sapp", "Sapport").SetValue(false));
            Menu.AddItem(new MenuItem("usespell", "Use spell ?").SetValue(true));
            Menu.AddItem(new MenuItem("harassheroes", "Harass in lasthit mode ?").SetValue(true));
            Menu.AddItem(new MenuItem("denied", "Deny creep ?").SetValue(true));
            Menu.AddItem(new MenuItem("AOC", "Atteck own creeps ?").SetValue(false));
            Menu.AddItem(new MenuItem("showatkrange", "Show attack range ?").SetValue(true));
            Menu.AddItem(new MenuItem("test", "Test Attack_Calc").SetValue(false));
            Menu.AddItem(new MenuItem("outrange", "Bonus range").SetValue(new Slider(100, 100, 500)));

            var subMenu = new Menu("Summons", "Summons", false);
            subMenu.AddItem(new MenuItem("enable", "Enable").SetValue(false).SetTooltip("Test!"));
            subMenu.AddItem(new MenuItem("harassheroes_sub", "Harass in lasthit mode ?").SetValue(false));
            subMenu.AddItem(new MenuItem("denied_sub", "Deny creep ?").SetValue(false));
            subMenu.AddItem(new MenuItem("AOC_sub", "Atteck own creeps ?").SetValue(false));
            subMenu.AddItem(new MenuItem("autoD", "Auto lasthit").SetValue(false).SetTooltip("Dont work properly!!!"));
            subMenu.AddItem(new MenuItem("autoF", "Auto farm").SetValue(false).SetTooltip("Dont work properly!!!"));
            Menu.AddSubMenu(subMenu);

            var subMenu2 = new Menu("Keys", " Keys", false);
            subMenu2.AddItem(new MenuItem("combatkey", "Chase mode").SetValue(new KeyBind(32, KeyBindType.Press)));
            subMenu2.AddItem(new MenuItem("harass", "La0sthit mode").SetValue(new KeyBind('C', KeyBindType.Press)));
            subMenu2.AddItem(new MenuItem("farmKey", "Farm mode").SetValue(new KeyBind('V', KeyBindType.Press)));
            subMenu2.AddItem(new MenuItem("kitekey", "Kite mode").SetValue(new KeyBind('H', KeyBindType.Press)));
            Menu.AddSubMenu(subMenu2);

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate_Summon;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Attack_Calc;

            Orbwalking.Load();
        }

        #region TEST

        private static void Attack_Calc(EventArgs args)
        {
            if (!_isloaded || Game.IsPaused || Game.IsChatOpen) return;
            if (!Menu.Item("test").GetValue<bool>()) return;
            if (!ObjectManager.GetEntities<Unit>().Any(x => x.Distance2D(_me) <= 2000 && x.IsAlive && x.Health > 0))
                return;
            var creeps =
                ObjectManager.GetEntities<Unit>()
                    .Where(x => x.Distance2D(_me) <= 2000 && x.IsAlive && x.Health > 0)
                    .ToList();
            foreach (var creep in creeps)
            {
                if (!_creeps.Any(x => x.AHealth(creep)))
                {
                    _creeps.Add(new DictionaryUnit {Unit = creep, Ht = new List<Ht>()});
                }
            }
            //if (Utils.SleepCheck("Test"))
            //{
            //    foreach (var creep in _creeps)
            //    {
            //        Console.WriteLine("Unit : {0}", creep.Unit.Handle);
            //        foreach (var ht in creep.Ht)
            //        {
            //            Console.WriteLine("Health - time : {0} - {1} - {2}", ht.Health, ht.Time, ht.ACreeps);
            //        }
            //    }
            //    Utils.Sleep(2000, "Test");
            //}
            if (!Utils.SleepCheck("Clear")) return;
            creeps = ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive).ToList();
            _creeps = (from creep in creeps
                where _creeps.Any(x => x.Unit.Handle == creep.Handle)
                select _creeps.Find(x => x.Unit.Handle == creep.Handle)).ToList();
            Utils.Sleep(10000, "Clear");
        }

        private static int Attack(Entity unit)
        {
            int t = 0;
            var upos = unit.Position;
            try
            {
                var creeps =
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                x.Distance2D(unit) <= x.AttackRange + 100 && x.IsAttacking() && x.IsAlive &&
                                x.Handle != unit.Handle && x.Team != unit.Team)
                        .ToList();
                foreach (var creep in creeps)
                {
                    var angle = creep.Rotation < 0 ? Math.Abs(creep.Rotation) : 360 - creep.Rotation;
                    if (Math.Abs(angle - creep.FindAngleForTurnTime(unit.Position)) <= 3) t++;
                }
            }
            catch (Exception)
            {
            }
            return t;
        }

        public static double Healthpredict(Unit unit, double time)
        {
            if (_creeps.Any(x => x.Unit.Handle == unit.Handle) && Menu.Item("test").GetValue<bool>())
            {
                try
                {
                    var hta = _creeps.First(x => x.Unit.Handle == unit.Handle).Ht.ToArray();
                    var length = hta.Length - 1;
                    if ((hta.Length - hta[length].ACreeps) >= 0)
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
                    //
                }
            }
            return 0;
        }

        #endregion

        #region OnGameUpdate

        private static void Game_OnUpdate_Summon(EventArgs args)
        {
            if (!_isloaded || Game.IsPaused || Game.IsChatOpen || !Menu.Item("enable").GetValue<bool>()) return;
            try
            {
                _summonsUnits = ObjectManager.GetEntities<Unit>().Where(
                    x =>
                        (x.Distance2D(_me) < 1500 || Menu.Item("autoD").GetValue<bool>() ||
                         Menu.Item("autoF").GetValue<bool>()) && x.IsAlive && x.Team == _me.Team && x.IsControllable &&
                        !x.Name.Contains("beastmaster_hawk") &&
                        (x.Modifiers.Any(m => m.Name == "modifier_kill") ||
                         x.Modifiers.Any(m => m.Name == "modifier_dominated") ||
                         x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo ||
                         x.ClassID == ClassID.CDOTA_Unit_SpiritBear ||
                         x.ClassID == ClassID.CDOTA_Unit_VisageFamiliar) &&
                        (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                         x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                         x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                         x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                         x.ClassID == ClassID.CDOTA_Unit_SpiritBear ||
                         x.ClassID == ClassID.CDOTA_Unit_VisageFamiliar ||
                         x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                         x.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling ||
                         x.IsIllusion ||
                         (x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo && _me.Handle != x.Handle &&
                          _r.Level > 0)))
                    .ToList();
            }
            catch (Exception)
            {
                //
            }
            var wait = false;
            if (!_summonsUnits.Any()) return;
            if (Game.IsKeyDown(Menu.Item("harass").GetValue<KeyBind>().Key) || Menu.Item("autoD").GetValue<bool>())
            {
                foreach (var summonsUnit in _summonsUnits)
                {
                    var attackRange = summonsUnit.AttackRange;
                    _creepTargetS = GetLowestHpCreep(summonsUnit, null);
                    _creepTargetS = KillableCreep(summonsUnit, false, _creepTargetS, ref wait, true, 3);
                    if (_creepTargetS != null && _creepTargetS.IsValid && _creepTargetS.IsVisible &&
                        _creepTargetS.IsAlive)
                    {
                        var getDamage = GetDamageOnUnit(summonsUnit, _creepTargetS, 0);
                        if (_creepTargetS.Distance2D(summonsUnit) <= attackRange)
                        {
                            if ((_creepTargetS.Health < getDamage ||
                                 _creepTargetS.Health < getDamage && _creepTargetS.Team == _me.Team &&
                                 (Menu.Item("denied_sub").GetValue<bool>() || Menu.Item("AOC_sub").GetValue<bool>())) && Utils.SleepCheck(summonsUnit.Handle + "attack"))
                            {
                                if (!summonsUnit.IsAttacking() || !Utils.SleepCheck(summonsUnit.Handle + "harass"))
                                {
                                    summonsUnit.Attack(_creepTargetS);
                                    Utils.Sleep(300 + Game.Ping, summonsUnit.Handle + "attack");
                                }
                            }
                            else if (summonsUnit.NetworkActivity == NetworkActivity.Move && Utils.SleepCheck(summonsUnit.Handle + "stop"))
                            {
                                summonsUnit.Stop();
                                Utils.Sleep(300 + Game.Ping, summonsUnit.Handle + "stop");
                            }
                        }
                        else if (summonsUnit.Distance2D(_creepTargetS) >= attackRange && Utils.SleepCheck(summonsUnit.Handle + "walk"))
                        {
                            summonsUnit.Move(_creepTargetS.Position);
                            Utils.Sleep(300 + Game.Ping, summonsUnit.Handle + "walk");
                        }
                    }
                    else
                    {
                        if (Menu.Item("harassheroes_sub").GetValue<bool>() && Utils.SleepCheck(summonsUnit.Handle + "harass"))
                        {
                            var target = _me.BestAATarget();
                            if (target == null)
                            {
                                target = _me.ClosestToMouseTarget(1000);
                            }
                            summonsUnit.Attack(target);
                            Utils.Sleep(1000 + Game.Ping, summonsUnit.Handle + "harass");
                        }
                    }
                }
            }
            else if (Game.IsKeyDown(Menu.Item("farmKey").GetValue<KeyBind>().Key) || Menu.Item("autoF").GetValue<bool>())
            {
                foreach (var summonsUnit in _summonsUnits)
                {
                    _creepTargetS = GetLowestHpCreep(summonsUnit, null);
                    _creepTargetS = KillableCreep(summonsUnit, false, _creepTargetS, ref wait, true, 30);
                    if (_creepTargetS != null && _creepTargetS.IsValid && _creepTargetS.IsVisible &&
                        _creepTargetS.IsAlive && Utils.SleepCheck(summonsUnit.Handle + "attack"))
                    {
                        if (!summonsUnit.IsAttacking())
                        {
                            summonsUnit.Attack(_creepTargetS);
                            Utils.Sleep(300 + Game.Ping, summonsUnit.Handle + "attack");
                        }
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!_isloaded)
            {
                _me = ObjectManager.LocalHero;
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                _lastRange = _me.GetAttackRange();
                _isloaded = true;
                _rangeDisplay = null;
                _target = null;
            }

            if (_me == null || !_me.IsValid)
            {
                _isloaded = false;
                _me = ObjectManager.LocalHero;
                if (_rangeDisplay == null)
                {
                    return;
                }
                _rangeDisplay = null;
                _target = null;
                return;
            }

            if (Game.IsPaused || Game.IsChatOpen)
            {
                return;
            }

            // Auto Attack Checker
            if (_stringList != Menu.Item("autoAttackMode").GetValue<StringList>().SelectedIndex)
            {
                switch (Menu.Item("autoAttackMode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        _autoAttack = 1;
                        Autoattack(_autoAttack);
                        _stringList = 0;
                        break;
                    case 1:
                        _autoAttack = 2;
                        Autoattack(_autoAttack);
                        _stringList = 1;
                        break;
                    case 2:
                        _autoAttack = 0;
                        Autoattack(_autoAttack);
                        _stringList = 2;
                        break;
                }
            }
            // Auto Attack Checker

            if (_target != null && (!_target.IsValid || !_target.IsVisible || !_target.IsAlive || _target.Health <= 0))
            {
                _target = null;
            }
            _q = _me.Spellbook.Spell1;
            _w = _me.Spellbook.Spell2;
            _e = _me.Spellbook.Spell3;
            _r = _me.Spellbook.Spell4;
            double apoint = 0;
            apoint = _me.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
                ? 0.3
                : UnitDatabase.Units.Find(x => x.UnitName == _me.Name).AttackPoint;
            _aPoint = apoint/(1 + _me.AttacksPerSecond*_me.BaseAttackTime/100)*500;
            _outrange = Menu.Item("outrange").GetValue<Slider>().Value;

            if (_me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord)
                _attackRange = _q.IsToggled ? 128 : _me.GetAttackRange();
            else
                _attackRange = _me.GetAttackRange();

            var wait = false;
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
            if (Menu.Item("showatkrange").GetValue<bool>())
            {
                if (_rangeDisplay == null)
                {
                    if (_me.IsAlive)
                    {
                        _rangeDisplay = _me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                        _rangeDisplay.SetControlPoint(1, new Vector3(255, 80, 50));
                        _rangeDisplay.SetControlPoint(3, new Vector3(20, 0, 0));
                        _rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
                    }
                }
                else
                {
                    if (!_me.IsAlive)
                    {
                        _rangeDisplay.Dispose();
                        _rangeDisplay = null;
                    }
                    else if (_lastRange != _attackRange)
                    {
                        _rangeDisplay.Dispose();
                        _lastRange = _attackRange;
                        _rangeDisplay = _me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                        _rangeDisplay.SetControlPoint(1, new Vector3(255, 80, 50));
                        _rangeDisplay.SetControlPoint(3, new Vector3(15, 0, 0));
                        _rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
                    }
                }
            }
            else
            {
                if (_rangeDisplay != null) _rangeDisplay.Dispose();
                _rangeDisplay = null;
            }

            if (Game.IsKeyDown(Menu.Item("harass").GetValue<KeyBind>().Key) && Utils.SleepCheck("cast"))
            {
                Autoattack(0);
                _creepTarget = GetLowestHpCreep(_me, null);
                _creepTarget = KillableCreep(_me, false, _creepTarget, ref wait, false, 2);
                if (Menu.Item("usespell").GetValue<bool>() && Utils.SleepCheck("cooldown"))
                    SpellCast();
                if (_creepTarget != null && _creepTarget.IsValid && _creepTarget.IsVisible && _creepTarget.IsAlive)
                {
                    var time = _me.IsRanged == false
                        ? _aPoint*2/1000 + _me.GetTurnTime(_creepTarget.Position)
                        : _aPoint*2/1000 + _me.GetTurnTime(_creepTarget.Position) +
                          _me.Distance2D(_creepTarget)/GetProjectileSpeed(_me);
                    var getDamage = GetDamageOnUnit(_me, _creepTarget, Healthpredict(_creepTarget, time));
                    if (_creepTarget.Distance2D(_me) <= _attackRange)
                    {
                        if (_creepTarget.Health < getDamage ||
                            _creepTarget.Health < getDamage && _creepTarget.Team == _me.Team &&
                            (Menu.Item("denied").GetValue<bool>() ||
                            Menu.Item("AOC").GetValue<bool>()))
                        {
                            if (!_me.IsAttacking())
                                _me.Attack(_creepTarget);
                        }
                        else if (_creepTarget.Health < getDamage*2 && _creepTarget.Health >= getDamage &&
                                 _creepTarget.Team != _me.Team && Utils.SleepCheck("stop"))
                        {
                            _me.Hold();
                            _me.Attack(_creepTarget);
                            Utils.Sleep(_aPoint + Game.Ping, "stop");
                        }
                    }
                    else if (_me.Distance2D(_creepTarget) >= _attackRange && Utils.SleepCheck("walk"))
                    {
                        _me.Move(_creepTarget.Position);
                        Utils.Sleep(100 + Game.Ping, "walk");
                    }
                }
                else
                {
                    if (_target != null && !_target.IsVisible)
                    {
                        var closestToMouse = _me.ClosestToMouseTarget(1000);
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
            else if (Game.IsKeyDown(Menu.Item("farmKey").GetValue<KeyBind>().Key) && Utils.SleepCheck("cast"))
            {
                Autoattack(0);
                _creepTarget = GetLowestHpCreep(_me, null);
                _creepTarget = KillableCreep(_me, true, _creepTarget, ref wait, false, 2);
                if (_creepTarget != null && _creepTarget.IsValid && _creepTarget.IsVisible && _creepTarget.IsAlive)
                {
                    if (Menu.Item("usespell").GetValue<bool>() && Utils.SleepCheck("cooldown"))
                        SpellCast();
                    Orbwalking.Orbwalk(_creepTarget);
                }
            }
            else if (Game.IsKeyDown(Menu.Item("combatkey").GetValue<KeyBind>().Key))
            {
                Orbwalking.Orbwalk(_target, attackmodifiers: true);
            }
            else if (Game.IsKeyDown(Menu.Item("kitekey").GetValue<KeyBind>().Key))
            {
                Orbwalking.Orbwalk(
                    _target,
                    attackmodifiers: true,
                    bonusWindupMs: Menu.Item("bonusWindup").GetValue<Slider>().Value);
            }
            else
            {
                if (!_check)
                {
                    _check = true;
                    Autoattack(_autoAttack);
                    _checkAA = false;
                }
            }
        }

        private static void Drawhpbar()
        {
            try
            {
                var creeps =
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
                                && x.Distance2D(_me) < _attackRange + _outrange);
                foreach (var creep in creeps)
                {
                    if ((Menu.Item("sapp").GetValue<bool>() && _me.Team != creep.Team) ||
                        (!Menu.Item("sapp").GetValue<bool>() && _me.Team == creep.Team))
                        continue;
                    var health = creep.Health;
                    var maxHealth = creep.MaximumHealth;
                    if (health == maxHealth) continue;
                    var damge = (float) GetDamageOnUnitForDrawhpbar(creep, 0);
                    var hpperc = health/maxHealth;

                    var hbarpos = HUDInfo.GetHPbarPosition(creep);

                    Vector2 screenPos;
                    var enemyPos = creep.Position + new Vector3(0, 0, creep.HealthBarOffset);
                    if (!Drawing.WorldToScreen(enemyPos, out screenPos)) continue;

                    var start = screenPos;

                    hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep)/2;
                    hbarpos.Y = start.Y;
                    var hpvarx = hbarpos.X;
                    var a = (float) Math.Floor(damge*HUDInfo.GetHPBarSizeX(creep)/creep.MaximumHealth);
                    var position = hbarpos + new Vector2(hpvarx*hpperc + 10, -12);
                    if (creep.ClassID == ClassID.CDOTA_BaseNPC_Tower)
                    {
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep)*6;
                        position = hbarpos + new Vector2(hpvarx*hpperc - 5, -1);
                    }
                    else if (creep.ClassID == ClassID.CDOTA_BaseNPC_Barracks)
                    {
                        hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep);
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep)*6;
                        position = hbarpos + new Vector2(hpvarx*hpperc + 10, -1);
                    }
                    else if (creep.ClassID == ClassID.CDOTA_BaseNPC_Building)
                    {
                        hbarpos.X = start.X - HUDInfo.GetHPBarSizeX(creep)/2;
                        hbarpos.Y = start.Y - HUDInfo.GetHpBarSizeY(creep);
                        position = hbarpos + new Vector2(hpvarx*hpperc + 10, -1);
                    }

                    Drawing.DrawRect(
                        position,
                        new Vector2(a, HUDInfo.GetHpBarSizeY(creep) - 4),
                        creep.Health > damge
                            ? creep.Health > damge*2 ? new Color(180, 205, 205, 40) : new Color(255, 0, 0, 60)
                            : new Color(127, 255, 0, 80));
                    Drawing.DrawRect(position, new Vector2(a, HUDInfo.GetHpBarSizeY(creep) - 4), Color.Black, true);


                    if (!Menu.Item("test").GetValue<bool>()) continue;
                    var time = _me.IsRanged == false
                        ? _aPoint*2/1000 + _me.GetTurnTime(_creepTarget.Position)
                        : _aPoint*2/1000 + _me.GetTurnTime(creep.Position) +
                          _me.Distance2D(creep)/GetProjectileSpeed(_me);
                    var damgeprediction = Healthpredict(creep, time);
                    var b = (float) Math.Round(damgeprediction*1*HUDInfo.GetHPBarSizeX(creep)/creep.MaximumHealth);
                    var position2 = position + new Vector2(a, 0);
                    Drawing.DrawRect(position2, new Vector2(b, HUDInfo.GetHpBarSizeY(creep) - 4), Color.YellowGreen);
                    Drawing.DrawRect(position2, new Vector2(b, HUDInfo.GetHpBarSizeY(creep) - 4), Color.Black, true);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        #endregion

        #region predict

        private static void Autoattack(int aa)
        {
            //if (Game.GetConsoleVar("dota_player_units_auto_attack_mode").GetInt() != aa)
            if (Game.IsKeyDown(Menu.Item("farmKey").GetValue<KeyBind>().Key) ||
                Game.IsKeyDown(Menu.Item("harass").GetValue<KeyBind>().Key) ||
                _stringList != Menu.Item("autoAttackMode").GetValue<StringList>().SelectedIndex
                || _checkAA)
            {
                if (_check)
                {
                    _check = false;
                    _checkAA = true;
                    Game.ExecuteCommand("dota_player_units_auto_attack_mode " + aa);
                }
            }
        }

        private static void SpellCast()
        {
            try
            {
                var creeps =
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
                                && x.Team != _me.Team && x.Distance2D(_me) < 800 + _outrange)
                        .OrderByDescending(creep => creep.Health);
                foreach (var creep in creeps)
                {
                    double damage = 0;
                    switch (_me.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Zuus:
                            if (_q.Level > 0 && _q.CanBeCasted() && _me.Distance2D(creep) > _attackRange)
                            {
                                damage = ((_q.Level - 1)*15 + 85)*(1 - creep.MagicDamageResist);
                                if (damage > creep.Health)
                                {
                                    _q.UseAbility(creep);
                                    Utils.Sleep(_q.GetCastPoint(_q.Level)*1000 + 50 + Game.Ping, "cast");
                                    Utils.Sleep(_q.GetCooldown(_q.Level)*1000 + 50 + Game.Ping, "cooldown");
                                }
                            }
                            break;
                        case ClassID.CDOTA_Unit_Hero_Bristleback:
                            if (_w.Level > 0 && _w.CanBeCasted() && _me.Distance2D(creep) > _attackRange)
                            {
                                double quillSprayDmg = 0;
                                if (
                                    creep.Modifiers.Any(
                                        x =>
                                            x.Name == "modifier_bristleback_quill_spray_stack" ||
                                            x.Name == "modifier_bristleback_quill_spray"))
                                    quillSprayDmg =
                                        creep.Modifiers.Find(
                                            x =>
                                                x.Name == "modifier_bristleback_quill_spray_stack" ||
                                                x.Name == "modifier_bristleback_quill_spray").StackCount*30 +
                                        (_w.Level - 1)*2;
                                damage = ((_w.Level - 1)*20 + 20 + quillSprayDmg)*
                                         (1 - 0.06*creep.Armor/(1 + 0.06*creep.Armor));
                                if (damage > creep.Health && _w.CastRange < _me.Distance2D(creep))
                                {
                                    _w.UseAbility();
                                    Utils.Sleep(_w.GetCastPoint(_w.Level)*1000 + 50 + Game.Ping, "cast");
                                    Utils.Sleep(_w.GetCooldown(_w.Level)*1000 + 50 + Game.Ping, "cooldown");
                                }
                            }
                            break;
                        case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                            if (_q.Level > 0 && _q.CanBeCasted() && _me.Distance2D(creep) > _attackRange)
                            {
                                var time = 300 + _me.Distance2D(creep)/_q.GetProjectileSpeed();
                                if (time < creep.SecondsPerAttack*1000)
                                    damage = ((_q.Level - 1)*40 + 60)*(1 - 0.06*creep.Armor/(1 + 0.06*creep.Armor));
                                if (damage > creep.Health)
                                {
                                    _q.UseAbility(creep);
                                    Utils.Sleep(_q.GetCastPoint(_q.Level)*1000 + 50 + Game.Ping, "cast");
                                    Utils.Sleep(6*1000 + Game.Ping, "cooldown");
                                }
                            }
                            break;
                        case ClassID.CDOTA_Unit_Hero_Pudge:
                            if (_w.Level > 0)
                            {
                                if (_creepTarget != null && creep.Handle == _creepTarget.Handle &&
                                    _me.Distance2D(creep) <= _attackRange)
                                {
                                    damage = GetDamageOnUnit(_me, creep, 0);
                                    if (damage > creep.Health && !_w.IsToggled && _me.IsAttacking())
                                    {
                                        _w.ToggleAbility();
                                        Utils.Sleep(200 + Game.Ping, "cooldown");
                                    }
                                }
                                if (_w.IsToggled)
                                {
                                    _w.ToggleAbility();
                                    Utils.Sleep(_aPoint*2 + Game.Ping, "cooldown");
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

        public static Unit GetAllLowestHpCreep(Unit source)
        {
            try
            {
                var lowestHp =
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
                                && x.Distance2D(source) < _attackRange + _outrange)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                return lowestHp;
            }
            catch (Exception)
            {
                Console.WriteLine("Error GetAllLowestHpCreep");
            }
            return null;
        }

        public static Unit GetLowestHpCreep(Unit source, Unit markedcreep)
        {
            try
            {
                var lowestHp =
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
                                && x.Team != source.Team && x.Distance2D(source) < _attackRange + _outrange &&
                                x != markedcreep)
                        .OrderBy(creep => creep.Health)
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                return lowestHp;
            }
            catch (Exception)
            {
                //
            }
            return null;
        }

        private static Unit KillableCreep(Unit unit, bool islaneclear, Unit minion, ref bool wait, bool summon,
            double modif)
        {
            try
            {
                //var apoint = unit.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
                //    ? 0.3
                //    : UnitDatabase.Units.Find(x => x.UnitName == unit.Name).AttackPoint;
                //apoint = apoint / (1 + unit.AttacksPerSecond * unit.BaseAttackTime / 100) * 500;
                //var time = unit.IsRanged == false
                //? apoint * 2 / 1000 + unit.GetTurnTime(minion.Position)
                //: apoint * 2 / 1000 + unit.GetTurnTime(minion.Position) + unit.Distance2D(minion) / GetProjectileSpeed(unit);
                var percent = minion.Health/minion.MaximumHealth*100;
                if (minion.Health < GetDamageOnUnit(unit, minion, 0)*modif &&
                    (percent < 75 || minion.Health < GetDamageOnUnit(unit, minion, 0))
                    && !Menu.Item("sapp").GetValue<bool>())
                {
                    if (unit.CanAttack())
                        return minion;
                }
                else if (islaneclear)
                {
                    return minion;
                }

                if (Menu.Item("denied").GetValue<bool>() && !summon ||
                    summon && Menu.Item("denied_sub").GetValue<bool>())
                {
                    var minion2 = GetAllLowestHpCreep(unit);
                    if (minion2.Health <= GetDamageOnUnit(unit, minion2, 0)*1.5 &&
                        minion2.Health <= minion2.MaximumHealth/2 &&
                        minion2.Team == unit.Team)
                    {
                        if (unit.CanAttack())
                            return minion2;
                    }
                }

                if (Menu.Item("AOC").GetValue<bool>() && !summon ||
                    summon && Menu.Item("AOC_sub").GetValue<bool>())
                {
                    var minion2 = GetAllLowestHpCreep(unit);
                    if (minion2.Health <= minion2.MaximumHealth/2
                        && minion2.Health > GetDamageOnUnit(unit, minion2, 0)*1.5
                        && minion2.Team == unit.Team)
                        if (unit.CanAttack())
                            return minion2;
                }
                return null;
            }
            catch (Exception)
            {
                //
            }
            return null;
        }

        private static double GetDamageOnUnitForDrawhpbar(Unit unit, double bonusdamage)
        {
            var quellingBlade = _me.FindItem("item_quelling_blade");
            double modif = 1;
            double magicdamage = 0;
            double physDamage = _me.MinimumDamage + _me.BonusDamage;
            if (quellingBlade != null && unit.Team != _me.Team)
            {
                if (_me.IsRanged)
                {
                    physDamage = _me.MinimumDamage*1.15 + _me.BonusDamage;
                }
                else
                {
                    physDamage = _me.MinimumDamage*1.4 + _me.BonusDamage;
                }
            }
            double bonusdamage2 = 0;
            switch (_me.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_AntiMage:
                    if (unit.MaximumMana > 0 && unit.Mana > 0 && _q.Level > 0 && unit.Team != _me.Team)
                        bonusdamage2 = (_q.Level - 1)*12 + 28*0.6;
                    break;
                case ClassID.CDOTA_Unit_Hero_Viper:
                    if (_w.Level > 0 && unit.Team != _me.Team)
                    {
                        var nethertoxindmg = _w.Level*2.5;
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
                    if (_me.Modifiers.Any(x => x.Name == "modifier_ursa_fury_swipes_damage_increase"))
                        furymodif =
                            unit.Modifiers.Find(x => x.Name == "modifier_ursa_fury_swipes_damage_increase").StackCount;
                    if (_e.Level > 0)
                    {
                        if (furymodif > 0)
                            bonusdamage2 = furymodif*((_e.Level - 1)*5 + 15);
                        else
                            bonusdamage2 = (_e.Level - 1)*5 + 15;
                    }
                    break;
                case ClassID.CDOTA_Unit_Hero_BountyHunter:
                    if (_w.Level > 0 && _w.AbilityState == AbilityState.Ready)
                        bonusdamage2 = physDamage*((_w.Level - 1)*0.25 + 0.50);
                    break;
                case ClassID.CDOTA_Unit_Hero_Weaver:
                    if (_e.Level > 0 && _e.AbilityState == AbilityState.Ready)
                        bonusdamage2 = physDamage;
                    break;
                case ClassID.CDOTA_Unit_Hero_Kunkka:
                    if (_w.Level > 0 && _w.AbilityState == AbilityState.Ready && _w.IsAutoCastEnabled)
                        bonusdamage2 = (_w.Level - 1)*15 + 25;
                    break;
                case ClassID.CDOTA_Unit_Hero_Juggernaut:
                    if (_e.Level > 0)
                        if (_me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage;
                    break;
                case ClassID.CDOTA_Unit_Hero_Brewmaster:
                    if (_e.Level > 0)
                        if (_me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage;
                    break;
                case ClassID.CDOTA_Unit_Hero_ChaosKnight:
                    if (_e.Level > 0)
                        if (_me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage*((_e.Level - 1)*0.5 + 0.25);
                    break;
                case ClassID.CDOTA_Unit_Hero_SkeletonKing:
                    if (_e.Level > 0)
                        if (_me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage*((_e.Level - 1)*0.5 + 0.5);
                    break;
                case ClassID.CDOTA_Unit_Hero_Life_Stealer:
                    if (_w.Level > 0)
                        bonusdamage2 = unit.Health*((_w.Level - 1)*0.01 + 0.045);
                    break;
                case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                    if (_r.Level > 0)
                        if (_me.NetworkActivity == NetworkActivity.Crit)
                            bonusdamage2 = physDamage*((_r.Level - 1)*1.1 + 1.3);
                    break;
            }

            if (_me.Modifiers.Any(x => x.Name == "modifier_bloodseeker_bloodrage"))
            {
                modif = modif*
                        (ObjectManager.GetEntities<Hero>()
                            .First(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker)
                            .Spellbook.Spell1.Level - 1)*0.05 + 1.25;
            }
            if (_me.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload"))
            {
                magicdamage = magicdamage + (_e.Level - 1)*20 + 30;
            }
            if (_me.Modifiers.Any(x => x.Name == "modifier_chilling_touch"))
            {
                magicdamage = magicdamage + (_e.Level - 1)*20 + 50;
            }
            if (_me.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && _w.Level > 0 && Menu.Item("usespell").GetValue<bool>() &&
                _me.Distance2D(unit) <= _attackRange)
            {
                magicdamage = magicdamage + (_w.Level - 1)*6 + 6;
            }

            bonusdamage = bonusdamage + bonusdamage2;
            var damageMp = 1 - 0.06*unit.Armor/(1 + 0.06*Math.Abs(unit.Armor));
            magicdamage = magicdamage*(1 - unit.MagicDamageResist);

            var realDamage = ((bonusdamage + physDamage)*damageMp + magicdamage)*modif;
            if (unit.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                unit.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage/2;
            }
            if (realDamage > unit.MaximumHealth)
                realDamage = unit.Health + 10;

            return realDamage;
        }

        private static double GetDamageOnUnit(Unit unit, Unit minion, double bonusdamage)
        {
            double modif = 1;
            double magicdamage = 0;
            double bonusdamage2 = 0;
            double physDamage = unit.MinimumDamage + unit.BonusDamage;
            if (unit.Handle == _me.Handle)
            {
                var quellingBlade = unit.FindItem("item_quelling_blade");
                if (quellingBlade != null && minion.Team != unit.Team)
                {
                    if (unit.IsRanged)
                    {
                        physDamage = unit.MinimumDamage*1.15 + unit.BonusDamage;
                    }
                    else
                    {
                        physDamage = unit.MinimumDamage*1.4 + unit.BonusDamage;
                    }
                }
                switch (unit.ClassID)
                {
                    case ClassID.CDOTA_Unit_Hero_AntiMage:
                        if (minion.MaximumMana > 0 && minion.Mana > 0 && _q.Level > 0 && minion.Team != unit.Team)
                            bonusdamage2 = (_q.Level - 1)*12 + 28*0.6;
                        break;
                    case ClassID.CDOTA_Unit_Hero_Viper:
                        if (_w.Level > 0 && minion.Team != unit.Team)
                        {
                            var nethertoxindmg = _w.Level*2.5;
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
                        if (_e.Level > 0)
                        {
                            if (furymodif > 0)
                                bonusdamage2 = furymodif*((_e.Level - 1)*5 + 15);
                            else
                                bonusdamage2 = (_e.Level - 1)*5 + 15;
                        }
                        break;
                    case ClassID.CDOTA_Unit_Hero_BountyHunter:
                        if (_w.Level > 0 && _w.AbilityState == AbilityState.Ready)
                            bonusdamage2 = physDamage*((_w.Level - 1)*0.25 + 0.50);
                        break;
                    case ClassID.CDOTA_Unit_Hero_Weaver:
                        if (_e.Level > 0 && _e.AbilityState == AbilityState.Ready)
                            bonusdamage2 = physDamage;
                        break;
                    case ClassID.CDOTA_Unit_Hero_Kunkka:
                        if (_w.Level > 0 && _w.AbilityState == AbilityState.Ready && _w.IsAutoCastEnabled)
                            bonusdamage2 = (_w.Level - 1)*15 + 25;
                        break;
                    case ClassID.CDOTA_Unit_Hero_Juggernaut:
                        if (_e.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage2 = physDamage;
                        break;
                    case ClassID.CDOTA_Unit_Hero_Brewmaster:
                        if (_e.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage2 = physDamage;
                        break;
                    case ClassID.CDOTA_Unit_Hero_ChaosKnight:
                        if (_e.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage2 = physDamage*((_e.Level - 1)*0.5 + 0.25);
                        break;
                    case ClassID.CDOTA_Unit_Hero_SkeletonKing:
                        if (_e.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage2 = physDamage*((_e.Level - 1)*0.5 + 0.5);
                        break;
                    case ClassID.CDOTA_Unit_Hero_Life_Stealer:
                        if (_w.Level > 0)
                            bonusdamage2 = minion.Health*((_w.Level - 1)*0.01 + 0.045);
                        break;
                    case ClassID.CDOTA_Unit_Hero_PhantomAssassin:
                        if (_r.Level > 0)
                            if (unit.NetworkActivity == NetworkActivity.Crit)
                                bonusdamage2 = physDamage*((_r.Level - 1)*1.1 + 1.3);
                        break;
                }
                if (unit.Modifiers.Any(x => x.Name == "modifier_storm_spirit_overload"))
                {
                    magicdamage = magicdamage + (_e.Level - 1)*20 + 30;
                }
                if (unit.Modifiers.Any(x => x.Name == "modifier_chilling_touch"))
                {
                    magicdamage = magicdamage + (_e.Level - 1)*20 + 50;
                }
                if (unit.ClassID == ClassID.CDOTA_Unit_Hero_Pudge && _w.Level > 0 &&
                    Menu.Item("usespell").GetValue<bool>() &&
                    unit.Distance2D(minion) <= _attackRange)
                {
                    magicdamage = magicdamage + (_w.Level - 1)*6 + 6;
                }
            }
            if (unit.Modifiers.Any(x => x.Name == "modifier_bloodseeker_bloodrage"))
            {
                modif = modif*
                        (ObjectManager.GetEntities<Hero>()
                            .First(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker)
                            .Spellbook.Spell1.Level - 1)*0.05 + 1.25;
            }
            if (_summonsUnits!= null && _summonsUnits.Any(x => x.Handle != unit.Handle))
            {
                if (_creepTarget == null || (_creepTarget.Handle != _creepTargetS.Handle && _me.Handle != unit.Handle))
                {
                    foreach (var summonsUnit in _summonsUnits.Where(x => x.Handle != unit.Handle))
                    {
                        if (summonsUnit.CanAttack() && (Math.Abs(summonsUnit.Distance2D(minion) - summonsUnit.AttackRange) < 50 
                            || Math.Abs(summonsUnit.Distance2D(minion) - unit.Distance2D(minion)) < 50))
                            bonusdamage2 = bonusdamage2 + summonsUnit.MinimumDamage + summonsUnit.BonusDamage;
                    }
                }
                else if (_creepTarget.Handle == _creepTargetS.Handle && _me.Handle == unit.Handle)
                {
                    foreach (var summonsUnit in _summonsUnits)
                    {
                        if (summonsUnit.Distance2D(minion) < summonsUnit.AttackRange && summonsUnit.CanAttack())
                            bonusdamage2 = bonusdamage2 + summonsUnit.MinimumDamage + summonsUnit.BonusDamage;
                    }
                }
                else if (_creepTarget.Handle == _creepTargetS.Handle && _me.Handle != unit.Handle &&
                         _summonsUnits.Any(x => x.Handle != unit.Handle))
                {
                    foreach (var summonsUnit in _summonsUnits.Where(x => x.Handle != unit.Handle))
                    {
                        if (summonsUnit.Distance2D(minion) < summonsUnit.AttackRange && summonsUnit.CanAttack())
                            bonusdamage2 = bonusdamage2 + summonsUnit.MinimumDamage + summonsUnit.BonusDamage;
                    }
                    if (_me.Distance2D(minion) < _me.AttackRange && _me.CanAttack())
                        bonusdamage2 = bonusdamage2 + _me.MinimumDamage + _me.BonusDamage;
                }
            }

            bonusdamage = bonusdamage + bonusdamage2;
            var damageMp = 1 - 0.06*minion.Armor/(1 + 0.06*Math.Abs(minion.Armor));
            magicdamage = magicdamage*(1 - minion.MagicDamageResist);

            var realDamage = ((bonusdamage + physDamage)*damageMp + magicdamage)*modif;
            if (minion.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                minion.ClassID == ClassID.CDOTA_BaseNPC_Tower)
            {
                realDamage = realDamage/2;
            }
            if (realDamage > minion.MaximumHealth)
                realDamage = minion.Health + 10;
            
            return realDamage;
        }

        public static float GetProjectileSpeed(Entity unit)
        {
            return (_me.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
                ? 800
                : UnitDatabase.GetByName(unit.Name).ProjectileSpeed);
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
            //try
            //{
            //    var cpos = Drawing.WorldToScreen(_creepTarget.Position);
            //    var c2pos = Drawing.WorldToScreen(_creepTargetS.Position);
            //    var hpos = Drawing.WorldToScreen(_me.Position);
            //    Drawing.DrawLine(hpos, cpos, Color.Gold);
            //    Drawing.DrawLine(hpos, c2pos, Color.Red);
            //}
            //catch (Exception)
            //{
            //    //
            //}

            //try
            //{
            //    //Drawing.DrawText( + " ", "", new Vector2(_me.Position.X, _me.Position.Y),new Vector2(40), Color.AliceBlue, FontFlags.Outline);
            //    var pos = Drawing.WorldToScreen(Game.MousePosition);
            //    //Drawing.DrawText("   " + _me.Rotation , "", new Vector2(pos.X, pos.Y - 20), new Vector2(30), Color.White, FontFlags.Outline);
            //    var units = ObjectManager.GetEntities<Unit>().Where(x => x.Distance2D(_me) < 1000);
            //    //Color[] color = new Color[]();
            //    foreach (var unit in units)
            //    {
            //        var upos = Drawing.WorldToScreen(unit.Position);
            //        var creeps =
            //            ObjectManager.GetEntities<Unit>()
            //                .Where(x => x.Distance2D(unit) <= x.AttackRange + 50 && x.IsAttacking() && x.IsAlive && x.Handle != unit.Handle && x.Team != unit.Team)
            //                .ToList();
            //        foreach (var creep in creeps)
            //        {
            //            var angle = Utils.RadianToDegree(creep.RotationRad < 0 ? Math.Abs(creep.RotationRad) : Math.PI * 2 - creep.RotationRad);
            //            if (Math.Abs(creep.FindAngleForTurnTime(unit.Position) - angle) <= 3)
            //            {
            //                var cpos = Drawing.WorldToScreen(creep.Position);
            //                Drawing.DrawLine(upos, cpos, Color.Gainsboro);
            //                Drawing.DrawText(Attack(unit).ToString(), "", new Vector2(cpos.X - 40, cpos.Y - 20), new Vector2(30), Color.White, FontFlags.Outline);
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    //
            //}
        }
    }
}