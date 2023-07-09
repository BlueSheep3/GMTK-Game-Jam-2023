using UnityEngine;

class Goal : MonoBehaviour
{
	// refs
	public GameObject winPoppup;

	void OnTriggerEnter2D(Collider2D other) {
		if(other.TryGetComponent<Player>(out Player player)) {
			if(Player.inst.currentLevelId > Savedata.savefile.maxLevelCompleted)
				Savedata.savefile.maxLevelCompleted = Player.inst.currentLevelId;
			Savedata.Save();
			winPoppup.SetActive(true);
			player.playbackHasFinished = true;
			player.hasWon = true;
		}
	}
}