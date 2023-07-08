using UnityEngine;

class Goal : MonoBehaviour
{
	// refs
	public GameObject winPoppup;

	void OnTriggerEnter2D(Collider2D other) {
		if(other.TryGetComponent<Player>(out Player player)) {
			Savedata.Save();
			winPoppup.SetActive(true);
			player.playbackHasFinished = true;
			player.hasWon = true;
		}
	}
}