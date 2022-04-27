using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaubleAmuletAthena : ItemBauble
{
    public int ExtraAttacks = 1;

    public override void OnEquip(Combatant c) {
        c.MaxAttacks += ExtraAttacks;
    }

    public override void OnUnequip(Combatant c) {
        c.MaxAttacks -= ExtraAttacks;
    }
}
