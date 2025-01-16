using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private enum BrainState { Idle, Following, Attacking, Stun, Dead };

	public int WALKSPEED = 2;
	public bool WALKENABLED = true;
	public int AGGRORANGE = 15;

	private Hurtbox hurtbox;
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
	private int stunStartFrame = 0;

	void Start () {
		hurtbox = GetComponentInChildren<Hurtbox>();
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator = GetComponentInChildren<Animator>();
		attributes = GetComponent<Attributes>();
		health = GetComponent<Health>();
	}

	void FixedUpdate () {
		currentState = GetBrainState();
		AnimateState(currentState);

		int now = Time.frameCount;
		
		// State-specific behaviour
		if (currentState == BrainState.Dead || currentState == BrainState.Idle) {
			moveDirection = Vector3.zero;
			MoveCharacter();
		} else if (currentState == BrainState.Stun) {
			if (now - stunStartFrame > 10) {
				currentState = BrainState.Idle;
			}
		} else if (currentState == BrainState.Following) {
			// Move toward aggro target
			WALKSPEED = attributes.CalculateWalkSpeed();
			if (WALKENABLED) {
				UpdateMoveDirection();
				MoveCharacter();
			}
		} else if (currentState == BrainState.Attacking) {
			// Create attack on first frame, destroy it on 30th
			moveDirection = Vector3.zero;
			MoveCharacter();

			if (activeAttack == null) {
				attackStartFrame = now;

				activeAttack = new Attack();
				activeAttack.baseDamage = 10;
				activeAttack.scaling = 1;
				activeAttack.scaleType = ScaleType.Strength;
				activeAttack.armorPenetration = 10;
			} else {
				int activeFrames = now - attackStartFrame;
				if (activeFrames >= 24) {
					CleanupAttack();
				} else {
					List<GameObject> targets = hurtbox.GetObjectsInBoxBounds();
					foreach (GameObject hit in targets) {
						if (activeAttack.HasHitTargetWithinFrames(hit, 24)) { continue; }
						activeAttack.HitTarget(hit);
						Health targetHp = hit.GetComponent<Health>();
						int damage = activeAttack.CalculateDamage(hit);
						targetHp.TakeDamage(damage);
					}
				}
			}
		}
	}

	private void CleanupAttack() {
		currentState = BrainState.Idle;
		activeAttack = null;
	}

	// Update the current brain state based on number of conditions
	private BrainState GetBrainState() {
		// Do not change state when stunned
		if (currentState == BrainState.Stun) {
			return currentState;
		}
		
		// If health is negative enemy is dead
		if (health.GetCurrentHealth() <= 0) {
			return BrainState.Dead;
		}

		// If we already found a target and can still see them, follow/attack them
		if (aggroTarget != null) {
			double dist = GetTargetDistance(aggroTarget);
			Health targHp = aggroTarget.GetComponent<Health>();
			if (((dist < 0.5) && (targHp.GetCurrentHealth() > 0)) || currentState == BrainState.Attacking) {
				return BrainState.Attacking;
			} else if ((dist < AGGRORANGE) && (targHp.GetCurrentHealth() > 0)) {
				return BrainState.Following;
			} else {
				aggroTarget = null;
			}
		} else {
			GameObject character = GameObject.FindGameObjectWithTag("Player");
			if (character != null) {
				Health playerHp = character.GetComponent<Health>();
				if ((playerHp.GetCurrentHealth() > 0) && (GetTargetDistance(character) < AGGRORANGE)) {
					aggroTarget = character;
				}
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
			spriteRenderer.flipX = (right < -0.5);
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

	private void HitStun() {
		if (currentState == BrainState.Dead) { return; }
		stunStartFrame = Time.frameCount;
		currentState = BrainState.Stun;
		animator.Play("Skeleton_Hit");

		Vector3 knockback = new Vector3(0, 0, 0);
		rigidBody.velocity = knockback;
	}

	// Move character by 1 frame in moveDirection
	public void MoveCharacter() {
		Vector3 moveVector = moveDirection * WALKSPEED;

		// Translate Character in this direction
		rigidBody.velocity = moveVector;
	}
}
