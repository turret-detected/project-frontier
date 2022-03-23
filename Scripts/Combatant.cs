using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Combatant : MonoBehaviour
{
    // GM
    private Gamemaster gm;

    // NAME, HEALTH, FACTION, MOVES
    public string UnitName;
    public UnitClass UnitClass;
    public string PrefabName;
    public int CurrentHealth;
    public int MaxHealth;
    public Faction UnitFaction;
    public int MaxMoves = 6;
    public int RemainingMoves;
    public int MaxAttacks = 1;
    public int RemainingAttacks;
    
    // ARMOR
    public string ArmorName;
    public GameObject ArmorItem;
    public GameObject ArmorModel;
    public int Armor;
    public int Weave;
    
    // WEAPON
    public string WeaponName;
    public GameObject WeaponItem;
    private GameObject WeaponModel;
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
    public void Start()
    {
        CurrentHealth = MaxHealth;
        RemainingAttacks = MaxAttacks;
        RemainingMoves = MaxMoves;

        gm = GameObject.Find("GameMaster").GetComponent<Gamemaster>();
        gm.AddCombatant(this);
        damageReductionMod = gm.GetDamageReductionMod();
             
        anim = GetComponent<Animator>();
        WeaponItem = Resources.Load<GameObject>("Weapons/"+WeaponName);
        SetWeapon(WeaponItem);

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

    public bool Move(int x, int z) {
        // TODO
        // ASK GM IF THERE'S A UNIT THERE
        // IF NOT
        // SEE IF I HAVE THE MOVES TO GET THERE (HOW CALC???)
        // IF YES, DO THE MOVE
        movementScript.MoveToSpace(x, z);

        return false;
    }

    public void Attack(Combatant opponent) {
        // do i have an attack?
        if (RemainingAttacks > 0) {
            // am i in range?
            if (Vector3.Distance(transform.position, opponent.transform.position) <= (AttackRange + 0.55f)) {
                /// attack!
                RemainingAttacks--;
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
            // TODO hit anim
            CurrentHealth = CurrentHealth - dmg;
            Debug.Log("Hit!");
            Debug.Log("I have " + CurrentHealth.ToString());
            // H > 0 test
            if (CurrentHealth < 0) {
                CurrentHealth = 0;
                Debug.Log(name + " has died!");
                // I AM DEAD, DO SOMETHING
                // TODO play death anim and then remove
                gm.RemoveCombatant(this);
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
        RemainingAttacks = MaxAttacks;
        RemainingMoves = MaxMoves;
    }

    public void SetWeapon(GameObject newWeapon) {
        SetWeapon(newWeapon.GetComponent<ItemWeapon>());
    }

    public void SetWeapon(ItemWeapon newWeapon) {
        //string path = "root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/middle_01_r";
        Transform bone = transform.Find(newWeapon.GetParentBone());
        if (bone == null) Debug.Log("Tried to equip weapon, couldn't find bone!");

        Destroy(WeaponModel);
        AttackDamage = newWeapon.Damage;
        AttackRange = newWeapon.Range;
        AttackType = newWeapon.DamageType;
        WeaponModel = Instantiate(newWeapon.model);

        WeaponModel.transform.position = bone.position;
        WeaponModel.transform.Rotate(newWeapon.rotation); // TODO make this relative and not absolute
        WeaponModel.transform.Translate(newWeapon.offset); // TODO make this relative and not absolute
        WeaponModel.transform.SetParent(bone);
    }

    public void SetArmor(GameObject newArmor) {
        SetArmor(newArmor.GetComponent<ItemArmor>());
    }

    public void SetArmor(ItemArmor newArmor) {
        Transform bone = transform.Find(newArmor.GetParentBone());
        if (bone == null) Debug.Log("Tried to equip armor, couldn't find bone!");

        Destroy(ArmorModel);
        Armor = newArmor.Armor;
        Weave = newArmor.Weave;
        ArmorModel = Instantiate(newArmor.model);

        // TODO FIX THIS DUPLICATE CODE
        ArmorModel.transform.position = bone.position;
        ArmorModel.transform.Rotate(newArmor.rotation); // TODO make this relative and not absolute
        ArmorModel.transform.Translate(newArmor.offset); // TODO make this relative and not absolute
        ArmorModel.transform.SetParent(bone);
    }



    public UnitData CreateDataClass() {
        return new UnitData(this);
    }

    
}
