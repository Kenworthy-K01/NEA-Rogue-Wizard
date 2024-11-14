﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

	public int WALKSPEED = 2;
	public bool WALKENABLED = true;
	private int SPRINTBOOST = 2;

	private Animator animator;
	private Rigidbody2D rigidBody;
	private SpriteRenderer spriteRenderer;
	private bool sprinting = false;

	void Start () {
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator = GetComponentInChildren<Animator>();
	}
	
	void FixedUpdate () {
		if (WALKENABLED) {
			Vector3 inputDirection = GetInputMoveDirection();
			MoveCharacter(inputDirection);
		}
	}

	private void AnimateState(Vector2 moveDirection) {
		float up = Vector2.Dot(moveDirection, Vector2.up);
		float right = Vector2.Dot(moveDirection, Vector2.right);

		bool walking = true;

		if (up > 0.5) {
			spriteRenderer.flipX = false;
			animator.SetInteger("direction", 0);
		} else if (right > 0.5) {
			spriteRenderer.flipX = false;
			animator.SetInteger("direction", 1);
		} else if (up < -0.5) {
			spriteRenderer.flipX = false;
			animator.SetInteger("direction", 2);
		} else if (right < -0.5) {
			spriteRenderer.flipX = true;
			animator.SetInteger("direction", 3);
		} else {
			walking = false;
		}

		animator.SetBool("idle", !walking);
		animator.SetBool("walking", walking);
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

		AnimateState(new Vector2(moveDirection.x, moveDirection.y));
		// Translate Character in this direction
		transform.Translate(moveVector);
	}
}
