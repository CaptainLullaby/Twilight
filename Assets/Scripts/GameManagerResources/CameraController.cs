using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector2 dir;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerChar")?.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
            return;

        dir = player.GetComponentInChildren<PlayerWeapon>().dir;

        Vector3 currentPos = new Vector3(transform.position.x, transform.position.y, -10f);
        Vector3 newPos = new Vector3(player.position.x + dir.x, player.position.y + dir.y, -10f);
        transform.position = Vector3.Slerp(currentPos, newPos, 5f * Time.deltaTime);
    }
}
