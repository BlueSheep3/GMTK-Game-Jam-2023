
using UnityEngine;

class Spring : MonoBehaviour
{
	public UnityEngine.RuntimeAnimatorController anim;
	
	void Start() {
		transform.GetComponent<Animator>().speed = 0.7f;
	}

	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag != "Player")
			return;
		Vector2 force = transform.up * 20;
		if(force.x != 0)
			Player.inst.rb.velocity = new Vector2(force.x * 2, Player.inst.rb.velocity.y + 5);
		if(force.y != 0)
			Player.inst.rb.velocity = new Vector2(Player.inst.rb.velocity.x, force.y);
		SwitchAnimation();
		transform.GetComponent<BoxCollider2D>().enabled = false;
		Invoke("EnableCollider", 0.5f);
	}

	void EnableCollider() {
		SwitchAnimation();
		transform.GetComponent<BoxCollider2D>().enabled = true;
	}

	void SwitchAnimation() {
		UnityEngine.RuntimeAnimatorController temp = transform.GetComponent<Animator>().runtimeAnimatorController;
		transform.GetComponent<Animator>().runtimeAnimatorController = anim;
		anim = temp;
	}
}