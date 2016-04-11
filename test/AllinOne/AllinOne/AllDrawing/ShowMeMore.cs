namespace AllinOne.AllDrawing
{
    using AllinOne.Menu;
    using AllinOne.Methods;
    using AllinOne.ObjectManager.Heroes;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ShowMeMore
    {
        #region Fields

        public static readonly Dictionary<Unit, List<ParticleEffect>> Effects = new Dictionary<Unit, List<ParticleEffect>>();

        public static readonly Dictionary<Unit, List<ParticleEffect>> EffectsTest = new Dictionary<Unit, List<ParticleEffect>>();

        private static readonly Dictionary<Unit, ParticleEffect> BaraIndicator = new Dictionary<Unit, ParticleEffect>();

        public static readonly Dictionary<Entity, Vector3> Mapicon = new Dictionary<Entity, Vector3>();

        public static Dictionary<Entity, Vector3> Courier = new Dictionary<Entity, Vector3>();

        #endregion Fields

        #region Methods

        public static void ClearParticle()
        {
            Effects.ForEach(x => x.Value.ForEach(y => y.Dispose()));
            Effects.Clear();
        }

        public static void DrawLastPosition(Hero hero)
        {
            var miniicon = Common.MinimapIcon(hero);
            var icon = Common.HeroIcon(hero);
            var size = new Vector2(50, 30);
            var minisize = new Vector2(28, 28);
            if (hero.IsVisible || !hero.IsAlive)
            {
                Mapicon.Remove(hero);
                return;
            }
            if (Mapicon.ContainsKey(hero))
            {
                Drawing.DrawRect(Drawing.WorldToScreen(Mapicon[hero]), size, Drawing.GetTexture(icon));
                //if (MenuVar.ShowLastPosMini ||)
                Drawing.DrawRect(Common.WorldToMinimap(Mapicon[hero], hero), minisize, Drawing.GetTexture(miniicon));
            }
            else
            {
                Vector2 newPos;
                if (!Drawing.WorldToScreen(hero.Position, out newPos)) return;
                Drawing.DrawRect(newPos, size, Drawing.GetTexture(icon));
                if (MenuVar.ShowLastPosMini)
                    Drawing.DrawRect(Common.WorldToMinimap(hero.Position, hero), minisize, Drawing.GetTexture(miniicon));
            }
        }

        public static void DrawShowMeMoreBara(Hero v)
        {
            var mod = v.HasModifier("modifier_spirit_breaker_charge_of_darkness_vision");
            if (mod)
            {
                if (Equals(Var.Me, v))
                {
                    Drawing.DrawRect(new Vector2(0, 0), new Vector2(Drawing.Width, Drawing.Height),
                        new Color(255, 0, 0, 10));
                }
                ParticleEffect eff;
                if (BaraIndicator.TryGetValue(v, out eff)) return;
                eff = new ParticleEffect("", v);
                eff.SetControlPointEntity(1, v);
                BaraIndicator.Add(v, eff);
                Common.GenerateSideMessage(v.Name.Replace("npc_dota_hero_", ""),
                    "spirit_breaker_charge_of_darkness");
            }
            else
            {
                ParticleEffect eff;
                if (!BaraIndicator.TryGetValue(v, out eff)) return;
                eff.Dispose();
                BaraIndicator.Remove(v);
            }
        }

        public static void ShowHeroEffect(int selected)
        {
            foreach (var illusion in EnemyHeroes.Illusions)
            {
                List<ParticleEffect> effects;
                if (Effects.TryGetValue(EnemyHeroes.Heroes.First(x => x.Name == illusion.Name), out effects))
                {
                    effects.ForEach(x => x.Dispose());
                    effects.AddRange(Particles.HeroEffect[selected].Select(EnemyHeroes.Heroes.First(x => x.Name == illusion.Name).AddParticleEffect));
                }
            }
        }

        public static void ShowHeroEffectTest()
        {
            if (EffectsTest.ContainsKey(Var.Me)) return;
            List<ParticleEffect> effects = new List<ParticleEffect>();
            ParticleEffect rr;
            //for (int j = 75; j < 100; j++)
            //{
            rr = new ParticleEffect(Particles.Partlist[81], Var.Me.Position + new Vector3(0, 0, 0));
            rr.SetControlPoint(1, Var.Me.Position);
            rr.SetControlPoint(2, Var.Me.Position + new Vector3(300, 0, 0));
            effects.Add(rr);
            //}
            EffectsTest.Add(Var.Me, effects);
        }

        public static void ShowIllusion()
        {
            try
            {
                List<Unit> listUnit = new List<Unit>();
                foreach (var illusion in EnemyHeroes.Illusions)
                {
                    AddParticle(illusion, Particles.IllusionsEffect[MenuVar.IllusionsEffectMenu]);
                    AddParticle(EnemyHeroes.Heroes.First(x => x.Name == illusion.Name && x.Distance2D(illusion) < 1000),
                        Particles.HeroEffect[MenuVar.HeroEffectMenu]);
                }
                foreach (var effect in Effects)
                {
                    if (!EnemyHeroes.Illusions.Contains(effect.Key) &&
                        !EnemyHeroes.Heroes.Contains(effect.Key))
                    {
                        effect.Value.ForEach(x => x.Dispose());
                        effect.Value.Clear();
                        listUnit.Add(effect.Key);
                    }
                }
                listUnit.ForEach(x => Effects.Remove(x));
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("Draw Illusions");
            }
        }

        public static void ShowIllusion(int selected)
        {
            foreach (var illusion in EnemyHeroes.Illusions)
            {
                ReloadParticle(illusion, Particles.IllusionsEffect[selected]);
            }
        }

        public static void Test()
        {
            var pe = Ensage.ObjectManager.ParticleEffects;
            foreach (var p in pe)
            {
                Console.WriteLine(p.Name);
            }
        }

        public static void Update(EntityEventArgs args)
        {
            if (args.Entity.Name == "CDOTA_BaseNPC" &&
                args.Entity.Team != Var.Me.Team)
            {
                Common.Sleep(300, "CDOTA_BaseNPC");
            }
        }

        private static void AddParticle(Unit unit, List<string> ss)
        {
            if (Effects.ContainsKey(unit)) return;
            List<ParticleEffect> effect = new List<ParticleEffect>();
            effect.AddRange(ss.Select(unit.AddParticleEffect));
            Effects.Add(unit, effect);
        }

        private static void ReloadParticle(Unit unit, List<string> ss)
        {
            if (unit.IsVisibleToEnemies)
            {
                ClearParticle();
                List<ParticleEffect> effect = new List<ParticleEffect>();
                effect.AddRange(ss.Select(unit.AddParticleEffect));
                Effects.Add(unit, effect);
            }
            else
            {
                if (!Effects.ContainsKey(unit)) return;
                Effects.Remove(unit);
            }
        }

        #endregion Methods

        public static void RoshanTimer()
        {
            var text = "";
            if (!Methods.ShowMeMore.RoshIsAlive)
            {
                var min = Methods.ShowMeMore.RoshanMinutes;
                var sec = Methods.ShowMeMore.RoshanSeconds;
                if (min < 8)
                    text = string.Format("Roshan: {0}:{1:0.} - {2}:{3:0.}", 7 - min, 59 - sec,
                        10 - min,
                        59 - sec);
                else if (min == 8)
                {
                    text = string.Format("Roshan: {0}:{1:0.} - {2}:{3:0.}", 8 - min, 59 - sec,
                        10 - min,
                        59 - sec);
                }
                else if (min == 9)
                {
                    text = string.Format("Roshan: {0}:{1:0.} - {2}:{3:0.}", 9 - min, 59 - sec,
                        10 - min,
                        59 - sec);
                }
                else
                {
                    text = string.Format("Roshan: {0}:{1:0.}", 0, 59 - sec);
                    if (59 - sec <= 1)
                    {
                        Methods.ShowMeMore.RoshIsAlive = true;
                    }
                }
            }
            Draw.DrawShadowText(Methods.ShowMeMore.RoshIsAlive ? "Roshan alive" : Methods.ShowMeMore.RoshdeathTime == 0 ? "Roshan death" : text, 217, 10,
                Methods.ShowMeMore.RoshIsAlive ? Color.Green : Color.Red, Fonts.RoshanFont);
        }
    }
}