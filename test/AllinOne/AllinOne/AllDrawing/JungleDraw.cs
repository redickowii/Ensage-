namespace AllinOne.AllDrawing
{
    using AllinOne.Menu;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class JungleDraw
    {
        #region Fields

        private static readonly Dictionary<Unit, ParticleEffect> CampUnitLine = new Dictionary<Unit, ParticleEffect>();

        #endregion Fields

        #region Methods

        public static void Clear()
        {
            if (CampUnitLine.Count > 0)
            {
                CampUnitLine.ForEach(x => x.Value.Dispose());
                CampUnitLine.Clear();
            }
        }

        public static void DrawCamp()
        {
            foreach (var camp in Var.Camps)
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
                    if (!MenuVar.MoreFps)
                        Draw.RoundedRectangle(position.X, position.Y, 30, 30, 10, new Color(100, 100 + alpha3, 100 - alpha3));
                    Draw.DrawShadowText(text, (int) position.X + 7, (int) position.Y + 3, color, Fonts.StackFont);
                }
            }
        }

        public static void DrawLine()
        {
            try
            {
                foreach (var camp in Var.Camps)
                {
                    var position = Drawing.WorldToScreen(camp.Position);
                    var alpha3 = Utils.IsUnderRectangle(Game.MouseScreenPosition, position.X, position.Y, 30, 30) ? 100 : 0;
                    if (camp.Unit != null && MenuVar.DrawStackLine)
                    {
                        ParticleEffect rr;
                        if (CampUnitLine.ContainsKey(camp.Unit))
                        {
                            CampUnitLine[camp.Unit].SetControlPoint(1, camp.Unit.Position);
                            CampUnitLine[camp.Unit].SetControlPoint(2, camp.Position);
                        }
                        else
                        {
                            rr = camp.Unit.AddParticleEffect(Particles.Partlist[81]);
                            rr.SetControlPoint(1, camp.Unit.Position);
                            rr.SetControlPoint(2, camp.Position);
                            CampUnitLine.Add(camp.Unit, rr);
                        }
                    }
                }
                if (CampUnitLine.Count > Var.Camps.Count(x => x.Unit != null))
                {
                    CampUnitLine.ForEach(x => x.Value.Dispose());
                    CampUnitLine.Clear();
                }
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("Draw Line Stack Error");
            }
        }

        #endregion Methods
    }
}