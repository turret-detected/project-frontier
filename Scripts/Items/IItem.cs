using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    string GetParentBone();
    Vector3 GetOffset();
    Vector3 GetRotation();
    string GetItemPrefabString();
    string GetItemName();
    string GetItemDesc();
    void OnEquip(Combatant c);
    void OnUnequip(Combatant c);
    GameObject GetGameObject();
}
