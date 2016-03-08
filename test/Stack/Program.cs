using System;
using System.Collections.Generic;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

using SharpDX;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using SharpDX.Direct3D9;
using SharpDX.Win32;


namespace Stack
{
    internal class Program
    {
        public class JungleCamps
        {
            public Unit Unit { get; set; }
            public Vector3 Position { get; set; }
            public Vector3 StackPosition { get; set; }
            public Vector3 WaitPosition { get; set; }
            public int Team { get; set; }
            public int Id { get; set; }
            public bool Stacked { get; set; }
            public bool Ancients { get; set; }
            public bool Empty { get; set; }
            public int State { get; set; }
            public int AttackTime { get; set; }
            public int Creepscount { get; set; }
            public int Starttime { get; set; }
        }

        private static readonly List<ParticleEffect> Effects = new List<ParticleEffect>();
        private static Ability Q, W, E, R;
        private static Item _manta;
        private static Hero _me;
        private static Unit closestNeutral;
        private static bool _stackKey, _load, _enable;
        private static readonly Menu Menu = new Menu("Stack", "Stack", true, "item_helm_of_the_dominator", true);
        private static MenuItem _subMenu1, _subMenu0;
        private static List<JungleCamps> Camps = new List<JungleCamps>();
        private static int _seconds;

        private static void Main(string[] args)
        {
            Camps.Add(new JungleCamps { Position = new Vector3(-1708, -4284, 256), StackPosition = new Vector3(-2776, -3144, 256), WaitPosition = new Vector3(-1971, -3949, 256), Team = 2, Id = 1, Empty = false, Stacked = false, Starttime = 55 });
            Camps.Add(new JungleCamps { Position = new Vector3(-266, -3176, 256), StackPosition = new Vector3(-522, -1351, 256), WaitPosition = new Vector3(-325, -2699, 256), Team = 2, Id = 2, Empty = false, Stacked = false, Starttime = 55 });
            Camps.Add(new JungleCamps { Position = new Vector3(3016, -4692, 384), StackPosition = new Vector3(4777, -4954, 384), WaitPosition = new Vector3(3132, -5000, 384), Team = 2, Id = 4, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(1656, -3714, 384), StackPosition = new Vector3(1263, -6041, 384), WaitPosition = new Vector3(1612, -4277, 384), Team = 2, Id = 3, Empty = false, Stacked = false, Starttime = 54 });
            Camps.Add(new JungleCamps { Position = new Vector3(4474, -3598, 384), StackPosition = new Vector3(2755, -4001, 384), WaitPosition = new Vector3(4121, -3902, 384), Team = 2, Id = 5, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-3617, 805, 384), StackPosition = new Vector3(-5268, 1400, 384), WaitPosition = new Vector3(-3835, 643, 384), Team = 2, Id = 6, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-4446, 3541, 384), StackPosition = new Vector3(-3953, 4954, 384), WaitPosition = new Vector3(-4251, 3760, 384), Team = 3, Id = 7, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-2981, 4591, 384), StackPosition = new Vector3(-3248, 5993, 384), WaitPosition = new Vector3(-3055, 4837, 384), Team = 3, Id = 8, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-392, 3652, 384), StackPosition = new Vector3(-224, 5088, 384), WaitPosition = new Vector3(-503, 3955, 384), Team = 3, Id = 9, Empty = false,  Stacked = false, Starttime = 55 });
            Camps.Add(new JungleCamps { Position = new Vector3(-1524, 2641, 256), StackPosition = new Vector3(-1266, 4273, 384), WaitPosition = new Vector3(-1465, 2908, 256), Team = 3, Id = 10, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(1098, 3338, 384), StackPosition = new Vector3(910, 5003, 384), WaitPosition = new Vector3(975, 3586, 384), Team = 3, Id = 11, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(4141, 554, 384), StackPosition = new Vector3(2987, -2, 384), WaitPosition = new Vector3(3876, 506, 384), Team = 3, Id = 12, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-2960, -126, 384), StackPosition = new Vector3(-3850, -1491, 384), WaitPosition = new Vector3(-2777, -230, 384), Team = 2, Id = 11, Empty = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(4000, -700, 256), StackPosition = new Vector3(1713, -134, 256), WaitPosition = new Vector3(3649, -721, 256), Team = 3, Id = 12, Empty = false, Stacked = false, Starttime = 53 });

            Events.OnLoad += Events_OnLoad;
            Events.OnClose += Events_OnClose;
        }
        private static void Events_OnClose(object sender, EventArgs e)
        {
            Game.OnUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            Game.OnWndProc -= Game_OnWndProc;
            Game.OnUpdate -= Game_Stack;
            _load = false;
        }
        private static void Events_OnLoad(object sender, EventArgs e)
        {
            _load = false;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_Stack;

            Menu.AddItem(new MenuItem("Stack", "Stack").SetValue(new KeyBind('F', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("drawline", "Draw line?").SetValue(false));
            Menu.AddToMainMenu();
            OnLoadMessage();
            _me = ObjectManager.LocalHero;
        }

        private static void Game_Stack(EventArgs args)
        {
            if (!Menu.Item("Stack").GetValue<KeyBind>().Active || !Game.IsInGame || _me == null || Game.IsPaused ||
                Game.IsChatOpen || !Utils.SleepCheck("wait")) return;

            foreach (var camp in Camps)
            {
                if (_seconds == 0) camp.Empty = false;
                if (_seconds > 47 && camp.Unit == null) camp.State = 5;
                if (_seconds < 10 ) camp.State = 0;
                if (camp.Unit == null) continue;
                if (camp.Unit.IsAlive) continue;
                camp.Unit = null;
                camp.State = 0;
            }

            foreach (var camp in Camps.Where(camp => camp.Stacked && camp.Unit != null))
            {
                var unit = camp.Unit;
                var time = (int)(camp.Starttime - (unit.Distance2D(camp.WaitPosition) / unit.MovementSpeed)-5 + Game.Ping/1000);
                switch (camp.State)
                {
                    case 0:
                        if (_seconds < time) continue;
                        if (unit.Distance2D(camp.WaitPosition) < 10)
                            camp.State = 1;
                        unit.Move(camp.WaitPosition);
                        Utils.Sleep(500, "wait");
                        break;
                    case 1:
                        var creepscount = CreepCount(unit, 800);
                        if (creepscount == 0 && unit.Distance2D(camp.WaitPosition) < 10)
                        {
                            camp.Empty = true;
                            camp.Unit = null;
                            camp.State = 0;
                            return;
                        }
                        if (_seconds < time) continue;
                        if (_seconds >= camp.Starttime - 5)
                        {
                            closestNeutral = GetNearestCreepToPull(unit, 800);
                            camp.AttackTime = (int) (camp.Starttime -
                                                     (unit.Distance2D(closestNeutral) -
                                                      (closestNeutral.IsRanged
                                                          ? Math.Max(closestNeutral.AttackRange, unit.AttackRange) - 100
                                                          : closestNeutral.RingRadius))/GetCreepStackMs(unit, 800));
                            camp.State = 2;
                        }
                        Utils.Sleep(500, "wait");
                        break;
                    case 2:
                        if (_seconds < time) continue;
                        if (_seconds >= camp.AttackTime)
                        {
                            closestNeutral = GetNearestCreepToPull(unit, 800);
                            unit.Attack(closestNeutral);
                            var tWait =
                                (int) (((unit.Distance2D(closestNeutral) -
                                         (closestNeutral.IsRanged
                                             ? Math.Max(closestNeutral.AttackRange, unit.AttackRange) - 100
                                             : closestNeutral.RingRadius))/GetCreepStackMs(unit, 800))*1000 + Game.Ping);
                            Utils.Sleep(tWait, "" + unit.Handle);
                            camp.State = 3;
                        }
                        break;
                    case 3:
                        closestNeutral = GetNearestCreepToPull(unit, 800);
                        if (Utils.SleepCheck("" + unit.Handle) || closestNeutral.IsMoving ||
                            closestNeutral.IsAttacking())
                        {
                            unit.Move(camp.StackPosition);
                            camp.State = 4;
                        }
                        break;
                    case 4:
                        if (_seconds == 0)
                            unit.Move(camp.WaitPosition);
                        else if (_seconds < 30 && _seconds > 10)
                            camp.State = 0;
                        Utils.Sleep(1000, "wait");
                        break;
                }
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame || _me == null || Game.IsPaused || Game.IsChatOpen ||
                !Menu.Item("Stack").GetValue<KeyBind>().Active && Utils.SleepCheck("wait"))
            {
                return;
            }

            _seconds = ((int) Game.GameTime)%60;

            Q = _me.Spellbook.Spell1;
            W = _me.Spellbook.Spell2;
            E = _me.Spellbook.Spell3;
            R = _me.Spellbook.Spell4;

            _manta = _me.FindItem("item_manta");
            if (_manta != null)
            {
                if (_subMenu1 == null)
                {
                    _subMenu1 = Menu.AddItem(
                                new MenuItem("item", "Autouse? ").SetValue(
                                    new AbilityToggler(new Dictionary<string, bool> { { "item_manta", true } })));
                }
                _enable = _subMenu1.GetValue<AbilityToggler>().IsEnabled("item_manta");
                if (GetFurtherCamp(_me).Id != 0)
                {
                    var time =
                        (int)
                            (GetFurtherCamp(_me).Starttime -
                             (_me.Distance2D(GetFurtherCamp(_me).WaitPosition) / _me.MovementSpeed) - 5 +
                             Game.Ping / 1000);
                    if (_enable && _manta.CanBeCasted() && Utils.SleepCheck("manta") && time < _seconds && time > 40 && _seconds < time + 5)
                    {
                        _manta.UseAbility();
                        Utils.Sleep(150 + Game.Ping, "manta");
                    }
                }
            }
            if (Utils.SleepCheck("Cooldown"))
            {
                var baseNpcCreeps =
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                x.IsAlive && x.Team == _me.Team && x.IsControllable &&
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                                 x.ClassID == ClassID.CDOTA_Unit_SpiritBear ||
                                 x.ClassID == ClassID.CDOTA_Unit_VisageFamiliar ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                                 x.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling ||
                                 x.IsIllusion ||
                                 (x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo && _me.Handle != x.Handle && R.Level > 0)))
                        .ToList();
                GetClosestCamp(baseNpcCreeps);
                Utils.Sleep(2000, "Cooldown");
            }

            switch (_me.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_Lycan:
                    if (!_load)
                    {
                        _subMenu0 =
                            Menu.AddItem(
                                new MenuItem("enable", "Autouse? ").SetValue(
                                    new AbilityToggler(new Dictionary<string, bool> { {"lycan_summon_wolves", true} })));
                        _load = true;
                    }
                    _enable = _subMenu0.GetValue<AbilityToggler>().IsEnabled("lycan_summon_wolves");
                    if (GetFurtherCamp(_me).Id != 0)
                    {
                        var time =
                            (int)
                                (GetFurtherCamp(_me).Starttime - (_me.Distance2D(GetFurtherCamp(_me).WaitPosition)/440) -
                                 5 + Game.Ping/1000);

                        if (_enable && Q.CanBeCasted() && Utils.SleepCheck("Q") && Q != null && time < _seconds && time > 5 && _seconds < time + 5)
                        {
                            Q.UseAbility();
                            Utils.Sleep(150 + Game.Ping, "Q");
                        }
                    }
                    break;
                case ClassID.CDOTA_Unit_Hero_Naga_Siren:
                    if (!_load)
                    {
                        _subMenu0 =
                            Menu.AddItem(
                                new MenuItem("enable", "Autouse? ").SetValue(
                                    new AbilityToggler(new Dictionary<string, bool> { {"naga_siren_mirror_image", true} })));
                        _load = true;
                    }
                    _enable = _subMenu0.GetValue<AbilityToggler>().IsEnabled("naga_siren_mirror_image");
                    if (GetFurtherCamp(_me).Id != 0)
                    {
                        var time =
                            (int)
                                (GetFurtherCamp(_me).Starttime -
                                 (_me.Distance2D(GetFurtherCamp(_me).WaitPosition)/_me.MovementSpeed) - 5 +
                                 Game.Ping/1000);

                        if (_enable && Q.CanBeCasted() && Utils.SleepCheck("Q") && Q != null && time < _seconds && time > 30 && _seconds < time + 5)
                        {
                            Q.UseAbility();
                            Utils.Sleep(150 + Game.Ping, "Q");
                        }
                    }
                    break;
            }
        }
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (ulong) Utils.WindowsMessages.WM_LBUTTONDOWN)
            {
                foreach (var camp in from camp in Camps
                    let position = Drawing.WorldToScreen(camp.Position)
                    where Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 40, 40)
                    select camp)
                {
                    camp.Stacked = (camp.Stacked == false) ? true : false;
                    camp.Unit = null;
                }
            }
            //else if(args.Msg == (ulong) Utils.WindowsMessages.WM_RBUTTONDOWN)
            //{
            //    foreach (var camp in from camp in Camps
            //                         let position = Drawing.WorldToScreen(camp.Position)
            //                         where Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 40, 40)
            //                         select camp)
            //    {
            //        if (!camp.Stacked) return;
            //        camp.Unit = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => x.IsAlive && x.);
                    
            //    }
            //}
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            //try
            //{
            //    var pos = Drawing.WorldToScreen(Game.MousePosition);
            //    var unit = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => x.Distance2D(Game.MousePosition) < 50);
            //    if (unit != null )
            //       Drawing.DrawText(unit.ClassID + "", "", new Vector2(pos.X, pos.Y + 20), new Vector2(40), Color.AliceBlue, FontFlags.Outline);
            //}
            //catch (Exception)
            //{
            //}
            foreach (var camp in Camps)
            {
                var position = Drawing.WorldToScreen(camp.Position);
                var alpha3 = Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 40, 40) ? 100 : 0;
                RoundedRectangle(position.X, position.Y, 40, 40, 10, new Color(100, 100, 100 + alpha3));
                var text = "✖";
                var unittext = "Null";
                var color = Color.DarkRed;
                if (camp.Stacked)
                {
                    text = "✔";
                    color = Color.DarkGreen;
                }
                if (camp.Unit !=null && Menu.Item("drawline").GetValue<bool>() )
                {
                    unittext = camp.Unit.Handle.ToString();
                    Drawing.DrawLine(position, Drawing.WorldToScreen(camp.Unit.Position),Color.Black);
                    //Drawing.DrawText(camp.Unit.ClassID.ToString(), "", new Vector2(position.X + 38, position.Y - 3), new Vector2(40), color, FontFlags.Outline);
                }
                Drawing.DrawText(text, "", new Vector2(position.X + 8, position.Y - 3), new Vector2(40), color, FontFlags.Outline);
                //Drawing.DrawText(camp.State.ToString(), "", new Vector2(position.X -20, position.Y - 3), new Vector2(40), color, FontFlags.Outline);
            }
        }

        private static void OnLoadMessage()
        {
            Game.PrintMessage("<font face='verdana' color='#00FF00'>Stack loaded !</font>", MessageType.LogMessage);
        }
        private static JungleCamps GetFurtherCamp(Hero h)
        {
            JungleCamps[] further = {new JungleCamps {WaitPosition = h.Position, Id = 0}};
            foreach (var camp in Camps)
            {
                if (camp.Empty || !camp.Stacked || camp.Unit != null) continue;
                if (h.Distance2D(camp.WaitPosition) > h.Distance2D(further[0].WaitPosition))
                    further[0] = camp;
            }
            return further[0];
        }
        private static bool Unittrue(Unit h)
        {
            foreach (var camp in Camps)
            {
                if (camp.Unit != null)
                {
                    if (camp.Unit.Handle == h.Handle)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private static JungleCamps GetClosestCampu(Unit h, int n)
        {
            JungleCamps[] closest =
            {
                new JungleCamps {WaitPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), Id = 0}
            };
            foreach (var camp in Camps.Where(x => x.State == 0 && x.Stacked))
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

                            if (camp.Unit != null)
                            {
                                if (camp.Unit.Handle != h.Handle)
                                {
                                    closest[0] = camp;
                                }
                            }
                            else if (camp.Unit == null)
                            {
                                closest[0] = camp;
                            }

                            break;
                    }
                }
            }
            return closest[0];
        }
        private static void GetClosestCamp(List<Unit> h)
        {
            foreach (var baseNpcCreep in h)
            {
                var num1 = Camps.Count(x => x.Stacked && x.State == 0);
                var num3 = h.Count;
                var num2 = Camps.Count(x => x.Stacked && x.State == 0 && x.Unit == null);

                //Game.PrintMessage(" " + num1 + " " + num2 + " " + num3, MessageType.ChatMessage);

                if (num1 == num2)
                {
                    Camps.First(x => GetClosestCampu(baseNpcCreep, 1).Id == x.Id).Unit = baseNpcCreep;
                }
                else if (num2 > 0 && num1 - num2 < num3)
                {
                    if (!Unittrue(baseNpcCreep)) continue;
                    Camps.First(x => GetClosestCampu(baseNpcCreep, 1).Id == x.Id).Unit = baseNpcCreep;
                }
                else if (num1 <= num3 && num2 == 0 && GetClosestCampu(baseNpcCreep, 2).Id != 0)
                {
                    //foreach (var camp in Camps.Where(x => x.State == 0 && x.Stacked))
                    //{
                    //    if (camp.Unit != null)
                    //    {
                    //        if (camp.Unit.Handle == baseNpcCreep.Handle)
                    //        {
                    //            camp.Unit = GetClosestCampu(baseNpcCreep, 2).Unit;
                    //        }
                    //    }
                    //    if (GetClosestCampu(baseNpcCreep, 2).Id == camp.Id)
                    //    {
                    //        camp.Unit = baseNpcCreep;
                    //    }
                    //}
                    try
                    {
                        //Game.PrintMessage("3 ", MessageType.ChatMessage);
                        Camps.First(x => x.Unit.Handle == baseNpcCreep.Handle).Unit = GetClosestCampu(baseNpcCreep, 2).Unit;
                        Camps.First(x => GetClosestCampu(baseNpcCreep, 2).Id == x.Id).Unit = baseNpcCreep;
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
                else if (num1 - num2 == num3 && num2 > 0 && GetClosestCampu(baseNpcCreep, 3).Id != 0)
                {
                    //Game.PrintMessage("4 ", MessageType.ChatMessage);
                    try
                    {
                        Camps.First(x => x.Unit.Handle == baseNpcCreep.Handle).Unit = GetClosestCampu(baseNpcCreep, 3).Unit;
                        Camps.First(x => GetClosestCampu(baseNpcCreep, 3).Id == x.Id).Unit = baseNpcCreep;
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
            }
        }
        private static int CreepCount(Unit h, float radius)
        {
            return
                ObjectManager.GetEntities<Unit>()
                    .Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && h.Distance2D(x) <= radius)
                    .ToList().Count;
        }
        private static Unit GetNearestCreepToPull(Unit h, float radius)
        {
            var neutrals = ObjectManager.GetEntities<Unit>().Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && h.Distance2D(x) <= radius).ToList();
            Unit bestCreep = null;
            var bestDistance = float.MaxValue;
            foreach (var neutral in neutrals.Where(neutral => h.Distance2D(neutral) < bestDistance))
            {
                bestDistance = h.Distance2D(neutral);
                bestCreep = neutral;
            }
            return bestCreep;
        }
        private static int GetCreepStackMs(Unit h, float radius)
        {
            var neutrals = ObjectManager.GetEntities<Unit>().Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && h.Distance2D(x) <= radius).ToList();
            int[] ms = {int.MaxValue};
            foreach (var neutral in neutrals.Where(neutral => neutral.MovementSpeed < ms[0]))
            {
                ms[0] = neutral.MovementSpeed;
            }
            return ms[0];
        }
        public static void RoundedRectangle(float x, float y, float w, float h, int iSmooth, Color color)
        {
            var pt = new Vector2[4];

            // Get all corners 
            pt[0].X = x + (w - iSmooth);
            pt[0].Y = y + (h - iSmooth);

            pt[1].X = x + iSmooth;
            pt[1].Y = y + (h - iSmooth);

            pt[2].X = x + iSmooth;
            pt[2].Y = y + iSmooth;

            pt[3].X = x + w - iSmooth;
            pt[3].Y = y + iSmooth;

            // Draw cross 
            Drawing.DrawRect(new Vector2(x, y + iSmooth), new Vector2(w, h - (iSmooth * 2)), color);

            Drawing.DrawRect(new Vector2(x + iSmooth, y), new Vector2(w - (iSmooth * 2), h), color);

            float fDegree = 0;

            for (var i = 0; i < 4; i++)
            {
                for (var k = fDegree; k < fDegree + ((Math.PI * 2) / 4f); k += (float) (1 * (Math.PI / 180.0f)))
                {
                    // Draw quarter circles on every corner
                    Drawing.DrawLine(
                        new Vector2(pt[i].X, pt[i].Y),
                        new Vector2(pt[i].X + (float) (Math.Cos(k) * iSmooth), pt[i].Y + (float) (Math.Sin(k) * iSmooth)),
                        color);
                }

                fDegree += (float) (Math.PI * 2) / 4; // quarter circle offset 
            }
        }
    }
}