using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    bool isValidTarget(Combatant caster, Combatant target);
    void performAbility(Combatant caster, Combatant target);
    void IncrementCooldown();
    int GetRemainingCooldown();
}
