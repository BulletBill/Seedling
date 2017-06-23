using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Pathfinder : MonoBehaviour {

	EnemyMover mover;

	//Design variables set in editor
	public bool loop = true;
    public path pathToFollow;

    //Variables used by the game
    int destinationPoint = 0;
	float x;
	float y;
    float wait = 0;
    int direction = 1;

	// Use this for initialization
	void Start () {
        //Cache components
		mover = GetComponent<EnemyMover>();
        //weapon = GetComponent<WeaponCollective>();
	}
	
	// Update is called once per frame
	void Update () {
		x = transform.position.x;
		y = transform.position.y;

		if (wait > 0) {
			wait -= Time.deltaTime;
		}

		if (destinationPoint < pathToFollow.waypoints.Count && destinationPoint >= 0 && wait <= 0) {
			mover.FacePoint(new Vector3(pathToFollow.waypoints[destinationPoint].x, pathToFollow.waypoints[destinationPoint].y));
			mover.MoveToFacing();

			float distanceToWaypoint = Mathf.Sqrt(Mathf.Pow((x - pathToFollow.waypoints[destinationPoint].x), 2) + Mathf.Pow((y - pathToFollow.waypoints[destinationPoint].y), 2));
			if (distanceToWaypoint < pathToFollow.waypoints[destinationPoint].radius) {
				HitWaypoint();
			}
		}
	}

	void HitWaypoint () {
		wait = pathToFollow.waypoints[destinationPoint].wait;
		ExecuteCommand(pathToFollow.waypoints[destinationPoint].command);
        destinationPoint += direction;

        if (destinationPoint < 0 || destinationPoint > pathToFollow.waypoints.Count - 1) {
            if (loop) {
                if (direction == 1) {
                    destinationPoint = 0;
                }
                if (direction == -1) {
                    destinationPoint = pathToFollow.waypoints.Count - 1;
                }
            }
        }
    }

	void ExecuteCommand(pathNode.commands command) {
		if (command == pathNode.commands.FIRE) {

		}
		if (command == pathNode.commands.REVERSE) {
            direction *= -1;
		}
		if (command == pathNode.commands.DETONATE) {
            GameObject.Destroy(this.gameObject);
        }
	}
}
