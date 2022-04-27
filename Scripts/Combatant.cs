using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class Combatant : MonoBehaviour
{
    // GM
    private Gamemaster gm;

    // FX
    [Header("FX")]
    public GameObject HealFX;

    // NAME, HEALTH, FACTION, MOVES
    [Header("Name & Faction")]
    public string UnitName;
    public UnitClass UnitClass;
    public string PrefabName;
    
    [Header("Health & Moves")]
    private int LastHealth;
    public int CurrentHealth;
    public int MaxHealth;
    public Faction UnitFaction;
    public int MaxMoves = 6;
    public int RemainingMoves;
    private Vector3 lastPos;
    public int MaxAttacks = 1;
    public int RemainingAttacks;
    public bool InCover = false;
    
    [Header("Equipment")]
    // ARMOR
    public string ArmorName;
    private GameObject ArmorItem;
    public int Armor;
    public int Weave;
    
    // WEAPON
    public string WeaponName;
    private GameObject WeaponItem;
    private GameObject WeaponModel;
    public int AttackDamage;
    public int AttackRange;
    public DamageType AttackType;

    // BAUBLE
    public string BaubleName;
    private GameObject BaubleItem;
    
    // CONSTS
    private float damageReductionMod;
    private GameObject selectionHighlightPrefab;
    private GameObject activeSelectionHighlight;
    private int ignoreActionLayer = 1 << 20;

    // ANIM & SOUND
    private Animator anim;
    private MovementAI movementScript;
    private bool attacking;
    private Combatant enemy;
    private AudioSource sound;
    private List<Transform> boneChildren = new List<Transform>();


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
        ArmorName = u.ArmorPrefabName;
        WeaponName = u.WeaponPrefabName;
        BaubleName = u.BaublePrefabName;
        PrefabName = u.UnitPrefabName;
    }

    public UnitData CreateDataClass() {
        return new UnitData(this);
    }


    // Start is called before the first frame update
    public void Start()
    {
        lastPos = transform.position;
        LastHealth = MaxHealth;
        CurrentHealth = MaxHealth;
        RemainingAttacks = MaxAttacks;
        RemainingMoves = MaxMoves;

        selectionHighlightPrefab = Resources.Load<GameObject>("Unit Selected Outline");

        gm = GameObject.Find("GameMaster").GetComponent<Gamemaster>();
        gm.AddCombatant(this);
        damageReductionMod = gm.GetDamageReductionMod();
             
        anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();

        try {
            EquipItem(Resources.Load<GameObject>("Weapons/"+WeaponName));
        } catch (NullReferenceException) {
            Debug.Log("Couldn't equip item named " + WeaponName);
        }
        try {
            EquipItem(Resources.Load<GameObject>("Armor/"+ArmorName));
        } catch (NullReferenceException) {
            Debug.Log("Couldn't equip item named " + ArmorName);
        }
        try {
            EquipItem(Resources.Load<GameObject>("Baubles/"+BaubleName));
        } catch (NullReferenceException) {
            Debug.Log("Couldn't equip item named " + BaubleName);
        }

        movementScript = GetComponent<MovementAI>();

        attacking = false;
        enemy = null;

        // Disable bone colliders
        GetAllChildren(transform.Find("Root").transform, ref boneChildren);
        foreach (Transform t in boneChildren) {
            t.GetComponent<Collider>().enabled = false;
            t.GetComponent<Rigidbody>().isKinematic = true;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
        if (CurrentHealth > LastHealth) {
            if (LastHealth == 0) {
                anim.SetTrigger("Get Up");
                gm.AddCombatant(this);
            }
            OnHeal();
        }
        LastHealth = CurrentHealth;


        if (attacking) {
            movementScript.turnTowardTarget(enemy.transform);
        }

        if (Vector3.Distance(lastPos, transform.position) > 0.96f) {
            lastPos = transform.position;
            RemainingMoves--;
        }
    }

    // code adapted from: sergiobd @ https://forum.unity.com/threads/loop-through-all-children.53473/
    public void GetAllChildren(Transform parent, ref List <Transform> transforms)
    {
        foreach (Transform t in parent) {
            transforms.Add(t);
            GetAllChildren(t, ref transforms);
        }
    }



    IEnumerator AttackAnim(Combatant me, Combatant enemy) {
        attacking = true;
        yield return new WaitForSeconds(1);
        attacking = false;
        WeaponModel.GetComponent<ItemWeapon>().WeaponSwing(this, enemy);
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
    }

    public bool Move(int x, int z) {
        // TODO
        // SEE IF I HAVE THE MOVES TO GET THERE (HOW CALC???)
        // IF YES, DO THE MOVE
        if (Vector3.Distance(this.transform.position, new Vector3(x, 0, z)) < RemainingMoves + 0.5f) {
            lastPos = transform.position;
            movementScript.MoveToSpace(x, z);
            return true;
        } else {
            return false;
        }
    }

    public void Attack(Combatant opponent) {
        // do i have an attack?
        if (RemainingAttacks > 0) {
            // am i in range?
            if (Vector3.Distance(transform.position, opponent.transform.position) <= (AttackRange + 0.55f)) {
                /// attack!
                RemainingAttacks--;
                if (RemainingAttacks == 0) { // end turn if last attack
                    RemainingMoves = 0;
                }
                enemy = opponent;
                StartCoroutine(AttackAnim(this, opponent));
                
                opponent.OnAttacked(this, IsTargetInCover(opponent.transform.position));
            } else {
                Debug.Log("Out of range!");
            }
        } else {
            Debug.Log("Already attacked!");
        }
    }

    public void OnAttacked(Combatant opponent, bool inCover) {
        int roll = UnityEngine.Random.Range(1, 19); // 1 to 20
        if (inCover) roll = roll - 5; //-5 penalty for target being in cover
        
        if (roll > 1) {
            // hit
            int dmg = opponent.AttackDamage;
            // dmg reduction (TODO allow for other damage types)
            if (opponent.AttackType == DamageType.PHYSICAL) {
                dmg = (int) (dmg * Mathf.Pow(damageReductionMod, Armor));
            } else if (opponent.AttackType == DamageType.MAGICAL) {
                dmg = (int) (dmg * Mathf.Pow(damageReductionMod, Weave));
            } else {
                dmg = (int) dmg * 1; // True damage
            }
            // do damage
            // TODO hit anim
            CurrentHealth = CurrentHealth - dmg;
            Debug.Log("Hit!");
            Debug.Log("I have " + CurrentHealth.ToString());
            // H > 0 test
            if (CurrentHealth <= 0) {
                CurrentHealth = 0;
                Debug.Log(name + " has been reduced to 0 health!");
                OnDowned();
            }
            gm.logDamage(opponent.UnitName + " dealt " + dmg + " damage to " + UnitName + "!\n");


        } else {
            Debug.Log("Missed!");
            gm.logDamage(opponent.UnitName + " missed!\n");

        }
    }

    public void TakeDamage(int damage, DamageType damageType) {
        if (damageType == DamageType.PHYSICAL) {
            damage = (int) (damage * Mathf.Pow(damageReductionMod, Armor));
        } else if (damageType == DamageType.MAGICAL) {
            damage = (int) (damage * Mathf.Pow(damageReductionMod, Weave));
        } else {
            damage = (int) damage * 1; // True damage
        }

        // H > 0 test
        if (CurrentHealth <= 0) {
            CurrentHealth = 0;
            Debug.Log(name + " has been reduced to 0 health!");
            OnDowned();
        }
        gm.logDamage(UnitName + " took " + damage + " damage!\n");
    }

    public void OnDowned() {
        gm.RemoveCombatant(this);
        if (UnitFaction == Faction.COMPUTER) {
            WeaponModel.transform.parent = null;
            GetComponent<Animator>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            foreach (Transform t in boneChildren) {
                t.GetComponent<Collider>().enabled = true;
                t.GetComponent<Rigidbody>().isKinematic = false;
            }


            // RAGDOLL & drop weapon
            // delete after timer
            // Destroy(gameObject);
        } else {
            // TRIGGER DOWNED POSE
            anim.SetTrigger("Downed");
            ResetActions();
        }
    }

    // TODO move this somewhere else
    public void SetSelected(bool b) { 
        if (b) {
            activeSelectionHighlight = Instantiate(selectionHighlightPrefab, transform.position, Quaternion.identity);
            activeSelectionHighlight.transform.SetParent(transform);
            var particle_shape = activeSelectionHighlight.GetComponentInChildren<ParticleSystem>().shape;
            particle_shape.radius = AttackRange;
        } else {
            Destroy(activeSelectionHighlight);
        }
    }

    public void ToggleRangeHighlight(bool b) {
        var particleEmission = activeSelectionHighlight.GetComponentInChildren<ParticleSystem>().emission;
        particleEmission.enabled = b; 
    }

    public void ResetActions() {
        if (CurrentHealth != 0) {
            RemainingAttacks = MaxAttacks;
            RemainingMoves = MaxMoves;
        } else {
            RemainingAttacks = 0;
            RemainingMoves = 0;
        }
        
    }

    public void EquipItem(GameObject itemObj) {
        IItem item = itemObj.GetComponent<IItem>();
        
        // Unequip existing item them change assignments
        if (item is ItemWeapon) {
            if (WeaponItem != null) {
                UnequipItem(WeaponItem.GetComponent<IItem>());
            }
            WeaponName = item.GetItemPrefabString();
            WeaponItem = itemObj;
            SetWeapon((ItemWeapon) item); // setup weapon model
        }
    
        if (item is ItemBauble) {
            if (BaubleItem != null) {
                UnequipItem(BaubleItem.GetComponent<IItem>());
            }

            BaubleName = item.GetItemPrefabString();
            BaubleItem = itemObj;       
        }

        if (item is ItemArmor) {
            if (ArmorItem != null) {
                UnequipItem(ArmorItem.GetComponent<IItem>());
            }
            ArmorName = item.GetItemPrefabString();
            ArmorItem = itemObj;
        }

        //stat adj
        item.OnEquip(this);

    }

    public void UnequipItem(IItem item) {
        item.OnUnequip(this);
        if (item is ItemWeapon) {
            WeaponItem = null;
            WeaponName = null;
            Destroy(WeaponModel);
        }
        if (item is ItemBauble) {
            BaubleItem = null;
            BaubleName = null;
        }
        if (item is ItemArmor) {
            ArmorItem = null;
            ArmorName = null;
        }
    }

    public void SetWeapon(ItemWeapon newWeapon) {
        ///"Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R"
        Transform bone = transform.Find(newWeapon.GetParentBone());
        if (bone == null) Debug.Log(newWeapon.GetItemName()+" couldn't find bone to attach weapon to. THIS IS A BUG.");
        Destroy(WeaponModel);
        
        //AttackDamage = newWeapon.Damage;
        //AttackRange = newWeapon.Range;
        //AttackType = newWeapon.DamageType;
       
        WeaponModel = Instantiate(newWeapon.gameObject);
        WeaponModel.transform.position = bone.position;
        WeaponModel.transform.SetParent(bone);
        WeaponModel.transform.Translate(newWeapon.offset); // TODO make this relative and not absolute
        WeaponModel.transform.Rotate(newWeapon.rotation); // TODO make this relative and not absolute
        
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

    public Animator GetAnimator() {
        return anim;
    }

    public void OnHeal() {
        // Play FX
        Instantiate(HealFX, transform.position, new Quaternion());
    }

    public void PlaySound(AudioClip clip) {
        sound.PlayOneShot(clip);
    } 

    
}
