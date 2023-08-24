using System.Collections;
using UnityEngine;

public class AIAttack: MonoBehaviour
{
    public SpriteRenderer weaponRenderer;
    public GameObject weapon;
    public Animator anim;
    public GameObject bulletprefab;
    public float speed;
    public Transform weaponBarrel;
    [SerializeField] int damage = 1;

    bool attkblk;
    float z;

    public Vector2 PointerPos { get; set; }

    public Transform circleOrigin;
    public float radius;

    public bool isAttacking { get; private set; }

    public void ResetAttack()
    {
        isAttacking = false;
    }

    private void Start()
    {
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();
        z = weapon.transform.localRotation.eulerAngles.z;
    }

    private void Update()
    {
        if (isAttacking)
            return;

        Vector2 dir = (PointerPos - (Vector2)transform.parent.position).normalized;
        transform.right = dir;

        int yScale = 1;
        if (dir.x < 0)
            yScale = -1;
        else if (dir.x > 1)
            yScale = 1;
        
        transform.localScale = new Vector3(1, yScale, 0);

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
            weaponRenderer.sortingOrder = 0;
        else
            weaponRenderer.sortingOrder = 2;
    }

    public void Attack()
    {
        if (attkblk)
            return;
        anim.SetTrigger("isAttack");
        attkblk = true;
        isAttacking = true;
        StartCoroutine(DelayAttack());
    }

    public void Fire()
    {
        if (attkblk)
            return;
        attkblk = true;
        isAttacking = true;
        Vector2 dir = (PointerPos - (Vector2)transform.parent.position).normalized;
        GameObject bullet = Instantiate(bulletprefab, weaponBarrel.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = dir * speed;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(0.3f);
        weapon.transform.localEulerAngles = new Vector3(0, 0, z);
        attkblk = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 pos = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(pos, radius);
    }

    public void DetectCollider()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            Health health;
            if (health = collider.gameObject.GetComponent<Health>())
                health.iHit(damage, transform.parent.gameObject);
        }

    }
}
