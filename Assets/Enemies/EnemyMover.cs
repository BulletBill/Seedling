using UnityEngine;
using System.Collections;

public struct MovementStats {
	public float acceleration;
	public float deceleration;
	public float speed;
	public float turnSpeed;
	public float currentSpeed;
}

//Rigidbody is used to move the enemy, so one is required
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMover: MonoBehaviour {

	//Holds values for movement. Can be set in editor
	public MovementStats movementStats;
	Rigidbody2D rigidBody;

	//Input sockets
	Vector2 movementInput;

	//Acceleration / Deceleration magic number helpers
	static int ACCELERATION_FIXER = 2;
	static int DECELERATION_FIXER = 500;

	// Use this for initialization
	void Start() {
		//Cache needed components
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		Mathf.Clamp(movementStats.acceleration, 0, 100);
		Mathf.Clamp(movementStats.deceleration, 0, 100);
	}
	
	// Update is called once per frame
	void Update() {
		movementStats.currentSpeed = Mathf.Abs(rigidBody.velocity.x) + Mathf.Abs(rigidBody.velocity.y);

		if (movementInput.x == 0 && movementInput.y == 0) {
			Decelerate();
		} else {
			Accelerate();
		}

		//Reset inputs
		movementInput.x = 0;
		movementInput.y = 0;
	}

	//Private functions
	void Decelerate() {
		rigidBody.velocity *= (DECELERATION_FIXER - movementStats.deceleration) / DECELERATION_FIXER;
	}

	void Accelerate() {
		rigidBody.AddForce(movementInput.normalized * (movementStats.acceleration / ACCELERATION_FIXER));
		rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, movementStats.speed);
	}

	bool InputMatchesVelocity() {
		Vector2 rigidBodySigns = new Vector2(Mathf.Sign(rigidBody.velocity.x), Mathf.Sign(rigidBody.velocity.y));
		Vector2 movementSigns = new Vector2(Mathf.Sign(movementInput.x), Mathf.Sign(movementInput.y));

        return rigidBodySigns == movementSigns;
	}

	//These functions are called by an input handler or ai script to move the ship
	public void MoveUp() {
		movementInput.y += 1;
	}
	public void MoveDown() {
		movementInput.y -= 1;
	}
	public void MoveLeft() {
		movementInput.x -= 1;
	}
	public void MoveRight() {
		movementInput.x += 1;
	}
	public void MoveToFacing() {
		Vector2 dir = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector3.up;
		movementInput = dir.normalized;
	}
	public void FacePoint (Vector3 facingTarget) {
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, facingTarget - transform.position), movementStats.turnSpeed * Time.deltaTime);
	}
}
