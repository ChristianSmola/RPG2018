using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Database  {
    public static List<Combat> Magic;
    public static List<Items> Item;
    
    public static void Moves()
    {
        Magic = new List<Combat>();

        Magic.Add(new Combat("Fireball", 10, 4, true, Combat.SpellElements.Fire, Combat.SpellEffect.Projectile));
        Magic.Add(new Combat("Lightning", 14, 8, false, Combat.SpellElements.Lightning, Combat.SpellEffect.Chain));
        Magic.Add(new Combat("Ice Spikes", 20, 12, false, Combat.SpellElements.Ice, Combat.SpellEffect.GroundSpikes));
        Magic.Add(new Combat("Earth Disk", 22, 18, true, Combat.SpellElements.Nature, Combat.SpellEffect.Projectile));
        Magic.Add(new Combat("Insect Swarm", 24, 14, false, Combat.SpellElements.Nature, Combat.SpellEffect.Swarm));
    }

    public static void ItemList()
    {
        Item = new List<Items>();

        Item.Add(new Items("Gold Coin", true, Items.Catagory.General, "A gold coin"));
        Item.Add(new Items("Large Mushroom", true, Items.Catagory.Food, "A large ediable mushroom."));

    }

}
