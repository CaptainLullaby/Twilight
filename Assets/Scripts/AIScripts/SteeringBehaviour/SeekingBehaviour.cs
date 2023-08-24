using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekingBehaviour : SteeringBehaviour
{
    [SerializeField] private float targetReachedThreshold = 0.5f;
    [SerializeField] private bool showGizmos = true;

    bool reachedLastTargt = true;

    private Vector2 TargetPosCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {

        if(reachedLastTargt)
        {
            if (aiData.targets == null || aiData.targets.Count <= 0)
            {
                aiData.currentTarget = null;
                return (danger, interest);
            }
            else
            {
                reachedLastTargt = false;
                aiData.currentTarget = aiData.targets.OrderBy(target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
            }
        }

        if (aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
            TargetPosCached = aiData.currentTarget.position;

        if(Vector2.Distance(transform.position, TargetPosCached) < targetReachedThreshold)
        {
            reachedLastTargt = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }

        Vector2 directionToTarget = (TargetPosCached - (Vector2)transform.position).normalized;
        for(int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget, Directions.eightdirections[i]);
            if(result > 0 )
            {
                float value = result;
                if(value > interest[i])
                    interest[i] = value;
            }
        }
        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if(showGizmos == false) 
            return;

        Gizmos.DrawSphere(TargetPosCached, 0.2f);

        if(Application.isPlaying && interestsTemp != null)
        {
            for (int i = 0; i < interestsTemp.Length; i++)
                Gizmos.DrawRay(transform.position, Directions.eightdirections[i] * interestsTemp[i]);
            if(reachedLastTargt == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(TargetPosCached, 0.1f);
            }
        }
    }
}