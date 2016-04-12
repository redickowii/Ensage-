using AllinOne.Menu;

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

        public static Dictionary<Unit, ParticleEffect> EffectForSpells = new Dictionary<Unit, ParticleEffect>();

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
                    var cd = Math.Floor(spell.Cooldown / spell.GetCooldown(spell.Level - 1) * 1000);
                    if (cd < 980)
                    {
                        if (!EffectForSpells.ContainsKey(hero) && cd > 800)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius));
                            EffectForSpells.Add(hero, effect);
                        }
                        else if ((!hero.IsAlive || cd <= 800) && EffectForSpells.ContainsKey(hero))
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
                    cd = Math.Floor(spell.Cooldown / spell.GetCooldown(spell.Level - 1) * 1000);
                    //Console.WriteLine(cd);
                    if (cd < 990)
                    {
                        if (!EffectForSpells.ContainsKey(hero) && cd > 90)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius));
                            EffectForSpells.Add(hero, effect);
                        }
                        else if ((!hero.IsAlive || cd <= 90) && EffectForSpells.ContainsKey(hero))
                        {
                            EffectForSpells[hero].ForceDispose();
                            EffectForSpells.Remove(hero);
                        }
                    }
                    break;

                case ClassID.CDOTA_Unit_Hero_Puck:
                    if (Prediction.IsTurning(hero)) return;
                    spell = hero.Spellbook.Spell1;
                    var spell4 = hero.Spellbook.Spell4;
                    if (spell == null || spell.CanBeCasted()) return;
                    cd = Math.Floor(spell.Cooldown / spell.GetCooldown(spell.Level - 1) * 1000);
                    if (cd < 995)
                    {
                        if (!EffectForSpells.ContainsKey(hero) && spell4.IsActivated)
                        {
                            var effect = new ParticleEffect(Particles.Partlist[83], hero.Position);
                            effect.SetControlPoint(1, hero.Position);
                            effect.SetControlPoint(2, Common.FindVector(hero.Position, hero.Rotation, spell.GetCastRange() + hero.HullRadius));
                            EffectForSpells.Add(hero, effect);
                        }
                        else if ((!hero.IsAlive || !spell4.IsActivated) && EffectForSpells.ContainsKey(hero))
                        {
                            EffectForSpells[hero].ForceDispose();
                            EffectForSpells.Remove(hero);
                        }
                    }
                    break;
            }
            try
            {
                var temp = SpellRadius;
                foreach (var x in temp.Where(x => x.Value.IsDestroyed))
                {
                    SpellRadius.Remove(x.Key);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        public static void Dodge()
        {
            if (!Common.SleepCheck("DodgeWait")) return;
            if (SpellRadius.Count > 0)
            {
                foreach (var spell in SpellRadius)
                {
                    AoeDodge(spell.Value.Position, spell.Value.GetControlPoint(2).X + 30);
                }
                Common.Sleep(MenuVar.DodgeFrequency, "DodgeWait");
            }
            if (EffectForSpells.Count > 0)
            {
                foreach (var effect in EffectForSpells)
                {
                    var pos1 = effect.Value.GetControlPoint(1);
                    var pos2 = effect.Value.GetControlPoint(2);
                    switch (effect.Key.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Pudge:
                            LineDodge(pos1, pos2, 130);
                            break;

                        case ClassID.CDOTA_Unit_Hero_Windrunner:
                            LineDodge(pos1, pos2, 170);
                            break;

                        case ClassID.CDOTA_Unit_Hero_Mirana:
                            LineDodge(pos1, pos2, 130);
                            break;

                        case ClassID.CDOTA_Unit_Hero_Puck:
                            LineDodge(pos1, pos2, 255);
                            break;
                    }
                }
                Common.Sleep(MenuVar.DodgeFrequency, "DodgeWait");
            }
        }

        public static void LineDodge(Vector3 pos1, Vector3 pos2, float radius)
        {
            var me = Var.Me;

            var calc1 =
                Math.Floor(Math.Sqrt(Math.Pow(pos2.X - me.Position.X, 2) + Math.Pow(pos2.Y - me.Position.Y, 2)));
            var calc2 =
                Math.Floor(Math.Sqrt(Math.Pow(pos1.X - me.Position.X, 2) + Math.Pow(pos1.Y - me.Position.Y, 2)));
            var calc4 = Math.Floor(Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2)));

            var perpendicular =
                Math.Floor(
                    Math.Abs((pos2.X - pos1.X) * (pos1.Y - me.Position.Y) -
                             (pos1.X - me.Position.X) * (pos2.Y - pos1.Y)) /
                    Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2)));
            var k = ((pos2.Y - pos1.Y) * (me.Position.X - pos1.X) - (pos2.X - pos1.X) * (me.Position.Y - pos1.Y)) /
                    (Math.Pow(pos2.Y - pos1.Y, 2) + Math.Pow(pos2.X - pos1.X, 2));
            var x4 = me.Position.X - k * (pos2.Y - pos1.Y);
            var z4 = me.Position.Y + k * (pos2.X - pos1.X);
            var calc3 =
                (Math.Floor(Math.Sqrt(Math.Pow(x4 - me.Position.X, 2) + Math.Pow(z4 - me.Position.Y, 2))));
            var dodgex = x4 + (radius / calc3) * (me.Position.X - x4);
            var dodgey = z4 + (radius / calc3) * (me.Position.Y - z4);

            if (perpendicular < radius && calc1 < calc4 && calc2 < calc4)
            {
                var dodgevector = new Vector3((float) dodgex, (float) dodgey, me.Position.Z);
                var dodgevector2 = new Vector3((float) (x4 + ((radius / 2.3) / calc3) * (me.Position.X - x4)),
                    (float) (z4 + ((radius / 2.3) / calc3) * (me.Position.Y - z4)), me.Position.Z);
                if (me.Distance2D(dodgevector) <= 5)
                {
                    //
                }
                else
                {
                    me.Move(dodgevector);
                }
            }
        }

        public static void AoeDodge(Vector3 pos, float radius)
        {
            var calc =
                Math.Floor(Math.Sqrt(Math.Pow(pos.X - Var.Me.Position.X, 2) + Math.Pow(pos.Y - Var.Me.Position.Y, 2)));
            var dodgex = (float) (pos.X + (radius / calc) * (Var.Me.Position.X - pos.X));
            var dodgey = (float) (pos.Y + (radius / calc) * (Var.Me.Position.Y - pos.Y));
            if (calc < radius)
            {
                var dodgevector = new Vector3(dodgex, dodgey, Var.Me.Position.Z);
                if (Var.Me.Distance2D(dodgevector) < 5)
                {
                    //dodging = false
                    //dodged = false
                }
                else
                {
                    Var.Me.Move(dodgevector);
                }
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