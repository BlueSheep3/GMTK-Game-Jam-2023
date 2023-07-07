using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Testing : MonoBehaviour
{
	public RuntimeAnimatorController[] animators = new RuntimeAnimatorController[4];

    void Start()
    {
        
    }

    void Update()
    {
		if(Input.GetKeyDown(KeyCode.Alpha1))
			SetAnim(AnimState.Idle);
		if(Input.GetKeyDown(KeyCode.Alpha2))
			SetAnim(AnimState.Run);
        if(Input.GetKeyDown(KeyCode.Alpha3))
			SetAnim(AnimState.Jump);
		if(Input.GetKeyDown(KeyCode.Alpha4))
			SetAnim(AnimState.Fall);
    }
    

	void SetAnim(AnimState state) {
		Animator anim = GetComponent<Animator>();
		anim.runtimeAnimatorController = animators[(int)state];
		SetAnimationSpeed(state, anim);
	}

	void SetAnimationSpeed(AnimState state, Animator anim) {
		switch(state) {
			case AnimState.Idle:
				anim.speed = 0.1f;
				break;
			case AnimState.Run:
				anim.speed = 0.6f;
				break;
			case AnimState.Jump:
				anim.speed = 0.2f;
				break;
			case AnimState.Fall:
				anim.speed = 0.1f;
				break;
		}
	

	}
}
