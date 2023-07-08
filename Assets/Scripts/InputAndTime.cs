using UnityEngine;
using TMPro;
using UnityEngine.UI;

class InputAndTime : MonoBehaviour
{
	// self refs
	public Image leftArrow;
	public Image upArrow;
	public Image rightArrow;
	public TMP_Text timeText;

	public void Initialize(PlayerInput input, int time) {
		if(input.left) leftArrow.color = new(0, 1, 0, 0.5f);
		if(input.jump) upArrow.color = new(0, 1, 0, 0.5f);
		if(input.right) rightArrow.color = new(0, 1, 0, 0.5f);
		timeText.text = time.ToString();
	}
}