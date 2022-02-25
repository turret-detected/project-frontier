using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    bool canMove;
    bool moving;
    public GameObject gameMaster;
    private Gamemaster gm;
    private MovementAI mover;
    private Combatant combatant;
    private float fullDistanceTraveled = 0;
    private float tempDistanceTraveled = 0;
    private Vector3 lastPos;
    private GameObject target;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MovementAI>();
        combatant = GetComponent<Combatant>();
        gm = gameMaster.GetComponent<Gamemaster>();
        canMove = false;
        moving = false;
    }

    public void gmMove() {
        //this tells the AI it can move!
        canMove = true;
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // IS IT MY TURN?
        if (canMove) {
            canMove = false;
            moving = true;
            // WHERE IS THE NEAREST ENEMY?
            target = gm.getNearestEnemy(gameObject);
            targetPos = target.transform.position;
            fullDistanceTraveled = 0;
            tempDistanceTraveled = 0; 
            mover.MoveToSpace((int) targetPos.x, (int) targetPos.z);        
        }


        if (moving) {

            fullDistanceTraveled += Vector3.Distance(lastPos, transform.position);
            tempDistanceTraveled += Vector3.Distance(lastPos, transform.position);
            lastPos = transform.position;


            // MOVE TOWARDS IT
            // STOP, TEST RANGE
            // MOVE UNTIL IN RANGE (ENSURE SNAP TO TILE)
            // ATTACK IT
            // TELL GM I AM DONE
            if (tempDistanceTraveled > 1) {
                // Do i have moves left?
                if (fullDistanceTraveled >= combatant.MaxMoves) {
                    Debug.Log("Couldn't get close enough to attack target");
                    mover.MoveToSpace((int) transform.position.x, (int) transform.position.z);
                    moving = false;
                    gm.AIFinishedAction();
                    // End turn
                }               
            } else {
                // test range
                if (Vector3.Distance(transform.position, targetPos) <= combatant.AttackRange) {
                    // In range, attack!
                    mover.MoveToSpace((int) transform.position.x, (int) transform.position.z);
                    moving = false;
                    Debug.Log("AI Attack!");
                    combatant.Attack(target.GetComponent<Combatant>());
                    gm.AIFinishedAction();
                    // END MY TURN
                } else {
                    // Not there yet, keep going!
                    tempDistanceTraveled = 0;
                }
            }
        }
    }
}
