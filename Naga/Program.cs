using System;
using System.Linq;
using System.Collections.Generic;
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
        public Hero illusion { get; set; }
        public Vector3 position { get; set; }
        public Vector3 stackPosition { get; set; }
        public Vector3 waitPosition { get; set; }
        public int team { get; set; }
        public int id { get; set; }
        public bool farming { get; set; }
        public int lvlReq { get; set; }
        public bool visible { get; set; }
        public int visTime { get; set; }
        public bool stacking { get; set; }
        public int stacked { get; set; }
        public bool ancients { get; set; }
        public bool empty { get; set; }
        public int State { get; set; }
        public int AttackTime { get; set; }
        public int creepscount { get; set; }
        public int starttime { get; set; }
    }

    internal class Program
    {
        /*JungleCamps,reg,stack,farm,auto,enemyjungle,*/
        private static Ability Q, E;
        private static Item radiance, manta, travels, octa;
        private static Hero me;
        private static Unit closestNeutral, closestCreep;
        private static Player player;
        private static readonly Menu Menu = new Menu("Naga", "Naga", true, "npc_dota_hero_naga_siren", true);
        private static bool toggle = true, isloaded;
        private static bool active, cr, noCreep, newVal;
        private static AbilityToggler menuValue;
        private static int creepscount;
        private static Vector3 stackPosition;
        private static Vector3[] mid;
        private static Vector3[] top;
        private static Vector3[] bot;

        private static Key KeyControl = Key.K;

        private static List<JungleCamps> JungleCamps = new List<JungleCamps>();

        private static List<Unit> neutrals;
        private static List<Hero> illusions;

        private static readonly Vector3[] Mid =
{
                new Vector3(-5589, -5098, 261),
                new Vector3(-4027, -3532, 137),
                new Vector3(-2470, -1960, 127),
                new Vector3(-891, -425, 55),
                new Vector3(1002, 703, 127),
                new Vector3(2627, 2193, 127),
                new Vector3(4382, 3601, 2562)};
        private static readonly Vector3[] Bot =
        {
                new Vector3 (-4077, -6160, 268),
                new Vector3 (-1875, -6125, 127),
                new Vector3 (325, -6217, 256),
                new Vector3 (2532, -6215, 256),
                new Vector3(5197, -5968, 384),
                new Vector3 (6090, -4318, 256),
                new Vector3 (6180, -2117, 256),
                new Vector3 (6242, 84, 256),
                new Vector3(6307, 2286, 141),
                new Vector3(6254, 3680, 256)};
        private static readonly Vector3[] Top =
        {
                new Vector3(-6400, -793, 256),
                new Vector3(-6356, 1141, 256),
                new Vector3(-6320, 3762, 256),
                new Vector3(-5300, 5924, 256),
                new Vector3(-3104, 5929, 256),
                new Vector3(-826, 5942, 256),
                new Vector3(1445, 5807, 256),
                new Vector3(3473, 5949, 256),
                new Vector3(-6506, -4701, 384)};

        private static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;

            var dict = new Dictionary<string, bool>
            {
                { "naga_siren_mirror_image", true },
                { "naga_siren_rip_tide", true }
            };

            JungleCamps.Add(new JungleCamps { position = new Vector3(-1708, -4284, 256), stackPosition = new Vector3(-2776, -3144, 256), waitPosition = new Vector3(-1971, -3949, 256), team = 2, id = 1, farming = false, empty = false, lvlReq = 8, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 55 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(-266, -3176, 256), stackPosition = new Vector3(-522, -1351, 256), waitPosition = new Vector3(-325, -2699, 256), team = 2, id = 2, farming = false, empty = false, lvlReq = 3, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 55 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(1656, -3714, 384), stackPosition = new Vector3(1263, -6041, 384), waitPosition = new Vector3(1612, -4277, 384), team = 2, id = 3, farming = false, empty = false, lvlReq = 8, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(3016, -4692, 384), stackPosition = new Vector3(4777, -4954, 384), waitPosition = new Vector3(3074, -4955, 384), team = 2, id = 4, farming = false, empty = false, lvlReq = 3, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(4474, -3598, 384), stackPosition = new Vector3(2755, -4001, 384), waitPosition = new Vector3(4121, -3902, 384), team = 2, id = 5, farming = false, empty = false, lvlReq = 1, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(-3617, 805, 384), stackPosition = new Vector3(-5268, 1400, 384), waitPosition = new Vector3(-3835, 643, 384), team = 2, id = 6, farming = false, empty = false, lvlReq = 12, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(-4446, 3541, 384), stackPosition = new Vector3(-3953, 4954, 384), waitPosition = new Vector3(-4251, 3760, 384), team = 3, id = 7, farming = false, empty = false, lvlReq = 8, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(-2981, 4591, 384), stackPosition = new Vector3(-3248, 5993, 384), waitPosition = new Vector3(-3055, 4837, 384), team = 3, id = 8, farming = false, empty = false, lvlReq = 3, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(-392, 3652, 384), stackPosition = new Vector3(-224, 5088, 384), waitPosition = new Vector3(-503, 3955, 384), team = 3, id = 9, farming = false, empty = false, lvlReq = 3, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 55 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(-1524, 2641, 256), stackPosition = new Vector3(-1266, 4273, 384), waitPosition = new Vector3(-1465, 2908, 256), team = 3, id = 10, farming = false, empty = false, lvlReq = 1, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(1098, 3338, 384), stackPosition = new Vector3(-1266, 4273, 384), waitPosition = new Vector3(975, 3586, 384), team = 3, id = 11, farming = false, empty = false, lvlReq = 8, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });
            JungleCamps.Add(new JungleCamps { position = new Vector3(4141, 554, 384), stackPosition = new Vector3(2987, -2, 384), waitPosition = new Vector3(3876, 506, 384), team = 3, id = 12, farming = false, empty = false, lvlReq = 12, visible = false, visTime = 0, stacking = false, stacked = 0, starttime = 53 });

            JungleCamps.Add(new JungleCamps { position = new Vector3(9999, 9999, 9999), id = 13 }); 

            Menu.AddItem(new MenuItem("enabledAbilities", "Abilities:").SetValue(new AbilityToggler(dict)));
            Menu.AddItem(new MenuItem("Stack", "Stack").SetValue(new KeyBind('F', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("LinePush", "Line Push").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("FarmJ", "Jungle Farm").SetValue(new KeyBind('H', KeyBindType.Toggle)));
            Menu.AddToMainMenu(); 

        }
        public static void Game_OnUpdate(EventArgs args)
        {
            if (!isloaded)
            {
                me = ObjectManager.LocalHero;
                if (!Game.IsInGame || me == null)
                {
                    return;
                }
                isloaded = true;
            }

            if (me == null || !me.IsValid)
            {
                isloaded = false;
                me = ObjectManager.LocalHero;
                return;
            }


            if (me.ClassID != ClassID.CDOTA_Unit_Hero_Naga_Siren || Game.IsPaused || Game.IsChatOpen)
            {
                return;
            }

            var StackKey = Menu.Item("Stack").GetValue<KeyBind>().Active;
            var LinePush = Menu.Item("LinePush").GetValue<KeyBind>().Active;
            var FarmJ = Menu.Item("FarmJ").GetValue<KeyBind>().Active;
            menuValue = Menu.Item("enabledAbilities").GetValue<AbilityToggler>();
            Q = me.Spellbook.Spell1;
            E = me.Spellbook.Spell3;
            var ERadius = E.GetCastRange() - 30;
            var movementspeed = me.MovementSpeed;
        
            radiance = me.FindItem("item_radiance");
            manta = me.FindItem("item_manta");
            travels = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_travel_boots"));
            octa = me.FindItem("item_octarine_core");

            var enemyTeam = me.GetEnemyTeam();
            var myteam = me.Team;
            /*
            var enemy = ObjectManager.GetEntities<Unit>().Where(x => x.Team == Team.Neutral || x.Team == enemyTeam && !x.IsIllusion && x.IsSpawned && x.IsVisible).ToList();
            var allies = ObjectManager.GetEntities<Hero>().Where(x => x.IsAlive && !x.IsIllusion && x.IsVisible && x.Team == me.Team).ToList();
            var enemies = ObjectManager.GetEntities<Hero>().Where(x => x.IsAlive && !x.IsIllusion && x.IsVisible && x.Team == enemyTeam).ToList();
            */
            illusions = ObjectManager.GetEntities<Hero>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Naga_Siren && x.IsIllusion && x.IsVisible && x.IsAlive && x.Team == myteam).ToList();
            
            var seconds = ((int)Game.GameTime) % 60;
            if (JungleCamps.FindAll(x => x.illusion != null).Count != illusions.Count || seconds == 1)
                {
                foreach (var illusion in JungleCamps)
                {
                    illusion.illusion = null;
                    illusion.stacking = false;
                    illusion.farming = false;
                    illusion.State = 0;
                }
            }
            #region linepush
            //--------------------------Line push------------------------
            if (illusions.Count > 0 && LinePush)
            {
                mid = Mid;
                bot = Bot;
                top = Top;

                    if (me.Team == myteam)
                {
                    foreach (Hero v in illusions)
                    {
                        if (v.Distance2D(me) > 700)
                        {
                            /******************************************MID******************************************/
                            if (v.Distance2D(mid[0]) <= 2300 && v.Distance2D(mid[1]) >= 2190 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[1]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[1]) <= 2400 && v.Distance2D(mid[2]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[2]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[2]) <= 2640 && v.Distance2D(mid[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[3]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[3]) <= 2340 && v.Distance2D(mid[4]) >= 1900 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[4]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[4]) <= 2400 && v.Distance2D(mid[5]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[5]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[5]) <= 2400 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[6]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }

                            /******************************************MID******************************************/
                            /******************************************BOT******************************************/


                            if (v.Distance2D(bot[0]) <= 2400 && v.Distance2D(bot[1]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[1]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[1]) <= 2440 && v.Distance2D(bot[2]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[2]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[2]) <= 2640 && v.Distance2D(bot[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[3]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[3]) <= 2340 && v.Distance2D(bot[4]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[4]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[4]) <= 2380 && v.Distance2D(bot[5]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[5]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[5]) <= 2380 && v.Distance2D(bot[6]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[6]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[6]) <= 2630 && v.Distance2D(bot[7]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[7]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[7]) <= 2630 && v.Distance2D(bot[8]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[8]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[8]) <= 2630 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[9]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            /******************************************BOT******************************************/
                            /******************************************TOP******************************************/
                            if (v.Distance2D(top[0]) <= 1900 && v.Distance2D(top[1]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[1]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[1]) <= 2640 && v.Distance2D(top[2]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[2]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[2]) <= 2640 && v.Distance2D(top[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[3]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[3]) <= 2340 && v.Distance2D(top[4]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[4]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[4]) <= 2380 && v.Distance2D(top[5]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[5]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[5]) <= 2380 && v.Distance2D(top[6]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[6]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[6]) <= 2630 && v.Distance2D(top[7]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[7]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[7]) <= 2630 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[8]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }


                            /******************************************TOP******************************************/

                        }
                    }
                }
                if (me.Team == enemyTeam)
                {
                    foreach (Hero v in illusions)
                    {
                        if (v.Distance2D(me) > 700)
                        {
                            /******************************************MID******************************************/
                            if (v.Distance2D(mid[6]) <= 2300 && v.Distance2D(mid[5]) >= 2190 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[5]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[5]) <= 2400 && v.Distance2D(mid[4]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[4]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[4]) <= 2640 && v.Distance2D(mid[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[3]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[3]) <= 2340 && v.Distance2D(mid[2]) >= 1900 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[2]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[2]) <= 2400 && v.Distance2D(mid[1]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[1]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(mid[1]) <= 2400 && v.Distance2D(mid[2]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(mid[0]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }

                            /******************************************MID******************************************/
                            /******************************************BOT******************************************/


                            if (v.Distance2D(bot[9]) <= 2400 && v.Distance2D(bot[8]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[8]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[8]) <= 2440 && v.Distance2D(bot[7]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[7]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[7]) <= 2640 && v.Distance2D(bot[6]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[6]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[6]) <= 2340 && v.Distance2D(bot[5]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[5]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[5]) <= 2380 && v.Distance2D(bot[4]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[4]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[4]) <= 500 && v.Distance2D(bot[3]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[3]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[3]) <= 2630 && v.Distance2D(bot[2]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[2]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[2]) <= 2630 && v.Distance2D(bot[1]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[1]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(bot[1]) <= 2630 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(bot[0]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            /******************************************BOT******************************************/
                            /******************************************TOP******************************************/
                            if (v.Distance2D(top[7]) <= 1900 && v.Distance2D(top[6]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[6]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[6]) <= 2640 && v.Distance2D(top[5]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[5]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[5]) <= 2640 && v.Distance2D(top[4]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[4]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[4]) <= 2340 && v.Distance2D(top[3]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[3]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[3]) <= 2380 && v.Distance2D(top[2]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[2]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[2]) <= 2600 && v.Distance2D(top[1]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[1]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }
                            if (v.Distance2D(top[1]) <= 3500 && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(top[8]);
                                Utils.Sleep(3700, v.Handle.ToString());
                            }


                            /******************************************TOP******************************************/

                        }
                    }
                }
            }
                #endregion linepush

            #region Stack
            else if (StackKey && illusions.Count > 0 && Utils.SleepCheck("wait"))
            {
                foreach (var illusion in illusions)
                {
                    if (!Check(illusion))
                    {
                        JungleCamps.Find(x => x.id == GetClosestCamp(illusion, false, false).id).illusion = illusion;
                        JungleCamps.Find(x => x.id == GetClosestCamp(illusion, false, false).id).stacking = true;
                    }
                    else
                    {
                        var illusionCamps = Checkillusion(illusion);
                        switch (illusionCamps.State)
                        {
                            case 0:
                                if (illusion.Distance2D(illusionCamps.waitPosition) < 5)
                                    illusionCamps.State = 1;
                                else
                                    illusion.Move(illusionCamps.waitPosition);
                                Utils.Sleep(500, "wait");
                                break;
                            case 1:
                                creepscount = Creepcount(illusionCamps.illusion, 800);
                                if (creepscount == 0)
                                {
                                    JungleCamps.Find(x => x.id == illusionCamps.id).illusion = null;
                                    JungleCamps.Find(x => x.id == illusionCamps.id).empty = true;
                                    JungleCamps.Find(x => x.id == illusionCamps.id).stacking = false;
                                    JungleCamps.Find(x => x.id == GetClosestCamp(illusion, false, false).id).illusion = illusion;
                                    JungleCamps.Find(x => x.id == GetClosestCamp(illusion, false, false).id).stacking = true;
                                    Game.PrintMessage("WARNING",MessageType.LogMessage);
                                }
                                else if (seconds >= illusionCamps.starttime - 2)
                                {
                                    closestNeutral = GetNearestCreepToPull(illusionCamps.illusion, 800);
                                    stackPosition = illusionCamps.stackPosition;
                                    var stackDuration = Math.Min((GetDistance2D(closestNeutral.Position, illusionCamps.stackPosition) + (creepscount * 45))/Math.Min(closestNeutral.MovementSpeed, movementspeed),9);
                                    if (closestNeutral.IsRanged && creepscount <= 4)
                                        stackDuration = Math.Min((GetDistance2D(closestNeutral.Position, illusionCamps.stackPosition) + closestNeutral.AttackRange + (creepscount * 45))/Math.Min(closestNeutral.MovementSpeed, movementspeed), 9);
                                    var moveTime = illusionCamps.starttime -(GetDistance2D(illusionCamps.illusion.Position,closestNeutral.Position) + 50)/movementspeed;
                                    if (stackDuration > 0)
                                        moveTime = illusionCamps.starttime - stackDuration - (GetDistance2D(illusionCamps.illusion.Position,closestNeutral.Position) + closestNeutral.RingRadius)/Math.Min(closestNeutral.MovementSpeed, movementspeed);
                                    //Game.PrintMessage(illusionCamps.starttime + " - " + stackDuration + " - " +(GetDistance2D(illusionCamps.illusion.Position,closestNeutral.Position) + closestNeutral.RingRadius)/Math.Min(closestNeutral.MovementSpeed, movementspeed), MessageType.LogMessage);
                                    illusionCamps.AttackTime = (int) moveTime;
                                    illusionCamps.State = 2;
                                }
                                Utils.Sleep(500, "wait");
                                break;
                            case 2:
                                if (seconds >= illusionCamps.AttackTime)
                                {
                                    closestNeutral = GetNearestCreepToPull(illusionCamps.illusion, 1200);
                                    stackPosition = GetClosestCamp(illusionCamps.illusion, false, false).stackPosition;
                                    illusionCamps.illusion.Attack(closestNeutral);
                                    illusionCamps.State = 3;
                                    var tWait =(int)(((GetDistance2D(illusionCamps.illusion.Position,closestNeutral.Position))/movementspeed)*1000 + Game.Ping);
                                    Utils.Sleep(tWait, "" + illusionCamps.illusion.Handle);
                                }
                                break;
                            case 3:
                                if (Utils.SleepCheck("" + illusionCamps.illusion.Handle))
                                {
                                    if (menuValue.IsEnabled(E.Name) && E.CanBeCasted() && Creepcountall(ERadius) > Creepcountall(600)/2)
                                        E.UseAbility();
                                    illusionCamps.illusion.Move(illusionCamps.stackPosition);
                                    illusionCamps.State = 4;
                                }
                                break;
                            case 4:
                                illusion.Move(illusionCamps.stackPosition);
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
            else if (FarmJ && illusions.Count > 0 && Utils.SleepCheck("farm"))
            {
                foreach (var illusion in illusions)
                {
                    if (!Check(illusion))
                    {
                        JungleCamps.Find(x => x.id == GetClosestCamp(illusion, false, false).id).illusion = illusion;
                        JungleCamps.Find(x => x.id == GetClosestCamp(illusion, false, false).id).farming = true;
                    }
                    else
                    {
                        var illusionCamps = Checkillusion(illusion);
                        if (illusion.Distance2D(illusionCamps.position) > 100)
                        {
                            illusion.Move(illusionCamps.position);
                        }
                        else if (menuValue.IsEnabled(E.Name) && E.CanBeCasted() && Creepcountall(ERadius) >= Creepcountall(600) / 2)
                        {
                            E.UseAbility();
                            illusion.Attack(GetNearestCreepToPull(illusionCamps.illusion, 500));
                        }
                    }
                }
                Utils.Sleep(1000, "farm");
            }
            #endregion Farm
        }

        public static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
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
        }

        private static JungleCamps GetClosestCamp(Hero illusion,bool stack, bool any)
        {
            var closest = JungleCamps[12];
            foreach (var x in JungleCamps)
            {
                if (illusion.Distance2D(x.position) < illusion.Distance2D(closest.position) && !x.farming && !x.stacking && !x.empty)
                    closest = x;
            }
            return closest;
        }

        private static JungleCamps Checkillusion(Hero illu)
        {
            var a = new JungleCamps(); 
            foreach (var x in JungleCamps)
            {
                if (x.illusion != null)
                    a = (x.illusion.Handle == illu.Handle ? x : a);
            }
            return a;
        }
        
        private static bool Check(Hero illu)
        {
            var a = false;
            foreach (var x in JungleCamps)
            {
                if (x.illusion != null)
                    a = (x.illusion.Handle == illu.Handle ? true : a);
            }
            return a;
        }

        private static int Creepcountall(float radius)
        {
            var a = 0;
            foreach (var Illusoin in illusions)
            {
                neutrals = ObjectManager.GetEntities<Unit>()
                        .Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && Illusoin.Distance2D(x) <= radius)
                        .ToList();
                a = a + neutrals.Count;
            }
            return a;
        }

        private static int Creepcount(Hero illu, float radius)
        {
            neutrals = ObjectManager.GetEntities<Unit>().Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && illu.Distance2D(x) <= radius).ToList();
            return neutrals.Count;
        }

        /*
        private static CreepWaves getClosestWave(Hero illu)
        {
            var closest = creepwaves[6];
            var lanecreeps = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
            creep.ClassID == ClassID.CDOTA_Unit_SpiritBear || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep &&
            creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Team == ObjectManager.LocalHero.GetEnemyTeam()).ToList();

            foreach (var wave in lanecreeps)
            {
                if (illu.Distance2D(wave) < illu.Distance2D(closest.position) && !closest.cutted)
                {
                    //Game.PrintMessage("farming " + camp.id, MessageType.LogMessage);
                    //Console.WriteLine("farming " + camp.id);
                    closest.position = wave.Position;
                }
            }
            return closest;
        }*/

        private static Unit GetNearestCreepToPull(Hero illusion, int dis)
        {
            var creeps =
                ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive && x.IsSpawned && x.IsVisible && illusion.Distance2D(x) <= dis && x.Team != me.Team).ToList();
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
