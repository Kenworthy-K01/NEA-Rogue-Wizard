using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour {

	private GameObject HeadsUpDisplay;
	private GameObject SPLabel;
	private GameObject InventoryCanvas;
	private GameObject ControlLabel;
	private GameObject relicIcon;
	private GameObject curseIcon;

	private Relic equippedRelic;
	private Relic equippedCurse;

	private Attributes playerAttr;
	private Health playerHealth;

	bool isOpen = false;

	private void Start() {
		// Get all UI objects
		HeadsUpDisplay = GameObject.FindGameObjectWithTag("HUD");
		SPLabel = HeadsUpDisplay.transform.Find("SPLabel").gameObject;
		InventoryCanvas = HeadsUpDisplay.transform.Find("Inventory").gameObject;
		ControlLabel = HeadsUpDisplay.transform.Find("TabBtn").gameObject;
		relicIcon = InventoryCanvas.transform.Find("Container").Find("RelicIcon").gameObject;
		curseIcon = InventoryCanvas.transform.Find("Container").Find("CurseIcon").gameObject;
		playerAttr = GetComponent<Attributes>();
		playerHealth = GetComponent<Health>();
		
		DontDestroyOnLoad(HeadsUpDisplay);
	}

	private void Update() {
		// Manage visibility of UI elements
		SPLabel.SetActive(playerAttr.skillPoints > 0);

		if (Input.GetKeyDown(KeyCode.Tab)) {
			ToggleInventory();
		}
	}

	public void EquipRelic(Relic relic) {
		if (relic.cursed) {
			equippedCurse = relic;
		} else {
			equippedRelic = relic;
		}
	}

	public Relic GetEquippedRelic() {
		return equippedRelic;
	}

	public Relic GetEquippedCurse() {
		return equippedCurse;
	}

	private void ToggleInventory() {
		// Toggle visibility and time scale to pause game
		if (isOpen) {
			isOpen = false;
		} else {
			UpdateAttributeLabels();
			isOpen = true;
		}
		Time.timeScale = isOpen ? 0 : 1;
		ControlLabel.SetActive(!isOpen);
		InventoryCanvas.SetActive(isOpen);
	}

	private void UpdateAttributeLabels() {
		// Update the display values of each attribute
		Transform container = InventoryCanvas.transform.Find("Container");
		Text pointLabel = container.Find("SkillPoints").gameObject.GetComponent<Text>();
		Text strLabel = container.Find("Strength").gameObject.GetComponent<Text>();
		Text ftdLabel = container.Find("Fortitude").gameObject.GetComponent<Text>();
		Text aglLabel = container.Find("Agility").gameObject.GetComponent<Text>();
		Text intLabel = container.Find("Intelligence").gameObject.GetComponent<Text>();
		Text chrLabel = container.Find("Charisma").gameObject.GetComponent<Text>();

		pointLabel.text = playerAttr.skillPoints.ToString();
		strLabel.text = playerAttr.strength.ToString();
		ftdLabel.text = playerAttr.fortitude.ToString();
		aglLabel.text = playerAttr.agility.ToString();
		intLabel.text = playerAttr.intelligence.ToString();
		chrLabel.text = playerAttr.charisma.ToString();

		if (equippedRelic != null) {
			Image icon = relicIcon.GetComponent<Image>();
			icon.sprite = equippedRelic.GetIconSprite();
		}
		if (equippedCurse != null) {
			Image icon = curseIcon.GetComponent<Image>();
			icon.sprite = equippedCurse.GetIconSprite();
		}
	}

	public void SpendSkillPoint(string attribute) {
		// 
		if (!isOpen || playerAttr.skillPoints < 1) { return; }
		if (attribute == "strength") {
			playerAttr.SetStrength(playerAttr.strength + 1);
		} else if (attribute == "fortitude") {
			playerAttr.SetFortitude(playerAttr.fortitude + 1);
			playerHealth.GetMaxHealth(true);
			playerHealth.ApplyHealing(2);
		} else if (attribute == "agility") {
			playerAttr.SetAgility(playerAttr.agility + 1);
		} else if (attribute == "intelligence") {
			playerAttr.SetIntelligence(playerAttr.intelligence + 1);
		} else if (attribute == "charisma") {
			playerAttr.SetCharisma(playerAttr.charisma + 1);
		}
		UpdateAttributeLabels();
	}

	public void QuitGame() {
		Time.timeScale = 1;
		SceneManager.LoadScene("MainMenu");
	}

	// For Relic effects that are applied on enemy death
	public void EnemyKilled() {
		if (equippedRelic == null) { return; }

		switch (equippedRelic.relicId) {
			case "Vampiric Ring":
				playerHealth.ApplyHealing(10);
				break;
			case "Didactic Thesis":
				playerAttr.AwardSkillPoints(3);
				break;
		}
	}
}
