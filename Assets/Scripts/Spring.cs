
using UnityEngine;

class Spring : MonoBehaviour
{
	public UnityEngine.RuntimeAnimatorController anim;
	

	void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log("spring");
		if (collision.gameObject.tag != "Player")
			return;
		Vector2 force = transform.up * 5;
		collision.gameObject.GetComponent<Rigidbody2D>().AddForce(force);
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