using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Items {
    public string ItemName;
    public string ItemDiscription;
    private bool Stackable;
    private Catagory catagory;
    public GameObject ItemMesh;
    public enum Catagory
    {
        General,
        Weapon,
        Armor,
        Food,
        Potion,
        Book
    }

    public Items(string name, bool stack, Catagory cat, string discription)
    {
        ItemName = name;
        Stackable = stack;
        catagory = cat;
        ItemDiscription = discription;
        ItemMesh = Resources.Load<GameObject>("Prefabs/Items/" + ItemName);

    }

}
