using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    void Start()
    {
        health = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0f)
            gameObject.SetActive(false);
    }
}
