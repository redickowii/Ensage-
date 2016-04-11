namespace AllinOne.Methods
{
    using AllinOne.ObjectManager.Heroes;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ShowMeMore
    {
        public static bool RoshIsAlive;

        public static float RoshdeathTime;

        public static double RoshanMinutes;

        public static double RoshanSeconds;

        private static Unit _arrowUnit;

        private static Vector3 _arrowPos;

        public static readonly Dictionary<Unit, ParticleEffect> Effects = new Dictionary<Unit, ParticleEffect>();

        public static readonly Dictionary<Unit, ParticleEffect> EffectForSpells = new Dictionary<Unit, ParticleEffect>();

        public static Dictionary<Unit, ParticleEffect> SpellRadius = new Dictionary<Unit, ParticleEffect>();

        public static readonly ShowMeMoreStruct[] ShowMeMoreEffects =
        {
            new ShowMeMoreStruct("modifier_invoker_sun_strike",
                "hero_invoker/invoker_sun_strike_team", 175),
            new ShowMeMoreStruct("modifier_lina_light_strike_array",
                "hero_lina/lina_spell_light_strike_array_ring_collapse", 225),
            new ShowMeMoreStruct("modifier_kunkka_torrent_thinker",
                "hero_kunkka/kunkka_spell_torrent_pool", 225),
            new ShowMeMoreStruct("modifier_leshrac_split_earth_thinker",
                "hero_leshrac/leshrac_split_earth_b", 225)
        };

        public static void Maphack()
        {
            //var testunit = ObjectMgr.GetEntities<Unit>().Where(x => x.HasModifiers(new[] { "modifier_teleporting" })).ToList();
            //foreach (var x in testunit)
            //{
            //    try
            //    {
            //        if (!AllDrawing.ShowMeMore.Courier.ContainsKey(x))
            //        {
            //            AllDrawing.ShowMeMore.Courier.Add(x, x.Position);
            //        }
            //        else if (AllDrawing.ShowMeMore.Courier[x] != x.Position)
            //        {
            //            AllDrawing.ShowMeMore.Courier[x] = x.Position;
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        //
            //    }
            //}
            if (!Common.SleepCheck("CDOTA_BaseNPC"))
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
                            if (hero.Name == "npc_dota_hero_mirana" && Common.SleepCheck("miranaarrow"))
                            {
                                if (p.ClassID == ClassID.CDOTA_BaseNPC && p.DayVision == 650)
                                {
                                    Common.Sleep(3000 / 857 * 1000 + 400 + Game.Ping, "DrawArrrow");
                                    Common.Sleep(100 + Game.Ping, "arrrowwait");
                                    _arrowPos = p.Position;
                                    _arrowUnit = p;
                                }
                                if (!AllDrawing.ShowMeMore.Mapicon.ContainsKey(hero))
                                    AllDrawing.ShowMeMore.Mapicon.Add(hero, p.Position);
                                else
                                    AllDrawing.ShowMeMore.Mapicon[hero] = p.Position;
                                Common.Sleep(10000, "miranaarrow");
                            }
                            else if (p.Owner != null && Equals(p.Owner, hero) && p.Name == "npc_dota_thinker" ||
                                        p.Name == "npc_dota_units_base")
                            {
                                if (!AllDrawing.ShowMeMore.Mapicon.ContainsKey(hero))
                                    AllDrawing.ShowMeMore.Mapicon.Add(hero, p.Position);
                                else if (AllDrawing.ShowMeMore.Mapicon[hero] != p.Position)
                                    AllDrawing.ShowMeMore.Mapicon[hero] = p.Position;
                            }
                            foreach (var show in ShowMeMoreEffects)
                            {
                                var mod = p.HasModifier(show.Modifier);
                                if (!mod) continue;
                                if (!SpellRadius.ContainsKey(p))
                                {
                                    var effect = p.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
                                    effect.SetControlPoint(1, new Vector3(255, 0, 0));
                                    effect.SetControlPoint(2, new Vector3(show.Range, 255, 0));
                                    SpellRadius.Add(p, effect);
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
                    var cd = Math.Floor(spell.Cooldown * 100);
                    if (cd < 880)
                    {
                        if (!EffectForSpells.ContainsKey(hero) && cd > 720)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, 2600));
                            EffectForSpells.Add(hero, effect);
                        }
                        else if ((!hero.IsAlive || cd <= 720) && EffectForSpells.ContainsKey(hero))
                        {
                            EffectForSpells[hero].ForceDispose();
                            EffectForSpells.Remove(hero);
                        }
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Mirana:
                    if (_arrowUnit != null && !Common.SleepCheck("DrawArrrow") && Common.SleepCheck("arrrowwait"))
                    {
                        if (_arrowUnit.IsValid && _arrowUnit.IsVisible)
                        {
                            if (!EffectForSpells.ContainsKey(_arrowUnit))
                            {
                                var effect = new ParticleEffect(Particles.Partlist[83], _arrowUnit.Position);
                                effect.SetControlPoint(1, _arrowUnit.Position);
                                effect.SetControlPoint(2,
                                    Common.FindVector(_arrowPos, Common.FindRet(_arrowPos, _arrowUnit.Position), 3000 - _arrowUnit.Speed * _arrowUnit.CreateTime));
                                EffectForSpells.Add(_arrowUnit, effect);
                            }
                        }
                    }
                    else if (Common.SleepCheck("DrawArrrow") &&
                        _arrowUnit != null &&
                        EffectForSpells.ContainsKey(_arrowUnit))
                    {
                        EffectForSpells[_arrowUnit].ForceDispose();
                        EffectForSpells.Remove(_arrowUnit);
                        _arrowUnit = null;
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Pudge:
                    if (Prediction.IsTurning(hero)) return;
                    spell = hero.Spellbook.Spell1;
                    if (spell == null || spell.CanBeCasted()) return;
                    cd = Math.Floor(spell.Cooldown / spell.GetCooldown(spell.Level - 1) * 100);
                    Console.WriteLine(cd);
                    if (cd < 99)
                    {
                        if (!EffectForSpells.ContainsKey(hero) && cd > 90)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, spell.CastRange + hero.HullRadius));
                            EffectForSpells.Add(hero, effect);
                        }
                        else if (!hero.IsAlive || cd <= 90 && EffectForSpells.ContainsKey(hero))
                        {
                            EffectForSpells[hero].ForceDispose();
                            EffectForSpells.Remove(hero);
                        }
                    }
                    break;
            }
            try
            {
                List<Unit> del = new List<Unit>();
                foreach (var x in SpellRadius.Where(x => !x.Key.IsAlive))
                {
                    x.Value.ForceDispose();
                    del.Add(x.Key);
                }
                del.ForEach(x => SpellRadius.Remove(x));
            }
            catch (Exception)
            {
                //
            }
        }

        public static void RoshanKill()
        {
            RoshdeathTime = Game.GameTime;
            RoshIsAlive = false;
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
    }
}