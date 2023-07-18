using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    public GameObject target;
    
    private float dist;
    private float rot;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("PlayerChar");
    }

    private void Update()
    {
        dist = Vector3.Distance(target.transform.position, transform.position);
        //rot = Vector3.Angle(target.transform.forward, transform.forward);
        Vector3 heading = transform.InverseTransformPoint(target.transform.position);
        rot = heading.x;
        //Debug.Log("Target Distance" + dist);
        //Debug.Log("Target Angle" + rot);
    }

    public float GetDistancetoPlayer()
    {
        return dist;
    }

    public float GetInversepositiontoPlayer()
    {
        return rot;
    }

    public GameObject GetTarget()
    {
        return target;
    }
}
