using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

	public int WALKSPEED = 2;
	public bool WALKENABLED = true;
	private int SPRINTBOOST = 3;

	private Hurtbox hurtbox;
	private Animator animator;
	private Attributes attributes;
	private Rigidbody2D rigidBody;
	private SpriteRenderer spriteRenderer;
	private Loadout spellLoadout;
	private bool sprinting = false;

	private int attackStartFrame = 0;
	private Attack activeAttack;
	private GameObject activeSpellVFX;

	void Start () {
		hurtbox = GetComponentInChildren<Hurtbox>();
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator = GetComponentInChildren<Animator>();
		attributes = GetComponent<Attributes>();
		spellLoadout = GetComponent<Loadout>();
	}
	
	void FixedUpdate () {
		WALKSPEED = attributes.CalculateWalkSpeed();
		if (WALKENABLED) {
			Vector3 inputDirection = GetInputMoveDirection();
			MoveCharacter(inputDirection);
		}

		float now = Time.frameCount;
		if (activeAttack != null) {
			if (now - attackStartFrame > 13) {
				CleanupActiveAttack();
			} else {
				List<GameObject> targets = hurtbox.GetObjectsInBoxBounds();
				foreach (GameObject hit in targets) {
					if (activeAttack.HasHitTargetWithinFrames(hit, 13)) { continue; }
					activeAttack.HitTarget(hit);
					Health targetHp = hit.GetComponent<Health>();
					int damage = activeAttack.CalculateDamage(hit);
					targetHp.TakeDamage(damage);
				}
			}
		}
		HandleCastInput();
	}

	// Changes animator parameters to switch between animation states
	private void AnimateState(Vector2 moveDirection) {
		float up = Vector2.Dot(moveDirection, Vector2.up);
		float right = Vector2.Dot(moveDirection, Vector2.right);

		bool walking = true;

		if (right > 0.5) {
			spriteRenderer.flipX = false;
			animator.SetInteger("direction", 1);
		} else if (right < -0.5) {
			spriteRenderer.flipX = true;
			animator.SetInteger("direction", 3);
		} else if (up < -0.5) {
			spriteRenderer.flipX = false;
			animator.SetInteger("direction", 2);
		} else if (up > 0.5) {
			spriteRenderer.flipX = false;
			animator.SetInteger("direction", 0);
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

		Vector3 moveVector = moveDirection * (WALKSPEED + sprintBoost);

		AnimateState(new Vector2(moveDirection.x, moveDirection.y));
		// Translate Character in this direction
		rigidBody.velocity = moveVector;
	}

	// Spell Casting
	private void HandleCastInput() {
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			CastAttack(1);
		} else if (Input.GetKeyDown(KeyCode.Q)) {
			CastAttack(2);
		} else if (Input.GetKeyDown(KeyCode.E)) {
			CastAttack(3);
		} else if (Input.GetKeyDown(KeyCode.R)) {
			CastAttack(4);
		}
	}

	private void CastAttack(int slot) {
		string spellId = spellLoadout.GetSpellIdFromSlot(slot);
		if (String.IsNullOrEmpty(spellId)) { return; } 
		if (activeAttack != null) { return; }

		attackStartFrame = Time.frameCount;
		
		hurtbox.enabled = true;

		activeAttack = new Attack();
		activeAttack.baseDamage = 15;
		activeAttack.scaling = 1;
		activeAttack.scaleType = ScaleType.Strength;
		activeAttack.armorPenetration = 0;

		Vector3 vfxPoint = transform.position;
		GameObject spellFX = Resources.Load<GameObject>("VisualEffects/" + spellId);
		activeSpellVFX = Instantiate(spellFX, vfxPoint, Quaternion.identity);
		activeSpellVFX.transform.SetParent(gameObject.transform);
	}

	private void CleanupActiveAttack() {
		Destroy(activeSpellVFX);
		hurtbox.enabled = false;

		attackStartFrame = 0;
		activeSpellVFX = null;
		activeAttack = null;
	}
}
