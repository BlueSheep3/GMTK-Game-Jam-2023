struct PlayerInput
{
	public bool left;
	public bool right;
	public bool jump;

	public PlayerInput(string input) {
		left = input[0] == 'l';
		right = input[1] == 'r';
		jump = input[2] == 'j';
	}

	public void ClearLeftRight() {
		left = false;
		right = false;
	}

	public void ClearJump() {
		jump = false;
	}

	public override string ToString()
		=> (left ? "l" : "-") + (right ? "r" : "-") + (jump ? "j" : "-");
}