using System.Collections.Generic;
using System.Collections.Specialized;

namespace AllinOne.Update
{
    using AllinOne.AllDrawing;
    using AllinOne.Menu;
    using AllinOne.Methods;
    using AllinOne.ObjectManager;
    using AllinOne.ObjectManager.Heroes;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using System;
    using System.Linq;

    internal class OnUpdate
    {
        #region Fields

        private static bool Clear;

        #endregion Fields

        #region Methods

        public static bool CanUpdate()
        {
            if (!MainMenu.Dev.Item("ON/OFF").GetValue<bool>())
            {
                if (!Clear)
                {
                    AllDrawing.ShowMeMore.Effects.ForEach(x => x.Value.ForEach(y => y.ForceDispose()));
                    AllDrawing.ShowMeMore.Effects.Clear();
                    AllDrawing.ShowMeMore.ClearParticle();
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

            //var tempSummonlist = Var.Summons;
            //if (MenuVar.LastHitEnable)
            //{
            //    if (Var.Summons.Count > 0 && Var.Summons.Any(x => !x.Key.IsAlive))
            //    {
            //        //foreach (var summon in Var.Summons.Where(x => !x.Key.IsAlive))
            //        //{
            //        //    tempSummonlist.Remove(summon.Key);
            //        //}
            //        //Var.Summons.Clear();
            //        //Var.Summons = tempSummonlist;
            //    }
            //}

            EnemyHeroes.Update();
            AllyHeroes.Update();
            Couriers.Update();

            if (MenuVar.Maphack || MenuVar.DodgeEnable)
                Methods.ShowMeMore.Maphack();

            MenuVar.TestEffectMenu = MainMenu.Menu.Item("effects");

            MenuVar.CameraDistance = MainMenu.MenuSettings.Item("cameradistance");

            if (!Utils.SleepCheck("Update.sleep")) return;
            Utils.Sleep(500, "Update.sleep");

            if (MenuVar.ShowRoshanTimer)
            {
                Methods.ShowMeMore.Roshan();
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

            if (!Towers.TowerLoad)
            {
                Buildings.GetBuildings();
                Towers.TowerLoad = true;
            }
            //else
            //{
            //    Towers.Load();
            //}

            #endregion Towers
        }

        #endregion Methods
    }
}