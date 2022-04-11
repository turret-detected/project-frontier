using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaubleAmuletNyx : ItemBauble
{
    public int WeaveBonus = 10;

    public override void OnEquip(Combatant c) {
        c.Weave += WeaveBonus;
    }

    public override void OnUnequip(Combatant c) {
        c.Weave -= WeaveBonus;
    }
}
