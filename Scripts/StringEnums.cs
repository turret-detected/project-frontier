using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringEnums
{
    public static string GetUnitClassString(UnitClass u) {
        if (u == UnitClass.WARRIOR) {
            return "Warrior";
        } else if (u == UnitClass.SCOUT) {
            return "Scout";
        } else if (u == UnitClass.MAGE) {
            return "Mage";
        } else if (u == UnitClass.CLERIC) {
            return "Cleric";
        } else {
            return "INVALID STRING REQUEST";
        }
    }
}
