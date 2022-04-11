using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MovementAI : MonoBehaviour
{
    private Seeker seeker;
    private CharacterController controller;
    private Animator anim;
    public Path path;
    public float speed = 2;
    private float nextWaypointDistance = 1; // how many points ahead it looks (why would this ever not be 1 on a grid?)
    private int currentWaypoint = 0;
    public bool reachedEndOfPath;
    private Vector3 destination;
    /*
    private bool isMoving = false;
    private bool wasMoving = false;
    private bool needToUpdate = false;
    
    private bool stopMoving = false;
    */


    // Pre move turn
    private bool turning = false;
    private Vector3 turnTarget = new Vector3(0, 0, 0);

    // Moving animation condition
    private bool moving;

    // Update graph when stopping movement
    private bool wasMoving;


    // Start is called before the first frame update
    void Start()
    {
        reachedEndOfPath = true;
        anim = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
    }

    public void MoveToSpace(int x, int z) {
        gameObject.layer = 0; // so it doesn't freak out about being an obstacle itself
        StartCoroutine(updateGraphLocalThenMove(x, z));
    }

    public void AlterPath(int x, int z) { // ONLY USE TO CHANGE THE PATH OF AN ALREADY MOVING CHARACTER
        destination = new Vector3(x, 0, z);
        seeker.StartPath(transform.position, destination, OnPathComplete);
    }

    public void OnPathComplete(Path p) {
        //Debug.Log("Path calculated: "+!p.error);
        if(!p.error) { //path works, set it
            path = p;
            currentWaypoint = 0;
        }
    }

    IEnumerator updateGraph() {
        yield return new WaitForSeconds(1);
        //AstarPath.active.Scan(); 
        // since we now remove our presence before moving, just update where we moved to
        AstarPath.active.UpdateGraphs(GetComponentInParent<CharacterController>().bounds);
    }

    IEnumerator updateGraphLocalThenMove(int x, int z) { 
        turnTarget = new Vector3(x, 1, z); 
        turning = true;
        AstarPath.active.UpdateGraphs(GetComponentInParent<CharacterController>().bounds);

        yield return new WaitForSeconds(1);
        turning = false;
        destination = new Vector3(x, 0, z);
        seeker.StartPath(transform.position, destination, OnPathComplete);
    }

    // Update is called once per frame
    void Update()
    {
        // AI pathing
        if (path != null) { // Move when given a path

            reachedEndOfPath = false;
            float distanceToWaypoint;

            while (true) {
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance) {
                    if (currentWaypoint + 1 < path.vectorPath.Count) {
                        currentWaypoint++;
                    } else {
                        reachedEndOfPath = true;
                        break;
                    }
                } else {
                    break;
                }
            }

            // Slow down at the end of path
            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;
            // TODO this speed factor might need to be applied to movement animation as well

            // Get direction
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            
            // DEBUG
            Debug.DrawLine(transform.position, path.vectorPath[currentWaypoint], Color.white, 5);

            // Face direction
            if (moving && Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) > 0.7) {
                turnTowardTarget(path.vectorPath[currentWaypoint]);
            }

            // Give it speed
            Vector3 velocity = dir * speed * speedFactor;

            // Move using char controller
            controller.Move(velocity * Time.deltaTime);

        }


        
        // If not attacking, set anim to move while moving
        if (!anim.GetBool("Attacking")) {
            moving = controller.velocity.magnitude > 0.2;
            //Debug.Log(controller.velocity);
            anim.SetBool("IsMoving", moving);
            //if (isMoving) Debug.Log(isMoving);
        }
        


        if(turning) {
            turnTowardTarget(turnTarget);
        }

        
        if (moving) {
            wasMoving = true;
        }

        if (wasMoving && !moving) {
            wasMoving = false;
            gameObject.layer = 8; // back to obstacle
            Debug.Log("Requested graph update!");
            StartCoroutine(updateGraph());
        }
    }

    public void turnTowardTarget(Vector3 pos) {
        pos = new Vector3(pos.x, 0, pos.z); //ignore y
        //Debug.Log("Turning!");

        Vector3 direction;
        Quaternion rotGoal;

        // Code from
        // https://www.youtube.com/watch?v=h0P2KP8Fyoo (Royal Skies)

        direction = (pos - transform.position).normalized; // gets the direction of the target relative to current pos.
        rotGoal = Quaternion.LookRotation(direction); // gets the angle
        transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, 0.1f); // interpolates and rotates
    }

    public void turnTowardTarget(Transform target) {
        turnTowardTarget(target.position);
    }
}
