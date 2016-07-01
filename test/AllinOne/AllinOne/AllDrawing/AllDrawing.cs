using Ensage;
using Ensage.Common;

namespace AllinOne.AllDrawing
{
    using AllinOne.Menu;
    using AllinOne.Methods;
    using AllinOne.ObjectManager;
    using AllinOne.ObjectManager.Heroes;
    using AllinOne.Update;
    using AllinOne.Variables;
    using SharpDX;
    using SharpDX.Direct3D9;
    using System;

    internal class Draw
    {
        #region Fields

        public static readonly Line Line = new Line(Ensage.Drawing.Direct3DDevice9);

        #endregion Fields

        #region Methods

        public static void Drawing(EventArgs args)
        {
            if (!OnUpdate.CanUpdate()) return;

            if (MenuVar.Maphack || MenuVar.ShowLastPos)
            {
                foreach (var hero in EnemyHeroes.Heroes)
                {
                    ShowMeMore.DrawLastPosition(hero);
                }
            }

            #region Hero radius

            MyHero.RadiusHeroParticleEffect("ExpRange", 1300, new Color(0, 100, 0), MenuVar.ShowExpRange.GetValue<bool>());
            MyHero.RadiusHeroParticleEffect("AttackRange", Var.Me.IsRanged
                ? MyHeroInfo.AttackRange() + 70
                : MyHeroInfo.AttackRange(), new Color(255, 0, 0), MenuVar.ShowAttackRange.GetValue<bool>());

            #endregion Hero radius

            #region Runes

            if (MenuVar.ShowRunesMinimap)
            {
                RunesOnMinimap.Draw();
            }

            #endregion Runes

            #region Lasthit

            if (MenuVar.LastHitEnable)
            {
                if (MenuVar.ShowHp)
                    Lasthit.Drawhpbar();
            }

            #endregion Lasthit

            #region JungleStack

            if (MenuVar.StackKey && MenuVar.DrawStackLine)
            {
                JungleDraw.DrawLine();
            }
            else
            {
                JungleDraw.Clear();
            }

            #endregion JungleStack

            #region ShowMeMore

            if (MenuVar.ShowIllusions && EnemyHeroes.Illusions.Count > 0)
            {
                ShowMeMore.ShowIllusion();
            }
            else
            {
                ShowMeMore.ClearParticle();
            }

            foreach (var hero in AllyHeroes.Heroes)
            {
                ShowMeMore.DrawShowMeMoreBara(hero);
            }

            #endregion ShowMeMore

            #region Top ovellay

            if (MenuVar.ShowTopOverlayEnemyHp)
                Overlay.DrawTopOverlayHealth(EnemyHeroes.Heroes, MenuVar.HealthHeightEnemy,
                    new Color(MenuVar.OverlayHealthEnemyRed, MenuVar.OverlayHealthEnemyGreen, MenuVar.OverlayHealthEnemyBlue, MenuVar.OverlayAlpha));
            if (MenuVar.ShowTopOverlayEnemyMp)
                Overlay.DrawTopOverlayMana(EnemyHeroes.Heroes, MenuVar.ManaHeightEnemy,
                    new Color(MenuVar.OverlayManaEnemyRed, MenuVar.OverlayManaEnemyGreen, MenuVar.OverlayManaEnemyBlue, MenuVar.OverlayAlpha));
            if (MenuVar.ShowTopOverlayEnemyUltLine)
                Overlay.DrawTopOverlayUltimateCooldownLine(EnemyHeroes.Heroes, MenuVar.UltimateHeightEnemy, MenuVar.OverlayAlpha);
            if (MenuVar.ShowTopOverlayEnemyUltText)
                Overlay.DrawTopOverlayUltimateCooldownText(EnemyHeroes.Heroes, MenuVar.OverlayAlpha);
            if (MenuVar.ShowTopOverlayEnemy)
                Overlay.DrawTopOverlay(EnemyHeroes.Heroes);

            if (MenuVar.ShowTopOverlayAllyHp)
                Overlay.DrawTopOverlayHealth(AllyHeroes.Heroes, MenuVar.HealthHeightAlly,
                    new Color(MenuVar.OverlayHealthAllyRed, MenuVar.OverlayHealthAllyGreen, MenuVar.OverlayHealthAllyBlue, MenuVar.OverlayAlpha));
            if (MenuVar.ShowTopOverlayAllyMp)
                Overlay.DrawTopOverlayMana(AllyHeroes.Heroes, MenuVar.ManaHeightAlly,
                    new Color(MenuVar.OverlayManaAllyRed, MenuVar.OverlayManaAllyGreen, MenuVar.OverlayManaAllyBlue, MenuVar.OverlayAlpha));
            if (MenuVar.ShowTopOverlayAllyUltLine)
                Overlay.DrawTopOverlayUltimateCooldownLine(AllyHeroes.Heroes, MenuVar.UltimateHeightAlly, MenuVar.OverlayAlpha);
            if (MenuVar.ShowTopOverlayAllyUltText)
                Overlay.DrawTopOverlayUltimateCooldownText(AllyHeroes.Heroes, MenuVar.OverlayAlpha);
            if (MenuVar.ShowTopOverlayAlly)
                Overlay.DrawTopOverlay(AllyHeroes.Heroes);

            Overlay.DrawTopHelpOverlay(EnemyHeroes.Heroes, 
                new Color(MenuVar.OverlayHealthEnemyRed, MenuVar.OverlayHealthEnemyGreen, MenuVar.OverlayHealthEnemyBlue, MenuVar.OverlayAlpha));
            Overlay.DrawTopHelpOverlay(AllyHeroes.Heroes, 
                new Color(MenuVar.OverlayHealthAllyRed, MenuVar.OverlayHealthAllyGreen, MenuVar.OverlayHealthAllyBlue, MenuVar.OverlayAlpha));

            #endregion Top ovellay

            #region JungleStack

            if (MenuVar.StackKey)
            {
                JungleDraw.DrawCamp();
            }

            //foreach (var x in ShowMeMore.Courier)
            //{
            //    try
            //    {
            //        var pos = Ensage.Drawing.WorldToScreen(x.Value);
            //        DrawText(x.Key.Name, (int) pos.X, (int) pos.Y, Color.Aqua, Var.VisibleFont);

            //    }
            //    catch (Exception)
            //    {
            //        //
            //    }
            //}

            //if (Utils.SleepCheck("111"))
            //{
            //    ShowMeMore.Courier.Clear();
            //    Utils.Sleep(3000, "111");
            //}

            #endregion JungleStack

            #region CouInfo

            if (MenuVar.CouForced)
                Overlay.DrawCouForce();
           
            if (MenuVar.CouAbuse)
                Overlay.DrawCouAbuse();
                
            if (MenuVar.CouLock)
                Overlay.DrawCouLock();

            #endregion CouInfo

            #region DevInfo

            if (MenuVar.ShowInfo)
            {
                Dev.DevInfo();
            }
            else
            {
                Dev.DevInfoDispose();
            }

            #endregion DevInfo
        }

        public static void Drawing_OnEndScene(EventArgs args)
        {
            if (Ensage.Drawing.Direct3DDevice9 == null ||
                Ensage.Drawing.Direct3DDevice9.IsDisposed ||
                !OnUpdate.CanUpdate())
                return;

            if (MenuVar.ShowRoshanTimer)
            {
                ShowMeMore.RoshanTimer();
            }
        }

        public static void Drawing_OnPostReset(EventArgs args)
        {
            Fonts.StackFont.OnResetDevice();
            Fonts.UltFont.OnResetDevice();
        }

        public static void Drawing_OnPreReset(EventArgs args)
        {
            Fonts.StackFont.OnLostDevice();
            Fonts.UltFont.OnLostDevice();
        }

        public static void DrawLine(Vector2 xy, Vector2 wh, Color color)
        {
            Ensage.Drawing.DrawRect(xy, wh, color);

            //var vLine = new Vector2[2];

            //Line.GLLines = true;
            //Line.Antialias = false;
            //Line.Width = wh.X;

            //vLine[0].X = xy.X + wh.X / 2;
            //vLine[0].Y = xy.Y;
            //vLine[1].X = xy.X + wh.X / 2;
            //vLine[1].Y = xy.Y + wh.Y;

            //Line.Begin();
            //Line.Draw(vLine, color);
            //Line.End();
        }

        public static void DrawShadowTextDX9(string stext, int x, int y, Color color, Font f)
        {
                f.DrawText(null, stext, x + 1, y + 1, Color.Black);
                f.DrawText(null, stext, x, y, color);
        }

        public static void DrawShadowText(string stext, int x, int y, Color color, Font f)
        {
            Ensage.Drawing.DrawText(stext, f.Description.FaceName, new Vector2(x+1, y+1), new Vector2(f.Description.Height, 0), Color.Black, FontFlags.Outline);
            Ensage.Drawing.DrawText(stext, f.Description.FaceName, new Vector2(x, y), new Vector2(f.Description.Height, 0), color, FontFlags.Outline);
        }
        
        public static void DrawText(string stext, int x, int y, Color color, Font f)
        {
            Ensage.Drawing.DrawText(stext, f.Description.FaceName, new Vector2(x, y), new Vector2(f.Description.Height, 0), color, FontFlags.Outline);
        }

        public static void RoundedRectangle(float x, float y, float w, float h, int iSmooth, Color color)
        {
            var pt = new Vector2[4];

            // Get all corners 
            pt[0].X = x + (w - iSmooth);
            pt[0].Y = y + (h - iSmooth);

            pt[1].X = x + iSmooth;
            pt[1].Y = y + (h - iSmooth);

            pt[2].X = x + iSmooth;
            pt[2].Y = y + iSmooth;

            pt[3].X = x + w - iSmooth;
            pt[3].Y = y + iSmooth;

            // Draw cross 
            Ensage.Drawing.DrawRect(new Vector2(x, y + iSmooth), new Vector2(w, h - (iSmooth * 2)), color);

            Ensage.Drawing.DrawRect(new Vector2(x + iSmooth, y), new Vector2(w - (iSmooth * 2), h), color);

            float fDegree = 0;

            for (var i = 0; i < 4; i++)
            {
                for (var k = fDegree; k < fDegree + ((Math.PI * 2) / 4f); k += (float)(1 * (Math.PI / 180.0f)))
                {
                    Ensage.Drawing.DrawLine(
                        new Vector2(pt[i].X, pt[i].Y),
                        new Vector2(pt[i].X + (float)(Math.Cos(k) * iSmooth), pt[i].Y + (float)(Math.Sin(k) * iSmooth)),
                        color);
                }
                fDegree += (float)(Math.PI * 2) / 4; 
            }
        }

        public static void OnLoad()
        {
            //
        }

        #endregion Methods
    }
}