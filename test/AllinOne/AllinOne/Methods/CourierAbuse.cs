using System;
using AllinOne.ObjectManager;

namespace AllinOne.Methods
{
    using System.Linq;
    using AllinOne.Menu;
    using AllinOne.ObjectManager.Heroes;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using AllinOne.Variables;

    internal class CourierAbuse
    {
        public static bool Following;

        public static void Main2()
        {
            if (!Utils.SleepCheck("Courier_rate") || 
                Couriers.AllyCouriers == null ||
                Buildings.AllyFountain == null)
                return;

            var courierfontain = ClosestToFontain();
            var courierhero = ClosestToMyHero();
            var couriermouse = ClosestToMouse();
            var courierbottle = HavingBottle();

            #region Avoid enemy

            foreach (var courier in Couriers.AllyCouriers)
            {
                if (MenuVar.CouAvoidEnemy)
                {
                    foreach (var enemy in EnemyHeroes.Heroes)
                    {
                        if (enemy.Distance2D(courier) < MenuVar.CouAvoidEnemyRange)
                        {
                            var burst = courier.Spellbook.SpellR;
                            if (courier.IsFlying && burst.CanBeCasted())
                                burst.UseAbility();
                        }
                    }
                }
                Utils.Sleep(MenuVar.CouCd, "Courier_rate");
            }

            #endregion Avoid enemy

            #region Anti reuse

            foreach (var courier in Couriers.AllyCouriers)
            {			
                if (MenuVar.CouForced && !MenuVar.CouAbuse)
                {
                    if (Var.Me.Inventory.StashItems.Any())
                    {
                        courierfontain.Spellbook.SpellD.UseAbility();
                    }
                    else if (courier.Inventory.Items.Any())
                    {
                        courier.Spellbook.SpellF.UseAbility();
                        courier.Spellbook.SpellQ.UseAbility(true);
                    }
                }
                Utils.Sleep(MenuVar.CouCd, "Courier_rate");
            }

            #endregion Anti reuse

            #region lock at base

            foreach (var courier in Couriers.AllyCouriers.Where(x => x.Distance2D(Buildings.AllyFountain) > 900))
            {
                if (MenuVar.CouLock && !MenuVar.CouForced && !MenuVar.CouAbuse)
                    courier.Spellbook.SpellQ.UseAbility();

                Utils.Sleep(MenuVar.CouCd, "Courier_rate");
            }

            #endregion lock at base

            #region abuse bottle

            foreach (var courier in Couriers.AllyCouriers)
            {

                if (MenuVar.CouAbuse)
                {
                    var bottle = Var.Me.Inventory.Items.FirstOrDefault(x => x.Name == "item_bottle");
                    var courBottle = courier.Inventory.Items.FirstOrDefault(x => x.Name == "item_bottle");

                    var distance = Var.Me.Distance2D(courier);

                    if (distance > 200 && !Following)
                    {
                        if (courier.HasModifier("modifier_fountain_aura_buff") && courBottle != null)
                        {
                            if (Var.Me.Inventory.StashItems.Any())
                                courier.Spellbook.SpellD.UseAbility();
                            courier.Spellbook.SpellF.UseAbility(true);
                            courier.Follow(Var.Me, true);
                            Following = true;
                        }
                        else if (courBottle == null)
                        {
                            courier.Stop();
                            courier.Follow(Var.Me, true);
                            Following = true;
                        }
                    }
                    else if (distance <= 200 && bottle != null && bottle.CurrentCharges == 0)
                    {
                        Var.Me.Stop();
                        Var.Me.GiveItem(bottle, courier);
                    }
                    else if (courBottle != null && courBottle.CurrentCharges < 3)
                    {
                        courier.Spellbook.SpellQ.UseAbility();
                        var burst = courier.Spellbook.SpellR;
                        if (courier.IsFlying && burst.CanBeCasted() && MenuVar.CouBurst)
                            burst.UseAbility();
                        Following = false;
                    }

                    Utils.Sleep(MenuVar.CouCd, "Courier_rate");
                }
                else if (Following)
                {
                    courier.Spellbook.SpellQ.UseAbility();
                    Following = false;
                }
            }

            #endregion abuse bottle
        }

        public static Courier ClosestToMyHero()
        {
            Courier[] closestCourier = { null };
            foreach (var cour in Couriers.AllyCouriers.Where(cour =>
                            closestCourier[0] == null ||
                            closestCourier[0].Distance2D(Var.Me.Position) > cour.Distance2D(Var.Me.Position)))
            {
                closestCourier[0] = cour;
            }
            return closestCourier[0];
        }

        public static Courier ClosestToFontain()
        {
            Courier[] closestCourier = { null };
            foreach (var cour in Couriers.AllyCouriers.Where(x =>
                            closestCourier[0] == null ||
                            closestCourier[0].Distance2D(Buildings.AllyFountain.Position) > x.Distance2D(Buildings.AllyFountain.Position)))
            {
                closestCourier[0] = cour;
            }
            return closestCourier[0];
        }

        public static Courier ClosestToMouse()
        {
            var mousePosition = Game.MousePosition;
            Courier[] closestCourier = { null };
            foreach (var cour in Couriers.AllyCouriers.Where(cour =>
                            closestCourier[0] == null ||
                            closestCourier[0].Distance2D(mousePosition) > cour.Distance2D(mousePosition)))
            {
                closestCourier[0] = cour;
            }
            return closestCourier[0];
        }

        public static Courier HavingBottle()
        {
            Courier[] closestCourier = { null };
            foreach (var cour in Couriers.AllyCouriers.Where(cour =>
                            cour.Inventory.Items.FirstOrDefault(x => x.Name == "item_bottle") != null))
            {
                closestCourier[0] = cour;
            }
            return closestCourier[0];
        }

    }
}
