using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KnockbackFeedback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    
    [SerializeField] private float str = 5f, delay = 0.15f;

    public UnityEvent OnBegin, OnDone;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    public void Knockback(GameObject target)
    {
        StopAllCoroutines();
        OnBegin?.Invoke();
        Vector2 dir = (transform.position - target.transform.position).normalized;
        rb2d.AddForce(dir * str, ForceMode2D.Impulse);
        StartCoroutine(Reset());

    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb2d.velocity = Vector3.zero;
        OnDone?.Invoke();
    }
}
