using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

class Player : MonoBehaviour, IRetryable
{
	public static Player inst;

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
	public GameObject explosionParticles;

	// fields
	bool grounded = false;
	bool recording = false;
	PlayerInput currentInput = new();
	int playbackIndex = 0;
	internal List<PlayerInput> inputs = new();
	internal bool isPlaying = true;
	internal bool playbackHasFinished = false;
	Vector3 startPos;
	internal bool hasWon = false;
	internal int currentLevelId = -1;
	int stepTimer = 0;
	Vector2 prevVelocity;


	void Awake() {
		inst = this;
		startPos = transform.position;

		#if UNITY_EDITOR
		if(Savedata.savefile == null)
			Savedata.Load();
		#endif

		string levelName = SceneManager.GetActiveScene().name;
		if(levelName.StartsWith("Level")) {
			currentLevelId = int.Parse(levelName.Substring(5));
			LoadRecording(currentLevelId);
		}

		if(currentLevelId == 7 || currentLevelId == 8)
			SoundHandler.PlayMusic("SkyPuzzle");
		else
			SoundHandler.PlayMusic("BubbleGumPuzzler");
	}

	void Update() {
		if(recording)
			RecordInputInUpdate();

		#if UNITY_EDITOR
		DebugHotKeys();
		#endif
	}

	void FixedUpdate() {
		bool wasOnGround = grounded;
		grounded = IsGrounded();
		if(!wasOnGround && grounded) {
			SoundHandler.PlaySound("Landing", Mathf.Lerp(0f, 0.75f, -prevVelocity.y / 20f));
		}

		if(recording) {
			DoInput(currentInput);
			RecordInputInFixedUpdate();
		}
		if(isPlaying && !playbackHasFinished) {
			DoInput(inputs[playbackIndex]);
			playbackIndex++;
			if(playbackIndex >= inputs.Count) {
				playbackHasFinished = true;
				Debug.Log("stopped playingback");
			}
		}
		DoMovement();
		UpdateAnimState();

		if(grounded && Mathf.Abs(rb.velocity.x) > 0.2f) {
			stepTimer++;
			if(stepTimer >= 10) {
				stepTimer = 0;
				SoundHandler.PlaySound("Step" + Random.Range(0, 5), 0.25f);
			}
		}
		prevVelocity = rb.velocity;
	}

	void DebugHotKeys() {
		if(Input.GetKeyDown(KeyCode.E)) {
			recording = !recording;
			if(recording)
				inputs.Clear();
			Debug.Log(recording ? "started recording" : "stopped recording");
		}
		if(Input.GetKeyDown(KeyCode.Return)) {
			SaveRecording();
		}
		if(Input.GetKeyDown(KeyCode.P)) {
			StartPlayback();
		}
		if(Input.GetKeyDown(KeyCode.O)) {
			StopPlayback();
		}
	}

	#region: input recording
	void RecordInputInUpdate() {
		currentInput.ClearLeftRight();
		currentInput.right = Input.GetKey(KeyCode.D);
		currentInput.left = Input.GetKey(KeyCode.A);
		if(Input.GetKeyDown(KeyCode.W))
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
		string recording = Resources.Load<TextAsset>($"LevelInputs/{id}")?.text;
		inputs.Clear();
		if(recording == null) return;
		foreach(string line in recording.Split('\n'))
			inputs.Add(new PlayerInput(line));
		Debug.Log($"loaded recording from LevelInputs/{id}.txt");
	}
	#endregion

	#region: movement
	void DoMovement() {
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
			SoundHandler.PlaySound("Jump", 0.65f);
		}

		int xInput = (input.left ? -1 : 0) + (input.right ? 1 : 0);
		newVel.x += xInput * (grounded ? GROUND_ACCEL : AIR_ACCEL);

		UpdateFacingDir(xInput);

		rb.velocity = newVel;
	}

	bool IsGrounded() {
		RaycastHit2D hit = Physics2D.Raycast(rb.position + new Vector2(-0.41f, -0.45f), Vector2.down, 0.2f, 1 << 6);
		if(hit.collider != null)
			return true;
		RaycastHit2D hit2 = Physics2D.Raycast(rb.position + new Vector2(0.38f, -0.45f), Vector2.down, 0.2f, 1 << 6);
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
	void OnCollisionEnter2D(Collision2D collision) {
		OnCollisionStay2D(collision);
	}

	void OnCollisionStay2D(Collision2D collision) {
		CustomRuleTile tile = GetCollisionTileInfo(collision);
		if (tile == null)
			return;

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
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		transform.position = new(-1000, -1000, 0);
		SoundHandler.PlaySound("Explosion", 1);
	}

	public void Retry() {
		transform.position = startPos;
		rb.velocity = Vector2.zero;
		transform.localScale = Vector3.one;
		StopPlayback();
	}
	#endregion

	#region: start / stop
	public void StartPlayback() {
		isPlaying = true;
		playbackHasFinished = false;
		playbackIndex = 0;
	}

	public void StopPlayback() {
		isPlaying = false;
		playbackHasFinished = true;
	}
	#endregion
}

enum AnimState {
	Idle, Run, Jump, Fall
}
