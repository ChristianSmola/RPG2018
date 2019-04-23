using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Combat {
    public enum SpellElements
    {
        Fire,
        Ice, 
        Lightning, 
        Psychic, 
        Summoning,
        Nature
    }
    public enum SpellEffect
    {
        Projectile,
        Chain, 
        Swarm,
        GroundSpikes

    }
    public bool Chargeable;
    public float ManaCost;
    public float Damage;
    public GameObject Spell;
    public SpellEffect Spelleffect;
    public SpellElements Spellelement;
    public string AttackName;

    public Combat(string Name, float damage, float Cost, bool chargeable, SpellElements element, SpellEffect effect)
    {
        AttackName = Name;
        Damage = damage;
        ManaCost = Cost;
        Chargeable = chargeable;
        Spellelement = element;
        Spelleffect = effect;
        Spell = Resources.Load<GameObject>("Prefabs/Spells/" + AttackName);
    }
}
