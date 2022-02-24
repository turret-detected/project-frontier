using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combatant : MonoBehaviour
{
    public GameObject gameMaster;
    public string UnitName;
    private int CurrentHealth;
    public int MaxHealth;
    public int Armor;
    public int Weave;
    public int AttackDamage;
    public int AttackRange;
    public DamageType AttackType;
    public Faction UnitFaction;
    private float damageReductionMod;
    public int MaxMoves = 6;
    private int remainingMoves;
    public int MaxAttacks = 1;
    private int remainingAttacks;
    public GameObject selectionHighlightPrefab;
    private GameObject activeSelectionHighlight;




    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        remainingAttacks = MaxAttacks;
        remainingMoves = MaxMoves;
        gameMaster.GetComponent<Gamemaster>().AddCombatant(this);
        damageReductionMod = gameMaster.GetComponent<Gamemaster>().GetDamageReductionMod();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentHealth() {
        return this.CurrentHealth;
    }

    public void Attack(Combatant opponent) {
        // do i have an attack?
        if (remainingAttacks > 0) {
            // am i in range?
            if (Vector3.Distance(transform.position, opponent.transform.position) < AttackRange) {
                /// attack!
                remainingAttacks--;
                opponent.OnAttacked(this);
            } else {
                Debug.Log("Out of range!");
            }
        } else {
            Debug.Log("Already attacked!");
        }
    }

    public void OnAttacked(Combatant opponent) {
        int roll = Random.Range(1, 19); // 1 to 20
        if (roll != 1) {
            // hit
            int dmg = opponent.AttackDamage;
            // dmg reduction
            if (opponent.AttackType == DamageType.PHYSICAL) {
                dmg = (int) (dmg * Mathf.Pow(damageReductionMod, Armor));
            } else {
                dmg = (int) (dmg * Mathf.Pow(damageReductionMod, Weave));
            }
            // do damage
            CurrentHealth = CurrentHealth - dmg;
            Debug.Log("Hit!");
            Debug.Log("I have " + CurrentHealth.ToString());
            // H > 0 test
            if (CurrentHealth < 0) {
                CurrentHealth = 0;
                Debug.Log(name + " has died!");
                // I AM DEAD, DO SOMETHING
                gameMaster.GetComponent<Gamemaster>().RemoveCombatant(this);
                Destroy(gameObject);
            }
        } else {
            Debug.Log("Missed!");
        }
    }

    public void SetSelected(bool b) { 
        if (b) {
            activeSelectionHighlight = Instantiate(selectionHighlightPrefab, transform.position, Quaternion.identity);
            activeSelectionHighlight.transform.SetParent(transform);
        } else {
            Destroy(activeSelectionHighlight);
        }
        
    }
}
