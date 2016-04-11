namespace AllinOne.AllDrawing
{
    using AllinOne.ObjectManager;
    using AllinOne.Variables;
    using Ensage;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class MyHero
    {
        public static void RadiusHeroParticleEffect(string s, float range, Color color, bool xx)
        {
            if (xx && !Var.RadiusHeroParticleEffect.ContainsKey(s))
            {
                var particle = Var.Me.AddParticleEffect(@"particles\ui_mouseactions\selected_ring.vpcf");
                particle.SetControlPoint(1, new Vector3(color.R, color.G, color.B));
                particle.SetControlPoint(2, new Vector3(range + Var.Me.HullRadius, 255, 0));
                particle.SetControlPoint(3, new Vector3(20, 0, 0));
                Var.RadiusHeroParticleEffect.Add(s, null);
            }
            else if (!xx && Var.RadiusHeroParticleEffect.ContainsKey(s))
            {
                if (Var.RadiusHeroParticleEffect[s] == null) return;
                Var.RadiusHeroParticleEffect[s].Dispose();
                Var.RadiusHeroParticleEffect[s] = null;
            }
            else if (!xx && !Var.RadiusHeroParticleEffect.ContainsKey(s))
            {
                //
            }
            else if (Var.RadiusHeroParticleEffect[s] == null)
            {
                Var.RadiusHeroParticleEffect[s] =
                    Var.Me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                Var.RadiusHeroParticleEffect[s].SetControlPoint(1, new Vector3(color.R, color.G, color.B));
                Var.RadiusHeroParticleEffect[s].SetControlPoint(2,
                    new Vector3(range + Var.Me.HullRadius, 255, 0));
                Var.RadiusHeroParticleEffect[s].SetControlPoint(3, new Vector3(20, 0, 0));
            }
            else if (Math.Abs(Var.RadiusHeroParticleEffect[s].GetControlPoint(2).X - (range + Var.Me.HullRadius)) > 0)
            {
                Var.RadiusHeroParticleEffect[s].Dispose();
                Var.RadiusHeroParticleEffect[s] =
                    Var.Me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                Var.RadiusHeroParticleEffect[s].SetControlPoint(1, new Vector3(color.R, color.G, color.B));
                Var.RadiusHeroParticleEffect[s].SetControlPoint(2,
                    new Vector3(range + Var.Me.HullRadius, 255, 0));
                Var.RadiusHeroParticleEffect[s].SetControlPoint(3, new Vector3(20, 0, 0));
            }
            else if (!Var.Me.IsAlive)
            {
                Var.RadiusHeroParticleEffect[s].Dispose();
                Var.RadiusHeroParticleEffect[s] = null;
            }
        }
    }
}