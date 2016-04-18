namespace AllinOne.AllEvents
{
    using AllinOne.AllDrawing;
    using AllinOne.CameraDistance;
    using AllinOne.Menu;
    using AllinOne.Methods;
    using AllinOne.ObjectManager.Heroes;
    using AllinOne.Update;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using System;
    using System.Collections.Generic;

    internal class AllinOneEvents
    {
        #region Methods

        public static void OnClose(object sender, EventArgs e)
        {
            Game.OnUpdate -= AllinOneMain.Game_OnUpdate;
            Game.OnUpdate -= OnUpdate.Update;
            Game.OnWndProc -= AllinOneMain.Game_OnWndProc;
            Game.OnWndProc -= Zoom.ChangeDistance;
            Game.OnFireEvent -= Common.FireEvent;

            Drawing.OnDraw -= Draw.Drawing;
            Drawing.OnEndScene -= Draw.Drawing_OnEndScene;
            Drawing.OnPostReset -= Draw.Drawing_OnPostReset;
            Drawing.OnPreReset -= Draw.Drawing_OnPreReset;

            ObjectManager.OnAddEntity -= AllDrawing.ShowMeMore.Update;

            Var.Loaded = false;
            MainMenu.UnLoad();
            if (Var.RadiusHeroParticleEffect != null && Var.RadiusHeroParticleEffect.Count > 0)
                foreach (var particleEffect in Var.RadiusHeroParticleEffect)
                {
                    if (particleEffect.Value != null)
                        particleEffect.Value.Dispose();
                }
            Var.RadiusHeroParticleEffect = null;
            Common.PrintEncolored("AllinOne UnLoaded", ConsoleColor.DarkRed);
        }

        public static void OnLoad(object sender, EventArgs e)
        {
            if (Var.Loaded || ObjectManager.LocalHero == null)
            {
                return;
            }
            Var.Me = ObjectManager.LocalHero;
            Var.RadiusHeroParticleEffect = new Dictionary<string, ParticleEffect>();
            Towers.TowerRange = new Dictionary<Entity, List<ParticleEffect>>();
            Towers.TowerLoad = false;
            Var.Target = null;
            Var.Loaded = true;
            Methods.ShowMeMore.RoshIsAlive = true;
            Var.Summons = new Dictionary<Unit, List<Unit>>();
            EnemyHeroes.Heroes = new List<Hero>();
            EnemyHeroes.Illusions = new List<Hero>();
            AllyHeroes.Heroes = new List<Hero>();
            EnemyHeroes.UsableHeroes = new Hero[] { };
            AllyHeroes.UsableHeroes = new Hero[] { };
            AllyHeroes.AbilityDictionary = new Dictionary<float, List<Ability>>();
            EnemyHeroes.AbilityDictionary = new Dictionary<float, List<Ability>>();
            AllyHeroes.ItemDictionary = new Dictionary<float, List<Item>>();
            EnemyHeroes.ItemDictionary = new Dictionary<float, List<Item>>();

            Methods.ShowMeMore.EffectForSpells = new Dictionary<Ability, ParticleEffect>();
            MainMenu.Load();
            MenuVar.ShowAttackRange = MainMenu.MenuSettings.Item("showatkrange");
            MenuVar.ShowExpRange = MainMenu.MenuSettings.Item("expRange");

            Game.OnUpdate += AllinOneMain.Game_OnUpdate;
            Game.OnUpdate += OnUpdate.Update;
            Game.OnWndProc += AllinOneMain.Game_OnWndProc;
            Game.OnWndProc += Zoom.ChangeDistance;
            Game.OnFireEvent += Common.FireEvent;

            Drawing.OnDraw += Draw.Drawing;
            Drawing.OnEndScene += Draw.Drawing_OnEndScene;
            Drawing.OnPostReset += Draw.Drawing_OnPostReset;
            Drawing.OnPreReset += Draw.Drawing_OnPreReset;

            ObjectManager.OnAddEntity += AllDrawing.ShowMeMore.Update;

            Orbwalking.Load();
            Draw.OnLoad();
            Zoom.Load();
            Game.PrintMessage("<font color='#3366cc'>AllinOne </font><font color='#00cc00'>Loaded</font>", MessageType.LogMessage);
            Common.PrintEncolored("AllinOne Loaded", ConsoleColor.DarkGreen);
        }

        #endregion Methods
    }
}