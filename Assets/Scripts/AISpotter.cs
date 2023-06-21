using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;

[RequireComponent(typeof(Health))]
[RequireComponent (typeof(Targeting))]
[RequireComponent (typeof(Movement))]
public class AISpotter : MonoBehaviour
{
    public int behaviour;
    public GameObject player;

    Movement mov;
    Health health;
    Targeting targetchar;

    Root tree;
    Blackboard blackboard;
    Debugger debugger;

    private float targethealth;
    Vector3 pos;
    Vector3 targetdist;
    GameObject target;

    void Start()
    {
        health = GetComponent<Health>();
        mov = GetComponent<Movement>();
        targetchar = GetComponent<Targeting>();
        player = GameObject.FindGameObjectWithTag("PlayerChar");
        //getdata();

        Clock clock = new Clock();
        blackboard = new Blackboard(clock);
        debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        StartTree();
    }

    private void Update()
    {
        //getdata();
        pos = player.transform.position;
        targethealth = player.GetComponent<Health>().health;
        targetdist = this.transform.InverseTransformDirection(pos);
    }

    //---------------------------------------
    //Data from detection andd others
    private void getdata()
    {
        if (targetchar.gettarget() != null)
        {
            target = targetchar.gettarget();
            targethealth = targetchar.gettarget().GetComponent<Health>().health;
            pos = targetchar.gettarget().transform.position;
            targetdist = this.transform.InverseTransformDirection(pos);
        }
        else
        {
            target = null;
            pos = Vector3.zero;
            targetdist = Vector3.zero;
            targethealth = 0f;
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
        blackboard["target"] = pos;
        blackboard["distancetotarget"] = Vector3.Distance(transform.position, pos);
        blackboard["targethealth"] = targethealth;
        blackboard["isTargetinFront"] = targetdist.normalized.z > 0;
        blackboard["isTargetonRight"] = targetdist.normalized.x;
        blackboard["offCentre"] = targetdist.normalized.x;
    }

    //---------------------------------------
    //All basic behaviours as node fucntions
    private Root Alert()
    {
        return new Root(blackboard, new Service(0.1f, UpdatePerception, new Selector(
                new BlackboardCondition("isTargetinFront", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, new Selector(
                    new BlackboardCondition("enemyDistance", Operator.IS_GREATER, 1f, Stops.IMMEDIATE_RESTART, Move()),
                    new BlackboardCondition("offCentre", Operator.IS_GREATER, 0.01f, Stops.IMMEDIATE_RESTART, Rotate((float)blackboard["offCentre"])),
                    new BlackboardCondition("offCentre", Operator.IS_SMALLER, -0.01f, Stops.IMMEDIATE_RESTART, Rotate(-1 * (float)blackboard["offCentre"]))
                    //, checkaround?
                    )),
                new BlackboardCondition("isTargetonRight", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, Rotate(2f)),
                new BlackboardCondition("isTargetonRight", Operator.IS_NOT_EQUAL, true, Stops.IMMEDIATE_RESTART, Rotate(-2f))
                )   ));
    }
    private Root Pursue()
        {
            return new Root(blackboard, new Service(0.1f, UpdatePerception, new Selector(
                new BlackboardCondition("isTargetinFront", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, new Selector(
                    new BlackboardCondition("enemyDistance", Operator.IS_GREATER, 1f, Stops.IMMEDIATE_RESTART, Move()),
                    new BlackboardCondition("offCentre", Operator.IS_GREATER, 0.01f, Stops.IMMEDIATE_RESTART, Rotate((float)blackboard["offCentre"])),
                    new BlackboardCondition("offCentre", Operator.IS_SMALLER, -0.01f, Stops.IMMEDIATE_RESTART, Rotate(-1 * (float)blackboard["offCentre"])),
                    Follow((Vector3)blackboard["target"])
                    )),
                new BlackboardCondition("isTargetonRight", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, Rotate(2f)),
                new BlackboardCondition("isTargetonRight", Operator.IS_NOT_EQUAL, true, Stops.IMMEDIATE_RESTART, Rotate(-2f))
                )   ));
        }
    private Root Idle()
    {
        return new Root(blackboard, new Service(0.1f, UpdatePerception,new Sequence(IdleWalk(), new Wait(UnityEngine.Random.Range(0f, 1f))) ));
    }
}
