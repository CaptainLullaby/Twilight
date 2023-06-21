using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float speed = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        transform.Translate(new Vector2(x, z) * Time.deltaTime * 2f * speed);  
        
    }
}
