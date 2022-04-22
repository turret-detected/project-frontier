using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBauble : MonoBehaviour, IItem
{
    public string PrefabName;
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

    public virtual void OnEquip(Combatant c) {
        Debug.Log("ITEM " + GetItemName() + " DOES NOT IMPLEMENT onEquip(). THIS IS A BUG!");
    }

    public virtual void OnUnequip(Combatant c) {
        Debug.Log("ITEM " + GetItemName() + " DOES NOT IMPLEMENT onUnequip(). THIS IS A BUG!");
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}

