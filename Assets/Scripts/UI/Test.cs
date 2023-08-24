using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject map;
    [SerializeField] Transform target;
    [SerializeField] GameObject navPoint;

    [SerializeField] bool test = true;
    
    AstarAlgorithm pathfind;
    int[,] mapArr;

    private void Start()
    {
        mapArr = map.GetComponent<MapGenerator>().GetMapArray();
        pathfind = new AstarAlgorithm(mapArr);
    }

    private void Update()
    {
        if (test == true)
        {
            GetPath(target);
            test = false;
        }
    }

    private void GetPath(Transform targetPos)
    {
        Vector3 start;
        Vector3 end;
        if (Vector3.Distance(transform.position, Vector3.zero) < Vector3.Distance(targetPos.position, Vector3.zero))
        {
            start = transform.position;
            end = targetPos.position;
        }
        else
        {
            start = targetPos.position;
            end = transform.position;
        }

        List<Vector3> pathList = pathfind.FindPath(start, end);

        if (pathList == null)
        {
            Debug.Log("travel not possible");
            return;
        }

        foreach (var path in pathList)
            Instantiate(navPoint, path, Quaternion.identity);
    }
}
