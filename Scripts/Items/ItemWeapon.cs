using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : MonoBehaviour, IItem
{
    public string PrefabName;
    public int Damage;
    public int Range;
    public DamageType DamageType;
    public string ItemDesc;
    public Vector3 offset;
    public Vector3 rotation;
    public string parent_bone;
    

    public string GetParentBone() {
        return parent_bone;
    }

    public Vector3 GetOffset() {
        return offset;
    }

    public Vector3 GetRotation() {
        return rotation;
    }
    
    public string GetItemPrefabString() {
        return PrefabName;
    }

    public string GetItemName() {
        return PrefabName;
    }

    public string GetItemDesc() {
        return ItemDesc;
    }

    public void OnEquip(Combatant c) {
        c.AttackDamage = Damage;
        c.AttackRange = Range;
        c.AttackType = DamageType;
    }

    public void OnUnequip(Combatant c) {
        c.AttackDamage = 1;
        c.AttackRange = 1;
        c.AttackType = DamageType.PHYSICAL;
    }

}
