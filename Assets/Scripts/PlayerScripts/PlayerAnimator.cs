using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Vector2 playermov = transform.parent.GetComponent<PlayerController>().GetMovement();
        float speedUp = transform.parent.GetComponent<PlayerController>().GetShift();
        if (playermov.magnitude !=0)
        {
            if (speedUp > 0)
                anim.SetInteger("isMoving", 2);
            else
                anim.SetInteger("isMoving", 1);
        }
        else
            anim.SetInteger("isMoving", 0);

        if (playermov.x < 0)
            transform.localScale = new Vector3(-2, 2, 0);
        else if (playermov.x > 0)
            transform.localScale = new Vector3(2, 2, 0);

    }
}
