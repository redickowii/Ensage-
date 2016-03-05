using System;
using System.Collections.Generic;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

using SharpDX;
using System.Linq;

namespace Lina
{
    internal class Program
    {
        private static Ability Q, W, R;
        private static Item Dagon, Hex, Ethereal, Veil, Orchid, Shiva, Eul, Blink;
        private static Hero _me;
        private static Hero _target;
        private static readonly Menu Menu = new Menu("Lina", "Lina", true, "npc_dota_hero_lina", true);
        private static bool _targetActive;
        private static AbilityToggler _menuValue;
        private static int _slider;

        private static void Main(string[] args)
        {
            var dict = new Dictionary<string, bool>
                           {
                                { "item_dagon", true },
                                { "item_sheepstick", true },
                                { "item_ethereal_blade", true },
                                { "item_veil_of_discord", true },
                                { "item_blink", true },
                                { "item_orchid", true },
                                { "item_shivas_guard", true },
                                { "item_cyclone", true }
                           };
            Menu.AddItem(new MenuItem("enabledAbilities", "    ").SetValue(new AbilityToggler(dict)));
            Menu.AddItem(new MenuItem("Cooombo", "Cooombo").SetValue(new KeyBind('6', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("distance", "Blink distance").SetValue(new Slider(575, 0, 1000)));
            
            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            _me = ObjectManager.LocalHero;
            if (!Game.IsInGame || _me == null || _me.ClassID != ClassID.CDOTA_Unit_Hero_Lina)
            {
                return;
            }
            if (Game.IsPaused || Game.IsChatOpen)
            {
                return;
            }

            _menuValue = Menu.Item("enabledAbilities").GetValue<AbilityToggler>();
            _slider = Menu.Item("distance").GetValue<Slider>().Value;

            Q = _me.Spellbook.Spell1;
            W = _me.Spellbook.Spell2;
            R = _me.Spellbook.Spell4;

            Dagon = _me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            Hex = _me.FindItem("item_sheepstick");
            Ethereal = _me.FindItem("item_ethereal_blade");
            Veil = _me.FindItem("item_veil_of_discord");
            Orchid = _me.FindItem("item_orchid");
            Shiva = _me.FindItem("item_shivas_guard");
            Eul = _me.FindItem("item_cyclone");
            Blink = _me.FindItem("item_blink");

            if (!Game.IsKeyDown(Menu.Item("Cooombo").GetValue<KeyBind>().Key))
            {
                _targetActive = false;
                return;
            }

            if (!_targetActive)
            {
                _target = _me.ClosestToMouseTarget(300);
                _targetActive = true;
            }
            else
            {
                var modifHex =
                    _target.Modifiers.Where(y => y.Name == "modifier_sheepstick_debuff")
                        .DefaultIfEmpty(null)
                        .FirstOrDefault();
                var modifEul =
                    _target.Modifiers.Where(y => y.Name == "modifier_eul_cyclone").DefaultIfEmpty(null).FirstOrDefault();

                if (_target == null || !_target.IsAlive || _target.IsIllusion || _target.IsMagicImmune()) return;

                if (Blink != null && Blink.CanBeCasted() && _me.Distance2D(_target) > _slider + 100 && _menuValue.IsEnabled("item_blink") && Utils.SleepCheck("blink"))
                {
                    Blink.UseAbility(PositionCalc(_me, _target, _slider));
                    Utils.Sleep(150 + Game.Ping, "blink");
                }
                else if (Eul != null && Eul.CanBeCasted() && Utils.SleepCheck("eul") && _menuValue.IsEnabled("item_cyclone") && Utils.SleepCheck("blink"))
                {
                    Eul.UseAbility(_target);
                    Utils.Sleep(4000 + Game.Ping, "eul");
                }
                else if (Eul == null || Eul.Cooldown != 0 || !_menuValue.IsEnabled("item_cyclone"))
                {
                    if (Orchid != null && Orchid.CanBeCasted() && Utils.SleepCheck("orchid") && modifEul == null &&
                        _menuValue.IsEnabled("item_orchid"))
                    {
                        Orchid.UseAbility(_target);
                        Utils.Sleep(150 + Game.Ping, "orchid");
                    }
                    else if (Shiva != null && Shiva.CanBeCasted() && Utils.SleepCheck("shiva") && modifEul == null &&
                             _menuValue.IsEnabled("item_shivas_guard"))
                    {
                        Shiva.UseAbility();
                        Utils.Sleep(150 + Game.Ping, "shiva");
                    }
                    else if (Veil != null && Veil.CanBeCasted() && Utils.SleepCheck("veil") && modifEul == null &&
                             _menuValue.IsEnabled("item_veil_of_discord"))
                    {
                        Veil.UseAbility(_target.Position);
                        Utils.Sleep(150 + Game.Ping, "veil");
                    }
                    else if (Ethereal != null && Ethereal.CanBeCasted() && Utils.SleepCheck("ethereal") &&
                             modifEul == null && _menuValue.IsEnabled("item_ethereal_blade"))
                    {
                        Ethereal.UseAbility(_target);
                        Utils.Sleep(150 + Game.Ping, "ethereal");
                    }
                    else if (Dagon != null && Dagon.CanBeCasted() && Utils.SleepCheck("dagon") &&
                             modifEul == null)
                    {
                        Dagon.UseAbility(_target);
                        Utils.Sleep(150 + Game.Ping, "dagon");
                    }
                    else if (Hex != null && Hex.CanBeCasted() && Utils.SleepCheck("hex") &&
                             !_target.IsStunned() &&
                             Utils.SleepCheck("eul") && _menuValue.IsEnabled("item_sheepstick"))
                    {
                        Hex.UseAbility(_target);
                        Utils.Sleep(150 + Game.Ping, "hex");
                    }
                    else if (W != null && W.CanBeCasted() && Utils.SleepCheck("w") &&
                             (modifEul != null && modifEul.RemainingTime <= W.GetCastDelay(_me, _target, true) + 0.5 ||
                              modifHex != null && modifHex.RemainingTime <= W.GetCastDelay(_me, _target, true) + 0.5 ||
                              (Hex == null || !_menuValue.IsEnabled("item_sheepstick") || Hex.Cooldown > 0) && 
                              (Eul == null || !_menuValue.IsEnabled("item_cyclone") || Eul.Cooldown <20)))
                    {
                        W.UseAbility(W.GetPrediction(_target, W.GetCastDelay(_me, _target)));
                        Utils.Sleep(150 + Game.Ping, "w");
                    }
                    else if (Q != null && Q.CanBeCasted() && Utils.SleepCheck("q") &&
                             modifEul == null)
                    {
                        Q.UseAbility(_target);
                        Utils.Sleep(150 + Game.Ping, "q");
                    }
                    else if (R != null && R.CanBeCasted() && Utils.SleepCheck("r") &&
                             modifEul == null)
                    {
                        R.UseAbility(_target);
                        Utils.Sleep(150 + Game.Ping, "r");
                    }
                    else if (!_me.IsChanneling() && NothingCanCast() &&
                             !_target.IsAttackImmune() && Utils.SleepCheck("attack"))
                    {
                        _me.Attack(_target);
                        Utils.Sleep(1000 + Game.Ping, "attack");
                    }
                }
            }
        }

        private static Vector3 PositionCalc(Hero me, Hero target, float M)
        {
            var l = (me.Distance2D(target) - M ) / M;
            var posA = me.Position;
            var posB = target.Position;
            var x = (posA.X + l * posB.X) / (1 + l);
            var y = (posA.Y + l * posB.Y) / (1 + l);
            return new Vector3((int) x, (int) y,posA.Z);
        }

        private static bool NothingCanCast()
        {
            return !Q.CanBeCasted() && !W.CanBeCasted() && !R.CanBeCasted() && !Dagon.CanBeCasted() &&
                   (!Ethereal.CanBeCasted() || !_menuValue.IsEnabled("item_ethereal_blade")) &&
                   (!Hex.CanBeCasted() || !_menuValue.IsEnabled("item_sheepstick")) &&
                   (!Shiva.CanBeCasted() || !_menuValue.IsEnabled("item_shivas_guard")) &&
                   (!Eul.CanBeCasted() || !_menuValue.IsEnabled("item_cyclone")) &&
                   (!Veil.CanBeCasted() || !_menuValue.IsEnabled("item_veil_of_discord")) &&
                   (!Orchid.CanBeCasted() || !_menuValue.IsEnabled("item_orchid"));
        }
    }
}
