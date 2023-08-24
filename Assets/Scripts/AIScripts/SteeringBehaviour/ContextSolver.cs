using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField] private bool showGizmos = true;
    Vector2 resultDir = Vector2.zero;
    private float rayLength = 1;

    public Vector2 GetDir(List<SteeringBehaviour> sb, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach (SteeringBehaviour s in sb)
            (danger, interest) = s.GetSteering(danger, interest, aiData);

        for (int i = 0; i < interest.Length; i++)
            interest[i] = interest[i] - danger[i];

        for (int i = 0; i < interest.Length; i++)
            resultDir += Directions.eightdirections[i] * interest[i];
        resultDir.Normalize();

        return resultDir;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDir * rayLength);
        }
    }
}
