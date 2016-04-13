using AllinOne.Menu;
using Ensage.Common;
using SharpDX;

namespace AllinOne.Methods
{
    using AllinOne.ObjectManager;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common.Extensions;
    using System;
    using System.Linq;

    internal class Common
    {
        #region Fields

        private static readonly int MinimapCorner = (int) Math.Floor(11.0 * Drawing.Height / 1080);
        private static readonly int MinimapHeight = (int) Math.Floor(260.0 * Drawing.Height / 1080);
        private static readonly int MinimapWidth = (int) Math.Floor(270.0 * Drawing.Height / 1080);

        #endregion Fields

        #region Methods

        public static void Autoattack(int a)
        {
            //if (Game.GetConsoleVar("dota_player_units_auto_attack_mode").GetInt() != aa)
            Game.ExecuteCommand("dota_player_units_auto_attack_mode " + a);
        }

        public static void AutoattackSummons(int a)
        {
            //if (Game.GetConsoleVar("dota_player_units_auto_attack_mode").GetInt() != aa)
            Game.ExecuteCommand("dota_summoned_units_auto_attack_mode " + a);
        }

        public static double FindRet(Vector3 first, Vector3 second)
        {
            var xAngle = Utils.RadianToDegree(Math.Atan(Math.Abs(second.X - first.X) / Math.Abs(second.Y - first.Y)));
            if (first.X <= second.X && first.Y >= second.Y)
            {
                return 270 + xAngle;
            }
            if (first.X >= second.X & first.Y >= second.Y)
            {
                return 90 - xAngle + 180;
            }
            if (first.X >= second.X && first.Y <= second.Y)
            {
                return 90 + xAngle;
            }
            if (first.X <= second.X && first.Y <= second.Y)
            {
                return 90 - xAngle;
            }
            return 0;
        }

        public static Vector3 FindVector(Vector3 first, double ret, float distance)
        {
            var retVector = new Vector3(first.X + (float) Math.Cos(Utils.DegreeToRadian(ret)) * distance,
                first.Y + (float) Math.Sin(Utils.DegreeToRadian(ret)) * distance, 100);

            return retVector;
        }

        public static void FireEvent(FireEventEventArgs args)
        {
            switch (args.GameEvent.Name)
            {
                case "dota_tower_kill":
                    Towers.TowerDestroyed();
                    break;

                case "dota_roshan_kill":
                    ShowMeMore.RoshanKill();
                    break;
            }
        }

        public static void GenerateSideMessage(string hero, string spellName)
        {
            var msg = new SideMessage(hero, new Vector2(200, 60));
            msg.AddElement(new Vector2(006, 11), new Vector2(72, 36),
                Drawing.GetTexture("materials/ensage_ui/heroes_horizontal/" + hero + ".vmat"));
            msg.AddElement(new Vector2(078, 17), new Vector2(64, 32),
                Drawing.GetTexture("materials/ensage_ui/other/arrow_usual.vmat"));
            msg.AddElement(new Vector2(142, 11), new Vector2(72, 36),
                Drawing.GetTexture("materials/ensage_ui/spellicons/" + spellName + ".vmat"));
            msg.CreateMessage();
        }

        public static float GetCooldown(Hero hero, Ability ability)
        {
            try
            {
                var cooldown = ability.GetCooldown(ability.Level - 1);
                if (Math.Abs(cooldown) <= 0) cooldown = ability.GetCooldown(0);
                if (hero.FindItem("item_octarine_core") != null)
                    cooldown *= (float) 0.75;
                return cooldown;
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("Error GetCooldown for " + ability.Name);
            }
            return 0;
        }

        public static float GetOutRange(Unit unit)
        {
            if (unit.Handle == Var.Me.Handle)
            {
                return MyHeroInfo.AttackRange() + MenuVar.Outrange;
            }
            else
            {
                if (unit.IsRanged)
                    return 500 + MenuVar.Outrange;
                else
                    return 200 + MenuVar.Outrange;
            }
        }

        public static string HeroIcon(Entity entity)
        {
            return "materials/ensage_ui/heroes_horizontal/" + entity.Name.Replace("npc_dota_hero_", "") + ".vmat";
        }

        public static string MinimapIcon(Entity entity)
        {
            return "materials/ensage_ui/miniheroes/" + entity.Name.Replace("npc_dota_hero_", "") + ".vmat";
        }

        public static void Print(string s, MessageType chatMessage = MessageType.ChatMessage)
        {
            if (true)
                Game.PrintMessage("[AllinOne]: " + s, chatMessage);
        }

        public static void PrintEncolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }

        public static void Sleep(float time, string s)
        {
            if (Var.SleepDic.Count == 0 || Var.SleepDic.All(x => x.Text != s))
            {
                Var.SleepDic.Add(new DictionarySleep { Text = s, Time = DateTime.UtcNow.ToFileTime(), Period = time * 10000 });
            }
            else
            {
                Var.SleepDic.First(x => x.Text == s).Time = DateTime.UtcNow.ToFileTime();
                Var.SleepDic.First(x => x.Text == s).Period = time * 10000;
            }
        }

        public static bool SleepCheck(string s)
        {
            try
            {
                if (Var.SleepDic.Any(x => x.Text == s))
                {
                    var sleep = Var.SleepDic.First(x => x.Text == s);
                    if (Math.Abs(DateTime.UtcNow.ToFileTime() - sleep.Time) >= sleep.Period)
                    {
                        Var.SleepDic.Remove(sleep);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("SleepCheck Error");
            }
            return true;
        }

        public static void UpdateAttackableSummons()
        {
            try
            {
                var summons = ObjectManager.GetEntities<Unit>().Where(
                    x =>
                        (x.Distance2D(Var.Me) < 1500 || MenuVar.SummonsAutoFarm || MenuVar.SummonsAutoLasthit) &&
                        x.IsAlive && x.Team == Var.Me.Team && x.IsControllable &&
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
                         (x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo && Var.Me.Handle != x.Handle &&
                          Var.Me.Spellbook.Spell4.Level > 0)))
                    .ToList();
                foreach (var summon in summons)
                {
                    if (Var.Summons.Count == 0 || Var.Summons.All(x => x.Key.Handle != summon.Handle))
                    {
                        Var.Summons.Add(summon, Lasthit.GetNearestCreep(summon, GetOutRange(summon)));
                    }
                    else
                    {
                        Var.Summons[summon] = Lasthit.GetNearestCreep(summon, GetOutRange(summon));
                    }
                }
                Var.StackableSummons = Var.Summons.Keys.Where(
                    x =>
                        x.IsAlive && x.Team == Var.Me.Team && x.IsControllable &&
                        !x.Name.Contains("beastmaster_hawk") &&
                        (x.Modifiers.Any(
                            m => m.Name == "modifier_kill" && (int) (m.Duration - m.ElapsedTime + Var.Seconds) >= 60) ||
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
                         (x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo && Var.Me.Handle != x.Handle &&
                          Var.R.Level > 0))).ToList();
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("UpdateSummons Error");
            }
        }

        public static Vector2 WorldToMinimap(Vector3 vector, Entity iconEntity = null)
        {
            if (iconEntity != null && (iconEntity.IsVisible || !iconEntity.IsAlive)) return new Vector2();
            int x = (int) Math.Floor((vector.X + 7500) * MinimapWidth / 15000);
            int y = (int) Math.Floor((vector.Y + 7000) * MinimapHeight / 14000);
            return new Vector2(x + MinimapCorner, Drawing.Height - y - MinimapCorner - 7);
        }

        public static Vector2 WorldToMinimap(Vector2 vector, Entity iconEntity = null)
        {
            if (iconEntity != null && (iconEntity.IsVisible || !iconEntity.IsAlive)) return new Vector2();
            int x = (int) Math.Floor((vector.X + 7500) * MinimapWidth / 15000);
            int y = (int) Math.Floor((vector.Y + 7000) * MinimapHeight / 14000);
            return new Vector2(x + MinimapCorner, Drawing.Height - y - MinimapCorner - 7);
        }

        #endregion Methods
    }
}