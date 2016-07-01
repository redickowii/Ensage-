namespace AllinOne.Variables
{
    using AllinOne.Methods;
    using Ensage;
    using SharpDX;
    using SharpDX.Direct3D9;
    using System.Collections.Generic;
    using System.Linq;

    public struct ShowMeMoreStruct
    {
        #region Fields

        public readonly string Effect;
        public readonly string Modifier;
        public readonly int Range;
        public readonly float Time;

        #endregion Fields

        #region Constructors

        public ShowMeMoreStruct(string modifier, string effect, int range, float time)
        {
            Modifier = modifier;
            Effect = effect;
            Range = range;
            Time = time;
        }

        #endregion Constructors
    }

    public class AoeSpellStruct
    {
        #region Fields

        public readonly Entity Hero;
        public readonly Vector3 Position;
        public readonly ParticleEffect Effect;
        public readonly string Modifier;
        public readonly int Range;
        public float Time;

        #endregion Fields

        #region Constructors

        public AoeSpellStruct(Entity hero, Vector3 position, string modifier, ParticleEffect effect, int range, float time)
        {
            Hero = hero;
            Position = position;
            Modifier = modifier;
            Effect = effect;
            Range = range;
            Time = time;
        }

        public bool ContainsHero(Entity hero)
        {
            return hero.Handle == Hero.Handle;
        }

        public bool ContainsModifier(string modifier)
        {
            return modifier == Modifier;
        }

        #endregion Constructors
    }

    public class DictionarySleep
    {
        #region Properties

        public float Period { get; set; }
        public string Text { get; set; }

        public long Time { get; set; }

        #endregion Properties
    }

    public class DictionaryUnit
    {
        #region Properties

        public List<Ht> Ht { get; set; }
        public Unit Unit { get; set; }

        #endregion Properties

        #region Methods

        public bool AHealth(Entity unit)
        {
            if (unit.Handle != Unit.Handle) return false;
            if (Ht.Any(x => x.Health - unit.Health < 10)) return true;
            Ht.Add(new Ht { Health = unit.Health, Time = Game.GameTime, ACreeps = Lasthit.Attack(unit) });
            return true;
        }

        #endregion Methods
    }

    public class Ht
    {
        #region Properties

        public int ACreeps { get; set; }
        public float Health { get; set; }
        public float Time { get; set; }

        #endregion Properties
    }

    public class JungleCamps
    {
        #region Properties

        public bool Ancients { get; set; }
        public int AttackTime { get; set; }
        public int Creepscount { get; set; }
        public bool Empty { get; set; }
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public bool Stacked { get; set; }
        public Vector3 StackPosition { get; set; }
        public int Starttime { get; set; }
        public int State { get; set; }
        public int Team { get; set; }
        public Unit Unit { get; set; }
        public Vector3 WaitPosition { get; set; }
        public Vector3 WaitPositionN { get; set; }

        #endregion Properties
    }

    internal class AllDodge
    {
        #region Fields

        public static readonly List<string> BlinkAbilities = new List<string>
        {
            "item_blink",
            "mirana_leap",
            "antimage_blink",
            "magnataur_skewer",
            "item_force_staff",
            "queenofpain_blink",
            "morphling_waveform",
            "sandking_burrowstrike",
            "faceless_void_time_walk",
            "phantom_lancer_doppelwalk",
            "earth_spirit_rolling_boulder",
            "ember_spirit_activate_fire_remnant"
        };

        public static readonly List<string> DefVsDamage = new List<string>
        {
            "nyx_assassin_spiked_carapace",
            "templar_assassin_refraction",
            "treant_living_armor",
            "abaddon_aphotic_shield",
            "item_blade_mail"
        };

        public static readonly List<string> DefVsDisable = new List<string>
        {
            "slark_dark_pact",
            "juggernaut_blade_fury",
            "life_stealer_rage",
            "omniknight_repel",
            "phantom_lancer_doppelwalk"
        };

        public static readonly List<string> DefVsMagic = new List<string>
        {
            "item_glimmer_cape",
            "item_hood_of_defiance",
            "oracle_fates_edict",
            "ember_spirit_flame_guard",
            "life_stealer_rage",
            "juggernaut_blade_fury",
            "omniknight_repel",
            "pugna_nether_ward",
            "item_pipe"
        };

        public static readonly List<string> DefVsPhys = new List<string>
        {
            "item_ghost",
            "pugna_decrepify",
            "windrunner_windrun",
            "winter_wyvern_cold_embrace",
            "lich_frost_armor",
            "arc_warden_magnetic_field",
            "item_crimson_guard",
            "item_shivas_guard",
            "item_buckler"
        };

        public static readonly List<string> Diffusal = new List<string>
        {
            "item_diffusal_blade",
            "item_diffusal_blade_2",
        };

        public static readonly List<string> Eul = new List<string>
        {
            "item_cyclone"
        };

        public static readonly List<string> Fist = new List<string>
        {
            "ember_spirit_sleight_of_fist"
        };

        public static readonly List<string> Greaves = new List<string>
        {
            "item_guardian_greaves"
        };

        public static readonly List<string> IgnoresMagicImmunity = new List<string>
        {
            "item_abyssal_blade"
        };

        public static readonly List<string> InstaDisable = new List<string>
        {
            "item_sheepstick",
            "item_orchid",
            "puck_waning_rift",
            "dragon_knight_dragon_tail",
            "lion_voodoo",
            "shadow_shaman_voodoo",
            "shadow_shaman_shackles",
            "rubick_telekinesis",
            "skywrath_mage_ancient_seal",
            "keeper_of_the_light_mana_leak",
            "crystal_maiden_frostbite",
            "ember_spirit_searing_chains",
            "item_abyssal_blade"
        };

        public static readonly List<string> Invis = new List<string>
        {
            "bounty_hunter_wind_walk",
            "clinkz_wind_walk",
            "sandking_sand_storm",
            "templar_assassin_meld",
            "weaver_shukuchi",
            "item_glimmer_cape",
            "item_invis_sword",
            "item_silver_edge"
        };

        public static readonly List<string> Invul = new List<string>
        {
            "shadow_demon_disruption",
            "obsidian_destroyer_astral_imprisonment",
            "bane_nightmare"
        };

        public static readonly List<string> Lotus = new List<string>
        {
            "item_lotus_orb"
        };

        public static readonly List<string> Manta = new List<string>
        {
            "item_manta"
        };

        public static readonly List<string> OffVsPhys = new List<string>
        {
            "item_ethereal_blade",
            "item_heavens_halberd",
            "item_solar_crest",
            "pugna_decrepify",
            "item_rod_of_atos",
            "keeper_of_the_light_blinding_light",
            "razor_static_link",
            "brewmaster_drunken_haze",
            "tinker_laser"
        };

        public static readonly List<string> Shift = new List<string>
        {
            "puck_phase_shift"
        };

        public static readonly List<string> SnowBall = new List<string>
        {
            "tusk_snowball"
        };

        #endregion Fields
    }

    internal class Fonts
    {
        #region Fields

        public static Font RoshanFont = new Font(
            Drawing.Direct3DDevice9,
            new FontDescription
            {
                FaceName = "Tahoma",
                Height = 15,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });

        public static Font StackFont = new Font(
                    Drawing.Direct3DDevice9,
            new FontDescription
            {
                FaceName = "Tahoma",
                Height = 23,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.ClearType
            });

        public static Font UltFont = new Font(
            Drawing.Direct3DDevice9,
            new FontDescription
            {
                FaceName = "Tahoma",
                Height = 20,
                OutputPrecision = FontPrecision.Outline,
                Quality = FontQuality.ClearType
            });

        public static Font VisibleFont = new Font(
                    Drawing.Direct3DDevice9,
            new FontDescription
            {
                FaceName = "Tahoma",
                Height = 12,
                OutputPrecision = FontPrecision.Outline,
                Quality = FontQuality.ClearType
            });

        public static Font HelpFont = new Font(
            Drawing.Direct3DDevice9,
            new FontDescription
            {
                FaceName = "Tahoma",
                Height = 14,
                OutputPrecision = FontPrecision.Outline,
                Quality = FontQuality.ClearType
            });

        #endregion Fields
    }

    internal class Particles
    {
        #region Fields

        public static readonly List<string> Partlist = new List<string>
        {
            @"particles\ui_mouseactions\ambient_gizmo.vpcf",
            @"particles\ui_mouseactions\ambient_gizmo_model.vpcf",
            @"particles\ui_mouseactions\ambient_gizmo_rays.vpcf",
            @"particles\ui_mouseactions\axis_gizmo.vpcf",
            @"particles\ui_mouseactions\axis_gizmo_b.vpcf",
            @"particles\ui_mouseactions\axis_gizmo_fwd.vpcf",
            @"particles\ui_mouseactions\axis_gizmo_fwd_camera.vpcf",
            @"particles\ui_mouseactions\axis_gizmo_fwd_thin.vpcf",
            @"particles\ui_mouseactions\axis_gizmo_right.vpcf",
            @"particles\ui_mouseactions\axis_gizmo_ring.vpcf",
            @"particles\ui_mouseactions\axis_gizmo_up.vpcf",
            @"particles\ui_mouseactions\bounding_area_view.vpcf",
            @"particles\ui_mouseactions\bounding_area_view_a.vpcf",
            @"particles\ui_mouseactions\bounding_area_view_a1.vpcf",
            @"particles\ui_mouseactions\bounding_area_view_a2.vpcf",
            @"particles\ui_mouseactions\bounding_area_view_b.vpcf",
            @"particles\ui_mouseactions\bounding_area_view_c.vpcf",
            @"particles\ui_mouseactions\bounding_area_view_d.vpcf",
            @"particles\ui_mouseactions\camera_gizmo.vpcf",
            @"particles\ui_mouseactions\clicked_arrow_trail.vpcf",
            @"particles\ui_mouseactions\clicked_attackmove_unused.vpcf",
            @"particles\ui_mouseactions\clicked_basemove.vpcf",
            @"particles\ui_mouseactions\clicked_hero_attacked.vpcf",
            @"particles\ui_mouseactions\clicked_moveto.vpcf",
            @"particles\ui_mouseactions\clicked_moveto_arrow.vpcf",
            @"particles\ui_mouseactions\clicked_moveto_ring.vpcf",
            @"particles\ui_mouseactions\clicked_redarrow.vpcf",
            @"particles\ui_mouseactions\clicked_rings.vpcf",
            @"particles\ui_mouseactions\clicked_rings_b.vpcf",
            @"particles\ui_mouseactions\clicked_rings_c.vpcf",
            @"particles\ui_mouseactions\clicked_rings_red.vpcf",
            @"particles\ui_mouseactions\clicked_unit_select.vpcf",
            @"particles\ui_mouseactions\clicked_unit_select_b.vpcf",
            @"particles\ui_mouseactions\clicked_unit_select_dashed.vpcf",
            @"particles\ui_mouseactions\clicked_unit_select_old.vpcf",
            @"particles\ui_mouseactions\drag_selected_ring.vpcf",
            @"particles\ui_mouseactions\drag_selected_ring_b.vpcf",
            @"particles\ui_mouseactions\drag_selected_ring_reselect.vpcf",
            @"particles\ui_mouseactions\drag_select_hit.vpcf",
            @"particles\ui_mouseactions\draw_commentator.vpcf",
            @"particles\ui_mouseactions\draw_commentator_b.vpcf",
            @"particles\ui_mouseactions\facing_gizmo.vpcf",
            @"particles\ui_mouseactions\hero_highlighter.vpcf",
            @"particles\ui_mouseactions\hero_highlighter_light.vpcf",
            @"particles\ui_mouseactions\hero_highlighter_playerglow.vpcf",
            @"particles\ui_mouseactions\hero_ring.vpcf",
            @"particles\ui_mouseactions\hero_ring_b.vpcf",
            @"particles\ui_mouseactions\hero_underglow.vpcf",
            @"particles\ui_mouseactions\icon_dropshadow.vpcf",
            @"particles\ui_mouseactions\icon_dropshadow_b.vpcf",
            @"particles\ui_mouseactions\origin_gizmo.vpcf",
            @"particles\ui_mouseactions\origin_gizmo_model.vpcf",
            @"particles\ui_mouseactions\origin_gizmo_pulse.vpcf",
            @"particles\ui_mouseactions\origin_spot_gizmo.vpcf",
            @"particles\ui_mouseactions\ping_circle.vpcf",
            @"particles\ui_mouseactions\ping_circle_static.vpcf",
            @"particles\ui_mouseactions\ping_danger.vpcf",
            @"particles\ui_mouseactions\ping_exclaim.vpcf",
            @"particles\ui_mouseactions\ping_flareup.vpcf",
            @"particles\ui_mouseactions\ping_flash.vpcf",
            @"particles\ui_mouseactions\ping_player.vpcf",
            @"particles\ui_mouseactions\ping_player_circle.vpcf",
            @"particles\ui_mouseactions\ping_player_flareup.vpcf",
            @"particles\ui_mouseactions\ping_player_flash.vpcf",
            @"particles\ui_mouseactions\ping_static.vpcf",
            @"particles\ui_mouseactions\ping_world.vpcf",
            @"particles\ui_mouseactions\range_display.vpcf",
            @"particles\ui_mouseactions\range_display_b.vpcf",
            @"particles\ui_mouseactions\range_display_grnd.vpcf",
            @"particles\ui_mouseactions\range_finder_aoe.vpcf",
            @"particles\ui_mouseactions\range_finder_aoe_b.vpcf",
            @"particles\ui_mouseactions\range_finder_aoe_glow.vpcf",
            @"particles\ui_mouseactions\range_finder_aoe_ward.vpcf",
            @"particles\ui_mouseactions\range_finder_b.vpcf",
            @"particles\ui_mouseactions\range_finder_c.vpcf",
            @"particles\ui_mouseactions\range_finder_cone.vpcf",
            @"particles\ui_mouseactions\range_finder_cone_body.vpcf",
            @"particles\ui_mouseactions\range_finder_cone_end.vpcf",
            @"particles\ui_mouseactions\range_finder_cp_color_aoe.vpcf",
            @"particles\ui_mouseactions\range_finder_cp_color_aoe_glow.vpcf",
            @"particles\ui_mouseactions\range_finder_cp_color_creep.vpcf",
            @"particles\ui_mouseactions\range_finder_d.vpcf",
            @"particles\ui_mouseactions\range_finder_directional.vpcf",
            @"particles\ui_mouseactions\range_finder_directional_b.vpcf",
            @"particles\ui_mouseactions\range_finder_directional_c.vpcf",
            @"particles\ui_mouseactions\range_finder_d_glow.vpcf",
            @"particles\ui_mouseactions\range_finder_generic.vpcf",
            @"particles\ui_mouseactions\range_finder_generic_aoe.vpcf",
            @"particles\ui_mouseactions\range_finder_generic_wardspot_model.vpcf",
            @"particles\ui_mouseactions\range_finder_generic_ward_aoe.vpcf",
            @"particles\ui_mouseactions\range_finder_generic_ward_model.vpcf",
            @"particles\ui_mouseactions\range_finder_generic_ward_model_glow.vpcf",
            @"particles\ui_mouseactions\range_finder_generic_ward_model_ring.vpcf",
            @"particles\ui_mouseactions\range_finder_generic_ward_model_wrong.vpcf",
            @"particles\ui_mouseactions\range_finder_line.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_a.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_a0.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_a0a.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_b.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_b0.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_rings.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_rings_b.vpcf",
            @"particles\ui_mouseactions\range_finder_targeted_aoe_rings_large.vpcf",
            @"particles\ui_mouseactions\range_finder_tower_aoe.vpcf",
            @"particles\ui_mouseactions\range_finder_tower_aoe_ring.vpcf",
            @"particles\ui_mouseactions\range_finder_tower_aoe_target.vpcf",
            @"particles\ui_mouseactions\range_finder_tower_aoe_target_ring.vpcf",
            @"particles\ui_mouseactions\range_finder_tower_line.vpcf",
            @"particles\ui_mouseactions\range_finder_tower_line_glow.vpcf",
            @"particles\ui_mouseactions\range_finder_unit_ring.vpcf",
            @"particles\ui_mouseactions\range_finder_ward_aoe.vpcf",
            @"particles\ui_mouseactions\rrange_finder_ward_aoe_ring.vpcf",
            @"particles\ui_mouseactions\rselected_ring.vpcf",
            @"particles\ui_mouseactions\selected_ring_b.vpcf",
            @"particles\ui_mouseactions\selected_ring_beam.vpcf",
            @"particles\ui_mouseactions\selected_ring_hit.vpcf",
            @"particles\ui_mouseactions\selected_ring_hit_b.vpcf",
            @"particles\ui_mouseactions\selected_ring_hit_c.vpcf",
            @"particles\ui_mouseactions\selected_ring_hit_old.vpcf",
            @"particles\ui_mouseactions\selected_ring_old.vpcf",
            @"particles\ui_mouseactions\select_hero_active.vpcf",
            @"particles\ui_mouseactions\select_unit.vpcf",
            @"particles\ui_mouseactions\spot_gizmo.vpcf",
            @"particles\ui_mouseactions\spot_gizmo_model.vpcf",
            @"particles\ui_mouseactions\unit_highlight.vpcf",
            @"particles\ui_mouseactions\unit_highlight_b.vpcf",
            @"particles\ui_mouseactions\unit_highlight_c.vpcf",
            @"particles\ui_mouseactions\unit_highlight_d.vpcf",
            @"particles\ui_mouseactions\unit_highlight_e.vpcf",
            @"particles\ui_mouseactions\unit_highlight_f.vpcf",
            @"particles\ui_mouseactions\vector_gizmo.vpcf",
            @"particles\ui_mouseactions\vector_gizmo_arrow.vpcf",
            @"particles\ui_mouseactions\waypoint_flag.vpcf"
        };

        public static Dictionary<int, List<string>> HeroEffect = new Dictionary<int, List<string>>
        {
            //{ 0, new List<string> { "particles/units/heroes/hero_batrider/batrider_stickynapalm_stack.vpcf"}},
            //{ 1, new List<string> { "particles/units/heroes/hero_juggernaut/jugg_crit_blur.vpcf" }},
            //{ 2, new List<string> { "particles/hw_fx/candy_fed.vpcf"}},
            //{ 3, new List<string> { "particles/hw_fx/cursed_rapier_d.vpcf" }},
            //{ 4, new List<string> { "particles/hw_fx/cursed_rapier_e.vpcf" }},
            //{ 5, new List<string> { "particles/hw_fx/fow_ghost.vpcf" }}

            //{ 0, new List<string> { "particles/ui_mouseactions/axis_gizmo.vpcf" }},
            //{ 1, new List<string> { "particles/ui_mouseactions/bounding_area_view.vpcf" }},
            //{ 2, new List<string> { "particles/ui_mouseactions/camera_gizmo.vpcf" }},
            //{ 3, new List<string> { "particles/ui_mouseactions/clicked_arrow_trail.vpcf" }},
            //{ 4, new List<string> { "particles/ui_mouseactions/clicked_attackmove_unused.vpcf" }},
            //{ 5, new List<string> { "particles/ui_mouseactions/facing_gizmo.vpcf"}},
            //{ 6, new List<string> { "particles/ui_mouseactions/vector_gizmo_arrow.vpcf" }}, //arrow
            //{ 7, new List<string> { "particles/ui_mouseactions/vector_gizmo.vpcf" }},
            //{ 8, new List<string> { "particles/units/unit_greevil/loot_greevil_ambient.vpcf"}},
            //{ 9, new List<string> { "particles/units/unit_greevil/loot_greevil_ambient_ember.vpcf" }},
            //{ 10, new List<string> { "particles/units/unit_greevil/loot_greevil_ambient_light.vpcf" }},
            //{ 11, new List<string> { "particles/hw_fx/cursed_rapier.vpcf"}},
            //{ 12, new List<string> { "particles/hw_fx/candy_carrying_stack.vpcf" }} //effect.SetControlPoint(2, new Vector3(1, 4, 0));
            { 0, new List<string> { "particles/hw_fx/candy_fed.vpcf" }},
            { 1, new List<string> { "particles/items2_fx/smoke_of_deceit_buff.vpcf" }},
            { 2, new List<string> { "particles/hw_fx/water_splash_hw.vpcf" }},
            { 3, new List<string> { "particles/showcase_fx/showcase_fx_base_1.vpcf" }},
            { 4, new List<string> { "particles/showcase_fx/showcase_fx_base_2.vpcf" }},
            { 5, new List<string> { "particles/ui/dark_swirl_smoke.vpcf"}},
            { 6, new List<string> { "particles/ui_mouseactions/vector_gizmo_arrow.vpcf" }}, //arrow
            { 7, new List<string> { "particles/units/unit_greevil/loot_greevil_ambient_light.vpcf" }},
            { 8, new List<string> { "particles/units/unit_greevil/loot_greevil_ambient.vpcf"}},
            { 9, new List<string> { "particles/units/unit_greevil/loot_greevil_ambient_ember.vpcf" }},
            { 10, new List<string> { "particles/units/unit_greevil/loot_greevil_ambient_light.vpcf" }},
            { 11, new List<string> { "particles/hw_fx/cursed_rapier.vpcf"}},
            { 12, new List<string> { "particles/hw_fx/candy_carrying_stack.vpcf" }}, //effect.SetControlPoint(2, new Vector3(1, 4, 0));
            { 13, new List<string> { "particles/items_fx/aura_shivas.vpcf" }}
        };

        public static Dictionary<int, List<string>> IllusionsEffect = new Dictionary<int, List<string>>
        {
            { 0, new List<string> { "particles/items2_fx/smoke_of_deceit_buff.vpcf"}},
            { 1, new List<string> { "particles/items2_fx/shadow_amulet_active_ground_proj.vpcf" }},
            { 2, new List<string> { "particles/items_fx/diffusal_slow.vpcf"}},
            { 3, new List<string>
            {
                "particles/items_fx/diffusal_slow.vpcf",
                "particles/items2_fx/shadow_amulet_active_ground_proj.vpcf",
                "particles/items2_fx/smoke_of_deceit_buff.vpcf"
            }},
            { 4, new List<string>
            {
                "particles/showcase_fx/showcase_fx_base_1.vpcf",
                "particles/showcase_fx/showcase_fx_base_2.vpcf",
                "particles/showcase_fx/showcase_fx_base_3.vpcf"
            }}
        };

        #endregion Fields
    }

    internal class Var
    {
        #region Fields

        public static int AutoAttackMode;
        public static bool AutoAttackTypeDef;
        public static List<Unit> Creeps;
        public static List<DictionaryUnit> CreepsDic = new List<DictionaryUnit>();
        public static Unit CreeptargetH;
        public static Unit CreeptargetS;
        public static bool DisableAaKeyPressed;
        public static double HeroAPoint;
        public static bool Loaded;
        public static Hero Me;
        public static Ability Q, W, E, R;
        public static Dictionary<string, ParticleEffect> RadiusHeroParticleEffect;
        public static int Seconds;
        public static List<DictionarySleep> SleepDic = new List<DictionarySleep>();
        public static List<Unit> StackableSummons = new List<Unit>();
        public static Dictionary<Unit, List<Unit>> Summons;
        public static bool SummonsAutoAttackTypeDef;
        public static bool SummonsDisableAaKeyPressed;
        public static Hero Target;

        #endregion Fields

        #region ListJungleCamps

        public static List<JungleCamps> Camps = new List<JungleCamps> {
            new JungleCamps
            {
                Position = new Vector3(-1708, -4284, 256),
                StackPosition = new Vector3(-1816, -2684, 256),
                WaitPosition = new Vector3(-1867, -4033, 256),
                WaitPositionN = new Vector3(-2041, -3790, 256),
                Team = 2,
                Id = 1,
                Empty = false,
                Stacked = false,
                Starttime = 56
            },
            new JungleCamps
            {
                Position = new Vector3(-266, -3176, 256),
                StackPosition = new Vector3(-522, -1351, 256),
                WaitPosition = new Vector3(-306, -2853, 256),
                WaitPositionN = new Vector3(-340, -2521, 256),
                Team = 2,
                Id = 2,
                Empty = false,
                Stacked = false,
                Starttime = 56
            },
            new JungleCamps
            {
                Position = new Vector3(1656, -3714, 384),
                StackPosition = new Vector3(1449, -5699, 384),
                WaitPosition = new Vector3(1637, -4009, 384),
                WaitPositionN = new Vector3(1647, -4651, 384),
                Team = 2,
                Id = 3,
                Empty = false,
                Stacked = false,
                Starttime = 54
            },
            new JungleCamps
            {
                Position = new Vector3(3016, -4692, 384),
                StackPosition = new Vector3(5032, -4826, 384),
                WaitPosition = new Vector3(3146, -5071, 384),
                WaitPositionN = new Vector3(3088, -5345, 384),
                Team = 2,
                Id = 4,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(4474, -3598, 384),
                StackPosition = new Vector3(2486, -4125, 384),
                WaitPosition = new Vector3(4121, -3902, 384),
                WaitPositionN = new Vector3(3714, -3941, 384),
                Team = 2,
                Id = 5,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(-3617, 805, 384),
                StackPosition = new Vector3(-5268, 1400, 384),
                WaitPosition = new Vector3(-3835, 643, 384),
                WaitPositionN = new Vector3(-4571, 795, 384),
                Team = 2,
                Id = 6,
                Empty = false,
                Stacked = false,
                Starttime = 54
            },
            new JungleCamps
            {
                Position = new Vector3(-4446, 3541, 384),
                StackPosition = new Vector3(-3953, 4954, 384),
                WaitPosition = new Vector3(-4251, 3760, 384),
                WaitPositionN = new Vector3(-4267, 4271, 384),
                Team = 3,
                Id = 7,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(-2981, 4591, 384),
                StackPosition = new Vector3(-3248, 5993, 384),
                WaitPosition = new Vector3(-3050, 4897, 384),
                WaitPositionN = new Vector3(-3077, 5111, 384),
                Team = 3,
                Id = 8,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(-392, 3652, 384),
                StackPosition = new Vector3(-224, 5088, 384),
                WaitPosition = new Vector3(-503, 3955, 384),
                WaitPositionN = new Vector3(-512, 4314, 384),
                Team = 3,
                Id = 9,
                Empty = false,
                Stacked = false,
                Starttime = 55
            },
            new JungleCamps
            {
                Position = new Vector3(-1524, 2641, 256),
                StackPosition = new Vector3(-1266, 4273, 384),
                WaitPosition = new Vector3(-1465, 2908, 256),
                WaitPositionN = new Vector3(-1508, 3328, 256),
                Team = 3,
                Id = 10,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(1098, 3338, 384),
                StackPosition = new Vector3(910, 5003, 384),
                WaitPosition = new Vector3(975, 3586, 384),
                WaitPositionN = new Vector3(983, 3949, 383),
                Team = 3,
                Id = 11,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(4141, 554, 384),
                StackPosition = new Vector3(2987, -2, 384),
                WaitPosition = new Vector3(3876, 506, 384),
                WaitPositionN = new Vector3(3152, 586, 384),
                Team = 3,
                Id = 12,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(-2960, -126, 384),
                StackPosition = new Vector3(-3850, -1491, 384),
                WaitPosition = new Vector3(-2777, -230, 384),
                WaitPositionN = new Vector3(-2340, -517, 384),
                Team = 2,
                Id = 13,
                Empty = false,
                Stacked = false,
                Starttime = 53
            },
            new JungleCamps
            {
                Position = new Vector3(4000, -700, 256),
                StackPosition = new Vector3(1713, -134, 256),
                WaitPosition = new Vector3(3649, -721, 256),
                WaitPositionN = new Vector3(3589, -141, 384),
                Team = 3,
                Id = 14,
                Empty = false,
                Stacked = false,
                Starttime = 53
            }};

        #endregion ListJungleCamps
    }
}