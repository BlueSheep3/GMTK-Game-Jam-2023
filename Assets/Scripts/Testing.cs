using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Testing : MonoBehaviour
{
    void Start()
    {
        
    }
	
	private Rigidbody2D rb;
	private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
			SetAnim(AnimState.Jump);
		if(Input.GetKeyDown(KeyCode.Alpha2))
			SetAnim(AnimState.Fall);
		if(Input.GetKeyDown(KeyCode.Alpha3))
			SetAnim(AnimState.Run);
		if(Input.GetKeyDown(KeyCode.Alpha4))
			SetAnim(AnimState.Idle);
    }
    

	void SetAnim(AnimState state) {
		Animator anim = GetComponent<Animator>();
		anim.runtimeAnimatorController = Resources.Load("Animations/" + state.ToString()) as RuntimeAnimatorController;
		anim.speed = 0.2f;
	}


	public LayerMask groundLayer; // The layer(s) representing the ground
    public float raycastDistance = 0.1f;

	    private bool IsGrounded()
    {
        // Cast a ray downwards from the Rigidbody's position
        RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, raycastDistance, groundLayer);

        // Check if the ray hit something
        if (hit.collider != null)
        {
            // The Rigidbody is grounded
            return true;
        }

        // The Rigidbody is not grounded
        return false;
    }
}


enum AnimState {
	Idle, Run, Jump, Fall
}
