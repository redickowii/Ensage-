namespace AllinOne.Methods
{
    using System.Linq;
    using AllinOne.Menu;
    using AllinOne.ObjectManager;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    internal class AutoUse
    {
        public static void AutoUseMain()
        {
            Bottle();
            Phase_boots();
            Magic_stick();
            Magic_wand();
            Arcane_boots();
            Sphere();
            Cheese();
            Midas();
        }

        private static bool CanUse(string itemName)
        {
            if (MyHeroInfo.GetItems().Any(x => x.Name == itemName) &&
                (!Var.Me.IsInvisible() || Var.Me.ClassID == ClassID.CDOTA_Unit_Hero_Riki) && !Var.Me.IsChanneling())
                return MyHeroInfo.GetItems().First(x => x.Name == itemName).CanBeCasted();
            return false;
        }

        public static void Bottle()
        {
            if (CanUse("item_bottle") && Var.Me.Modifiers.Any(x => x.Name == "modifier_fountain_aura_buff") &&
                MenuVar.ItemBottleUse && Utils.SleepCheck("AutoUse.item_bottle"))
            {
                MyHeroInfo.GetItem("item_bottle").UseAbility(Var.Me, false);
                Utils.Sleep(500, "AutoUse.item_bottle");
            }
        }

        public static void Phase_boots()
        {
            if (CanUse("item_phase_boots") &&
                Var.Me.NetworkActivity == NetworkActivity.Move && MenuVar.ItemPhaseBootsUse &&
                Utils.SleepCheck("AutoUse.item_phase_boots"))
            {
                MyHeroInfo.GetItem("item_phase_boots").UseAbility(false);
                Utils.Sleep(500, "AutoUse.item_phase_boots");
            }
        }

        public static void Magic_stick()
        {
            if (CanUse("item_magic_stick") && (double) Var.Me.Health/Var.Me.MaximumHealth < MenuVar.PercentStickUse &&
                MenuVar.ItemMagicStickUse && Utils.SleepCheck("AutoUse.item_magic_stick"))
            {
                if (MyHeroInfo.GetItem("item_magic_stick").CurrentCharges > 0)
                    MyHeroInfo.GetItem("item_magic_stick").UseAbility(false);
                Utils.Sleep(500, "AutoUse.item_magic_stick");
            }
        }

        public static void Magic_wand()
        {
            if (CanUse("item_magic_wand") && (double) Var.Me.Health/Var.Me.MaximumHealth < MenuVar.PercentStickUse &&
                MenuVar.ItemMagicWandUse && Utils.SleepCheck("AutoUse.item_magic_wand"))
            {
                if (MyHeroInfo.GetItem("item_magic_wand").CurrentCharges > 0)
                    MyHeroInfo.GetItem("item_magic_wand").UseAbility(false);
                Utils.Sleep(500, "AutoUse.item_magic_wand");
            }
        }

        public static void Arcane_boots()
        {
            if (CanUse("item_arcane_boots") && (double) Var.Me.Mana/Var.Me.MaximumMana < MenuVar.PercentArcaneUse &&
                MenuVar.ItemArcaneBootsUse && Utils.SleepCheck("AutoUse.item_arcane_boots"))
            {
                MyHeroInfo.GetItem("item_arcane_boots").UseAbility(false);
                Utils.Sleep(500, "AutoUse.item_arcane_boots");
            }
        }

        public static void Sphere()
        {
            if (CanUse("item_sphere") && MenuVar.ItemArcaneBootsUse && Utils.SleepCheck("AutoUse.item_sphere"))
            {
                MyHeroInfo.GetItem("item_sphere").UseAbility(false);
                Utils.Sleep(500, "AutoUse.item_sphere");
            }
        }

        public static void Cheese()
        {
            if (CanUse("item_cheese") && (double) Var.Me.Health/Var.Me.MaximumHealth < MenuVar.PercentCheeseUse &&
                MenuVar.ItemCheeseUse && Utils.SleepCheck("AutoUse.item_cheese"))
            {
                MyHeroInfo.GetItem("item_cheese").UseAbility(false);
                Utils.Sleep(500, "AutoUse.item_cheese");
            }
        }

        public static void Midas()
        {
            if (!MenuVar.ItemHandOfMidasUse || !Utils.SleepCheck("AutoUse.item_hand_of_midas")) return;
            var midas = FindMidas(Var.Me);
            var midasOwner = (Unit) midas?.Owner;
            if (midasOwner != null && !midasOwner.IsChanneling() && !midasOwner.IsInvisible())
            {
                UseMidas(midas, midasOwner);
                Utils.Sleep(500, "AutoUse.item_hand_of_midas");
            }
        }

        private static Item FindMidas(Unit entity)
        {
            if (entity.ClassID.Equals(ClassID.CDOTA_Unit_Hero_LoneDruid))
            {
                var bear = ObjectManager.GetEntities<Unit>().Where(unit => unit.ClassID.Equals(ClassID.CDOTA_Unit_SpiritBear) && unit.IsAlive && unit.Team.Equals(Var.Me.Team) && unit.IsControllable).ToList();
                var heroMidas = entity.FindItem("item_hand_of_midas");
                if (heroMidas.CanBeCasted()) return heroMidas;
                return bear.Any() ? bear.First().FindItem("item_hand_of_midas") : null;
            }
            else
            {
                var heroMidas = entity.FindItem("item_hand_of_midas");
                return heroMidas;
            }
        }

        private static void UseMidas(Ability midas, Unit owner)
        {
            if (midas.CanBeCasted() && owner.CanUseItems())
            {
                if (MenuVar.MidasAllUse)
                {
                    var creeps =
                        ObjectManager.GetEntities<Creep>()
                            .Where(
                                creep =>
                                    creep.Team != owner.Team && creep.IsAlive && creep.IsVisible && creep.IsSpawned &&
                                    !creep.IsAncient && creep.Health > 0 &&
                                    creep.Distance2D(owner) < midas.CastRange + 25)
                            .ToList();
                    if (!creeps.Any()) return;
                    midas.UseAbility(creeps.First());
                }
                else
                {
                    var creeps =
                        ObjectManager.GetEntities<Creep>()
                            .Where(
                                creep =>
                                    creep.Team != owner.Team && creep.IsAlive && creep.IsVisible && creep.IsSpawned &&
                                    !creep.IsAncient && creep.Health > 950 &&
                                    creep.Distance2D(owner) < midas.CastRange + 25)
                            .ToList();
                    if (!creeps.Any()) return;
                    midas.UseAbility(creeps.First());
                }
            }
        }
    }
}
