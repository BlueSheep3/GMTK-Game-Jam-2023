using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
	public Animator animator;
	public Transform visualTransform;

	// refs
	public RuntimeAnimatorController[] animatorControllers = new RuntimeAnimatorController[4];

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

		#if UNITY_EDITOR
			DebugHotKeys();
		#endif
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
		UpdateAnimState();
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

		int xInput = (input.left ? -1 : 0) + (input.right ? 1 : 0);
		newVel.x += xInput * (grounded ? GROUND_ACCEL : AIR_ACCEL);

		UpdateFacingDir(xInput);

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

	void UpdateFacingDir(int xInput) {
		if(xInput == 0) return;
		Vector3 scale = visualTransform.localScale;
		scale.x = xInput;
		visualTransform.localScale = scale;
	}
	#endregion

	#region: animation
	void UpdateAnimState() {
		AnimState state = AnimState.Idle;
		if(Mathf.Abs(rb.velocity.x) > 0.05f)
			state = AnimState.Run;
		if(!grounded)
			state = rb.velocity.y > 0 ? AnimState.Jump : AnimState.Fall;
		SetAnim(state);
	}

	void SetAnim(AnimState state) {
		animator.runtimeAnimatorController = animatorControllers[(int)state];
		SetAnimationSpeed(state);
	}

	void SetAnimationSpeed(AnimState state) {
		animator.speed = state switch {
			AnimState.Idle => 0.1f,
			AnimState.Run => 0.6f,
			AnimState.Jump => 0.2f,
			AnimState.Fall => 0.1f,
			_ => throw new System.Exception("invalid anim state")
		};
	}
	#endregion

	#region: collision
	void OnCollisionStay2D(Collision2D collision) {
		CustomRuleTile tile = GetCollisionTileInfo(collision);

		switch(tile?.type) {
			case CustomRuleTile.Type.Deadly: Die(); break;
			case null: Debug.LogWarning("trigger tile was null"); break;
		}
	}

	CustomRuleTile GetCollisionTileInfo(Collision2D collision) {
		if(!collision.collider.TryGetComponent<Tilemap>(out var map)) return null;
		var grid = map.layoutGrid;

		// Find the coordinates of the tile we hit.
		var contact = collision.GetContact(collision.contactCount - 1);
		Vector3 contactPoint = contact.point - 0.05f * contact.normal;
		Vector3 gridPosition = grid.transform.InverseTransformPoint(contactPoint);
		Vector3Int cell = grid.LocalToCell(gridPosition);

		// extract the tile and check if it's ice
		var tile = map.GetTile(cell);
		if(tile is not CustomRuleTile tile2) return null;
		return tile2;
	}
	#endregion

	#region: death
	void Die() {
		// TODO
		Debug.Log("died");
	}
	#endregion
}

enum AnimState {
	Idle, Run, Jump, Fall
}
