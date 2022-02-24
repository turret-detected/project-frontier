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

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<MovementAI>();
        gm = gameMaster.GetComponent<Gamemaster>();
        canMove = false;
        canMove = false;
    }

    public void gmMove() {
        //this tells the AI it can move!
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        // IS IT MY TURN?
        if (canMove || moving) {
            canMove = false;
            moving = true;
            gm.getNearestEnemy(gameObject);

            // WHERE IS THE NEAREST ENEMY?
            // MOVE TOWARDS IT
            // STOP, TEST RANGE
            // MOVE UNTIL IN RANGE (ENSURE SNAP TO TILE)
            // ATTACK IT
            // TELL GM I AM DONE
        }
    }
}
