using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NodeRemoveConnection : MonoBehaviour
{
    public GameObject connectedNode;

    int getGridConnValueFromVector3(Vector3 vector) {
        // USAGE:
        // Vector3 vector = NODE1.transform.position-NODE2.transform.position;
        // int value = getGridConnValueFromVector3(vector);

		//         Z
		//         |
		//         |
		//
		//      6  2  5
		//       \ | /
		// --  3 - X - 1  ----- X
		//       / | \
		//      7  0  4
		//
		//         |
		//         |

        
        float x_diff = vector.x;
        float z_diff = vector.z;

        // Squared
        if(z_diff > 0 && x_diff == 0) {
            return 0;
        }

        if(z_diff < 0 && x_diff == 0) {
            return 2;
        }

        if(z_diff == 0 && x_diff > 0) {
            return 3;
        }

        if(z_diff == 0 && x_diff < 0) {
            return 1;
        }

        //Angled // Untested (does the use case even exist?)
        if (x_diff > 0 && z_diff > 0) {
            return 7;
        }

        if (x_diff < 0 && z_diff > 0) {
            return 4;
        }

        if (x_diff > 0 && z_diff < 0) {
            return 6;
        }

        if (x_diff < 0 && z_diff < 0) {
            return 5;
        }
        return -1;
    }

    int invertGridConnValue(int value) {
        if (value == 0 || value == 1 || value == 4 || value == 5) {
            return value + 2;
        } else {
            return value - 2;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {
            var n1 = AstarPath.active.GetNearest(transform.position).node as GridNode;
            var n2 = AstarPath.active.GetNearest(connectedNode.transform.position).node as GridNode;
            Vector3 diff = transform.position-connectedNode.transform.position;

            int connValue = getGridConnValueFromVector3(diff);
            int invertedConn = invertGridConnValue(connValue);

            n1.SetConnectionInternal(connValue, false);
            n2.SetConnectionInternal(invertedConn, false);

            //if (n1.ContainsConnection(n2))
            //Debug.Log("Connection exists"); //false
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
