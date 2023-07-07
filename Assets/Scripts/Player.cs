using System.Collections.Generic;
using UnityEngine;

class Player : MonoBehaviour
{
	bool recording = false;
	List<Input> inputs = new();


	void Update() {
		Input input = new Input();
		if(Input.GetKeyDown(KeyCode.D))
			input.right = true;
		if(Input.GetKeyDown(KeyCode.A))
			input.left = true;
		if(Input.GetKeyDown(KeyCode.Space))
			input.jump = true;

		if(recording)
			inputs.Add(input);
		else
			ApplyInput(input);
	}
	
	

	void FixedUpdate() {

	}

	void SaveRecording() {

	}
}

class Input {
	bool left = false;
	bool right = false;
	bool jump = false;
}