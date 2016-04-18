namespace AllinOne.AllDrawing
{
    using AllinOne.Menu;
    using AllinOne.Methods;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Overlay
    {
        #region Fields

        private static readonly StatusInfo[] SInfo = new StatusInfo[10];
        private static readonly Dictionary<uint, Vector2> TopPos = new Dictionary<uint, Vector2>();

        #endregion Fields

        #region Methods

        public static void DrawTopOverlay(List<Hero> heroes)
        {
            foreach (var hero in heroes.Where(x => x.IsAlive))
            {
                var pos = GetTopPanelPosition(hero) + GetPos(hero, 0);
                var size = GetTopPanelSize(hero);
                DrawVisionChecker(hero, pos, size);
            }
        }

        public static void DrawTopOverlayHealth(List<Hero> heroes, int height, Color colorF)
        {
            foreach (var hero in heroes.Where(x => x.IsAlive))
            {
                var position = GetTopPanelPosition(hero) + GetPos(hero, 1);
                var size = GetTopPanelSize(hero);
                var colorE = new Color(102, 0, 0, (int) colorF.A);
                var healthdelta = new Vector2(hero.Health * size.X / hero.MaximumHealth, 0);
                DrawTopPanel(position, size, healthdelta, colorF, colorE, height);
            }
        }

        public static void DrawTopOverlayMana(List<Hero> heroes, int height, Color colorF)
        {
            foreach (var hero in heroes.Where(x => x.IsAlive))
            {
                var position = GetTopPanelPosition(hero) + GetPos(hero, 2);
                var size = GetTopPanelSize(hero);
                var colorE = new Color(128, 128, 128, (int) colorF.A);
                var manaDelta = new Vector2(hero.Mana * size.X / hero.MaximumMana, 0);
                DrawTopPanel(position, size, manaDelta, colorF, colorE, height);
            }
        }

        //public static void DrawTopOverlayMore(List<Hero> heroes)
        //{
        //    foreach (var hero in heroes)
        //    {
        //        var position = GetTopPanelPosition(hero) + new Vector2(1, 6);
        //        var size = GetTopPanelSize(hero);
        //        var manaDelta = new Vector2(hero.Mana * size.X / hero.MaximumMana, 0);
        //        DrawTopPanel(position, size, manaDelta, new Color(0, 0, 255, 200), Color.Gray);
        //    }
        //}

        public static void DrawTopOverlayUltimateCooldownLine(List<Hero> heroes, int height, int alpha)
        {
            foreach (var hero in heroes)
            {
                var colorE = new Color(128, 128, 128, alpha);
                var ultimate = hero.Spellbook.SpellR;
                if (ultimate != null && ultimate.Level > 0)
                {
                    var y = hero.IsAlive ? GetPos(hero, 3) : new Vector2(1, 1);
                    var position = GetTopPanelPosition(hero) + y;
                    var size = GetTopPanelSize(hero);
                    var delta = GetTopPanelSize(hero);
                    Color colorF = new Color(0, 230, 0, alpha);
                    if (ultimate.Cooldown > 0)
                    {
                        var e = ultimate.Cooldown / Common.GetCooldown(hero, ultimate);
                        switch (ultimate.AbilityState)
                        {
                            case AbilityState.NotEnoughMana:
                                colorF = new Color(0, 102, 255, alpha);
                                delta = new Vector2(size.X * (1 - e), 0);
                                break;

                            case AbilityState.OnCooldown:
                                colorF = new Color((int) (255 * e), (int) (255 * (1 - e)), 0, alpha);
                                delta = new Vector2(size.X * (1 - e), 0);
                                break;
                        }
                    }
                    if (delta.X > size.X)
                        delta.X = size.X;
                    DrawTopPanel(position, size, delta, colorF, colorE, height);
                }
            }
        }

        public static void DrawTopOverlayUltimateCooldownText(List<Hero> heroes, int alpha)
        {
            foreach (var hero in heroes)
            {
                var ultimate = hero.Spellbook.SpellR;
                if (ultimate != null && ultimate.Level > 0)
                {
                    var position = GetTopPanelPosition(hero);
                    var size = GetTopPanelSize(hero);
                    Color colortext = new Color(255, 255, 0, alpha);
                    var cooldown = (int) Math.Round(ultimate.Cooldown + 0.5);
                    float r;
                    if (ultimate.AbilityState == AbilityState.NotEnoughMana)
                        colortext = new Color(0, 102, 255, alpha);
                    string text = cooldown.ToString();
                    if (cooldown / 100 > 0)
                    {
                        r = size.X / 2 - 11;
                        Draw.DrawLine(position + new Vector2(r - 1, 25), new Vector2(28, 16), new Color(0, 0, 0, alpha));
                    }
                    else if (cooldown / 10 > 0)
                    {
                        r = size.X / 2 - 7;
                        Draw.DrawLine(position + new Vector2(r - 1, 25), new Vector2(18, 16), new Color(0, 0, 0, alpha));
                    }
                    else if (cooldown != 0)
                    {
                        r = size.X / 2 - 3;
                        Draw.DrawLine(position + new Vector2(r - 3, 25), new Vector2(12, 16), new Color(0, 0, 0, alpha));
                    }
                    else
                    {
                        colortext = new Color(0, 255, 0, alpha);
                        if (Math.Abs(Common.GetCooldown(hero, ultimate)) <= 0)
                        {
                            text = ultimate.Level.ToString();
                            r = size.X / 2 - 3;
                            Draw.DrawLine(position + new Vector2(r - 1, 26), new Vector2(12, 15), new Color(0, 0, 0, alpha));
                        }
                        else
                        {
                            text = "Ready";
                            r = size.X / 2 - 20;
                            Draw.DrawLine(position + new Vector2(r - 1, 25), new Vector2(48, 16), new Color(0, 0, 0, alpha));
                        }
                    }
                    Draw.DrawText(text, (int) (position.X + r), (int) (position.Y + 23), colortext, Fonts.UltFont);
                }
            }
        }

        private static void DrawTopPanel(Vector2 pos, Vector2 size, Vector2 delta, Color colorF, Color colorE, int height = 4)
        {
            if (size.X - delta.X > 0)
                Draw.DrawLine(pos + new Vector2(delta.X, size.Y), new Vector2(size.X - delta.X, height), colorE);
            if (delta.X > 0)
                Draw.DrawLine(pos + new Vector2(0, size.Y), new Vector2(delta.X, height), colorF);
            if (delta.X > 0 && delta.X < size.X - 1)
                Draw.DrawLine(pos + new Vector2(delta.X, size.Y), new Vector2(1, height), Color.Black);
        }

        private static void DrawVisionChecker(Hero hero, Vector2 pos, Vector2 size, int height = 4)
        {
            string text;
            var color = Color.White;
            if (!hero.IsVisibleToEnemies && hero.Team == Var.Me.Team)
            {
                text = "under vision";
                color = Color.Green;
            }
            else
            {
                var handle = hero.Player.ID;
                if (SInfo[handle] == null || !SInfo[handle].GetHero().IsValid)
                {
                    SInfo[handle] = new StatusInfo(hero, Game.GameTime);
                }
                text = SInfo[handle].GetTime();
            }
            Draw.DrawLine(pos + new Vector2(0, size.Y), new Vector2(size.X, height * 3), new Color(0, 0, 0, 180));

            Draw.DrawText(text, (int) pos.X + 3, (int) (pos.Y + size.Y), color, Fonts.VisibleFont);
        }

        private static Vector2 GetPos(Entity hero, int s)
        {
            bool myteam = Var.Me.Team == hero.Team;
            switch (s)
            {
                case 0:
                    return new Vector2(1, 1);

                case 1:
                    if (myteam)
                    {
                        if (MenuVar.ShowTopOverlayAlly)
                            return new Vector2(1, 14);
                        else
                            return new Vector2(1, 1);
                    }
                    else
                    {
                        if (MenuVar.ShowTopOverlayEnemy)
                            return new Vector2(1, 14);
                        else
                            return new Vector2(1, 1);
                    }
                case 2:
                    if (myteam)
                    {
                        if (MenuVar.ShowTopOverlayAllyHp)
                        {
                            if (MenuVar.ShowTopOverlayAlly)
                                return new Vector2(1, 15 + MenuVar.HealthHeightAlly);
                            else
                                return new Vector2(1, 2 + MenuVar.HealthHeightAlly);
                        }
                        else
                        {
                            if (MenuVar.ShowTopOverlayAlly)
                                return new Vector2(1, 14);
                            else
                                return new Vector2(1, 1);
                        }
                    }
                    else
                    {
                        if (MenuVar.ShowTopOverlayEnemyHp)
                        {
                            if (MenuVar.ShowTopOverlayEnemy)
                                return new Vector2(1, 15 + MenuVar.HealthHeightEnemy);
                            else
                                return new Vector2(1, 2 + MenuVar.HealthHeightEnemy);
                        }
                        else
                        {
                            if (MenuVar.ShowTopOverlayEnemy)
                                return new Vector2(1, 14);
                            else
                                return new Vector2(1, 1);
                        }
                    }

                case 3:
                    if (myteam)
                    {
                        if (MenuVar.ShowTopOverlayAllyMp)
                        {
                            if (MenuVar.ShowTopOverlayAllyHp)
                            {
                                if (MenuVar.ShowTopOverlayAlly)
                                    return new Vector2(1, 16 + MenuVar.HealthHeightAlly + MenuVar.ManaHeightAlly);
                                else
                                    return new Vector2(1, 3 + MenuVar.HealthHeightAlly + MenuVar.ManaHeightAlly);
                            }
                            else
                            {
                                if (MenuVar.ShowTopOverlayAlly)
                                    return new Vector2(1, 15 + MenuVar.ManaHeightAlly);
                                else
                                    return new Vector2(1, 2 + MenuVar.ManaHeightAlly);
                            }
                        }
                        else
                        {
                            if (MenuVar.ShowTopOverlayAllyHp)
                            {
                                if (MenuVar.ShowTopOverlayAlly)
                                    return new Vector2(1, 15 + MenuVar.HealthHeightAlly);
                                else
                                    return new Vector2(1, 2 + MenuVar.HealthHeightAlly);
                            }
                            else
                            {
                                if (MenuVar.ShowTopOverlayAlly)
                                    return new Vector2(1, 14);
                                else
                                    return new Vector2(1, 1);
                            }
                        }
                    }
                    else
                    {
                        if (MenuVar.ShowTopOverlayEnemyMp)
                        {
                            if (MenuVar.ShowTopOverlayEnemyHp)
                            {
                                if (MenuVar.ShowTopOverlayEnemy)
                                    return new Vector2(1, 16 + MenuVar.HealthHeightEnemy + MenuVar.ManaHeightEnemy);
                                else
                                    return new Vector2(1, 3 + MenuVar.HealthHeightEnemy + MenuVar.ManaHeightEnemy);
                            }
                            else
                            {
                                if (MenuVar.ShowTopOverlayEnemy)
                                    return new Vector2(1, 15 + MenuVar.ManaHeightEnemy);
                                else
                                    return new Vector2(1, 2 + MenuVar.ManaHeightEnemy);
                            }
                        }
                        else
                        {
                            if (MenuVar.ShowTopOverlayEnemyHp)
                            {
                                if (MenuVar.ShowTopOverlayEnemy)
                                    return new Vector2(1, 15 + MenuVar.HealthHeightEnemy);
                                else
                                    return new Vector2(1, 2 + MenuVar.HealthHeightEnemy);
                            }
                            else
                            {
                                if (MenuVar.ShowTopOverlayEnemy)
                                    return new Vector2(1, 14);
                                else
                                    return new Vector2(1, 1);
                            }
                        }
                    }
            }
            return new Vector2();
        }

        private static Vector2 GetTopPanelPosition(Hero hero)
        {
            Vector2 vector2;
            var handle = hero.Handle;
            if (TopPos.TryGetValue(handle, out vector2)) return vector2;
            vector2 = HUDInfo.GetTopPanelPosition(hero);
            TopPos.Add(handle, vector2);
            return vector2;
        }

        private static Vector2 GetTopPanelSize(Hero hero)
        {
            return new Vector2((float) HUDInfo.GetTopPanelSizeX(hero) - 2, (float) HUDInfo.GetTopPanelSizeY(hero));
        }

        #endregion Methods
    }

    internal class StatusInfo
    {
        #region Constructors

        public StatusInfo(Hero hero, float time)
        {
            _hero = hero;
            _time = time;
            _status = "";
        }

        #endregion Constructors

        #region Fields

        private readonly Hero _hero;
        private string _status;
        private float _time;

        #endregion Fields

        #region Methods

        public Hero GetHero()
        {
            return _hero;
        }

        public string GetStatus()
        {
            return !_hero.IsValid ? "heh" : _hero.IsInvisible() ? "invis" : _hero.IsVisible ? "visible" : "in fog";
        }

        public string GetTime()
        {
            var curStat = GetStatus();

            if (_status != GetStatus())
            {
                _time = Game.GameTime;
                _status = curStat;
            }
            if (curStat == "visible") return curStat;
            return curStat + " " + (int) (Game.GameTime - _time);
        }

        #endregion Methods
    }
}