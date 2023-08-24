using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public UnityEvent<Vector2> mousemovement;
    public UnityEvent onAttack;

    [SerializeField] public InputActionReference movement, pointerPos, speedUp, swipe, fire;
    private Rigidbody2D rb2d;

    bool swipeReady = false;
    Vector2 playermovement;

    private float maxSpeed, Accel = 50, deAccel = 100, speed = 0;
    private Vector2 oldinp;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        playermovement = GetMovement();
        mousemovement?.Invoke(GetPointer());

        if (GetSpace() == 1)
            Swipe();

        if (GetAttack() == 1)
            onAttack?.Invoke();
    }

    private void FixedUpdate()
    {
        maxSpeed = 2f;
        if(playermovement.magnitude > 0 && speed >= 0)
        {
            oldinp = playermovement;
            if (GetShift() == 1)
                maxSpeed = 4F;
            speed += Accel * maxSpeed * Time.deltaTime;
        }
        else
            speed -= deAccel * maxSpeed * Time.deltaTime;
        speed = Mathf.Clamp(speed, 0, maxSpeed);
        rb2d.velocity = oldinp * speed;
    }

    private void Swipe()
    {
        if (swipeReady)
            return;

        Vector2 dir = (GetPointer() - (Vector2)transform.position).normalized;
        transform.position = (Vector2)transform.position + dir;
        swipeReady = true;
        StartCoroutine(Delay(2.5f));
    }

    private IEnumerator Delay(float delayval)
    {
        yield return new WaitForSeconds(delayval);
        swipeReady = false;
    }

    public Vector2 GetMovement()
    {
        return movement.action.ReadValue<Vector2>().normalized;
    }

    private Vector2 GetPointer()
    {
        Vector3 mousePos = pointerPos.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public float GetShift()
    {
        return speedUp.action.ReadValue<float>();
    }

    private float GetSpace()
    {
        return swipe.action.ReadValue<float>();
    }

    private float GetAttack()
    {
        return fire.action.ReadValue<float>();
    }
}
