using System;

using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

using SharpDX;
using System.Linq;

namespace Creepstop
{
    internal class Program
    {
        private static readonly Menu Menu = new Menu("Creepstop", "Creepstop", true);
        private static Hero _me;
        private static Vector3 startingpoint;
        private static Vector3 startingpoint2;
        private static Vector3 endingpoint;
        private static double starttime, r;
        private static bool _firstmove = false;

        private static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("block", "block creep").SetValue(new KeyBind('6', KeyBindType.Press)));

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {

            _me = ObjectManager.LocalHero;
            if (!Game.IsInGame || _me == null)
            {
                return;
            }
            if (Game.IsPaused || Game.IsChatOpen)
            {
                return;
            }

            if (_me.Team == Team.Radiant)
            {
                startingpoint = new Vector3(-4781, -3969, 261);
                startingpoint2 = new Vector3(-4250, -3983, 273);
                endingpoint = new Vector3(-1159, -725, 132);
                starttime = 0.48;
            }
            else
            {
                startingpoint = new Vector3(3929, 3420, 263);
                startingpoint2 = new Vector3(3854, 3319, 191);
                endingpoint = new Vector3(116, 250, 127);
                starttime = 0.30;
            }                                                                                                 

            if (Game.IsKeyDown(Menu.Item("block").GetValue<KeyBind>().Key))
            {
                    if (Game.GameTime >=
                        (starttime - Game.Ping/1000 -
                         GetDistance2D(startingpoint, startingpoint2)/_me.MovementSpeed)/10)
                    {
                        if (_me.Distance2D(startingpoint2) < 10 || _me.Distance2D(endingpoint) < 4000)
                        {
                            _firstmove = true;
                        }
                        if (!_firstmove && Utils.SleepCheck("firstmove"))
                        {
                            _me.Move(startingpoint2);
                            Utils.Sleep(125, "firstmove");
                        }
                            var closestCreep = ObjectManager.GetEntities<Creep>()
                            .Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                                         || x.ClassID == ClassID.CDOTA_BaseNPC_Creep && x.IsAlive && x.IsVisible
                                         && x.Team != _me.Team && x.Distance2D(_me) < 500))
                            .OrderBy(creep => creep.Distance2D(endingpoint))
                            .DefaultIfEmpty(null)
                            .FirstOrDefault();
                        if (closestCreep != null && closestCreep.Distance2D(_me) < 350 && Utils.SleepCheck("wait"))
                        {
                            var creeprotR = closestCreep.RotationRad;
                            if ((creeprotR > 1.20 || creeprotR < 0.40) && _me.Team == Team.Radiant) creeprotR = (float)0.80;
                            if ((creeprotR > 4.1 || creeprotR < 3.4) && _me.Team == Team.Dire) creeprotR = (float)3.8;
                            /*if (_me.Distance2D(endingpoint) < 3000 && Utils.SleepCheck("r"))
                            {
                                if (r > 0) r = -0.3;
                                else r = 0.3;
                                creeprotR = creeprotR + (float)r;
                                Utils.Sleep(125, "r");
                            }*/
                            var p =
                                new Vector3(
                                    (float)(closestCreep.Position.X + Math.Max(_me.Distance2D(closestCreep) / closestCreep.MovementSpeed * 1000, 100) * Math.Cos(creeprotR)),
                                    (float)(closestCreep.Position.Y + Math.Max(_me.Distance2D(closestCreep) / closestCreep.MovementSpeed * 1000, 100) * Math.Sin(creeprotR)),
                                    closestCreep.Position.Z);
                            //Game.PrintMessage("Go " + p.X + " " + p.Y, MessageType.ChatMessage);
                            _me.Move(p);
                            if (_me.Distance2D(endingpoint) < 4600 &&
                                _me.Distance2D(closestCreep) > (25 + Game.Ping/1000) &&
                                (_me.Distance2D(endingpoint) + 50) < closestCreep.Distance2D(endingpoint) &&
                                Utils.SleepCheck("stop"))
                                {
                                    var stop = _me.Distance2D(closestCreep)/closestCreep.MovementSpeed*1000 + Game.Ping;
                                    //Game.PrintMessage("Stop " + (int)stop + " CreeprotR " + creeprotR, MessageType.ChatMessage);
                                    _me.Stop();
                                    Utils.Sleep(stop, "stop");
                                }
                            Utils.Sleep(50, "wait");
                    }
                }
                else if (Game.GameTime < 0 && _me.Distance2D(startingpoint) > 10 && Utils.SleepCheck("move"))
                    {
                        _me.Move(startingpoint);
                        Utils.Sleep(1000, "move");
                    }
            }
        }
        private static float GetDistance2D(Vector3 p1, Vector3 p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }
}
