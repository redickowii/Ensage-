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
            public bool Farming { get; set; }
            public bool Visible { get; set; }
            public bool Stacking { get; set; }
            public bool Stacked { get; set; }
            public bool Ancients { get; set; }
            public bool Empty { get; set; }
            public int State { get; set; }
            public int AttackTime { get; set; }
            public int Creepscount { get; set; }
            public int Starttime { get; set; }
        }

        private static Ability Q, W, E, R;
        private static Item Manta;
        private static Hero _me;
        private static bool _stackKey, _load;
        private static readonly Menu Menu = new Menu("Stack", "Stack", true, "item_helm_of_the_dominator", true);
        private static MenuItem _subMenu1, _subMenu0;
        private static List<JungleCamps> Camps = new List<JungleCamps>();
        private static int _seconds;

        private static void Main(string[] args)
        {
            Camps.Add(new JungleCamps { Position = new Vector3(-1708, -4284, 256), StackPosition = new Vector3(-2776, -3144, 256), WaitPosition = new Vector3(-1971, -3949, 256), Team = 2, Id = 1, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 55 });
            Camps.Add(new JungleCamps { Position = new Vector3(-266, -3176, 256), StackPosition = new Vector3(-522, -1351, 256), WaitPosition = new Vector3(-325, -2699, 256), Team = 2, Id = 2, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 55 });
            Camps.Add(new JungleCamps { Position = new Vector3(3016, -4692, 384), StackPosition = new Vector3(4777, -4954, 384), WaitPosition = new Vector3(3132, -5000, 384), Team = 2, Id = 4, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(1656, -3714, 384), StackPosition = new Vector3(1263, -6041, 384), WaitPosition = new Vector3(1612, -4277, 384), Team = 2, Id = 3, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 54 });
            Camps.Add(new JungleCamps { Position = new Vector3(4474, -3598, 384), StackPosition = new Vector3(2755, -4001, 384), WaitPosition = new Vector3(4121, -3902, 384), Team = 2, Id = 5, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-3617, 805, 384), StackPosition = new Vector3(-5268, 1400, 384), WaitPosition = new Vector3(-3835, 643, 384), Team = 2, Id = 6, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-4446, 3541, 384), StackPosition = new Vector3(-3953, 4954, 384), WaitPosition = new Vector3(-4251, 3760, 384), Team = 3, Id = 7, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-2981, 4591, 384), StackPosition = new Vector3(-3248, 5993, 384), WaitPosition = new Vector3(-3055, 4837, 384), Team = 3, Id = 8, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-392, 3652, 384), StackPosition = new Vector3(-224, 5088, 384), WaitPosition = new Vector3(-503, 3955, 384), Team = 3, Id = 9, Farming = false, Empty = false, Visible = false,  Stacking = false, Stacked = false, Starttime = 55 });
            Camps.Add(new JungleCamps { Position = new Vector3(-1524, 2641, 256), StackPosition = new Vector3(-1266, 4273, 384), WaitPosition = new Vector3(-1465, 2908, 256), Team = 3, Id = 10, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(1098, 3338, 384), StackPosition = new Vector3(910, 5003, 384), WaitPosition = new Vector3(975, 3586, 384), Team = 3, Id = 11, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(4141, 554, 384), StackPosition = new Vector3(2987, -2, 384), WaitPosition = new Vector3(3876, 506, 384), Team = 3, Id = 12, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(-2960, -126, 384), StackPosition = new Vector3(-3850, -1491, 384), WaitPosition = new Vector3(-2777, -230, 384), Team = 2, Id = 11, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });
            Camps.Add(new JungleCamps { Position = new Vector3(4000, -700, 256), StackPosition = new Vector3(1713, -134, 256), WaitPosition = new Vector3(3649, -721, 256), Team = 3, Id = 12, Farming = false, Empty = false, Visible = false, Stacking = false, Stacked = false, Starttime = 53 });

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
            if (!Menu.Item("Stack").GetValue<KeyBind>().Active ||
                !Game.IsInGame || _me == null || Game.IsPaused || Game.IsChatOpen
                )
                return;

            if (!Utils.SleepCheck("wait")) return;

            try
            {
                foreach (var camp in Camps)
                {
                    if (camp.Unit == null) continue;
                    if (camp.Unit.IsAlive) continue;
                    camp.Unit = null;
                    camp.State = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("1 "+e);
            }
            foreach (var camp in Camps.Where(camp => camp.Stacked && camp.Unit != null))
            {
                var unit = camp.Unit;
                var time = (int)(camp.Starttime - (unit.Distance2D(camp.WaitPosition) / unit.MovementSpeed)-5 + Game.Ping/1000);
                switch (camp.State)
                {
                    case 0:
                        if (_seconds < time) continue;
                        //Game.PrintMessage("case 0", MessageType.LogMessage);
                        if (unit.Distance2D(camp.WaitPosition) < 5)
                            camp.State = 1;
                        else
                            unit.Move(camp.WaitPosition);
                        Utils.Sleep(500, "wait");
                        break;
                    case 1:
                        if (_seconds < time) continue;
                        //Game.PrintMessage("case 1", MessageType.LogMessage);
                        var creepscount = CreepCount(unit, 800);
                        if (creepscount == 0)
                        {
                            camp.Empty = true;
                            camp.Unit = null;
                            camp.State = 0;
                            return;
                        }
                        else if (_seconds >= camp.Starttime - 5)
                        {
                            var closestNeutral = GetNearestCreepToPull(unit, 800);
                            var moveTime = camp.Starttime -
                                           (unit.Distance2D(closestNeutral) -
                                            (closestNeutral.IsRanged
                                                ? Math.Max(closestNeutral.AttackRange, unit.AttackRange)-100 
                                                : closestNeutral.RingRadius))/Math.Min(unit.MovementSpeed, closestNeutral.MovementSpeed);
                            camp.AttackTime = (int) moveTime;
                            camp.State = 2;
                        }
                        Utils.Sleep(500, "wait");
                        break;
                    case 2:
                        if (_seconds < time) continue;
                        //Game.PrintMessage("case 2", MessageType.LogMessage);
                        if (_seconds >= camp.AttackTime)
                        {
                            var closestNeutral = GetNearestCreepToPull(unit, 800);
                            unit.Attack(closestNeutral);
                            camp.State = 3;
                            var tWait =
                                (int) (((unit.Distance2D(closestNeutral) -
                                            (closestNeutral.IsRanged
                                                ? Math.Max(closestNeutral.AttackRange, unit.AttackRange) - 100
                                                : closestNeutral.RingRadius)) / Math.Max(unit.MovementSpeed, closestNeutral.MovementSpeed)) *1000 + Game.Ping);
                            Utils.Sleep(tWait, "" + unit.Handle);
                        }
                        break;
                    case 3:
                        //Game.PrintMessage("case 3", MessageType.LogMessage);
                        if (Utils.SleepCheck("" + unit.Handle))
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
                    default:
                        camp.State = 0;
                        break;
                }
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame || _me == null || Game.IsPaused || Game.IsChatOpen || !Menu.Item("Stack").GetValue<KeyBind>().Active && Utils.SleepCheck("wait"))
            {
                return;
            }
            _seconds = ((int) Game.GameTime)%60;
            Q = _me.Spellbook.Spell1;
            W = _me.Spellbook.Spell2;
            E = _me.Spellbook.Spell3;
            R = _me.Spellbook.Spell4;

            Manta = _me.FindItem("item_manta");

            switch (_me.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_Lycan:
                    if (!_load)
                    {
                        var dict = new Dictionary<string, bool>
                        {
                            {"lycan_summon_wolves", true}
                        };
                        _subMenu0 =
                            Menu.AddItem(new MenuItem("enabledAbilities", "Autouse? ").SetValue(new AbilityToggler(dict)));
                        _load = true;
                    }
                    var enabledAbilities = _subMenu0.GetValue<AbilityToggler>().IsEnabled("lycan_summon_wolves");
                    if (GetFurtherCamp(_me) != null)
                    {
                        var time =
                            (int)
                                (GetFurtherCamp(_me).Starttime - (_me.Distance2D(GetFurtherCamp(_me).WaitPosition)/440) -
                                 5 + Game.Ping/1000);
                        
                        if (enabledAbilities && Q.CanBeCasted() && Utils.SleepCheck("Q") && Q != null && time < _seconds)
                        {
                            Q.UseAbility();
                            Utils.Sleep(4000 + Game.Ping, "Q");
                        }
                    }
                    break;
            }
            var baseNpcCreeps = ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                                (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                                 x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || 
                                 x.IsIllusion || 
                                 (x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo && _me.Handle != x.Handle && R.Level>0)
                                 )
                                && x.IsSpawned && x.IsVisible &&
                                x.IsAlive && x.Team == _me.Team && x.IsControllable )
                        .ToList();
            UnitsS(baseNpcCreeps);
        }
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (ulong) Utils.WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            foreach (var camp in from camp in Camps let position = Drawing.WorldToScreen(camp.Position) where Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 40, 40) select camp)
            {
                camp.Stacked = (camp.Stacked == false) ? true : false;
                camp.Unit = null;
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                var pos = Drawing.WorldToScreen(Game.MousePosition);
                var unit = ObjectManager.GetEntities<Unit>().Where(x => x.IsAlive && x.Distance2D(Game.MousePosition) < 50).DefaultIfEmpty(null).FirstOrDefault();
                Drawing.DrawText(unit.Position.X + " " + unit.Position.Y, "", new Vector2(pos.X, pos.Y + 20), new Vector2(40), Color.AliceBlue, FontFlags.Outline);
            }
            catch (Exception)
            {

            }

            foreach (var camp in Camps)
            {
                var position = Drawing.WorldToScreen(camp.Position);
                var alpha3 = Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 40, 40) ? 100 : 0;
                RoundedRectangle(position.X, position.Y, 70, 40, 10, new Color(100, 100, 100 + alpha3));
                var text = "✖";
                var unittext = "Null";
                var color = Color.DarkRed;
                if (camp.Stacked)
                {
                    text = "✔";
                    color = Color.DarkGreen;
                }
                if (camp.Unit !=null && Menu.Item("drawline").GetValue<bool>())
                {
                    unittext = camp.Unit.Handle.ToString();
                    Drawing.DrawLine(position, Drawing.WorldToScreen(camp.Unit.Position),Color.Black);
                }
                
                Drawing.DrawText(text, "", new Vector2(position.X + 8, position.Y - 3), new Vector2(40), color, FontFlags.Outline);
                Drawing.DrawText(camp.State.ToString(), "", new Vector2(position.X +38, position.Y - 3), new Vector2(40), color, FontFlags.Outline);
            }
        }
        private static void OnLoadMessage()
        {
            Game.PrintMessage("<font face='verdana' color='#00FF00'>Stack loaded !</font>", MessageType.LogMessage);
        }
        private static JungleCamps GetClosestCamp(Unit h)
        {
            JungleCamps[] closest = 
            {
                new JungleCamps {WaitPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue)}
            };
            try
            {
                foreach (var camp in Camps.Where(camp => camp.Unit == null && !camp.Empty && camp.Stacked).Where(camp => h.Distance2D(camp.WaitPosition) < h.Distance2D(closest[0].WaitPosition)))
                {
                    closest[0] = camp;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("3 Error");
            }

            if (closest[0].Id == 0)
                return null;

            foreach (var camp in Camps)
            {
                if (camp.Unit != null && camp.Unit.Handle == h.Handle)
                {
                    if (h.Distance2D(camp.WaitPosition) - 10 > h.Distance2D(closest[0].WaitPosition) && camp.State < 3)
                    {
                        camp.Unit = null;
                        camp.State = 0;
                        return closest[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return closest[0];
        }
        private static JungleCamps GetFurtherCamp(Unit h)
        {
            JungleCamps[] further =
            {
                new JungleCamps {WaitPosition = h.Position}
            };
            try
            {
                foreach (var camp in Camps.Where(camp => camp.Unit == null && !camp.Empty && camp.Stacked).Where(camp => h.Distance2D(camp.WaitPosition) > h.Distance2D(further[0].WaitPosition)))
                {
                    further[0] = camp;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("3 Error");
            }
            if (further[0].Id == 0)
                return null;
                

            foreach (var camp in Camps)
            {
                if (camp.Unit != null && camp.Unit.Handle == h.Handle)
                {
                    if (h.Distance2D(camp.WaitPosition) + 10 < h.Distance2D(further[0].WaitPosition) && camp.State < 3)
                    {
                        camp.Unit = null;
                        camp.State = 0;
                        return further[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return further[0];
        }

        private static void UnitsS(List<Unit> h)
        {
            foreach (var baseNpcCreep in h)
            {
                if (GetClosestCamp(baseNpcCreep) == null) continue;
                foreach (var camp in Camps)
                {
                    if (camp.Id != GetClosestCamp(baseNpcCreep).Id) continue;
                    camp.State = 0;
                    camp.Unit = baseNpcCreep;
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
