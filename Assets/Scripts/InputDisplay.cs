using System.Collections.Generic;
using UnityEngine;

class InputDisplay : MonoBehaviour
{
	// refs
	public GameObject inputAndTimePrefab;

	// fields
	Player player;
	List<(PlayerInput input, int time)> levelInputs;
	List<(PlayerInput input, int time)> remainingInputs;

	void Start() {
		// NOTE player must have already set up the inputs list
		player = GameObject.FindObjectOfType<Player>();
		List<PlayerInput> playerInputs = player.inputs;

		if(playerInputs != null && playerInputs.Count > 0)
			GetInputsFromPlayerInputs(playerInputs);

		CreateVisuals();

		remainingInputs = new(levelInputs);
	}

	void FixedUpdate() {
		if(player.playingback)
			AdvancePlayback();
	}

	void GetInputsFromPlayerInputs(List<PlayerInput> playerInputs) {
		levelInputs = new();
		int currentCount = 0;
		PlayerInput currentInput = playerInputs[0];

		for(int i = 0; i < playerInputs.Count; i++) {
			if(playerInputs[i] == currentInput) {
				currentCount++;
			} else {
				levelInputs.Add((currentInput, currentCount));
				currentInput = playerInputs[i];
				currentCount = 1;
			}
		}
		levelInputs.Add((currentInput, currentCount));
	}

	void CreateVisuals() {
		foreach(Transform child in transform)
			Destroy(child.gameObject);

		for(int i = 0; i < levelInputs.Count; i++) {
			var (input, time) = levelInputs[i];
			GameObject inputAndTime = Instantiate(inputAndTimePrefab, transform);
			inputAndTime.GetComponent<InputAndTime>().Initialize(input, time);
			inputAndTime.GetComponent<RectTransform>().anchoredPosition += i * 120 * Vector2.down;
		}
	}

	void AdvancePlayback() {
		var current = remainingInputs[0];

		if(current.time <= 1) {
			Destroy(transform.GetChild(0).gameObject);
			remainingInputs.RemoveAt(0);

			// move others back up
			foreach(Transform child in transform)
				child.GetComponent<RectTransform>().anchoredPosition += 120 * Vector2.up;

			return;
		}

		current.time--;
		remainingInputs[0] = current;
	}
}