namespace AllinOne.ObjectManager
{
    using AllinOne.Methods;
    using AllinOne.Variables;
    using Ensage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Runes
    {
        public static Rune TopRune;

        public static Rune BotRune;

        public static void Update()
        {
            if (Common.SleepCheck("RuneSlowTop"))
            {
                TopRune = null;
                Common.Sleep(10000, "RuneSlowTop");
            }
            if (Common.SleepCheck("RuneSlowBot"))
            {
                BotRune = null;
                Common.Sleep(10000, "RuneSlowBot");
            }
            try
            {
                var runes = Ensage.ObjectManager.GetEntities<Rune>().Where(x => x.RuneType != RuneType.None).ToList();
                foreach (var rune in runes)
                {
                    if (rune.Position.X < 0)
                    {
                        TopRune = rune;
                    }
                    else
                    {
                        BotRune = rune;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }

        public static void ChatTop()
        {
            var color = "#FF0000";
            switch (TopRune.RuneType)
            {
                case RuneType.Illusion:
                    color = "#cca300";
                    break;

                case RuneType.Arcane:
                    color = "#c61aff";
                    break;

                case RuneType.Bounty:
                    color = "#e65c00";
                    break;

                case RuneType.DoubleDamage:
                    color = "#3333ff";
                    break;

                case RuneType.Haste:
                    color = "#ff0000";
                    break;

                case RuneType.Invisibility:
                    color = "#808080";
                    break;

                case RuneType.Regeneration:
                    color = "#00cc00";
                    break;
            }
            Game.PrintMessage(
                "<font color='#00aaff'> Rune: <font color='" + color + "'>" + TopRune.RuneType +
                "<font color='#00aaff'> on <font color='#FF0000'>Top </font>",
                MessageType.LogMessage);
        }

        public static void ChatBot()
        {
            var color = "#FF0000";
            switch (TopRune.RuneType)
            {
                case RuneType.Illusion:
                    color = "#cca300";
                    break;

                case RuneType.Arcane:
                    color = "#c61aff";
                    break;

                case RuneType.Bounty:
                    color = "#e65c00";
                    break;

                case RuneType.DoubleDamage:
                    color = "#3333ff";
                    break;

                case RuneType.Haste:
                    color = "#ff0000";
                    break;

                case RuneType.Invisibility:
                    color = "#808080";
                    break;

                case RuneType.Regeneration:
                    color = "#00cc00";
                    break;
            }
            Game.PrintMessage(
                "<font color='#00aaff'> Rune: <font color='" + color + "'>" + BotRune.RuneType +
                "<font color='#00aaff'> on <font color='#FF0000'>Bot </font>",
                MessageType.LogMessage);
        }
    }
}