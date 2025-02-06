using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterControl : MonoBehaviour {

	private enum HumanState { Idle, Walking, Attacking, Stunned, Frozen, Dead };

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

	private Vector2 mouseDirectionV2;
	private Vector3 mouseDirectionV3;
	private int stunStartFrame = 0;
	private int diedFrame = 0;
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

		// Update attack button cooldown displays
		Transform AttackCtrls = HeadsUpDisplay.transform.Find("AttackControls");
		Image m1Cd = AttackCtrls.Find("M1").Find("Cooldown").gameObject.GetComponent<Image>();
		Image qCd = AttackCtrls.Find("Q").Find("Cooldown").gameObject.GetComponent<Image>();
		Image eCd = AttackCtrls.Find("E").Find("Cooldown").gameObject.GetComponent<Image>();
		Image rCd = AttackCtrls.Find("R").Find("Cooldown").gameObject.GetComponent<Image>();

		m1Cd.fillAmount = spellLoadout.GetSpellCooldown(0);
		qCd.fillAmount = spellLoadout.GetSpellCooldown(1);
		eCd.fillAmount = spellLoadout.GetSpellCooldown(2);
		rCd.fillAmount = spellLoadout.GetSpellCooldown(3);

		// Update healthbar
		GameObject healthbar = HeadsUpDisplay.transform.Find("HealthBarFill").gameObject;
		Image fillImage = healthbar.GetComponent<Image>();
		fillImage.fillAmount = ((float)health.GetCurrentHealth() / (float)health.GetMaxHealth(false));

		mouseDirectionV3 =(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
		mouseDirectionV2 = new Vector2(mouseDirectionV3.x, mouseDirectionV3.y);

		// Cleanup inactive attacks
		if (activeAttack != null && spellLoadout.GetSpellCooldown(activeAttack.slot) == 0) {
			currentState = HumanState.Idle;
			CleanupActiveAttack();
		}

		Vector3 moveDirection = Vector3.zero;

		// Frozen state for manual control only
		if (currentState == HumanState.Frozen) { return; }

		if (currentState == HumanState.Stunned) {
			spriteRenderer.material = FlashMaterial;
			MoveCharacter(Vector3.zero);
			AnimateState(currentState, moveDirection);
			return;
		} else if (currentState == HumanState.Dead) {
			if (diedFrame == 0) {
				diedFrame = now;
				GameObject deathScreen = HeadsUpDisplay.transform.Find("DeathScreen").gameObject;
				deathScreen.SetActive(true);
			} else if (now - diedFrame > 360) {
				// RESPAWN by reloading the current scene.
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
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
		}  else if (currentState == HumanState.Attacking) {
			moveDirection = MoveCharacter(Vector3.zero);

			if (activeAttack != null) {
				List<GameObject> targets = hurtbox.GetObjectsInBoxBounds();
				foreach (GameObject hit in targets) {
					if (activeAttack.HasHitTarget(hit)) { continue; }
					activeAttack.HitTarget(hit);
					Health targetHp = hit.GetComponent<Health>();
					int damage = activeAttack.CalculateDamage(hit);
					targetHp.TakeDamage(damage);
					Knockback(hit);
				}
			}
		} else if (currentState == HumanState.Walking) {
			Vector3 inputDirection = GetInputMoveDirection();
			if (inputDirection.magnitude == 0 || !WALKENABLED) {
				currentState = HumanState.Idle;
			} else {
				WALKSPEED = attributes.CalculateWalkSpeed();
				moveDirection = MoveCharacter(inputDirection);
			}
		}

		spriteRenderer.material = DefaultMaterial;
		AnimateState(currentState, moveDirection);
		HandleCastInput();
	}

	private void Knockback(GameObject entity) {
		Vector3 dir = (entity.transform.position - transform.position);
		dir.Normalize();

		Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
		rb.velocity = dir * 7;
	}

	private HumanState GetHumanState(int now) {
		HumanState state = HumanState.Idle;

		if (health.GetCurrentHealth() <= 0) {
			state = HumanState.Dead;
		} else if (now - stunStartFrame < 5) {
			state = HumanState.Stunned;
		} else if (activeAttack != null) {
			state = HumanState.Attacking;
		} else if (GetInputMoveDirection().magnitude > 0) {
			state = HumanState.Walking;
		}

		return state;
	}

	// Changes animator parameters to switch between animation states
	private void AnimateState(HumanState state, Vector3 moveDirection) {
		if (state == HumanState.Idle) {
			animator.SetBool("idle", true);
			animator.SetBool("walking", false);
		}  else if (state == HumanState.Attacking) {
			float up = Vector2.Dot(mouseDirectionV2, Vector2.up);
			float right = Vector2.Dot(mouseDirectionV2, Vector2.right);

			if (Mathf.Abs(right) > 0.3) {
				spriteRenderer.flipX = (right < -0.5);
				animator.Play("Player_AttackRight");
			} else if (up < -0.5) {
				animator.Play("Player_AttackDown");
			} else if (up > 0.5) {
				animator.Play("Player_AttackUp");
			}
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
			CastAttack(0);
		} else if (Input.GetKeyDown(KeyCode.Q)) {
			CastAttack(1);
		} else if (Input.GetKeyDown(KeyCode.E)) {
			CastAttack(2);
		} else if (Input.GetKeyDown(KeyCode.R)) {
			CastAttack(3);
		}
	}

	private void CastAttack(int slot) {
		if (spellLoadout.GetSpellCooldown(slot) != 0) { return; }

		string spellId = spellLoadout.GetSpellIdFromSlot(slot);
		if (String.IsNullOrEmpty(spellId)) { return; } 
		if (activeAttack != null) { return; }

		spellLoadout.SpellStartCooldown(slot, 1f/2f);

		hurtbox.enabled = true;

		activeAttack = new Attack();
		activeAttack.slot = slot;
		activeAttack.baseDamage = 15;
		activeAttack.scaling = 1;
		activeAttack.scaleType = ScaleType.Strength;
		activeAttack.armorPenetration = 0;

		Vector3 dir = mouseDirectionV3;
		dir.z = 0;
		dir.Normalize();
		Vector3 vfxPoint = transform.position + (dir)*2;

		// REFERENCE: UNITY DOCS QUATERNION ROTATION
		Vector3 relPoint = transform.position - vfxPoint;
		Vector3 lookVector = new Vector3(relPoint.y, -relPoint.x, 0) * Mathf.Sign(-relPoint.x);
		Quaternion vfxRot = Quaternion.LookRotation(-Vector3.forward, lookVector);
		// END REFERENCE

		GameObject attkHurtbox = transform.Find("Hurtbox").gameObject;
		attkHurtbox.transform.position = vfxPoint - dir;
		attkHurtbox.transform.rotation = vfxRot;

		GameObject spellFX = Resources.Load<GameObject>("VisualEffects/" + spellId);
		activeSpellVFX = Instantiate(spellFX, vfxPoint, vfxRot);
		activeSpellVFX.transform.SetParent(gameObject.transform);
	}

	private void CleanupActiveAttack() {
		Destroy(activeSpellVFX);
		hurtbox.enabled = false;

		activeSpellVFX = null;
		activeAttack = null;
	}

	private void HitStun() {
		stunStartFrame = Time.frameCount;
		spriteRenderer.material = FlashMaterial;

		Vector3 knockback = new Vector3(0, 0, 0);
		rigidBody.velocity = knockback;
	}

	public void UpdateClearCount() {
		// Update the label to show what percent of enemies have been killed
		// At 100% the level is complete

		// Get the enemy counts from the dungeon generator
		DungeonGeneration generator = GameObject.FindGameObjectWithTag("Generator").GetComponent<DungeonGeneration>();
		int numEnemies = generator.CountRemainingEnemies();
		int totalEnemies = generator.levelStartEnemies;

		// Calculate the percentage of enemies that have been killed so far
		float percent = (1-((float)numEnemies / (float)totalEnemies))*100;

		if (percent >= 100) {
			// Cleared this level
			GameObject winScreen = HeadsUpDisplay.transform.Find("WinScreen").gameObject;
			winScreen.SetActive(true);
			generator.OnLevelCleared();
			currentState = HumanState.Frozen;
		}

		int displayPercent = Mathf.RoundToInt(percent);

		Text label = HeadsUpDisplay.transform.Find("ClearCount").GetComponent<Text>();
		label.text = displayPercent.ToString() + "%";
	}
}
