namespace AllinOne.Methods
{
    using AllinOne.Menu;
    using AllinOne.ObjectManager;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Linq;

    internal class Dodge
    {
        #region Fields

        private static Vector3 _position;

        #endregion Fields

        #region Methods

        public static void AoeDodge(Vector3 pos, float radius, float delay = 0)
        {
            var calc =
                Math.Floor(Math.Sqrt(Math.Pow(pos.X - Var.Me.Position.X, 2) + Math.Pow(pos.Y - Var.Me.Position.Y, 2)));
            var dodgex = (float) (pos.X + (radius / calc) * (Var.Me.Position.X - pos.X));
            var dodgey = (float) (pos.Y + (radius / calc) * (Var.Me.Position.Y - pos.Y));
            if (calc < radius)
            {
                var dodgevector = new Vector3(dodgex, dodgey, Var.Me.Position.Z);
                var turntime = Math.Max(
                        Math.Abs(Var.Me.RotationRad - Utils.DegreeToRadian(Common.FindRet(Var.Me.Position, dodgevector))) -
                        0.69, 0) / (0.6 * (1 / 0.03));
                if ((turntime + Var.Me.Distance2D(dodgevector) / Var.Me.MovementSpeed) * 1000 + Game.Ping > delay && !NothingCanCast())
                {
                    UseSpell(delay);
                }
                else if (Var.Me.Position != _position)
                {
                    Var.Me.Move(dodgevector);
                    _position = Var.Me.Position;
                }
                else
                {
                    var x1 = (float) (pos.X + (Var.Me.Position.X - pos.X) * Math.Cos(0.5) - (Var.Me.Position.Y - pos.Y) * Math.Sin(0.5));
                    var y1 = (float) (pos.Y + (Var.Me.Position.Y - pos.Y) * Math.Sin(0.5) + (Var.Me.Position.X - pos.X) * Math.Cos(0.5));
                    var dodgevector3 = new Vector3(x1, y1, Var.Me.Position.Z);
                    Var.Me.Move(dodgevector3);
                }
            }
        }

        public static void Check()
        {
            if (!Utils.SleepCheck("Dodge.Wait")) return;
            if (ShowMeMore.SpellRadius.Count(x => !x.Effect.IsDestroyed) > 0)
            {
                foreach (AoeSpellStruct t in ShowMeMore.SpellRadius)
                {
                    AoeDodge(t.Position, t.Range + Var.Me.HullRadius + 30, t.Time);
                    t.Time -= MenuVar.DodgeFrequency;
                }
                Utils.Sleep(MenuVar.DodgeFrequency, "Dodge.Wait");
            }
            if (ShowMeMore.EffectForSpells.Count(x => !x.Value.IsDestroyed) > 0)
            {
                foreach (var effect in ShowMeMore.EffectForSpells)
                {
                    var pos1 = effect.Value.GetControlPoint(1);
                    var pos2 = effect.Value.GetControlPoint(2);
                    switch (effect.Key.ClassID)
                    {
                        case ClassID.CDOTA_Ability_Pudge_MeatHook:
                            LineDodge(pos1, pos2, 100 + Var.Me.HullRadius + 30, 1600);
                            break;

                        case ClassID.CDOTA_Ability_Windrunner_Powershot:
                            LineDodge(pos1, pos2, 125 + Var.Me.HullRadius + 30, 3000, 600);
                            break;

                        case ClassID.CDOTABaseAbility:
                            LineDodge(pos1, pos2, 100 + Var.Me.HullRadius + 30, 857);
                            break;

                        case ClassID.CDOTA_Ability_Puck_IllusoryOrb:
                            LineDodge(pos1, pos2, 225 + Var.Me.HullRadius + 30, 650);
                            break;

                        case ClassID.CDOTA_Ability_Jakiro_IcePath:
                            LineDodge(pos1, pos2, 150 + Var.Me.HullRadius + 30, float.MaxValue, 500 + 650 - 220);
                            break;

                        case ClassID.CDOTA_Ability_Jakiro_Macropyre:
                            LineDodge(pos1, pos2, 240 + Var.Me.HullRadius + 30, float.MaxValue, 650 + 650 - 220);
                            break;
                    }
                }
                Utils.Sleep(MenuVar.DodgeFrequency, "Dodge.Wait");
            }
        }

        public static void LineDodge(Vector3 pos1, Vector3 pos2, float radius, float speed, float delay = 0)
        {
            var calc1 =
                Math.Floor(Math.Sqrt(Math.Pow(pos2.X - Var.Me.Position.X, 2) + Math.Pow(pos2.Y - Var.Me.Position.Y, 2)));
            var calc2 =
                Math.Floor(Math.Sqrt(Math.Pow(pos1.X - Var.Me.Position.X, 2) + Math.Pow(pos1.Y - Var.Me.Position.Y, 2)));
            var calc4 = Math.Floor(Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2)));

            var perpendicular =
                Math.Floor(
                    Math.Abs((pos2.X - pos1.X) * (pos1.Y - Var.Me.Position.Y) -
                             (pos1.X - Var.Me.Position.X) * (pos2.Y - pos1.Y)) /
                    Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2)));
            var k = ((pos2.Y - pos1.Y) * (Var.Me.Position.X - pos1.X) - (pos2.X - pos1.X) * (Var.Me.Position.Y - pos1.Y)) /
                    (Math.Pow(pos2.Y - pos1.Y, 2) + Math.Pow(pos2.X - pos1.X, 2));
            var x4 = Var.Me.Position.X - k * (pos2.Y - pos1.Y);
            var z4 = Var.Me.Position.Y + k * (pos2.X - pos1.X);
            var calc3 =
                (Math.Floor(Math.Sqrt(Math.Pow(x4 - Var.Me.Position.X, 2) + Math.Pow(z4 - Var.Me.Position.Y, 2))));
            var dodgex = x4 + (radius / calc3) * (Var.Me.Position.X - x4);
            var dodgey = z4 + (radius / calc3) * (Var.Me.Position.Y - z4);

            if (perpendicular < radius && calc1 < calc4 && calc2 < calc4)
            {
                var dodgevector = new Vector3((float) dodgex, (float) dodgey, Var.Me.Position.Z);
                var dodgevector2 = new Vector3((float) (x4 + (radius / 5 / calc3) * (Var.Me.Position.X - x4)),
                    (float) (z4 + (radius / 5 / calc3) * (Var.Me.Position.Y - z4)), Var.Me.Position.Z);

                delay = Var.Me.Distance2D(pos1) / speed * 1000 + delay;
                var turntime =
                    (Math.Max(
                        Math.Abs(Var.Me.RotationRad - Utils.DegreeToRadian(Common.FindRet(Var.Me.Position, dodgevector))) -
                        0.69, 0) / (0.6 * (1 / 0.03)));
                if ((turntime + Var.Me.Distance2D(dodgevector) / Var.Me.MovementSpeed) * 1000 + Game.Ping > delay && !NothingCanCast())
                {
                    UseSpell();
                }
                else if (Var.Me.Distance2D(dodgevector) > 5)
                {
                    Var.Me.Move(dodgevector);
                }
            }
        }

        public static void UseSpell(float delay = 0)
        {
            var ability = MyHeroInfo.GetAbilities();
            var listitems = MyHeroInfo.GetItems();
        }

        private static bool NothingCanCast()
        {
            var listitems = MyHeroInfo.GetItems();
            if (listitems.Any(x => AllDodge.Eul.Contains(x.Name)))
                if (listitems.First(x => AllDodge.Eul.Contains(x.Name)).CanBeCasted())
                    return false;
            if (listitems.Any(x => AllDodge.BlinkAbilities.Contains(x.Name)))
                if (listitems.First(x => AllDodge.BlinkAbilities.Contains(x.Name)).CanBeCasted())
                    return false;
            return true;
        }

        #endregion Methods
    }
}