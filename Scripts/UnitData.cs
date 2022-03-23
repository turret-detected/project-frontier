using System.Collections;
using System.Collections.Generic;
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
        ArmorPrefabName = null;
        WeaponPrefabName = combatant.WeaponName;
        UnitPrefabName = combatant.PrefabName;
        Position = combatant.transform.position;
        Rotation = combatant.transform.rotation;
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
        UnitPrefabName = null;
        Position = new Vector3();
        Rotation = new Quaternion();

        /// GENERATE STATS HERE
    }
}
