using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
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
            public Vector3 WaitPositionN { get; set; }
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
        private static readonly List<JungleCamps> Camps = new List<JungleCamps>();
        private static int _seconds;
        private static Font _text;
        public static Line _line = new Line(Drawing.Direct3DDevice9);

        private static void Main(string[] args)
        {
            _text = new Font(
               Drawing.Direct3DDevice9,
               new FontDescription
               {
                   FaceName = "Monospace",
                   Height = 35,
                   OutputPrecision = FontPrecision.Default,
                   Quality = FontQuality.ClearType
               });

            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-1708, -4284, 256),
                StackPosition = new Vector3(-1816, -2684, 256),
                WaitPosition = new Vector3(-1867, -4033, 256),
                WaitPositionN = new Vector3(-2041, -3790, 256),
                Team = 2,
                Id = 1,
                Empty = false,
                Stacked = false,
                Starttime = 56
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-266, -3176, 256),
                StackPosition = new Vector3(-522, -1351, 256),
                WaitPosition = new Vector3(-306, -2853, 256),
                WaitPositionN = new Vector3(-340, -2521, 256),
                Team = 2,
                Id = 2,
                Empty = false,
                Stacked = false,
                Starttime = 56
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(1656, -3714, 384),
                StackPosition = new Vector3(1449, -5699, 384),
                WaitPosition = new Vector3(1637, -4009, 384),
                WaitPositionN = new Vector3(1647, -4651, 384),
                Team = 2,
                Id = 3,
                Empty = false,
                Stacked = false,
                Starttime = 54
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(3016, -4692, 384),
                StackPosition = new Vector3(5032, -4826, 384),
                WaitPosition = new Vector3(3146, -5071, 384),
                WaitPositionN = new Vector3(3088, -5345, 384),
                Team = 2,
                Id = 4,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(4474, -3598, 384),
                StackPosition = new Vector3(2486, -4125, 384),
                WaitPosition = new Vector3(4121, -3902, 384),
                WaitPositionN = new Vector3(3714, -3941, 384),
                Team = 2,
                Id = 5,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-3617, 805, 384),
                StackPosition = new Vector3(-5268, 1400, 384),
                WaitPosition = new Vector3(-3835, 643, 384),
                WaitPositionN = new Vector3(-4571, 795, 384),
                Team = 2,
                Id = 6,
                Empty = false,
                Stacked = false,
                Starttime = 54
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-4446, 3541, 384),
                StackPosition = new Vector3(-3953, 4954, 384),
                WaitPosition = new Vector3(-4251, 3760, 384),
                WaitPositionN = new Vector3(-4267, 4271, 384),
                Team = 3,
                Id = 7,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-2981, 4591, 384),
                StackPosition = new Vector3(-3248, 5993, 384),
                WaitPosition = new Vector3(-3050, 4897, 384),
                WaitPositionN = new Vector3(-3077, 5111, 384),
                Team = 3,
                Id = 8,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-392, 3652, 384),
                StackPosition = new Vector3(-224, 5088, 384),
                WaitPosition = new Vector3(-503, 3955, 384),
                WaitPositionN = new Vector3(-512, 4314, 384),
                Team = 3,
                Id = 9,
                Empty = false,
                Stacked = false,
                Starttime = 55
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-1524, 2641, 256),
                StackPosition = new Vector3(-1266, 4273, 384),
                WaitPosition = new Vector3(-1465, 2908, 256),
                WaitPositionN = new Vector3(-1508, 3328, 256),
                Team = 3,
                Id = 10,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(1098, 3338, 384),
                StackPosition = new Vector3(910, 5003, 384),
                WaitPosition = new Vector3(975, 3586, 384),
                WaitPositionN = new Vector3(983, 3949, 383),
                Team = 3,
                Id = 11,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(4141, 554, 384),
                StackPosition = new Vector3(2987, -2, 384),
                WaitPosition = new Vector3(3876, 506, 384),
                WaitPositionN = new Vector3(3152, 586, 384),
                Team = 3,
                Id = 12,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(-2960, -126, 384),
                StackPosition = new Vector3(-3850, -1491, 384),
                WaitPosition = new Vector3(-2777, -230, 384),
                WaitPositionN = new Vector3(-2340, -517, 384),
                Team = 2,
                Id = 13,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });
            Camps.Add(new JungleCamps
            {
                Position = new Vector3(4000, -700, 256),
                StackPosition = new Vector3(1713, -134, 256),
                WaitPosition = new Vector3(3649, -721, 256),
                WaitPositionN = new Vector3(3589, -141, 384),
                Team = 3,
                Id = 14,
                Empty = false,
                Stacked = false,
                Starttime = 53
            });

            Events.OnLoad += Events_OnLoad;
            Events.OnClose += Events_OnClose;
        }

        private static void Events_OnClose(object sender, EventArgs e)
        {
            Game.OnUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            Game.OnWndProc -= Game_OnWndProc;
            Game.OnUpdate -= Game_Stack;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
            _load = false;
        }

        private static void Events_OnLoad(object sender, EventArgs e)
        {
            _load = false;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_Stack;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;

            Menu.AddItem(new MenuItem("FPS", "More Fps").SetValue(false));
            Menu.AddItem(new MenuItem("Stack", "Stack").SetValue(new KeyBind('F', KeyBindType.Toggle)));
            Menu.AddItem(new MenuItem("drawline", "Draw line?").SetValue(false));
            Menu.AddToMainMenu();
            OnLoadMessage();
            _me = ObjectManager.LocalHero;
        }
        
        private static void Game_Stack(EventArgs args)
        {
            if (!Menu.Item("Stack").GetValue<KeyBind>().Active || !Game.IsInGame || _me == null || Game.IsPaused ||
                Game.IsChatOpen) return;

            foreach (var camp in Camps)
            {
                if (_seconds == 1)
                {
                    camp.Empty = false;
                    camp.Unit = null;
                    camp.State = 0;
                }
                if (_seconds > 47 && camp.Unit == null) camp.State = 5;
                if (camp.Unit == null) continue;
                if (camp.Unit.IsAlive) continue;
                camp.Unit = null;
                camp.State = 0;
            }
            if (!Camps.Any(camp => camp.Stacked && camp.Unit != null)) return;
            foreach (var camp in Camps.Where(camp => camp.Stacked && camp.Unit != null))
            {
                var unit = camp.Unit;
                if (!Utils.SleepCheck("wait" + unit.Handle)) continue;
                var time =
                    (int)
                        (camp.Starttime - unit.Distance2D(camp.WaitPosition)/unit.MovementSpeed - 5 + Game.Ping/1000);
                switch (camp.State)
                {
                    case 0:
                        if (_seconds < time) continue;
                        unit.Move(camp.WaitPosition);
                        camp.State = 1;
                        Utils.Sleep(500, "wait" + unit.Handle);
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
                        if (_seconds < time) continue;
                        if (unit.Distance2D(camp.WaitPosition) < 10)
                            camp.State = 2;
                        unit.Move(camp.WaitPosition);
                        Utils.Sleep(500, "wait" + unit.Handle);
                        break;
                    case 2:
                        if (_seconds >= camp.Starttime - 5)
                        {
                            closestNeutral = GetNearestCreepToPull(unit, 800);
                            creepscount = CreepCount(unit, 800);
                            var creeps = creepscount >= 6 ? creepscount*75/1000 : 0;
                            float distance = 0;
                            if (unit.AttackRange < unit.Distance2D(closestNeutral))
                            {
                                distance = (unit.Distance2D(closestNeutral) - unit.AttackRange)/unit.MovementSpeed;
                            }
                            camp.AttackTime =
                                (int)
                                    Math.Round(camp.Starttime - creeps - distance -
                                               (unit.IsRanged ? unit.SecondsPerAttack - unit.BaseAttackTime/3 : 0));
                            camp.State = 3;
                            unit.Move(PositionCalc(unit, closestNeutral));
                        }
                        Utils.Sleep(500, "wait" + unit.Handle);
                        break;
                    case 3:
                        if (_seconds >= camp.AttackTime)
                        {
                            closestNeutral = GetNearestCreepToPull(unit, 800);
                            float distance = 0;
                            if (unit.AttackRange < unit.Distance2D(closestNeutral))
                            {
                                distance = (unit.Distance2D(closestNeutral) - unit.AttackRange) / unit.MovementSpeed;
                            }
                            var tWait = (distance + unit.SecondsPerAttack - unit.BaseAttackTime/3) * 1000 + Game.Ping;
                            unit.Attack(closestNeutral);
                            Utils.Sleep(tWait, "twait" + unit.Handle);
                            camp.State = 4;
                        }
                        break;
                    case 4:
                        if (unit.Distance2D(closestNeutral) <= 150 || Utils.SleepCheck("twait" + unit.Handle))
                        {
                            unit.Stop();
                            unit.Move(camp.StackPosition);
                            camp.State = 6;
                        }
                        break;
                    case 6:
                        if (_seconds == 0)
                        {
                            unit.Move(camp.WaitPositionN);
                            camp.State = 0;
                        }
                        Utils.Sleep(1000, "wait" + unit.Handle);
                        break;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame || _me == null || Game.IsPaused || Game.IsChatOpen ||
                !Menu.Item("Stack").GetValue<KeyBind>().Active)
            {
                return;
            }

            _seconds = (int) Game.GameTime%60;

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
                            new AbilityToggler(new Dictionary<string, bool> {{"item_manta", true}})));
                }
                _enable = _subMenu1.GetValue<AbilityToggler>().IsEnabled("item_manta");
                if (GetFurtherCamp(_me).Id != 0)
                {
                    var time =
                        (int)
                            (GetFurtherCamp(_me).Starttime -
                             _me.Distance2D(GetFurtherCamp(_me).WaitPosition)/_me.MovementSpeed - 5 +
                             Game.Ping/1000);
                    if (_enable && _manta.CanBeCasted() && Utils.SleepCheck("manta") && time < _seconds && time > 40 &&
                        _seconds < time + 5)
                    {
                        _manta.UseAbility();
                        Utils.Sleep(150 + Game.Ping, "manta");
                    }
                }
            }
            if (Utils.SleepCheck("Cooldown") && _seconds < 45)
            {
                try
                {
                    var baseNpcCreeps =
                        ObjectManager.GetEntities<Unit>()
                            .Where(
                                x =>
                                    x.IsAlive && x.Team == _me.Team && x.IsControllable &&
                                    !x.Name.Contains("beastmaster_hawk") &&
                                    (x.Modifiers.Any(m =>m.Name == "modifier_kill" && (int) (m.Duration - m.ElapsedTime + _seconds) >= 60) ||
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
                                      R.Level > 0)))
                            .ToList();
                    GetClosestCamp(baseNpcCreeps);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error baseNpcCreeps");
                }

                Utils.Sleep(1000, "Cooldown");
            }

            switch (_me.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_Lycan:
                    if (!_load)
                    {
                        _subMenu0 =
                            Menu.AddItem(
                                new MenuItem("enable", "Autouse? ").SetValue(
                                    new AbilityToggler(new Dictionary<string, bool> {{"lycan_summon_wolves", true}})));
                        _load = true;
                    }
                    _enable = _subMenu0.GetValue<AbilityToggler>().IsEnabled("lycan_summon_wolves");
                    if (GetFurtherCamp(_me).Id != 0)
                    {
                        var time =
                            (int)
                                (GetFurtherCamp(_me).Starttime - _me.Distance2D(GetFurtherCamp(_me).WaitPosition)/440 -
                                 5 + Game.Ping/1000);

                        if (_enable && Q.CanBeCasted() && Utils.SleepCheck("Q") && Q != null && time < _seconds &&
                            time > 5 && _seconds < time + 5)
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
                                    new AbilityToggler(new Dictionary<string, bool> {{"naga_siren_mirror_image", true}})));
                        _load = true;
                    }
                    _enable = _subMenu0.GetValue<AbilityToggler>().IsEnabled("naga_siren_mirror_image");
                    if (GetFurtherCamp(_me).Id != 0)
                    {
                        var time =
                            (int)
                                (GetFurtherCamp(_me).Starttime -
                                 _me.Distance2D(GetFurtherCamp(_me).WaitPosition)/_me.MovementSpeed - 5 +
                                 Game.Ping/1000);

                        if (_enable && Q.CanBeCasted() && Utils.SleepCheck("Q") && Q != null && time < _seconds &&
                            time > 30 && _seconds < time + 5)
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
                //Console.WriteLine("Vector3({0}, {1}, {2}),",
                //    (int) Game.MousePosition.X,
                //    (int) Game.MousePosition.Y,
                //    (int) Game.MousePosition.Z);
                foreach (var camp in from camp in Camps
                    let position = Drawing.WorldToScreen(camp.Position)
                    where Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 40, 40)
                    select camp)
                {
                    camp.Stacked = camp.Stacked == false ? true : false;
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

        private static void DrawShadowText(string stext, int x, int y, Color color, Font f)
        {
            f.DrawText(null, stext, x + 1, y + 1, Color.Black);
            f.DrawText(null, stext, x, y, color);
        }

        private static void DrawFilledBox(float x, float y, float w, float h, Color color)
        {
            var vLine = new Vector2[2];

            _line.GLLines = true;
            _line.Antialias = false;
            _line.Width = w;

            vLine[0].X = x + w / 2;
            vLine[0].Y = y;
            vLine[1].X = x + w / 2;
            vLine[1].Y = y + h;

            _line.Begin();
            _line.Draw(vLine, color);
            _line.End();
        }

        private static void RoundedRectangle(float x, float y, float w, float h, int iSmooth, Color color)
        {
            var pt = new Vector2[4];
            var vLine = new Vector2[2];
            // Get all corners 
            pt[0].X = x + (w - iSmooth);
            pt[0].Y = y + (h - iSmooth);

            pt[1].X = x + iSmooth;
            pt[1].Y = y + (h - iSmooth);

            pt[2].X = x + iSmooth;
            pt[2].Y = y + iSmooth;

            pt[3].X = x + w - iSmooth;
            pt[3].Y = y + iSmooth;

            DrawFilledBox(x+1, y + iSmooth, w-1, h - iSmooth * 2, color);
            DrawFilledBox(x + iSmooth, y+1, w - iSmooth * 2, h-1, color);
            float fDegree = 0;
            _line.Width = 1;
            for (var i = 0; i < 4; i++)
            {
                for (var k = fDegree; k < fDegree + Math.PI * 2 / 4f; k += (float) (1 * (Math.PI / 180.0f)))
                {
                    vLine[0].X = pt[i].X;
                    vLine[0].Y = pt[i].Y;
                    vLine[1].X = pt[i].X + (float) (Math.Cos(k) * iSmooth);
                    vLine[1].Y = pt[i].Y + (float) (Math.Sin(k) * iSmooth);
                    // Draw quarter circles on every corner
                    _line.Begin();
                    _line.Draw(vLine, color);
                    _line.End();
                }

                fDegree += (float) (Math.PI * 2) / 4; // quarter circle offset 
            }
        }

        private static void Drawing_OnPostReset(EventArgs args)
        {
            _text.OnResetDevice();
        }

        private static void Drawing_OnPreReset(EventArgs args)
        {
            _text.OnLostDevice();
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed 
                || !Game.IsInGame || !Menu.Item("Stack").GetValue<KeyBind>().Active)
                return;

            foreach (var camp in Camps)
            {
                var position = Drawing.WorldToScreen(camp.Position);
                var text = "✖";
                var color = Color.DarkRed;
                if (camp.Stacked)
                {
                    text = "✔";
                    color = Color.DarkGreen;
                }
                var alpha3 = Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 40, 40) ? 50 : 0;
                if (position.Y < 840 && position.Y > 43)
                {
                    if (!Menu.Item("FPS").GetValue<bool>())
                        RoundedRectangle(position.X, position.Y, 30, 30, 10, new Color(100, 100 + alpha3, 100 - alpha3));
                    DrawShadowText(text, (int) position.X + 5, (int) position.Y - 4, color, _text);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //try
            //{
            //    _seconds = ((int) Game.GameTime) % 60;
            //    var pos = Drawing.WorldToScreen(Game.MousePosition);
            //    var unit = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => x.Distance2D(Game.MousePosition) < 50);
            //    if (unit != null)
            //        Drawing.DrawText(unit.Team + "", "", new Vector2(pos.X, pos.Y + 20), new Vector2(40), Color.AliceBlue, FontFlags.Outline);
            //}
            //catch (Exception)
            //{
            //}
            if (!Menu.Item("Stack").GetValue<KeyBind>().Active) return;
            foreach (var camp in Camps)
            {
                var position = Drawing.WorldToScreen(camp.Position);
                var alpha3 = Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 30, 30) ? 100 : 0;
                if (camp.Unit != null && Menu.Item("drawline").GetValue<bool>())
                {
                    //unittext = camp.Unit.Handle.ToString();
                    Drawing.DrawLine(position, Drawing.WorldToScreen(camp.Unit.Position), Color.GreenYellow);
                    //Drawing.DrawText(camp.Unit.ClassID.ToString(), "", new Vector2(position.X + 38, position.Y - 3), new Vector2(40), color, FontFlags.Outline);
                }
                //Drawing.DrawText(text, "", new Vector2(position.X + 8, position.Y - 3), new Vector2(40), color,
                //    FontFlags.Outline);
                //Drawing.DrawLine(position, Drawing.WorldToScreen(camp.WaitPosition), Color.DarkRed);
                //Drawing.DrawLine(Drawing.WorldToScreen(camp.StackPosition), Drawing.WorldToScreen(camp.WaitPosition), Color.DarkRed);
                //Drawing.DrawLine(Drawing.WorldToScreen(camp.WaitPositionN), Drawing.WorldToScreen(camp.WaitPosition), Color.DarkGreen);
                //Drawing.DrawText(camp.Id.ToString(), "", new Vector2(position.X - 20, position.Y - 3), new Vector2(40), color, FontFlags.Outline);
                //var mposition = Game.MouseScreenPosition;
                //Drawing.DrawText(mposition.ToString(), "", new Vector2(mposition.X - 20, mposition.Y - 3), new Vector2(40), Color.DarkGreen, FontFlags.Outline);
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
                            closest[0] = camp;
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
                try
                {
                    var bid = 0;
                    var bwaitPosition = new Vector3();
                    var cont = true;
                    foreach (
                        var camp in
                            Camps.Where(x => x.Stacked)
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
                        Camps.First(x => GetClosestCampu(baseNpcCreep, 1).Id == x.Id).Unit = baseNpcCreep;
                    }
                    else if (num2 > 0 && num1 - num2 < num3)
                    {
                        if (!Unittrue(baseNpcCreep)) continue;
                        Camps.First(x => GetClosestCampu(baseNpcCreep, 1).Id == x.Id).Unit = baseNpcCreep;
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
                            Camps.First(x => x.Id == id).Unit = baseNpcCreep;
                            Camps.First(x => x.Id == bid).Unit = unit;
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
                            Camps.First(x => x.Id == id).Unit = baseNpcCreep;
                            Camps.First(x => x.Id == bid).Unit = unit;
                        }
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine("Error GetClosestCamp");
                }
            }
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

        private static Unit GetNearestCreepToPull(Unit h, float radius)
        {
            var neutrals =
                ObjectManager.GetEntities<Unit>()
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

        private static uint GetCreepStackAttackRange(Unit h, float radius)
        {
            var neutrals =
                ObjectManager.GetEntities<Unit>()
                    .Where(x => x.Team == Team.Neutral && x.IsSpawned && x.IsVisible && h.Distance2D(x) <= radius)
                    .ToList();
            uint[] ar = {uint.MinValue};
            foreach (var neutral in neutrals.Where(neutral => neutral.AttackRange > ar[0]))
            {
                ar[0] = neutral.AttackRange;
            }
            return ar[0];
        }

        private static Vector3 PositionCalc(Unit me, Unit target)
        {
            var l = (me.Distance2D(target) - me.Distance2D(target) + 2)/(me.Distance2D(target) - 2);
            var posA = me.Position;
            var posB = target.Position;
            var x = (posA.X + l*posB.X)/(1 + l);
            var y = (posA.Y + l*posB.Y)/(1 + l);
            return new Vector3((int) x, (int) y, posA.Z);
        }

    }
}