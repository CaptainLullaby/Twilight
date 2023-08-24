using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField] float radius = 2f, agentColliderSize = 0.6f;
    [SerializeField] private bool showGizmo = true;
    float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        foreach(Collider2D obstacle in aiData.obstacles)
        {
            if(obstacle == null)
                continue;

            Vector2 directionToObstacle = obstacle.ClosestPoint(transform.position) - (Vector2)transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            float weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;
            Vector2 dir = directionToObstacle.normalized;

            for (int i = 0; i < Directions.eightdirections.Count; i++)
            {
                float result = Vector2.Dot(dir, Directions.eightdirections[i]);
                float value = result * weight;
                if (value > danger[i])
                    danger[i] = value;
            }
        }
        dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo == false)
            return;

        if(Application.isPlaying && dangersResultTemp!=null)
        {
            Gizmos.color = Color.red;
            for(int i = 0; i<dangersResultTemp.Length;i++)
                Gizmos.DrawRay(transform.position, Directions.eightdirections[i] * dangersResultTemp[i]);
        }
        else
        {
            Gizmos.color= Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}