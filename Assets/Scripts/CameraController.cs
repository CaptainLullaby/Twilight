using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerChar");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(new Vector3(transform.position.x, transform.position.y, -10f), new Vector3(player.transform.position.x, player.transform.position.y, -10f), 5f * Time.deltaTime);
    }
}
