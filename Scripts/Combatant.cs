using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combatant : MonoBehaviour
{
    // GM
    public GameObject gameMaster;
    private Gamemaster gm;

    // NAME, HEALTH, FACTION, MOVES
    public string UnitName;
    private int CurrentHealth;
    public int MaxHealth;
    public Faction UnitFaction;
    public int MaxMoves = 6;
    private int remainingMoves;
    public int MaxAttacks = 1;
    private int remainingAttacks;
    
    // ARMOR
    public int Armor;
    public int Weave;
    
    // WEAPON
    public GameObject weapon;
    private GameObject weaponModel;
    public int AttackDamage;
    public int AttackRange;
    public DamageType AttackType;
    
    // CONSTS
    private float damageReductionMod;
    public GameObject selectionHighlightPrefab;
    private GameObject activeSelectionHighlight;

    // ANIM
    private Animator anim;
    private MovementAI movementScript;
    private bool attacking;
    private Combatant enemy;


    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        remainingAttacks = MaxAttacks;
        remainingMoves = MaxMoves;

        gm = gameMaster.GetComponent<Gamemaster>();
        gm.AddCombatant(this);
        damageReductionMod = gm.GetDamageReductionMod();
        
        anim = GetComponent<Animator>();
        SetWeapon(weapon);

        movementScript = GetComponent<MovementAI>();

        attacking = false;
        enemy = null;

    }

    // Update is called once per frame
    void Update()
    {
        if (attacking) {
            movementScript.turnTowardTarget(enemy.transform);
        }
    }

    IEnumerator AttackAnim() {
        attacking = true;
        yield return new WaitForSeconds(1);
        attacking = false;
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("Attacking", false);
    }

    public int GetCurrentHealth() {
        return this.CurrentHealth;
    }

    public bool Move(int x, int z) {
        // TODO
        // ASK GM IF THERE'S A UNIT THERE
        // IF NOT
        // SEE IF I HAVE THE MOVES TO GET THERE (HOW CALC???)
        // IF YES, DO THE MOVE

        return false;
    }

    public void Attack(Combatant opponent) {
        // do i have an attack?
        if (remainingAttacks > 0) {
            // am i in range?
            if (Vector3.Distance(transform.position, opponent.transform.position) <= (AttackRange + 0.55f)) {
                /// attack!
                remainingAttacks--;
                enemy = opponent;               
                StartCoroutine(AttackAnim());
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
                // TODO play death anim and then remove
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

    public void ResetActions() {
        remainingAttacks = MaxAttacks;
        remainingMoves = MaxMoves;
    }

    /*
    public void SetWeapon(GameObject obj) { // atm, melee only // TODO make sure this handles multiple weapon types
        string path = "root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/middle_01_r";
        Transform hand = transform.Find(path);
        if (hand == null) Debug.Log("didn't find it");
        weapon = Instantiate(obj);
        weapon.transform.position = hand.position;
        weapon.transform.Rotate(90, 0, 0);
        weapon.transform.Translate(0, 0, .05f);
        weapon.transform.SetParent(hand);
    }
    */

    public void SetWeapon(GameObject newWeapon) {
        SetWeapon(newWeapon.GetComponent<Weapon>());
    }

    public void SetWeapon(Weapon newWeapon) {
        string path = "root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/middle_01_r";
        Transform hand = transform.Find(path);
        if (hand == null) Debug.Log("Tried to equip weapon, couldn't find hand!");

        Destroy(weaponModel);
        AttackDamage = newWeapon.Damage;
        AttackRange = newWeapon.Range;
        AttackType = newWeapon.DamageType;
        weaponModel = Instantiate(newWeapon.model);

        weaponModel.transform.position = hand.position;
        weaponModel.transform.Rotate(newWeapon.rotation);
        weaponModel.transform.Translate(newWeapon.offset);
        weaponModel.transform.SetParent(hand);

    }

    
}
