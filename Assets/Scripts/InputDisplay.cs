using System.Collections.Generic;
using UnityEngine;

class InputDisplay : MonoBehaviour
{
	// fields
	List<(PlayerInput, int)> inputs;

	void Start() {
		// player must have already set up the inputs list
		Player player = GameObject.FindObjectOfType<Player>();
		List<PlayerInput> playerInputs = player.inputs;

		inputs = new();
		int currentCount = 0;
		PlayerInput currentInput = playerInputs[0];

		for(int i = 0; i < playerInputs.Count; i++) {
			if(playerInputs[i] == currentInput) {
				currentCount++;
			} else {
				inputs.Add((currentInput, currentCount));
				currentInput = playerInputs[i];
				currentCount = 1;
			}
		}
		inputs.Add((currentInput, currentCount));
	}
}