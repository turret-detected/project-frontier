using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityClericHeal : MonoBehaviour, IAbility
{
    public AudioClip healSound;
    private AudioSource charAudio;
    public int AbilityCooldown;
    private int RemainingCooldown;

    void Start() {
        RemainingCooldown = 0;
        charAudio = GetComponent<AudioSource>();
    }

    public bool isValidTarget(Combatant caster, Combatant target) {
        return (target.UnitFaction == Faction.PLAYER && RemainingCooldown < 1); // ANY ALLY
    }

    public void performAbility(Combatant caster, Combatant target) {
        // animation
        // turn toward?
        caster.GetAnimator().SetTrigger("Power 2");
        
        // audio
        charAudio.clip = healSound;
        charAudio.PlayDelayed(1);

        // heal
        target.CurrentHealth += target.MaxHealth/2;
        target.CurrentHealth = Mathf.Min(target.CurrentHealth, target.MaxHealth); // can't overheal
        
        // cd
        RemainingCooldown = AbilityCooldown;
    }

    public void IncrementCooldown() {
        RemainingCooldown--;
    }

    public int GetRemainingCooldown() {
        return RemainingCooldown;
    }
}
