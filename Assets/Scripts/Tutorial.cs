using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

	// Object references
	private GameObject player;
	private GameObject HUD;
	private GameObject chatContainer;
	private Text chatTextLabel;
	private DungeonGeneration generator;

	private CharacterControl controls;

	// Chat states and queue
	private Queue<string> chatQueue = new Queue<string>();
	private string currentMsg = "";
	private string currentContent = "";
	private int nextChar = 0;
	private bool awaitingInput = false;

	private bool battleStarted = false;
	
	// Get objects and start tutorial sequence
	private void Start() {
		
		player = GameObject.FindGameObjectWithTag("Player");
		HUD = GameObject.FindGameObjectWithTag("HUD");
		chatContainer = HUD.transform.Find("Speech").gameObject;
		chatTextLabel = chatContainer.transform.Find("Content").gameObject.GetComponent<Text>();
		generator = GameObject.FindGameObjectWithTag("Generator").GetComponent<DungeonGeneration>();
		
		controls = player.GetComponent<CharacterControl>();

		StartTutorial();
	}

	// Loop through chat queue
	// Display one character of front line.
	private void Update() {
		if (awaitingInput) {
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				awaitingInput = false;
			}
		}
		
		if (currentMsg != "") {
			if (currentMsg != currentContent) {
				currentContent += currentMsg[nextChar];
				chatTextLabel.text = currentContent;
				nextChar += 1;
			} else {
				currentMsg = "";
				currentContent = "";
				nextChar = 0;
				awaitingInput = true;
			}
		} else if (chatQueue.Count == 0 && !awaitingInput) {
			chatContainer.SetActive(false);
			// Spawn enemy when last message sent
			StartTutorialBattle();
			return;
		} else if (!awaitingInput) {
			currentMsg = chatQueue.Dequeue();
			chatContainer.SetActive(true);
		}
	}

	// Send message to chat queue
	private void Chat(string msg) {
		chatQueue.Enqueue(msg);
	}

	// Main tutorial sequence
	private void StartTutorial() {
		controls.WALKENABLED = false;
		
		Chat("Welcome to the Rogue Wizard Tutorial. Click the LEFT MOUSE BUTTON to Continue.");
		Chat("In this game there are 3 levels. Each level is a different floor of the dungeon.");
		Chat("Once you clear all of the enemies in one floor, you will be sent to the next floor.");
		Chat("If you die, you will have to restart the level.");
		Chat("Use the Buttons in the bottom left of the screen to cast your magic spells.");
		Chat("Some spells will inflict STATUS EFFECTS on enemies. Each STATUS EFFECT applies a specific debuff to those inflicted.");
		Chat("The POISON and BURN effects will take away 1/16th of the enemies maximum HP each tick.");
		Chat("The CHILL effect slows down your enemies to 1/2 of their original walk speed.");
		Chat("The SHOCK effect will completely paralyze your enemies.");
		Chat("When an enemy is defeated, the player gains SKILL POINTS. This is shown on the right side of the screen. To spend SKILL POINTS, press Tab to open the inventory.");
		Chat("You will now be tested. Defeat this enemy to start the game...");
	}

	// Spawn tutorial enemy
	private void StartTutorialBattle() {
		if (battleStarted) { return; }

		battleStarted = true;
		controls.WALKENABLED = true;
		
		GameObject entityOriginal = Resources.Load<GameObject>("Entities/Skeleton");
		Vector3 atPosition = new Vector3(0, 2, 0);
		GameObject enemy = Instantiate(entityOriginal, atPosition, Quaternion.identity);
		generator.AddEnemy(enemy);
	}

}
