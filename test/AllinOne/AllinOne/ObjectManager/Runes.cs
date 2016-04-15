namespace AllinOne.ObjectManager
{
    using AllinOne.Methods;
    using Ensage;
    using System.Linq;

    internal class Runes
    {
        #region Fields

        public static Rune BotRune;
        public static Rune TopRune;

        #endregion Fields

        #region Methods

        public static void ChatBot()
        {
            if (TopRune != null) return;
            var color = "#FF0000";
            switch (BotRune.RuneType)
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

        public static void ChatTop()
        {
            if (TopRune != null) return;
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

        public static void Update()
        {
            if (Common.SleepCheck("RuneSlow"))
            {
                Common.Sleep(10000, "RuneSlow");
                TopRune = null;
                BotRune = null;
            }
            if (ObjectManager.GetEntities<Rune>().All(x => x.RuneType == RuneType.None)) return;
            var runes = ObjectManager.GetEntities<Rune>().Where(x => x.RuneType != RuneType.None).ToList();
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

        #endregion Methods
    }
}