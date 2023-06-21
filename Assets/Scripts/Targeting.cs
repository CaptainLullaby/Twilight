using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public GameObject target;

    private Vector3 pos;
    private float dist;
    private float rot;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PlayerChar");
    }

    private void Update()
    {
        pos = target.transform.position;
        dist = Vector3.Distance(target.transform.position, transform.position);
        rot = Vector3.Angle(target.transform.forward, transform.forward);
        //Debug.Log("Target Distance" + dist);
        //Debug.Log("Target Angle" + rot);
    }

    public GameObject gettarget()
    {
        if (dist <= 10f)
            return target;
        else
            return null;
    }
}
