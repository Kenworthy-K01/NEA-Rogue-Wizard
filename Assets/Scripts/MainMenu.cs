using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	private GameObject controlScrn;
	private GameObject creditScrn;

	private bool controlsOpen = false;
	private bool creditsOpen = false;

	private void Start() {
		// Get credits and controls frames
		controlScrn = transform.GetChild(5).gameObject;
		creditScrn = transform.GetChild(6).gameObject;
	}

	public void OnPlayBtnClicked() {
		SceneManager.LoadScene("Tutorial");
	}

	public void OnControlsBtnClicked() {
		// Toggle visibility of control frame
		if (controlsOpen) {
			controlsOpen = false;
		} else {
			controlsOpen = true;
		}
		controlScrn.SetActive(controlsOpen);
	}

	public void OnCreditsBtnClicked() {
		// Toggle visibility of credits frame
		if (creditsOpen) {
			creditsOpen = false;
		} else {
			creditsOpen = true;
		}
		creditScrn.SetActive(creditsOpen);
	}

}
