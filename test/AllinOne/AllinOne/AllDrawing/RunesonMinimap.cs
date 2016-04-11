namespace AllinOne.AllDrawing
{
    using Ensage;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class RunesOnMinimap
    {
        private static readonly Dictionary<RuneType, string> RuneType = new Dictionary<RuneType, string>
        {
            {Ensage.RuneType.Arcane,"materials/ensage_ui/runes/arcane.vmat" },
            {Ensage.RuneType.Bounty,"materials/ensage_ui/runes/bounty.vmat" },
            {Ensage.RuneType.DoubleDamage,"materials/ensage_ui/runes/doubledamage.vmat" },
            {Ensage.RuneType.Haste,"materials/ensage_ui/runes/haste.vmat" },
            {Ensage.RuneType.Illusion,"materials/ensage_ui/runes/illusion.vmat" },
            {Ensage.RuneType.Invisibility,"materials/ensage_ui/runes/invis.vmat" },
            {Ensage.RuneType.Regeneration,"materials/ensage_ui/runes/regen.vmat" }
        };

        public static void Draw()
        {
            try
            {
                var botRune = AllinOne.ObjectManager.Runes.BotRune;
                var topRune = AllinOne.ObjectManager.Runes.TopRune;
                if (botRune != null)
                    Drawing.DrawRect(new Vector2(193, 970), new Vector2(28, 28),
                        Drawing.GetTexture(RuneType[botRune.RuneType]));
                if (topRune != null)
                    Drawing.DrawRect(new Vector2(100, 895), new Vector2(28, 28),
                        Drawing.GetTexture(RuneType[topRune.RuneType]));
            }
            catch (Exception)
            {
                //
            }
        }
    }
}