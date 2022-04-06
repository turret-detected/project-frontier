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
    public bool InCover = false;
    
    // ARMOR
    public string ArmorName;
    private GameObject ArmorItem;
    private GameObject ArmorModel;
    public int Armor;
    public int Weave;
    
    // WEAPON
    public string WeaponName;
    private GameObject WeaponItem;
    private GameObject WeaponModel;
    public int AttackDamage;
    public int AttackRange;
    public DamageType AttackType;
    
    // CONSTS
    private float damageReductionMod;
    private GameObject selectionHighlightPrefab;
    private GameObject activeSelectionHighlight;
    private int ignoreActionLayer = 1 << 20;

    // ANIM
    private Animator anim;
    private MovementAI movementScript;
    private bool attacking;
    private Combatant enemy;


    // LOAD AND SAVE
    public void SetUnitData(UnitData u) {
        UnitClass = u.UnitClass;
        UnitName = u.UnitName;
        MaxHealth = u.MaxHealth;
        CurrentHealth = u.CurrentHealth;
        UnitFaction = u.UnitFaction;
        MaxMoves = u.MaxMoves;
        RemainingMoves = u.RemainingMoves;
        MaxAttacks = u.MaxAttacks;
        RemainingAttacks = u.RemainingAttacks;
        Armor = u.Armor;
        Weave = u.Weave;

        // TODO proper setup of weapons
        //c.ArmorPrefabName = null;
        //c.WeaponPrefabName = u.WeaponName;
        //c.UnitPrefabName = u.PrefabName;
    }

    public UnitData CreateDataClass() {
        return new UnitData(this);
    }


    // Start is called before the first frame update
    public void Start()
    {
        CurrentHealth = MaxHealth;
        RemainingAttacks = MaxAttacks;
        RemainingMoves = MaxMoves;

        selectionHighlightPrefab = Resources.Load<GameObject>("Unit Selected Outline");

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
                opponent.OnAttacked(this, IsTargetInCover(opponent.transform.position));
            } else {
                Debug.Log("Out of range!");
            }
        } else {
            Debug.Log("Already attacked!");
        }
    }

    public void OnAttacked(Combatant opponent, bool inCover) {
        int roll = Random.Range(1, 19); // 1 to 20
        if (inCover) roll = roll - 5; //-5 penalty for target being in cover
        
        if (roll > 1) {
            // hit
            int dmg = opponent.AttackDamage;
            // dmg reduction (TODO allow for other damage types)
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
                // Death todo
                // If player unit, log death with save somehow
                // All units:
                // Ragdoll, drop weapon
                // timed remove corpse
                gm.RemoveCombatant(this);
                Destroy(gameObject);
            }
        } else {
            Debug.Log("Missed!");
        }
    }

    // TODO move this somewhere else
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
        //string path = "Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R";
        Transform bone = transform.Find(newWeapon.GetParentBone());
        if (bone == null) Debug.Log("Tried to equip weapon, couldn't find bone!");

        Destroy(WeaponModel);
        AttackDamage = newWeapon.Damage;
        AttackRange = newWeapon.Range;
        AttackType = newWeapon.DamageType;
        WeaponModel = Instantiate(newWeapon.model);

        WeaponModel.transform.position = bone.position;
        WeaponModel.transform.SetParent(bone);
        WeaponModel.transform.Translate(newWeapon.offset); // TODO make this relative and not absolute
        WeaponModel.transform.Rotate(newWeapon.rotation); // TODO make this relative and not absolute
        
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


    public bool IsTargetInCover(Vector3 pos) {
        Vector3 adj = new Vector3(0, 1, 0); // since the linecast is at feet? ensure it'll hit higher hitbox for cover
        RaycastHit hit;
        bool temp = Physics.Linecast(transform.position+adj, pos+adj, out hit, ignoreActionLayer);
        Debug.Log("Cover hit: " + temp + " @ " + hit.point.x + " " + hit.point.y + " " + hit.point.z);
        return temp;
    }

    public void SetSelfInCover(bool b) {
        InCover = b;
    }

    
}
