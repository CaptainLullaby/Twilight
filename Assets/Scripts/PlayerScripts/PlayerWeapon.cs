using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Vector2 playerPointer { get; set; }
    public GameObject weapon;
    public GameObject bulletprefab;
    public float speed;
    public SpriteRenderer weaponRenderer;
    public Transform weaponBarrel;

    bool refreshAttack = false;
    public Vector2 dir;


    private void Update()
    {
        dir = (playerPointer - (Vector2)transform.parent.position).normalized;
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

    public void Fire()
    {
        if (refreshAttack)
            return;

        refreshAttack = true;
        GameObject bullet = Instantiate(bulletprefab, weaponBarrel.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = dir * speed;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(1f);
        refreshAttack = false;
    }
}
