namespace AllinOne.ObjectManager.Heroes
{
    using AllinOne.Menu;
    using AllinOne.Methods;
    using AllinOne.Variables;
    using Ensage;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class EnemyHeroes
    {
        #region Fields

        public static Dictionary<float, List<Ability>> AbilityDictionary;

        public static List<Hero> Heroes;

        public static List<Hero> Illusions;
        public static Dictionary<float, List<Item>> ItemDictionary;
        public static Hero[] UsableHeroes;

        #endregion Fields

        #region Methods

        public static void Update()
        {
            if (Common.SleepCheck("getheroes"))
            {
                UpdateHeroes();
                Common.Sleep(1000, "getheroes");
            }
            if (Common.SleepCheck("getillu"))
            {
                UpdateIllusions();
                Common.Sleep(100, "getillu");
            }
            Illusions = Illusions.Where(x => x.IsValid).ToList();
            Heroes = Heroes.Where(x => x.IsValid).ToList();
            UsableHeroes = Heroes.Where(x => x.Health > 0 && x.IsAlive && x.IsVisible).ToArray();
            if (Common.SleepCheck("enemyHeroesCheckValid") || UsableHeroes.Any(x => !ItemDictionary.ContainsKey(x.Handle)))
            {
                Common.Sleep(2000, "enemyHeroesCheckValid");
                foreach (var hero in UsableHeroes)
                {
                    var handle = hero.Handle;
                    var items = hero.Inventory.Items.ToList();
                    if (ItemDictionary.ContainsKey(handle))
                    {
                        ItemDictionary[handle] =
                            items.Where(
                                x => x.AbilityType != AbilityType.Attribute && x.AbilityType != AbilityType.Hidden)
                                .ToList();
                        continue;
                    }
                    var itemlist =
                        items.Where(x => x.AbilityType != AbilityType.Attribute && x.AbilityType != AbilityType.Hidden)
                            .ToList();
                    ItemDictionary.Add(handle, itemlist);
                }
            }
        }

        private static void UpdateHeroes()
        {
            try
            {
                var list = ObjectManager.GetEntities<Hero>().Where(x => x.Team != Var.Me.Team).ToList();
                //if (list.Count < Heroes.Count) Heroes.Clear();
                var heroeslist = new List<Hero>(Heroes);
                foreach (var hero in list.Where(x => x.Team != Var.Me.Team && x.IsValid && x.IsVisible && !x.IsIllusion))
                {
                    var handle = hero.Handle;
                    var spells = hero.Spellbook.Spells.ToList();
                    if (!heroeslist.Contains(hero))
                    {
                        Heroes.Add(hero);
                    }
                    var abilitylist =
                        spells.Where(x => x.AbilityType != AbilityType.Attribute && x.AbilityType != AbilityType.Hidden)
                            .ToList();
                    if (AbilityDictionary.ContainsKey(handle))
                    {
                        AbilityDictionary[handle] = abilitylist;
                        continue;
                    }
                    AbilityDictionary.Add(handle, abilitylist);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        private static void UpdateIllusions()
        {
            try
            {
                var illusionslist = Illusions.Where(illusion => !illusion.IsAlive).ToList();
                illusionslist.ForEach(x => Illusions.Remove(x));
                illusionslist = new List<Hero>(Illusions);
                var list = ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != Var.Me.Team && x.IsValid && x.IsVisible && x.IsIllusion && x.IsAlive).ToList();
                foreach (var illusion in list)
                {
                    if (!illusionslist.Contains(illusion))
                    {
                        Illusions.Add(illusion);
                    }
                }
            }
            catch (Exception)
            {
                if (MenuVar.ShowErrors)
                    Console.WriteLine("Update Illusions Error");
            }
        }

        #endregion Methods
    }
}