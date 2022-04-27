using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityWarriorSurge : MonoBehaviour, IAbility
{

    public string Name;
    public int AbilityCooldown;
    private int RemainingCooldown;

    public bool isValidTarget(Combatant caster, Combatant target) {
        return (caster == target && RemainingCooldown < 1); // SELF ONLY
    }

    public void performAbility(Combatant caster, Combatant target) {
        target.GetAnimator().SetTrigger("Power 1");
        target.RemainingAttacks += 1;
        RemainingCooldown = AbilityCooldown;
    }

    public void IncrementCooldown() {
        RemainingCooldown--;
    }

    public int GetRemainingCooldown() {
        return RemainingCooldown;
    }

        public string GetName() {
        return Name;
    }
}
