 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class path : MonoBehaviour {

    //Array of Path nodes
    public List<pathNode> waypoints;

    //Worldspace coords

	// Use this for initialization
	void Start () {
        foreach (pathNode p in waypoints) {
            p.align(transform.position);
        }
    }

    // Update is called once per frame
    void Update () {
	
    }

    void OnDrawGizmosSelected() {
        if (waypoints.Count <= 0)
            return;

        for (int i = 0; i < waypoints.Count; i++) {
            Gizmos.color = Color.cyan;
            Vector3 currentPos = new Vector3(transform.position.x + waypoints[i].relx, transform.position.y + waypoints[i].rely);
            Gizmos.DrawSphere(currentPos, waypoints[i].radius);

            if (i + 1 < waypoints.Count) {
                Gizmos.color = Color.yellow;
                Vector3 nextPos = new Vector3(transform.position.x + waypoints[i + 1].relx, transform.position.y + waypoints[i + 1].rely);
                Gizmos.DrawLine(currentPos, nextPos);
            }
        }
    }
}
