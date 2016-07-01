namespace AllinOne.Methods
{
    using AllinOne.Menu;
    using AllinOne.ObjectManager;
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

        public static readonly Dictionary<Unit, ParticleEffect> Effects = new Dictionary<Unit, ParticleEffect>();

        public static readonly ShowMeMoreStruct[] ShowMeMoreEffects =
        {
            new ShowMeMoreStruct("modifier_invoker_sun_strike",
                "hero_invoker/invoker_sun_strike_team", 175 , 1700 -100),
            new ShowMeMoreStruct("modifier_lina_light_strike_array",
                "hero_lina/lina_spell_light_strike_array_ring_collapse", 225, 500 -100),
            new ShowMeMoreStruct( "modifier_kunkka_torrent_thinker",
                "hero_kunkka/kunkka_spell_torrent_pool", 225, 1600 -100),
            new ShowMeMoreStruct( "modifier_leshrac_split_earth_thinker",
                "hero_leshrac/leshrac_split_earth_b", 225, 350),
            new ShowMeMoreStruct( "modifier_skywrath_mage_mystic_flare",
                "hero_skywrath_mage/skywrath_mage_mystic_flare", 200, 0)
        };

        private static Unit _arrowUnit;
        public static Dictionary<Ability, ParticleEffect> EffectForSpells = new Dictionary<Ability, ParticleEffect>();
        public static double RoshanMinutes;
        public static double RoshanSeconds;
        public static float RoshdeathTime;
        public static bool RoshIsAlive;
        public static List<AoeSpellStruct> SpellRadius = new List<AoeSpellStruct>();
        private static Vector3 _arrowPos;

        #endregion Fields

        #region Methods

        public static void ClearEffectsVisible()
        {
            if (Effects.Count == 0) return;
            Effects.ForEach(x => x.Value.ForceDispose());
            Effects.Clear();
        }

        public static void DrawShowMeMoreSpells(Hero hero)
        {
            switch (hero.ClassID)
            {
                case ClassID.CDOTA_Unit_Hero_Windrunner:
                    if (Prediction.IsTurning(hero)) return;
                    var spell = hero.Spellbook.Spell2;
                    if (spell == null || spell.CanBeCasted()) return;
                    var cd = Math.Floor(spell.Cooldown / Common.GetCooldown(hero, spell) * 1000);
                    if (cd < 980)
                    {
                        if (!EffectForSpells.ContainsKey(spell) && cd > 800)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius));
                            EffectForSpells.Add(spell, effect);
                        }
                        else if ((!hero.IsAlive || cd <= 800) && EffectForSpells.ContainsKey(spell))
                        {
                            EffectForSpells[spell].ForceDispose();
                            EffectForSpells.Remove(spell);
                        }
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Mirana:
                    spell = hero.Spellbook.Spell2;
                    if (spell == null) return;
                    if (_arrowUnit != null && !Utils.SleepCheck("DrawArrrow") && Utils.SleepCheck("arrrowwait"))
                    {
                        if (_arrowUnit.IsValid && _arrowUnit.IsVisible)
                        {
                            if (!EffectForSpells.ContainsKey(spell))
                            {
                                var effect = new ParticleEffect(Particles.Partlist[83], _arrowUnit.Position);
                                effect.SetControlPoint(1, _arrowUnit.Position);
                                effect.SetControlPoint(2,
                                    Common.FindVector(_arrowPos, Common.FindRet(_arrowPos, _arrowUnit.Position), 3000 - _arrowUnit.Speed * _arrowUnit.CreateTime));
                                EffectForSpells.Add(spell, effect);
                            }
                        }
                    }
                    else if (Utils.SleepCheck("DrawArrrow") && EffectForSpells.ContainsKey(spell))
                    {
                        EffectForSpells[spell].ForceDispose();
                        EffectForSpells.Remove(spell);
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Pudge:
                    if (Prediction.IsTurning(hero)) return;
                    spell = hero.Spellbook.Spell1;
                    if (spell == null || spell.CanBeCasted()) return;
                    cd = Math.Floor(spell.Cooldown / Common.GetCooldown(hero, spell) * 1000);
                    if (cd < 990)
                    {
                        if (!EffectForSpells.ContainsKey(spell) && cd > 900)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius + 100));
                            EffectForSpells.Add(spell, effect);
                        }
                        else if ((!hero.IsAlive || cd <= 900) && EffectForSpells.ContainsKey(spell))
                        {
                            EffectForSpells[spell].ForceDispose();
                            EffectForSpells.Remove(spell);
                        }
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Puck:
                    if (Prediction.IsTurning(hero)) return;
                    spell = hero.Spellbook.Spell1;
                    var spell4 = hero.Spellbook.Spell4;
                    if (spell == null || spell.CanBeCasted()) return;
                    cd = Math.Floor(spell.Cooldown / Common.GetCooldown(hero, spell) * 1000);
                    if (cd < 995)
                    {
                        if (!EffectForSpells.ContainsKey(spell) && spell4.IsActivated)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius + 225));
                            EffectForSpells.Add(spell, effect);
                        }
                        else if ((!hero.IsAlive || !spell4.IsActivated || cd < 700) && EffectForSpells.ContainsKey(spell))
                        {
                            EffectForSpells[spell].ForceDispose();
                            EffectForSpells.Remove(spell);
                        }
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Jakiro:
                    spell = hero.Spellbook.Spell2;
                    if (spell != null)
                    {
                        if (spell.IsInAbilityPhase)
                        {
                            if (!EffectForSpells.ContainsKey(spell) && spell.CanBeCasted() &&
                                Common.Wait(220, 500, "wait_ice_path"))
                            {
                                var effect = new ParticleEffect(Particles.Partlist[81], hero.Position);
                                effect.SetControlPoint(1, hero.Position);
                                effect.SetControlPoint(2,
                                    Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius + 150));
                                EffectForSpells.Add(spell, effect);
                            }
                        }
                        else if (!hero.IsAlive || EffectForSpells.ContainsKey(spell))
                        {
                            EffectForSpells[spell].ForceDispose();
                            EffectForSpells.Remove(spell);
                        }
                    }
                    spell = hero.Spellbook.Spell4;
                    if (spell != null)
                    {
                        if (spell.IsInAbilityPhase || (spell.Cooldown > 0 && spell.GetCooldown(spell.Level - 1) - spell.Cooldown <= 10))
                        {
                            if (!EffectForSpells.ContainsKey(spell) && Common.Wait(220, 500, "wait_macropyre"))
                            {
                                var effect = new ParticleEffect(Particles.Partlist[81], hero.Position);
                                effect.SetControlPoint(1, hero.Position);
                                effect.SetControlPoint(2,
                                    Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius + 240));
                                EffectForSpells.Add(spell, effect);
                            }
                            else if (EffectForSpells.ContainsKey(spell) && Common.GetCooldown(hero, spell) - spell.Cooldown > 10)
                            {
                                EffectForSpells[spell].ForceDispose();
                                EffectForSpells.Remove(spell);
                            }
                        }
                        else if (!hero.IsAlive || EffectForSpells.ContainsKey(spell))
                        {
                            EffectForSpells[spell].ForceDispose();
                            EffectForSpells.Remove(spell);
                        }
                    }

                    break;

                case ClassID.CDOTA_Unit_Hero_Kunkka:
                    spell = hero.Spellbook.Spell4;
                    if (spell != null)
                    {
                        if (spell.IsInAbilityPhase)
                        {
                            if (!SpellRadius.Any(x => x.ContainsHero(hero)) && spell.CanBeCasted() &&
                                Common.Wait(220, 500, "wait_ship"))
                            {
                                var effect = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf",
                                    Common.FindVector(hero.Position, hero.Rotation, 1000));
                                effect.SetControlPoint(1, new Vector3(255, 0, 0));
                                effect.SetControlPoint(2, new Vector3(425, 255, 0));
                                SpellRadius.Add(new AoeSpellStruct(hero, Common.FindVector(hero.Position, hero.Rotation, 1000), "", effect, 425, 310 + 80));
                            }
                        }
                        else if (SpellRadius.Any(x => x.ContainsHero(hero)) && (Common.GetCooldown(hero, spell) - spell.Cooldown) * 100 > 310)
                        {
                            SpellRadius.First(x => x.ContainsHero(hero)).Effect.ForceDispose();
                            SpellRadius.RemoveAt(SpellRadius.FindIndex(x => x.ContainsHero(hero)));
                        }
                    }
                    break;
            }
            try
            {
                var temp = SpellRadius;
                foreach (var x in temp.Where(x => x.Effect.IsDestroyed))
                {
                    SpellRadius.RemoveAt(SpellRadius.FindIndex(e => e.Effect.IsDestroyed));
                }
                var temp2 = EffectForSpells;
                foreach (var x in temp2.Where(x => x.Value.IsDestroyed))
                {
                    EffectForSpells.Remove(x.Key);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        public static void Maphack()
        {
            if (!Utils.SleepCheck("CDOTA_BaseNPC"))
            {
                try
                {
                    var pe = ObjectManager.GetEntities<Unit>().ToList();
                    foreach (
                        var p in
                            pe.Where(
                                x =>
                                    (x.Name == "npc_dota_thinker" || x.Name == "npc_dota_units_base" ||
                                     x.ClassID == ClassID.CDOTA_BaseNPC) && x.Team != Var.Me.Team))
                    {
                        foreach (var hero in EnemyHeroes.Heroes)
                        {
                            if (hero.Name == "npc_dota_hero_mirana" && Utils.SleepCheck("miranaarrow"))
                            {
                                if (p.ClassID == ClassID.CDOTA_BaseNPC && p.DayVision == 650)
                                {
                                    Utils.Sleep(3000 / 857 * 1000 + 400 + Game.Ping, "DrawArrrow");
                                    Utils.Sleep(100 + Game.Ping, "arrrowwait");
                                    _arrowPos = p.Position;
                                    _arrowUnit = p;
                                }
                                if (!AllDrawing.ShowMeMore.Mapicon.ContainsKey(hero))
                                    AllDrawing.ShowMeMore.Mapicon.Add(hero, p.Position);
                                else
                                    AllDrawing.ShowMeMore.Mapicon[hero] = p.Position;
                                Utils.Sleep(10000, "miranaarrow");
                            }
                            else if (p.Owner != null && Equals(p.Owner, hero) && p.Name == "npc_dota_thinker" ||
                                        p.Name == "npc_dota_units_base")
                            {
                                if (!AllDrawing.ShowMeMore.Mapicon.ContainsKey(hero))
                                {
                                    AllDrawing.ShowMeMore.Mapicon.Add(hero, p.Position);
                                    //Console.WriteLine("1");
                                    //Game.ExecuteCommand("cl_particles_dumplist");
                                }
                                else if (AllDrawing.ShowMeMore.Mapicon[hero] != p.Position)
                                {
                                    AllDrawing.ShowMeMore.Mapicon[hero] = p.Position;
                                    //Game.ExecuteCommand("cl_particles_dumplist");
                                    //Console.WriteLine("2");
                                }
                            }
                            foreach (var show in ShowMeMoreEffects)
                            {
                                var mod = p.HasModifier(show.Modifier);
                                if (!mod) continue;
                                if (!SpellRadius.Any(x => x.ContainsHero(p)))
                                {
                                    var effect = p.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                                    effect.SetControlPoint(1, new Vector3(255, 0, 0));
                                    effect.SetControlPoint(2, new Vector3(show.Range, 255, 0));
                                    SpellRadius.Add(new AoeSpellStruct(p, p.Position, show.Modifier, effect, show.Range, show.Time));
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        public static void Roshan()
        {
            var tickDelta = Game.GameTime - RoshdeathTime;
            RoshanMinutes = Math.Floor(tickDelta / 60);
            RoshanSeconds = tickDelta % 60;
            var roshan = ObjectManager.GetEntities<Unit>()
                    .FirstOrDefault(unit => unit.ClassID == ClassID.CDOTA_Unit_Roshan && unit.IsAlive);
            if (roshan != null)
            {
                RoshIsAlive = true;
            }
        }

        public static void RoshanKill()
        {
            RoshdeathTime = Game.GameTime;
            RoshIsAlive = false;
        }

        public static void ShowVisible(Unit unit)
        {
            if (unit == null) return;
            Vector2 screenPos;
            ParticleEffect effect;

            var enemyPos = unit.Position + new Vector3(0, 0, unit.HealthBarOffset);
            if (Drawing.WorldToScreen(enemyPos, out screenPos) && unit.IsVisibleToEnemies && unit.IsAlive)
            {
                if (Effects.TryGetValue(unit, out effect)) return;
                effect = unit.AddParticleEffect(Particles.HeroEffect[13].First());
                Effects.Add(unit, effect);
            }
            else
            {
                if (!Effects.TryGetValue(unit, out effect)) return;
                effect.Dispose();
                Effects.Remove(unit);
            }
        }

        #endregion Methods
    }
}