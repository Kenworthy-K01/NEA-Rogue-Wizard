using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class CharacterControl : MonoBehaviour {

	private enum HumanState { Idle, Walking, Attacking, Stunned, Dead };

	public int WALKSPEED = 2;
	public bool WALKENABLED = true;
	public Material FlashMaterial;
	private Material DefaultMaterial;
	private int SPRINTBOOST = 3;

	private GameObject HeadsUpDisplay;

	private Hurtbox hurtbox;
	private Animator animator;
	private Attributes attributes;
	private Health health;
	private Rigidbody2D rigidBody;
	private SpriteRenderer spriteRenderer;
	private Loadout spellLoadout; 
	private bool sprinting = false;

	private int stunStartFrame = 0;
	private int attackStartFrame = 0;
	private HumanState currentState = HumanState.Idle;
	private Attack activeAttack;
	private GameObject activeSpellVFX;

	void Start () {
		HeadsUpDisplay = GameObject.FindGameObjectWithTag("HUD");
		hurtbox = GetComponentInChildren<Hurtbox>();
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		animator = GetComponentInChildren<Animator>();
		attributes = GetComponent<Attributes>();
		health = GetComponent<Health>();
		spellLoadout = GetComponent<Loadout>();
		DefaultMaterial = spriteRenderer.material;
	}
	
	void FixedUpdate () {
		int now = Time.frameCount;
		currentState = GetHumanState(now);

		GameObject healthbar = HeadsUpDisplay.transform.Find("HealthBarFill").gameObject;
		Image fillImage = healthbar.GetComponent<Image>();
		fillImage.fillAmount = ((float)health.GetCurrentHealth() / (float)health.GetMaxHealth(false));

		if (activeAttack != null && now - attackStartFrame > 13) {
			currentState = HumanState.Idle;
			CleanupActiveAttack();
		}

		Vector3 moveDirection = Vector3.zero;
		if (currentState == HumanState.Stunned) {
			spriteRenderer.material = FlashMaterial;
			MoveCharacter(Vector3.zero);
			AnimateState(currentState, moveDirection);
			return;
		} else if (currentState == HumanState.Dead) {
			spriteRenderer.material = DefaultMaterial;
			MoveCharacter(Vector3.zero);
			AnimateState(currentState, moveDirection);
			return;
		} else if (currentState == HumanState.Idle) {
			Vector3 inputDirection = GetInputMoveDirection();
			if (inputDirection.magnitude > 0 && WALKENABLED) {
				currentState = HumanState.Walking;
			} else {
				MoveCharacter(Vector3.zero);
			}
		} else if (currentState == HumanState.Walking) {
			Vector3 inputDirection = GetInputMoveDirection();
			if (inputDirection.magnitude == 0 || !WALKENABLED) {
				currentState = HumanState.Idle;
			} else {
				WALKSPEED = attributes.CalculateWalkSpeed();
				moveDirection = MoveCharacter(inputDirection);
			}
		} else if (currentState == HumanState.Attacking) {
			if (activeAttack != null) {
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

		spriteRenderer.material = DefaultMaterial;
		AnimateState(currentState, moveDirection);
		HandleCastInput();
	}

	private HumanState GetHumanState(int now) {
		HumanState state = HumanState.Idle;

		if (health.GetCurrentHealth() <= 0) {
			state = HumanState.Dead;
		} else if (now - stunStartFrame < 5) {
			state = HumanState.Stunned;
		} else if (GetInputMoveDirection().magnitude > 0) {
			state = HumanState.Walking;
		} else if (activeAttack != null) {
			state = HumanState.Attacking;
		}

		return state;
	}

	// Changes animator parameters to switch between animation states
	private void AnimateState(HumanState state, Vector3 moveDirection) {
		if (state == HumanState.Idle) {
			animator.SetBool("idle", true);
			animator.SetBool("walking", false);
		} else if (state == HumanState.Walking) {
			float up = Vector2.Dot(moveDirection, Vector2.up);
			float right = Vector2.Dot(moveDirection, Vector2.right);

			spriteRenderer.flipX = false;
			if (right > 0.5) {
				animator.SetInteger("direction", 1);
			} else if (right < -0.5) {
				animator.SetInteger("direction", 3);
				spriteRenderer.flipX = true;
			} else if (up < -0.5) {
				animator.SetInteger("direction", 2);
			} else if (up > 0.5) {
				animator.SetInteger("direction", 0);
			}

			animator.SetBool("idle", false);
			animator.SetBool("walking", true);
		} else if (state == HumanState.Stunned) {
			//	animator.Play("Stun");
		} else if (state == HumanState.Dead) {
			animator.Play("Player_Death");
			spriteRenderer.sortingOrder = -2;
		}
	}

	// Get player input direction
	private Vector3 GetInputMoveDirection() {
		// Input direction floats
		float right = Input.GetAxisRaw("Horizontal");
		float up = Input.GetAxisRaw("Vertical");

		// Normalized unit direction vector, multiply by speed float
		Vector3 inputDirection = (new Vector3(right, up, 0)).normalized;

		return inputDirection;
	}

	// Move character by 1 frame in moveDirection
	public Vector3 MoveCharacter(Vector3 moveDirection) {
		sprinting = Input.GetKey(KeyCode.LeftShift);
		int sprintBoost = sprinting ? SPRINTBOOST : 0;

		Vector3 moveVector = moveDirection * (WALKSPEED + sprintBoost);

		// Translate Character in this direction
		rigidBody.velocity = moveVector;
		return moveVector;
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

	private void HitStun() {
		stunStartFrame = Time.frameCount;
		spriteRenderer.material = FlashMaterial;

		Vector3 knockback = new Vector3(0, 0, 0);
		rigidBody.velocity = knockback;
	}
}
