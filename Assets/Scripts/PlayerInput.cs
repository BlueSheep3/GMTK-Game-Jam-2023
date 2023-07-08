using System;

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

	public override bool Equals(object obj)
		=> obj is PlayerInput input && input == this;

	public override int GetHashCode()
		=> HashCode.Combine(left, right, jump);

	public static bool operator ==(PlayerInput a, PlayerInput b)
		=> a.left == b.left && a.right == b.right && a.jump == b.jump;

	public static bool operator !=(PlayerInput a, PlayerInput b)
		=> a.left != b.left || a.right != b.right || a.jump != b.jump;
}
