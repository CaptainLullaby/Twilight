using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    private Animator agentAnimator;

    private void Start()
    {
        agentAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 movement = transform.parent.GetComponent<AIMovment>().AgentMovement;

        if (movement.magnitude > 0)
            agentAnimator.SetInteger("isMoving", 1);
        else
            agentAnimator.SetInteger("isMoving", 0);

        if (movement.x < 0)
            transform.localScale = new Vector3(-2, 2, 0);
        else if (movement.x > 0)
            transform.localScale = new Vector3(2, 2, 0);
    }
}
