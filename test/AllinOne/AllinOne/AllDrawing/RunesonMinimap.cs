using AllinOne.Menu;
using AllinOne.Methods;

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
                var runescale = new Vector2(MenuVar.RuneScale, MenuVar.RuneScale);
                var botRune = AllinOne.ObjectManager.Runes.BotRune;
                var topRune = AllinOne.ObjectManager.Runes.TopRune;
                if (botRune != null)
                    Drawing.DrawRect(Common.WorldToMinimap(botRune.Position) - runescale / 3, runescale,
                        Drawing.GetTexture(RuneType[botRune.RuneType]));
                if (topRune != null)
                    Drawing.DrawRect(Common.WorldToMinimap(topRune.Position) - runescale / 3, runescale,
                        Drawing.GetTexture(RuneType[topRune.RuneType]));
            }
            catch (Exception)
            {
                //
            }
        }
    }
}