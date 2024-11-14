using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

	public int WALKSPEED = 2;
	public bool WALKENABLED = true;
	private int SPRINTBOOST = 2;

	private Rigidbody2D rigidBody;
	private bool sprinting = false;

	void Start () {
		rigidBody = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
		if (WALKENABLED) {
			Vector3 inputDirection = GetInputMoveDirection();
			MoveCharacter(inputDirection);
		}
	}

	// Get player input direction
	private Vector3 GetInputMoveDirection() {
		// Input direction floats
		float right = Input.GetAxisRaw("Horizontal");
		float up = Input.GetAxisRaw("Vertical");

		// Normalized unit direction vector, multiply by speed float
		Vector3 moveDirection = (new Vector3(right, up, 0)).normalized;

		return moveDirection;
	}

	// Move character by 1 frame in moveDirection
	public void MoveCharacter(Vector3 moveDirection) {
		sprinting = Input.GetKey(KeyCode.LeftShift);
		int sprintBoost = sprinting ? SPRINTBOOST : 0;

		Vector3 moveVector = moveDirection * (WALKSPEED + sprintBoost) * Time.deltaTime;

		// Translate Character in this direction
		transform.Translate(moveVector);
	}
}
