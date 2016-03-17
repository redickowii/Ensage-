using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using Ensage.Common.Menu;

namespace NagaSharp
{

    public class JungleCamps
    {
        public Hero Illusion { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 StackPosition { get; set; }
        public Vector3 WaitPosition { get; set; }
        public int Team { get; set; }
        public int Id { get; set; }
        public bool Farming { get; set; }
        public int LvlReq { get; set; }
        public bool Visible { get; set; }
        public int VisTime { get; set; }
        public bool Stacking { get; set; }
        public bool Stacked { get; set; }
        public bool Ancients { get; set; }
        public bool Empty { get; set; }
        public int State { get; set; }
        public int AttackTime { get; set; }
        public int Creepscount { get; set; }
        public int Starttime { get; set; }
    }

    public class CreepWaves
    {
        public Hero Illusion { get; set; }
        public List<Unit> Creeps { get; set; }
        public Vector3 Position { get; set; }
    }

    internal class Program
    {
        
        private static Ability Q, E;
        private static Item radiance, manta, travels, octa;
        private static Hero _me;
        private static Unit _closestNeutral, _clos7EstCreep;
        private static readonly Menu Menu = new Menu("Naga", "Naga", true, "npc_dota_hero_naga_siren", true);
        private static bool _isloaded;
        private static AbilityToggler _menuValue;
        private static int _creepscount;
        private static Vector3 _stackPosition;

        private static List<CreepWaves> _creepWaves = new List<CreepWaves>();
        private static readonly List<JungleCamps> JungleCamps = new List<JungleCamps>();

        private static List<Unit> _neutrals;
        private static List<Hero> _illusions;

        private static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;

            var dict = new Dictionary<string, bool>
            {
                { "naga_siren_mirror_image", true },
                { "naga_siren_rip_tide", true }
            };

            #region array

            _creepWaves.Add(new CreepWaves
            {
                Illusion = null,
                Creeps = new List<Unit>()
            });
            _creepWaves.Add(new CreepWaves
            {
                Illusion = null,
                Creeps = new List<Unit>()
            });
            _creepWaves.Add(new CreepWaves
            {
                Illusion = null,
                Creeps = new List<Unit>()
            });

            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-1708, -4284, 256),
                StackPosition = new Vector3(-1816, -2684, 256),
                WaitPosition = new Vector3(-1867, -4033, 256),
                Team = 2,
                Id = 1,
                Empty = false,
                Stacked = false,
                Starttime = 55
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-266, -3176, 256),
                StackPosition = new Vector3(-522, -1351, 256),
                WaitPosition = new Vector3(-306, -2853, 256),
                Team = 2,
                Id = 2,
                Empty = false,
                Stacked = false,
                Starttime = 55
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(1656, -3714, 384),
                StackPosition = new Vector3(48, -4225, 384),
                WaitPosition = new Vector3(1637, -4009, 384),
                Team = 2,
                Id = 3,
                Empty = false,
                Stacked = false,
                Starttime = 54
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(3016, -4692, 384),
                StackPosition = new Vector3(3952, -6417, 384),
                WaitPosition = new Vector3(3146, -5071, 384),
                Team = 2,
                Id = 4,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(4474, -3598, 384),
                StackPosition = new Vector3(2486, -4125, 384),
                WaitPosition = new Vector3(4121, -3902, 384),
                Team = 2,
                Id = 5,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-3617, 805, 384),
                StackPosition = new Vector3(-5268, 1400, 384),
                WaitPosition = new Vector3(-3835, 643, 384),
                Team = 2,
                Id = 6,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-4446, 3541, 384),
                StackPosition = new Vector3(-3953, 4954, 384),
                WaitPosition = new Vector3(-4251, 3760, 384),
                Team = 3,
                Id = 7,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-2981, 4591, 384),
                StackPosition = new Vector3(-3248, 5993, 384),
                WaitPosition = new Vector3(-3050, 4897, 384),
                Team = 3,
                Id = 8,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-392, 3652, 384),
                StackPosition = new Vector3(-224, 5088, 384),
                WaitPosition = new Vector3(-503, 3955, 384),
                Team = 3,
                Id = 9,
                Empty = false,
                Stacked = false,
                Starttime = 55
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-1524, 2641, 256),
                StackPosition = new Vector3(-1266, 4273, 384),
                WaitPosition = new Vector3(-1465, 2908, 256),
                Team = 3,
                Id = 10,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(1098, 3338, 384),
                StackPosition = new Vector3(910, 5003, 384),
                WaitPosition = new Vector3(975, 3586, 384),
                Team = 3,
                Id = 11,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(4141, 554, 384),
                StackPosition = new Vector3(2987, -2, 384),
                WaitPosition = new Vector3(3876, 506, 384),
                Team = 3,
                Id = 12,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(-2960, -126, 384),
                StackPosition = new Vector3(-3850, -1491, 384),
                WaitPosition = new Vector3(-2777, -230, 384),
                Team = 2,
                Id = 13,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            JungleCamps.Add(new JungleCamps
            {
                Position = new Vector3(4000, -700, 256),
                StackPosition = new Vector3(1713, -134, 256),
                WaitPosition = new Vector3(3649, -721, 256),
                Team = 3,
                Id = 14,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
       #endregion

            Menu.AddItem(new MenuItem("enabledAbilities", "Abilities:").SetValue(new AbilityToggler(dict)));
            Menu.AddItem(new MenuItem("Stack", "Stack").SetValue(new KeyBind('F', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("LinePush", "Line Push").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("FarmJ", "Jungle Farm").SetValue(new KeyBind('H', KeyBindType.Toggle)));
            Menu.AddToMainMenu();
        
        }
        public static void Game_OnUpdate(EventArgs args)
        {
            if (!_isloaded)
            {
                _me = ObjectManager.LocalHero;
                if (!Game.IsInGame || _me == null)
                {
                    return;
                }
                _isloaded = true;
            }

            if (_me == null || !_me.IsValid)
            {
                _isloaded = false;
                _me = ObjectManager.LocalHero;
                return;
            }


            if (_me.ClassID != ClassID.CDOTA_Unit_Hero_Naga_Siren || Game.IsPaused || Game.IsChatOpen)
            {
                return;
            }

            var stackKey = Menu.Item("Stack").GetValue<KeyBind>().Active;
            var linePush = Menu.Item("LinePush").GetValue<KeyBind>().Active;
            var farmJ = Menu.Item("FarmJ").GetValue<KeyBind>().Active;
            _menuValue = Menu.Item("enabledAbilities").GetValue<AbilityToggler>();
            Q = _me.Spellbook.Spell1;
            E = _me.Spellbook.Spell3;
            var eRadius = E.GetCastRange() - 30;
            var movementspeed = _me.MovementSpeed;
        
            radiance = _me.FindItem("item_radiance");
            manta = _me.FindItem("item_manta");
            travels = _me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_travel_boots"));
            octa = _me.FindItem("item_octarine_core");

            _illusions =
                ObjectManager.GetEntities<Hero>()
                    .Where(
                        x =>
                            x.ClassID == ClassID.CDOTA_Unit_Hero_Naga_Siren && x.IsIllusion && x.IsVisible && x.IsAlive &&
                            x.Team == _me.Team)
                    .ToList();
            
            var seconds = ((int)Game.GameTime) % 60;
            if (JungleCamps.FindAll(x => x.Illusion != null).Count != _illusions.Count || seconds == 1)
                {
                foreach (var camp in JungleCamps)
                {
                    camp.Illusion = null;
                    camp.Stacking = false;
                    camp.Farming = false;
                    camp.State = 0;
                }
            }
            if (seconds == 0)
            {
                foreach (var camp in JungleCamps)
                {
                    camp.Illusion = null;
                    camp.Stacking = false;
                    camp.Farming = false;
                    camp.Empty = false;
                    camp.State = 0;
                }
            }
            #region linepush

            if (linePush && Utils.SleepCheck("linePush"))
            {
                    var creeps =
                    ObjectManager.GetEntities<Creep>()
                        .Where(
                            x =>
                                x.IsAlive && x.IsVisible && x.Team == _me.GetEnemyTeam() && (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege))
                                .OrderByDescending(x => x.Distance2D(new Vector3(0,0,0))).ToList();
                try
                {
                    var creepdel = new List<Unit>();
                    foreach (var creepWave in _creepWaves)
                    {
                        creepdel.AddRange(creepWave.Creeps.Where(creep => creeps.All(x => x.Handle != creep.Handle)));
                        foreach (var creep in creepdel)
                            creepWave.Creeps.Remove(creep);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erroe LinePush 1" + e);
                }
                try
                {
                    foreach (var creepWave in _creepWaves)
                    {
                        if (creepWave.Creeps.Count == 0)
                        {
                            creepWave.Position = creeps.First().Position;
                            creepWave.Creeps.Add(creeps.First());
                            creeps.Remove(creeps.First());
                        }
                        var creepdel = new List<Creep>();
                        foreach (var creep in creeps)
                        {
                            if (creepWave.Creeps.Any(x => x.Handle == creep.Handle))
                            {
                                creepdel.Add(creep);
                            }
                            if (!(creepWave.Position.Distance2D(creep) < 2000) ||
                                creepWave.Creeps.Any(x => x.Handle == creep.Handle)) continue;
                            creepWave.Creeps.Add(creep);
                            creepdel.Add(creep);
                        }
                        foreach (var creep in creepdel)
                            creeps.Remove(creep);
                    }
                    foreach (var creepWave in _creepWaves)
                    {
                        creepWave.Position = new Vector3(
                            creepWave.Creeps.Average(x => x.Position.X),
                            creepWave.Creeps.Average(x => x.Position.Y),
                            creepWave.Creeps.Average(x => x.Position.Z));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erroe LinePush 2" + e);
                }
                //try
                //{
                //    if (Utils.SleepCheck("Print"))
                //    {
                //        foreach (var creepWave in _creepWaves)
                //        {
                //            Console.WriteLine("illusion {0} , Position {1} , Count {2}", creepWave.Illusion, creepWave.Position, creepWave.Creeps.Count);
                //            foreach (var creep in creepWave.Creeps)
                //                Console.WriteLine(creep.Handle);
                //        }
                //        Utils.Sleep(2000, "Print");
                //    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine("Erroe LinePush 3" + e);
                //}

                if (_illusions.Count > 0)
                {
                    foreach (var creepWave in _creepWaves.Where(creepWave => _illusions.Count > 0))
                    {
                        if (creepWave.Illusion == null)
                        {
                            creepWave.Illusion = _illusions.First();
                            creepWave.Illusion.Move(creepWave.Position);
                            _illusions.Remove(_illusions.First());
                        }
                        else if (_illusions.Any(x => x.Handle == creepWave.Illusion.Handle))
                        {
                            if (!creepWave.Illusion.IsAttacking() && creepWave.Creeps.Count > 0)
                            {
                                creepWave.Illusion.Attack(GetClosestCreep(_illusions.First(), creepWave.Creeps));
                            }
                            else
                            {
                                creepWave.Illusion.Move(GetClosestCreep(creepWave.Illusion).Position);
                            }
                        }
                        else if (_illusions.All(x => x.Handle != creepWave.Illusion.Handle))
                        {
                            creepWave.Illusion = null;
                        }
                    }
                }
                Utils.Sleep(1000, "linePush");
            }
            #endregion

            #region Stack

            else if (stackKey && _illusions.Count > 0 && Utils.SleepCheck("wait"))
            {
                foreach (var illusion in _illusions)
                {
                    if (!Check(illusion))
                    {
                        JungleCamps.Find(x => x.Id == GetClosestCamp(illusion, false, false).Id).Illusion = illusion;
                        JungleCamps.Find(x => x.Id == GetClosestCamp(illusion, false, false).Id).Stacking = true;
                    }
                    else
                    {
                        var illusionCamps = Checkillusion(illusion);
                        switch (illusionCamps.State)
                        {
                            case 0:
                                if (illusion.Distance2D(illusionCamps.WaitPosition) < 5)
                                    illusionCamps.State = 1;
                                else
                                    illusion.Move(illusionCamps.WaitPosition);
                                Utils.Sleep(500, "wait");
                                break;
                            case 1:
                                _creepscount = CreepCount(illusionCamps.Illusion, 800);
                                if (_creepscount == 0)
                                {
                                    JungleCamps.Find(x => x.Id == illusionCamps.Id).Illusion = null;
                                    JungleCamps.Find(x => x.Id == illusionCamps.Id).Empty = true;
                                    JungleCamps.Find(x => x.Id == illusionCamps.Id).Stacking = false;
                                    JungleCamps.Find(x => x.Id == GetClosestCamp(illusion, false, false).Id).Illusion =
                                        illusion;
                                    JungleCamps.Find(x => x.Id == GetClosestCamp(illusion, false, false).Id).Stacking =
                                        true;
                                }
                                else if (seconds >= illusionCamps.Starttime - 5)
                                {
                                    _closestNeutral = GetNearestCreepToPull(illusionCamps.Illusion, 800);
                                    _stackPosition = illusionCamps.StackPosition;
                                    var moveTime = illusionCamps.Starttime -
                                                   (GetDistance2D(illusionCamps.Illusion.Position,
                                                       _closestNeutral.Position) +
                                                    (_closestNeutral.IsRanged
                                                        ? _closestNeutral.AttackRange
                                                        : _closestNeutral.RingRadius))/movementspeed;
                                    illusionCamps.AttackTime = (int) moveTime;
                                    illusionCamps.State = 2;
                                }
                                Utils.Sleep(500, "wait");
                                break;
                            case 2:
                                if (seconds >= illusionCamps.AttackTime)
                                {
                                    _closestNeutral = GetNearestCreepToPull(illusionCamps.Illusion, 1200);
                                    _stackPosition = GetClosestCamp(illusionCamps.Illusion, false, false).StackPosition;
                                    illusionCamps.Illusion.Attack(_closestNeutral);
                                    illusionCamps.State = 3;
                                    var tWait =
                                        (int)
                                            (((GetDistance2D(illusionCamps.Illusion.Position, _closestNeutral.Position))/
                                              movementspeed)*1000 + Game.Ping);
                                    Utils.Sleep(tWait, "" + illusionCamps.Illusion.Handle);
                                }
                                break;
                            case 3:
                                if (Utils.SleepCheck("" + illusionCamps.Illusion.Handle))
                                {
                                    if (_menuValue.IsEnabled(E.Name) && E.CanBeCasted() &&
                                        Creepcountall(eRadius) > Creepcountall(600)/2)
                                        E.UseAbility();
                                    illusionCamps.Illusion.Move(illusionCamps.StackPosition);
                                    illusionCamps.State = 4;
                                }
                                break;
                            case 4:
                                illusion.Move(illusionCamps.StackPosition);
                                Utils.Sleep(1000, "wait");
                                break;
                            default:
                                illusionCamps.State = 0;
                                break;
                        }
                    }
                }

            }
            #endregion Stack or farm

            #region Farm

            else if (farmJ && _illusions.Count > 0 && Utils.SleepCheck("farm"))
            {
                foreach (var illusion in _illusions)
                {
                    if (!Check(illusion))
                    {
                        JungleCamps.Find(x => x.Id == GetClosestCamp(illusion, false, false).Id).Illusion = illusion;
                        JungleCamps.Find(x => x.Id == GetClosestCamp(illusion, false, false).Id).Farming = true;
                    }
                    else
                    {
                        var illusionCamps = Checkillusion(illusion);
                        if (illusion.Distance2D(illusionCamps.Position) > 100)
                        {
                            illusion.Move(illusionCamps.Position);
                        }
                        else
                        {
                            if (E.CanBeCasted() && _menuValue.IsEnabled(E.Name) &&
                                Creepcountall(eRadius) >= Creepcountall(600)/2)
                                E.UseAbility();
                            illusion.Attack(GetNearestCreepToPull(illusionCamps.Illusion, 500));
                        }
                    }
                }
                Utils.Sleep(1000, "farm");
            }

            #endregion Farm
        }

        public static void Game_OnWndProc(WndEventArgs args)
        {
            if (Game.IsChatOpen) return;
            if (Game.IsKeyDown(Menu.Item("Stack").GetValue<KeyBind>().Key))
            {
                Menu.Item("LinePush").SetValue(new KeyBind(Menu.Item("LinePush").GetValue<KeyBind>().Key, KeyBindType.Toggle));
                Menu.Item("FarmJ").SetValue(new KeyBind(Menu.Item("FarmJ").GetValue<KeyBind>().Key, KeyBindType.Toggle));
            }
            if (Game.IsKeyDown(Menu.Item("LinePush").GetValue<KeyBind>().Key))
            {
                Menu.Item("Stack").SetValue(new KeyBind(Menu.Item("Stack").GetValue<KeyBind>().Key, KeyBindType.Toggle));
                Menu.Item("FarmJ").SetValue(new KeyBind(Menu.Item("FarmJ").GetValue<KeyBind>().Key, KeyBindType.Toggle));
            }
            if (Game.IsKeyDown(Menu.Item("FarmJ").GetValue<KeyBind>().Key))
            {
                Menu.Item("Stack").SetValue(new KeyBind(Menu.Item("Stack").GetValue<KeyBind>().Key, KeyBindType.Toggle));
                Menu.Item("LinePush").SetValue(new KeyBind(Menu.Item("LinePush").GetValue<KeyBind>().Key, KeyBindType.Toggle));
            }
        }

        private static JungleCamps GetClosestCamp(Hero illusion,bool stack, bool any)
        {
            JungleCamps[] closest =
            {
                new JungleCamps {WaitPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), Id = 0}
            };
            foreach (var x in JungleCamps.Where(x => illusion.Distance2D(x.WaitPosition) < illusion.Distance2D(closest[0].WaitPosition) && !x.Farming && !x.Stacking && !x.Empty))
            {
                closest[0] = x;
            }
            return closest[0];
        }

        private static JungleCamps Checkillusion(Hero illu)
        {
            var a = new JungleCamps();
            return JungleCamps.Where(x => x.Illusion != null).Aggregate(a, (current, x) => (x.Illusion.Handle == illu.Handle ? x : current));
        }

        private static bool Check(Hero illu)
        {
            return JungleCamps.Where(x => x.Illusion != null).Aggregate(false, (current, x) => (x.Illusion.Handle == illu.Handle || current));
        }

        private static int Creepcountall(float radius)
        {
            var a = 0;
            foreach (var illusoin in _illusions)
            {
                _neutrals = ObjectManager.GetEntities<Unit>()
                        .Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && illusoin.Distance2D(x) <= radius)
                        .ToList();
                a = a + _neutrals.Count;
            }
            return a;   
        }

        private static int CreepCount(Unit h, float radius)
        {
            try
            {
                return
                    ObjectManager.GetEntities<Unit>()
                        .Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && h.Distance2D(x) <= radius)
                        .ToList().Count;
            }
            catch (Exception)
            {
                //
            }
            return 0;
        }

        private static Unit GetClosestCreep(Entity hero)
        {
            Unit closest = null;
            float[] distance = { float.MaxValue };
            var creeps = ObjectManager.GetEntities<Unit>().Where(x => x.Team == _me.Team && x.IsAlive && hero.Distance2D(x) <= 2000);
            foreach (var creep in creeps.Where(creep => distance[0] > hero.Distance2D(creep.Position)))
            {
                distance[0] = hero.Distance2D(creep.Position);
                closest = creep;
            }
            return closest;
        }

        private static Unit GetClosestCreep(Entity hero, IEnumerable<Unit> creeps)
        {
            float[] distance = {float.MaxValue};
            Unit closest = null;
            foreach (var creep in creeps.Where(creep => distance[0] > hero.Distance2D(creep.Position)))
            {
                distance[0] = hero.Distance2D(creep.Position);
                closest = creep;
            }
            return closest;
        }

        private static Unit GetNearestCreepToPull(Hero illusion, int dis)
        {
            var creeps =
                ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive && x.IsSpawned && x.IsVisible && illusion.Distance2D(x) <= dis && x.Team != _me.Team).ToList();
            Unit bestCreep = null;
            var bestDistance = float.MaxValue;
            foreach (var creep in creeps)
            {
                var distance = GetDistance2DFast(illusion, creep);
                if (bestCreep == null || distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCreep = creep;
                }

            }
            return bestCreep;
        }

        private static float GetDistance2DFast(Entity e1, Entity e2)
        {
            return (float)(Math.Pow(e1.Position.X - e2.Position.X, 2) + Math.Pow(e1.Position.Y - e2.Position.Y, 2));
        }

        private static float GetDistance2D(Vector3 p1, Vector3 p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

    }
}
