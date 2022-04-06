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
    public int Weave;

    [DataMember]
    public int Armor;
    
    [DataMember]
    public string WeaponPrefabName;
    
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
        Armor = combatant.Armor;
        Weave = combatant.Weave;
        ArmorPrefabName = combatant.ArmorName;
        WeaponPrefabName = combatant.WeaponName;
        UnitPrefabName = combatant.PrefabName;
        Position = combatant.transform.position;
        Rotation = combatant.transform.rotation;
        UnitDisplayName = UnitName + " (" + StringEnums.GetUnitClassString(UnitClass) + ")";
        Debug.Log(UnitDisplayName);
    }

    public UnitData(string Name, UnitClass c) {
        UnitName = Name;
        UnitClass = c;
        MaxHealth = 100;
        CurrentHealth = 100;
        UnitFaction = Faction.PLAYER;
        MaxMoves = 6;
        RemainingMoves = 6;
        MaxAttacks = 1;
        RemainingAttacks = 1;
        Armor = 0;
        Weave = 0;
        ArmorPrefabName = null;
        WeaponPrefabName = null;
        UnitPrefabName = GetClassPrefab(c);
        Position = new Vector3();
        Rotation = new Quaternion();
        UnitDisplayName = UnitName + " (" + StringEnums.GetUnitClassString(UnitClass) + ")";
        Debug.Log(UnitDisplayName);

        /// GENERATE STATS HERE
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

}
