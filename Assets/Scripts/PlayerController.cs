using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private int mov;

    float X;
    float Y;

    private void Start()
    {
        animator = GetComponent<Animator>();
        X = this.transform.localScale.x;
        Y = this.transform.localScale.y;
    }

    void Update()
    {
        //Movement
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float speed = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        transform.Translate(new Vector2(x, z) * Time.deltaTime * 2f * speed);

        //Animation Controll
        if (x != 0 || z != 0)
        {
            if (speed > 1f)
                mov = 2;
            else
                mov = 1;
        }
        else
            mov = 0;
        animator.SetInteger("isMoving", mov);

        if (x > 0)
            transform.localScale = new Vector3(X, Y, 0);
        if (x < 0)
            transform.localScale = new Vector3(-X, Y, 0);
    }
}
