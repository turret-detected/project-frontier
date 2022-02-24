using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MovementAI : MonoBehaviour
{
    private Seeker seeker;
    private CharacterController controller;
    public Path path;
    public float speed = 2;
    private float nextWaypointDistance = 1; // how many points ahead it looks
    private int currentWaypoint = 0;
    public bool reachedEndOfPath;

    // Start is called before the first frame update
    void Start()
    {
        reachedEndOfPath = true;
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        //seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
    }

    public void MoveToSpace(int x, int z) {
        seeker.StartPath(transform.position, new Vector3(x, 0, z), OnPathComplete);
    }

    public void OnPathComplete(Path p) {
        Debug.Log("Path calculated: "+!p.error);
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

            // Give it speed
            Vector3 velocity = dir * speed * speedFactor;

            // Move using char controller
            controller.SimpleMove(velocity);

        }
    }
}
