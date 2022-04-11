using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    bool canMove;
    bool moving;
    public int weight_DISTANCE_TO_ENEMY = 100;
    public int weight_ENEMY_DEFENSE = 0;
    public int weight_ENEMY_HEALTH = 0;
    private Gamemaster gm;
    private MovementAI mover;
    private Combatant combatant;
    private float fullDistanceTraveled = 0;
    //private float tempDistanceTraveled = 0;
    private Vector3 lastPos;
    private GameObject target;
    private Vector3 targetPos;

    // Start is called before the first frame update
    public void Start()
    {
        mover = GetComponent<MovementAI>();
        combatant = GetComponent<Combatant>();
        gm = GameObject.Find("GameMaster").GetComponent<Gamemaster>();
        canMove = false;
        moving = false;
    }

    public void gmMove() {
        //this tells the AI it can move!
        canMove = true;
        lastPos = transform.position;
    }

    public GameObject calculateTarget() {

        // GET A LIST OF ALL ENEMIES THIS AI KNOWS ABOUT
        // FOR EACH ENEMY ASSIGN A VALUE FOR:
        // DISTANCE
        // HEALTH
        // DEFENSES (ARMOR & WEAVE)
        // RETURN THIS ENEMY



        return null;

    }

    // Update is called once per frame
    void Update()
    {
        // IS IT MY TURN?
        if (canMove) {
            canMove = false;
            moving = true;
            // WHERE IS THE ~~NEAREST ENEMY?~~ BEST TARGET?
            target = gm.getNearestEnemy(gameObject); // TODO REPLACE WITH calculateTarget()
            targetPos = target.transform.position;
            fullDistanceTraveled = 0;
            //tempDistanceTraveled = 0; 
            mover.MoveToSpace((int) Mathf.Round(targetPos.x), (int) Mathf.Round(targetPos.z));        
        }


        if (moving) {

            fullDistanceTraveled += Vector3.Distance(lastPos, transform.position);
            lastPos = transform.position;
            //Debug.Log(tempDistanceTraveled + " | " + fullDistanceTraveled);

            //While I have moves
            if (fullDistanceTraveled < combatant.MaxMoves) {
                // test range
                if (Vector3.Distance(transform.position, targetPos) <= combatant.AttackRange + 0.55f) {
                    // In range, attack!
                    mover.AlterPath((int) Mathf.Round(transform.position.x), (int) Mathf.Round(transform.position.z));
                    moving = false;
                    Debug.Log("AI Attack!");
                    StartCoroutine(doAttack());
                    
                }
            } else {
                Debug.Log("Couldn't get close enough to attack target");
                mover.AlterPath((int) Mathf.Round(transform.position.x), (int) Mathf.Round(transform.position.z));
                moving = false;
                gm.AITurnComplete(this);
                // End turn
            }
        }
    }

    IEnumerator doAttack() {
        yield return new WaitForSeconds(1); //Wait until movement stops hopefully
        combatant.Attack(target.GetComponent<Combatant>());
        gm.AITurnComplete(this);
        // End turn
    }
}
