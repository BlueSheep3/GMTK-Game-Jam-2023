using System.Collections.Generic;
using UnityEngine;

class Player : MonoBehaviour
{
	// constants
	const float JUMP_STRENGTH = 16f;
	const float GROUND_ACCEL = 2.7f;
	const float AIR_ACCEL = 1.5f;
	const float FRICTION = 0.75f;
	const float DRAG = 0.86f;
	const float GRAVITY = -1f;

	// self refs
	public Rigidbody2D rb;
	public SpriteRenderer spriteRenderer;

	// fields
	bool grounded = false;
	bool playingback = false;
	bool recording = false;
	PlayerInput currentInput = new();
	List<PlayerInput> inputs = new();
	int playbackIndex = 0;


	void Update() {
		if(recording)
			RecordInputInUpdate();

		DebugHotKeys();
	}

	void FixedUpdate() {
		grounded = IsGrounded();

		if(recording) {
			DoInput(currentInput);
			RecordInputInFixedUpdate();
		}
		if(playingback) {
			DoInput(inputs[playbackIndex]);
			playbackIndex++;
			if(playbackIndex >= inputs.Count) {
				playingback = false;
				Debug.Log("stopped playingback");
			}
		}
		DoMovement();
	}

	void DebugHotKeys() {
		if(Input.GetKeyDown(KeyCode.R)) {
			recording = !recording;
			if(recording)
				inputs.Clear();
			Debug.Log(recording ? "started recording" : "stopped recording");
		}
		if(Input.GetKeyDown(KeyCode.Return)) {
			SaveRecording();
		}
		if(Input.GetKeyDown(KeyCode.P)) {
			playingback = !playingback;
			playbackIndex = 0;
			Debug.Log(playingback ? "started playingback" : "stopped playingback");
		}
		if(Input.GetKeyDown(KeyCode.Alpha0)) LoadRecording(0);
		if(Input.GetKeyDown(KeyCode.Alpha1)) LoadRecording(1);
		if(Input.GetKeyDown(KeyCode.Alpha2)) LoadRecording(2);
		if(Input.GetKeyDown(KeyCode.Alpha3)) LoadRecording(3);
		if(Input.GetKeyDown(KeyCode.Alpha4)) LoadRecording(4);
		if(Input.GetKeyDown(KeyCode.Alpha5)) LoadRecording(5);
		if(Input.GetKeyDown(KeyCode.Alpha6)) LoadRecording(6);
	}

	#region: input recording
	void RecordInputInUpdate() {
		currentInput.ClearLeftRight();
		currentInput.right = Input.GetKey(KeyCode.D);
		currentInput.left = Input.GetKey(KeyCode.A);
		if(Input.GetKeyDown(KeyCode.Space))
			currentInput.jump = true;
	}

	void RecordInputInFixedUpdate() {
		inputs.Add(currentInput);
		currentInput.ClearJump();
	}

	void SaveRecording() {
		string path = "Assets/Resources/Inputs.txt";
		string output = string.Join("\n", inputs);
		System.IO.File.WriteAllText(path, output);
		Debug.Log("saved recording to " + path);
	}

	void LoadRecording(int id) {
		string recording = Resources.Load<TextAsset>($"Inputs{id}").text;
		inputs.Clear();
		foreach(string line in recording.Split('\n'))
			inputs.Add(new PlayerInput(line));
		Debug.Log($"loaded recording from Inputs{id}.txt");
	}
	#endregion

	#region: movement
	void DoMovement() {
		// temporarily just change sr color
		// Color color = Color.black;
		// color.r = grounded ? 1 : 0;
		// spriteRenderer.color = color;

		Vector2 newVel = rb.velocity;

		newVel.x *= grounded ? FRICTION : DRAG;

		newVel.y += GRAVITY;

		rb.velocity = newVel;
	}

	void DoInput(PlayerInput input) {
		Vector2 newVel = rb.velocity;

		if(grounded && input.jump) {
			newVel.y = JUMP_STRENGTH;
			grounded = false;
		}

		float x = (input.left ? -1 : 0) + (input.right ? 1 : 0);
		newVel.x += x * (grounded ? GROUND_ACCEL : AIR_ACCEL);

		rb.velocity = newVel;
	}

	bool IsGrounded() {
		RaycastHit2D hit = Physics2D.Raycast(rb.position + new Vector2(-0.4f, -0.45f), Vector2.down, 0.2f, 1 << 6);
		if(hit.collider != null)
			return true;
		RaycastHit2D hit2 = Physics2D.Raycast(rb.position + new Vector2(0.35f, -0.45f), Vector2.down, 0.2f, 1 << 6);
		if(hit2.collider != null)
			return true;
		return false;
	}
	#endregion
}
