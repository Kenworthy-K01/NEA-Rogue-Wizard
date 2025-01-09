using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private enum BrainState { Idle, Following, Attacking, Dead };

	public int WALKSPEED = 2;
	public bool WALKENABLED = true;
	public int AGGRORANGE = 15;

	private BoxCollider2D hurtbox;
	private Animator animator;
	private Attributes attributes;
	private Rigidbody2D rigidBody;
	private SpriteRenderer spriteRenderer;
	private Health health;

	private Attack activeAttack;
	private Vector3 moveDirection = Vector3.zero;
	private BrainState currentState = BrainState.Idle;
	private GameObject aggroTarget;
	private int attackStartFrame = 0;

	void Start () {
		hurtbox = GetComponentInChildren<BoxCollider2D>();
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator = GetComponentInChildren<Animator>();
		attributes = GetComponent<Attributes>();
		health = GetComponent<Health>();
	}

	void FixedUpdate () {
		currentState = GetBrainState();
		AnimateState(currentState);

		// State-specific behaviour
		if (currentState == BrainState.Dead || currentState == BrainState.Idle) {
			moveDirection = Vector3.zero;
			MoveCharacter();
			return;
		} else if (currentState == BrainState.Following) {
			// Move toward aggro target
			WALKSPEED = attributes.CalculateWalkSpeed();
			if (WALKENABLED) {
				UpdateMoveDirection();
				MoveCharacter();
			}
		} else if (currentState == BrainState.Attacking) {
			// Create attack on first frame, destroy it on 7th
			moveDirection = Vector3.zero;
			MoveCharacter();

			if (activeAttack == null) {
				attackStartFrame = Time.frameCount;
				hurtbox.enabled = true;

				activeAttack = new Attack();
				activeAttack.baseDamage = 10;
				activeAttack.scaling = 1;
				activeAttack.scaleType = ScaleType.Strength;
				activeAttack.armorPenetration = 10;
			} else {
				int activeFrames = Time.frameCount - attackStartFrame;
				if (activeFrames >= 30) {
					CleanupAttack();
				}
			}
		}
	}

	private void CleanupAttack() {
		activeAttack = null;
		hurtbox.enabled = false;
		currentState = BrainState.Idle;
	}

	// Update the current brain state based on number of conditions
	private BrainState GetBrainState() {
		// If health is negative enemy is dead
		if (health.GetCurrentHealth() <= 0) {
			return BrainState.Dead;
		}

		// If we already found a target and can still see them, follow/attack them
		if (aggroTarget != null) {
			double dist = GetTargetDistance(aggroTarget);
			if (dist < 0.5 || currentState == BrainState.Attacking) {
				return BrainState.Attacking;
			} else if (dist < AGGRORANGE) {
				return BrainState.Following;
			} else {
				aggroTarget = null;
			}
		} else {
			GameObject character = GameObject.FindGameObjectWithTag("Player");
			if (character != null && GetTargetDistance(character) < AGGRORANGE) {
				aggroTarget = character;
			}
		}

		// Default to idle state
		return BrainState.Idle;
	}

	// Simple magnitude check
	private double GetTargetDistance(GameObject target) {
		Vector3 mp = transform.position;
		Vector3 tp = target.transform.position;
		double dist = (mp-tp).magnitude;

		return dist;
	}

	// Changes animator parameters to switch between animation states
	private void AnimateState(BrainState state) {
		if (state == BrainState.Idle) {
			animator.SetBool("idle", true);
			animator.SetBool("walking", false);
			return;
		} else if (state == BrainState.Following) {
			animator.SetBool("walking", true);
			animator.SetBool("idle", false);

			float right = Vector2.Dot(moveDirection, Vector2.right);

			spriteRenderer.flipX = false;
			if (right < -0.5) {
				spriteRenderer.flipX = true;
			}
		} else if (state == BrainState.Attacking) {
			animator.Play("Skeleton_Attack");
		} else if (state == BrainState.Dead) {
			animator.Play("Skeleton_Death");
		}
	}

	// Get follow direction
	private void UpdateMoveDirection() {
		// Normalized unit direction vector, multiply by speed float
		moveDirection = Vector3.zero;
		if (aggroTarget != null) {
			moveDirection = (aggroTarget.transform.position - transform.position).normalized;
		}
	}

	// Move character by 1 frame in moveDirection
	public void MoveCharacter() {
		Vector3 moveVector = moveDirection * WALKSPEED;

		// Translate Character in this direction
		rigidBody.velocity = moveVector;
	}

	public void HurtboxHit(Collision2D hit) {
		Debug.Log("hit");
		if (activeAttack == null) { return; }

		GameObject target = hit.gameObject;
		Health targetHp = target.GetComponent<Health>();
		int damage = activeAttack.CalculateDamage(target);
		targetHp.TakeDamage(damage);
		Debug.Log(targetHp.GetCurrentHealth());
	}
}
