using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIBulletScript : MonoBehaviour
{
    private void Update()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "PlayerChar")
        {
            collision.gameObject.GetComponent<Health>().iHit(2, transform.gameObject);
            Destroy(this.gameObject);
        }

        if (collision.transform.tag == "Wall")
            Destroy(this.gameObject);
    }
}
