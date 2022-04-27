using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMageMeteor : MonoBehaviour, IAbility
{

    public string Name;
    public AudioClip castSound;
    public GameObject fireballPrefab;
    private AudioSource charAudio;
    public int AbilityCooldown;
    private int RemainingCooldown;
    public int DamageValue;

    void Start() {
        RemainingCooldown = 0;
        charAudio = GetComponent<AudioSource>();
    }

    public bool isValidTarget(Combatant caster, Combatant target) {
        return (target.UnitFaction == Faction.COMPUTER && RemainingCooldown < 1); // ANY ENEMY
    }

    public void performAbility(Combatant caster, Combatant target) {
        // animation
        // turn toward?
        caster.GetAnimator().SetTrigger("Power 2");
        
        // audio
        charAudio.clip = castSound;
        charAudio.PlayDelayed(1);

        // create fireball
        Instantiate(fireballPrefab, target.transform.position, new Quaternion());
        target.TakeDamage(DamageValue, DamageType.MAGICAL);
        
        // cd
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
