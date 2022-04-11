using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.Serialization;

[DataContract]
public class UnitData
{
    [DataMember]
    public string UnitName;

    [DataMember]
    public UnitClass UnitClass;
        
    [DataMember]
    public int MaxHealth;
    
    [DataMember]
    public int CurrentHealth;
    
    [DataMember]
    public Faction UnitFaction;
    
    [DataMember]
    public int MaxMoves;
    
    [DataMember]
    public int RemainingMoves;
    
    [DataMember]
    public int MaxAttacks;
    
    [DataMember]
    public int RemainingAttacks;
    
    [DataMember]
    public string ArmorPrefabName;
    
    [DataMember]
    public string WeaponPrefabName;

    [DataMember]
    public string BaublePrefabName;
    
    [DataMember]
    public string UnitPrefabName;
    
    [DataMember]
    public Vector3 Position;

    [DataMember]
    public Quaternion Rotation;

    [DataMember]    
    public string UnitDisplayName;

    public UnitData(Combatant combatant) {
        UnitName = combatant.UnitName;
        UnitClass = combatant.UnitClass;
        MaxHealth = combatant.MaxHealth;
        CurrentHealth = combatant.CurrentHealth;
        UnitFaction = combatant.UnitFaction;
        MaxMoves = combatant.MaxMoves;
        RemainingMoves = combatant.RemainingMoves;
        MaxAttacks = combatant.MaxAttacks;
        RemainingAttacks = combatant.RemainingAttacks;
        ArmorPrefabName = combatant.ArmorName;
        WeaponPrefabName = combatant.WeaponName;
        BaublePrefabName = combatant.BaubleName;
        UnitPrefabName = combatant.PrefabName;
        Position = combatant.transform.position;
        Rotation = combatant.transform.rotation;
        UnitDisplayName = UnitName + " (" + StringEnums.GetUnitClassString(UnitClass) + ")";
        Debug.Log(UnitDisplayName);
    }

    public UnitData(string Name, UnitClass c) {
        UnitName = Name;
        UnitClass = c;
        MaxHealth = 10 * UnityEngine.Random.Range(8, 11); // 8 to 10 -> 80, 90, or 100
        CurrentHealth = MaxHealth;
        UnitFaction = Faction.PLAYER;
        MaxMoves = UnityEngine.Random.Range(5, 8); // 5 to 7;
        RemainingMoves = MaxMoves;
        MaxAttacks = 1;
        RemainingAttacks = 1;
        BaublePrefabName = null;
        ArmorPrefabName = GetClassArmorPrefab(c);
        WeaponPrefabName = GetClassWeaponPrefab(c);
        UnitPrefabName = GetClassPrefab(c);
        Position = new Vector3();
        Rotation = new Quaternion();
        UnitDisplayName = UnitName + " (" + StringEnums.GetUnitClassString(UnitClass) + ")";
        Debug.Log(UnitDisplayName);
    }


    public string GetClassPrefab(UnitClass c) {
        if (c == UnitClass.WARRIOR) {
            return "Player Warrior";
        }

        if (c == UnitClass.CLERIC) {
            return "Player Cleric";
        }

        if (c == UnitClass.MAGE) {
            return "Player Mage";
        }

        if (c == UnitClass.SCOUT) {
            return "Player Scout";
        }
        throw new ArgumentException("Invalid class");

    }

    public string GetClassWeaponPrefab(UnitClass c) {
        if (c == UnitClass.WARRIOR) {
            return "Longsword";
        }

        if (c == UnitClass.CLERIC) {
            return "Broadsword";
        }

        if (c == UnitClass.MAGE) {
            return "Mage Staff";
        }

        if (c == UnitClass.SCOUT) {
            return "Rifle";
        }
        throw new ArgumentException("Invalid class");
    }

    public string GetClassArmorPrefab(UnitClass c) {
        if (c == UnitClass.WARRIOR) {
            return "Iron Armor";
        }

        if (c == UnitClass.CLERIC) {
            return "Leather Armor";
        }

        if (c == UnitClass.MAGE) {
            return "Mage Robes";
        }

        if (c == UnitClass.SCOUT) {
            return "Leather Armor";
        }
        throw new ArgumentException("Invalid class");
    }

}
