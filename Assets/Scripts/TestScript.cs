using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private AstarAlgorithm pathfind;
    public GameObject target;
    public GameObject duplicate;

    private List<Vector3> path;
    List<GameObject> pathList;

    // Start is called before the first frame update
    void Start()
    {
        pathfind = new AstarAlgorithm(100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            SetPath();
    }

    public void SetPath()
    {
        path = new List<Vector3>();
        pathList = new List<GameObject>();
        path = pathfind.FindPath(transform.position, target.transform.position);
        foreach (Vector3 p in path)
        {
            Debug.Log(p);
            pathList.Add(Instantiate(duplicate, p,Quaternion.identity));
        }
    }

}
