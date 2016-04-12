using AllinOne.AllDrawing;

namespace AllinOne.Update
{
    using AllinOne.Menu;
    using AllinOne.Methods;
    using AllinOne.ObjectManager;
    using AllinOne.ObjectManager.Heroes;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using SharpDX.Direct3D9;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal class OnUpdate
    {
        private static bool Clear;

        public static bool CanUpdate()
        {
            if (!MainMenu.Dev.Item("ON/OFF").GetValue<bool>())
            {
                if (!Clear)
                {
                    ShowMeMore.Effects.ForEach(x => x.Value.ForceDispose());
                    ShowMeMore.Effects.Clear();
                    AllDrawing.ShowMeMore.ClearParticle();
                    Towers.TowerDestroyed();
                    Var.RadiusHeroParticleEffect.ForEach(x => x.Value.ForceDispose());
                    Var.RadiusHeroParticleEffect.Clear();
                    Dev.DevInfoDispose();
                    Game.ExecuteCommand("cl_particle_stop_all");
                    Clear = true;
                }
            }
            else
            {
                Clear = false;
            }

            return !Game.IsPaused && Game.IsInGame && Var.Me != null &&
                Var.Me.IsValid && !Game.IsChatOpen && !Clear;
        }

        public static void Update(EventArgs args)
        {
            if (!CanUpdate()) return;

            if (MenuVar.LastHitEnable)
            {
                if (Var.Summons.Count > 0 && Var.Summons.Any(x => !x.Key.IsAlive))
                {
                    foreach (var summon in Var.Summons.Where(x => !x.Key.IsAlive))
                    {
                        Var.Summons.Remove(summon.Key);
                    }
                }
            }

            EnemyHeroes.Update();
            AllyHeroes.Update();

            if (MenuVar.Maphack)
                ShowMeMore.Maphack();

            MenuVar.TestEffectMenu = MainMenu.Menu.Item("effects");

            MenuVar.CameraDistance = MainMenu.MenuSettings.Item("cameradistance");

            if (!Common.SleepCheck("tick")) return;
            Common.Sleep(500, "tick");

            if (MenuVar.ShowRoshanTimer)
            {
                ShowMeMore.Roshan();
            }

            #region Runes

            Runes.Update();

            #endregion Runes

            if (MenuVar.LastHitEnable && MenuVar.SummonsEnable || MenuVar.StackKey)
            {
                Common.UpdateAttackableSummons();
            }

            #region Menu

            MainMenu.Update();

            #endregion Menu

            #region Hero

            Var.Q = Var.Me.Spellbook.SpellQ;
            Var.W = Var.Me.Spellbook.SpellW;
            Var.E = Var.Me.Spellbook.SpellE;
            Var.R = Var.Me.Spellbook.SpellR;

            double apoint = Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_ArcWarden
                ? 0.3
                : UnitDatabase.Units.Find(x => x.UnitName == Var.Me.Name).AttackPoint;
            Var.HeroAPoint = apoint / (1 + Var.Me.AttacksPerSecond * Var.Me.BaseAttackTime / 100) * 1000;

            #endregion Hero

            #region Autoattack

            if (Var.AutoAttackMode != MenuVar.AutoAttackMode)
            {
                switch (MenuVar.AutoAttackMode)
                {
                    case 0:
                        Var.AutoAttackMode = 0;
                        Common.Autoattack(Var.AutoAttackMode);
                        break;

                    case 1:
                        Var.AutoAttackMode = 1;
                        Common.Autoattack(Var.AutoAttackMode);
                        break;

                    case 2:
                        Var.AutoAttackMode = 2;
                        Common.Autoattack(Var.AutoAttackMode);
                        break;
                }
            }

            #endregion Autoattack

            #region Target

            if (Var.Target != null && !Var.Target.IsVisible && !Orbwalking.AttackOnCooldown(Var.Target))
            {
                Var.Target = TargetSelector.ClosestToMouse(Var.Me);
            }
            else if (Var.Target == null || !Orbwalking.AttackOnCooldown(Var.Target))
            {
                var bestAa = Var.Me.BestAATarget();
                if (bestAa != null)
                {
                    Var.Target = Var.Me.BestAATarget();
                }
            }

            #endregion Target

            #region Towers

            if (!Var.TowerLoad)
            {
                Buildings.GetBuildings();
                Var.TowerLoad = true;
            }
            if (MenuVar.EnemiesTowers)
            {
                Towers.UpdateDicTowers(Buildings.Towers.Where(x => x.Team != Var.Me.Team).ToList());
            }
            else
            {
                foreach (var x in Var.EnemyTowerRange.Values.ToList())
                {
                    x.Dispose();
                }
                Var.EnemyTowerRange.Clear();
            }
            if (MenuVar.OwnTowers)
            {
                Towers.UpdateDicTowers(Buildings.Towers.Where(x => x.Team == Var.Me.Team).ToList());
            }
            else
            {
                foreach (var x in Var.OwnTowerRange.Values.ToList())
                {
                    x.Dispose();
                }
                Var.OwnTowerRange.Clear();
            }

            #endregion Towers
        }
    }
}