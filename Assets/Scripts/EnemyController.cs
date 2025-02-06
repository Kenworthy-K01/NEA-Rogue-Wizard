using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private enum BrainState { Idle, Following, Attacking, Stunned, Dead };

	// Enemy Configs
	public int WALKSPEED = 2;
	public bool WALKENABLED = true;
	public int AGGRORANGE = 15;

	// Other enemy behaviours
	private Hurtbox hurtbox;
	private Animator animator;
	private Attributes attributes;
	private Rigidbody2D rigidBody;
	private SpriteRenderer spriteRenderer;
	private Health health;

	// Changing states and attributes
	private Attack activeAttack;
	private Vector3 moveDirection = Vector3.zero;
	private BrainState currentState = BrainState.Idle;
	private GameObject aggroTarget;
	private int attackStartFrame = 0;
	private int stunStartFrame = 0;
	private int diedAtFrame = 0;

	void Start () {
		hurtbox = GetComponentInChildren<Hurtbox>();
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator = GetComponentInChildren<Animator>();
		attributes = GetComponent<Attributes>();
		health = GetComponent<Health>();
	}

	void FixedUpdate () {
		// Update the current state
		currentState = GetBrainState();
		AnimateState(currentState);

		int now = Time.frameCount;
		
		// State-specific behaviour
		if (currentState == BrainState.Dead || currentState == BrainState.Idle) {
			if (currentState == BrainState.Dead && now - diedAtFrame > 180) {
				// We've been dead for 180 frames, cleanup the model
				OnDeath();
				return;
			}
			moveDirection = Vector3.zero;
			MoveCharacter();
		} else if (currentState == BrainState.Stunned) {
			if (now - stunStartFrame > 10) {
				// Return to Idle after being Stunned
				currentState = BrainState.Idle;
			}
		} else if (currentState == BrainState.Following) {
			// Move toward aggro target & update walk speed
			WALKSPEED = attributes.CalculateWalkSpeed();
			if (WALKENABLED) {
				UpdateMoveDirection();
				MoveCharacter();
			}
		} else if (currentState == BrainState.Attacking) {
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
				} else if (activeFrames >= 20) {
					// Every frame, check if a new object has entered the hitbox
					List<GameObject> targets = hurtbox.GetObjectsInBoxBounds();
					foreach (GameObject hit in targets) {
						// Have we already hit this target
						if (activeAttack.HasHitTarget(hit)) { continue; }
						activeAttack.HitTarget(hit);

						// Deal damage
						Health targetHp = hit.GetComponent<Health>();
						int damage = activeAttack.CalculateDamage(hit);
						targetHp.TakeDamage(damage);
					}
				}
			}
		}
	}

	private void OnDeath() {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		DungeonGeneration generator = GameObject.FindGameObjectWithTag("Generator").GetComponent<DungeonGeneration>();
		// Reward player
		generator.SendMessage("EnemyKilled", gameObject);
		player.SendMessage("UpdateClearCount");
		Attributes attr = player.GetComponent<Attributes>();
		attr.AwardSkillPoints(3);

		Destroy(gameObject);
	}

	private void CleanupAttack() {
		currentState = BrainState.Idle;
		activeAttack = null;
	}

	// Update the current brain state based on number of conditions
	private BrainState GetBrainState() {
		// Do not change state when stunned
		if (currentState == BrainState.Stunned) {
			return currentState;
		}
		
		// If health is negative enemy is dead
		if (health.GetCurrentHealth() <= 0 ) {
			if (currentState != BrainState.Dead) {
				diedAtFrame = Time.frameCount;
			}
			return BrainState.Dead;
		}

		// If we already found a target and can still see them, follow/attack them
		if (aggroTarget != null) {
			double dist = GetTargetDistance(aggroTarget);
			Health targHp = aggroTarget.GetComponent<Health>();
			int now = Time.frameCount;
			if (((dist < 1) && (targHp.GetCurrentHealth() > 0) && (now - attackStartFrame > 60)) || currentState == BrainState.Attacking) {
				return BrainState.Attacking;
			} else if ((dist > 1) && (dist < AGGRORANGE) && (targHp.GetCurrentHealth() > 0)) {
				return BrainState.Following;
			} else {
				aggroTarget = null;
			}
		} else {
			// Find the player object and check if they are visible
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
		} else if (state == BrainState.Following) {
			animator.SetBool("walking", true);
			animator.SetBool("idle", false);

			float right = Vector2.Dot(moveDirection, Vector2.right);
			spriteRenderer.flipX = (right < -0.5);
		} else if (state == BrainState.Attacking) {
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Skeleton_Attack")) {
				animator.Play("Skeleton_Attack");
			}
		} else if (state == BrainState.Dead) {
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Skeleton_Death")) {
				animator.Play("Skeleton_Death");
			}
		}
	}

	// Get direction toward target
	private void UpdateMoveDirection() {
		// Normalized unit direction vector, multiply by speed float
		moveDirection = Vector3.zero;
		if (aggroTarget != null) {
			Vector3 dir = (aggroTarget.transform.position - transform.position).normalized;
			moveDirection = dir;
		}
	}

	// Inflict "Stunned" when hit
	private void HitStun() {
		if (currentState == BrainState.Dead) { return; }
		// Inflict stun when hit
		stunStartFrame = Time.frameCount;
		currentState = BrainState.Stunned;
		animator.Play("Skeleton_Hit");

		// Knocked back away from the attack
		Vector3 knockback = new Vector3(0, 0, 0);
		rigidBody.velocity = knockback;
	}

	// Update rigidBody velocity in moveDirection
	public void MoveCharacter() {
		Vector3 moveVector = moveDirection * WALKSPEED;

		// Translate Character in this direction
		rigidBody.velocity = moveVector;
	}
}
