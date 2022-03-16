using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

[DataContract]
public class UnitData
{
    [DataMember]
    public string Name;
    
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
        Name = combatant.name;
        MaxHealth = combatant.MaxHealth;
        CurrentHealth = combatant.GetCurrentHealth();
        UnitFaction = combatant.UnitFaction;
        MaxMoves = combatant.MaxMoves;
        RemainingMoves = combatant.remainingMoves;
        MaxAttacks = combatant.MaxAttacks;
        RemainingAttacks = combatant.remainingAttacks;
        Armor = combatant.Armor;
        Weave = combatant.Weave;
        ArmorPrefabName = null;
        WeaponPrefabName = combatant.WeaponName;
        UnitPrefabName = combatant.prefabName;
        Position = combatant.transform.position;
        Rotation = combatant.transform.rotation;
    }
}
