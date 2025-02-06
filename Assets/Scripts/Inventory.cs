using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour {

	GameObject HeadsUpDisplay;
	GameObject SPLabel;
	GameObject InventoryCanvas;
	GameObject ControlLabel;

	Attributes playerAttr;

	bool isOpen = false;

	private void Start() {
		// Get all UI objects
		HeadsUpDisplay = GameObject.FindGameObjectWithTag("HUD");
		SPLabel = HeadsUpDisplay.transform.Find("SPLabel").gameObject;
		InventoryCanvas = HeadsUpDisplay.transform.Find("Inventory").gameObject;
		ControlLabel = HeadsUpDisplay.transform.Find("TabBtn").gameObject;
		playerAttr = GetComponent<Attributes>();
	}

	private void Update() {
		// Manage visibility of UI elements
		SPLabel.SetActive(playerAttr.skillPoints > 0);

		if (Input.GetKeyDown(KeyCode.Tab)) {
			ToggleInventory();
		}
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
	}

	public void SpendSkillPoint(string attribute) {
		// 
		if (!isOpen || playerAttr.skillPoints < 1) { return; }
		if (attribute == "strength") {
			playerAttr.SetStrength(playerAttr.strength + 1);
		} else if (attribute == "fortitude") {
			playerAttr.SetFortitude(playerAttr.fortitude + 1);
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
}
