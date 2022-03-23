using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    string GetParentBone();
    Vector3 GetOffset();
    Vector3 GetRotation();
    GameObject GetModel();
    string GetItemPrefabString();
}
