using UnityEngine;
using System.Collections.Generic;
using NPBehave;

[RequireComponent(typeof(Health))]
[RequireComponent (typeof(Targeting))]
[RequireComponent (typeof(Movement))]
public class BaseAI : MonoBehaviour
{
    public int behaviour;

    Movement mov;
    Health health;
    //Targeting detection;

    Root tree;
    Blackboard blackboard;
    Debugger debugger;

    float targetdist;
    float inverserot;
    GameObject target;

    public GameObject map;
    private MapGenerator gen;

    private List<Vector2> path;

    void Start()
    {
        health = GetComponent<Health>();
        mov = GetComponent<Movement>();
        target = GameObject.FindGameObjectWithTag("PlayerChar");
        map = GameObject.FindGameObjectWithTag("Map");
        gen = map.GetComponent<MapGenerator>();

        //detection = GetComponent<Targeting>();

        //GetDataFromWorld();

        Clock clock = new Clock();
        blackboard = new Blackboard(clock);
        debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        StartTree();
    }

    private void Update()
    {
        //GetDataFromWorld ();
    }

    //---------------------------------------
    private void SerachAllRooms()
    {
        path = new List<Vector2>();
        Vector3 center;
        List<BoundsInt> rooms = gen.GetRooms();
        Vector2 size = gen.GetMapSize();
        foreach (var room in rooms)
        {
            center = new Vector3(room.center.x - size.x / 2 + 0.5f, room.center.y - size.y / 2 + 0.5f, 0f);
            path.Add(center);
        }
    }

    //---------------------------------------
    //Node Definitions

    private Node Move(float speed = 1f)
    {
        return new Action(() => mov.Move(speed));
    }

    private Node Rotate(float speed = 1f)
    {
        return new Action(() => mov.Rotate(speed));
    }

    private Node Follow(Vector3 target)
    {
        return new Action(() => mov.Follow(target));
    }
    
    private Node IdleWalk()
    {
        return new Action(() => mov.RandomWalk());
    }
    //---------------------------------------
    //Behaviour tree

    private Root CreateBehaviourTree()
    {
        switch (behaviour)
        {
            case 0: return Pursue();
            case 1: return Alert();
            default: return Idle();
        }
    }

    private void StartTree()
    {
        Root t = CreateBehaviourTree();
        if (tree != null)
            tree.Stop();
        tree = t;
        debugger.BehaviorTree = tree;
        tree.Start();
    }
    //---------------------------------------
    //Behaviour tree perception
    private void UpdatePerception()
    {
        Vector3 pos = this.transform.InverseTransformPoint(target.transform.position);
        blackboard["detectionRange"] = pos.magnitude;
        blackboard["inFront"] = pos.x > 0;
        blackboard["isPlayer"] = target.GetComponent<Health>().health <= 0;
        blackboard["offCenter"] = pos.x;
    }

    //---------------------------------------
    //All basic behaviours as node fucntions
    private Root Idle()
    {
        return new Root(blackboard, new Service(1f, UpdatePerception, new Sequence(IdleWalk(), new Wait(UnityEngine.Random.Range(0, 1)) ) ));
    }

    private Root Alert()
    {
        return null;
    }

    private Root Pursue()
    {
        return null;
    }
}
