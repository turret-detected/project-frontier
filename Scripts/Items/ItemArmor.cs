using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemArmor : MonoBehaviour, IItem
{
    public string PrefabName;
    public int Armor;
    public int Weave;
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
        c.Armor += Armor;
        c.Weave += Weave;
    }

    public void OnUnequip(Combatant c) {
        c.Armor -= Armor;
        c.Weave -= Weave;
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
