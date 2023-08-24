using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private void Update()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Wall" || collision.transform.tag == "Shield")
            Destroy(this.gameObject);


        if (collision.transform.tag == "AI")
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
