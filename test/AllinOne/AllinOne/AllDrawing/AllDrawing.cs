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

            #endregion Top ovellay

            #region JungleStack

            if (MenuVar.StackKey)
            {
                JungleDraw.DrawCamp();
            }

            if (MenuVar.ShowRoshanTimer)
            {
                ShowMeMore.RoshanTimer();
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

            //if (Common.SleepCheck("111"))
            //{
            //    ShowMeMore.Courier.Clear();
            //    Common.Sleep(3000, "111");
            //}

            #endregion JungleStack
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
            var vLine = new Vector2[2];

            Line.GLLines = true;
            Line.Antialias = false;
            Line.Width = wh.X;

            vLine[0].X = xy.X + wh.X / 2;
            vLine[0].Y = xy.Y;
            vLine[1].X = xy.X + wh.X / 2;
            vLine[1].Y = xy.Y + wh.Y;

            Line.Begin();
            Line.Draw(vLine, color);
            Line.End();
        }

        public static void DrawShadowText(string stext, int x, int y, Color color, Font f)
        {
            f.DrawText(null, stext, x + 1, y + 1, Color.Black);
            f.DrawText(null, stext, x, y, color);
        }

        public static void DrawText(string stext, int x, int y, Color color, Font f)
        {
            f.DrawText(null, stext, x, y, color);
        }

        public static void RoundedRectangle(Vector2 xy, Vector2 wh, int iSmooth, Color color)
        {
            var pt = new Vector2[4];
            var vLine = new Vector2[2];

            pt[0].X = xy.X + (wh.X - iSmooth);
            pt[0].Y = xy.Y + (wh.Y - iSmooth);

            pt[1].X = xy.X + iSmooth;
            pt[1].Y = xy.Y + (wh.Y - iSmooth);

            pt[2].X = xy.X + iSmooth;
            pt[2].Y = xy.Y + iSmooth;

            pt[3].X = xy.X + wh.X - iSmooth;
            pt[3].Y = xy.Y + iSmooth;

            DrawLine(xy + new Vector2(1, iSmooth), wh - new Vector2(1, iSmooth * 2), color);
            DrawLine(xy + new Vector2(iSmooth, 1), wh - new Vector2(iSmooth * 2, 1), color);
            float fDegree = 0;
            Line.Width = 1;
            for (var i = 0; i < 4; i++)
            {
                for (var k = fDegree; k < fDegree + Math.PI * 2 / 4f; k += (float) (1 * (Math.PI / 180.0f)))
                {
                    vLine[0].X = pt[i].X;
                    vLine[0].Y = pt[i].Y;
                    vLine[1].X = pt[i].X + (float) (Math.Cos(k) * iSmooth);
                    vLine[1].Y = pt[i].Y + (float) (Math.Sin(k) * iSmooth);

                    Line.Begin();
                    Line.Draw(vLine, color);
                    Line.End();
                }

                fDegree += (float) (Math.PI * 2) / 4;
            }
        }

        public static void OnLoad()
        {
            //
        }

        #endregion Methods
    }
}