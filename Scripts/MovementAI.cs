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

    // Start is called before the first frame update
    void Start()
    {
        reachedEndOfPath = true;
        anim = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        //seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
    }

    public void MoveToSpace(int x, int z) {
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

    public bool IsMoving() {
        return !reachedEndOfPath;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("IsMoving", IsMoving());

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

            // Get direction
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            
            // DEBUG
            Debug.DrawLine(transform.position, path.vectorPath[currentWaypoint], Color.white, 5);

            // Face direction
            if (IsMoving()) {
                turnTowardTarget(path.vectorPath[currentWaypoint]);
            }

            // Give it speed
            Vector3 velocity = dir * speed * speedFactor;

            // Move using char controller
            controller.Move(velocity * Time.deltaTime);

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
