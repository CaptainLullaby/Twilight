using UnityEngine;

public class AIObstacleDetector : AITargeting
{
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] private bool showGizmos = true;

    Collider2D[] colliders;

    public override void Detect(AIData aiData)
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);
        aiData.obstacles = colliders;
    }

    private void OnDrawGizmos()
    {
        if(showGizmos == false)
            return;

        //Debug.Log("Pass");
        if(Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider2D collider in colliders)
                Gizmos.DrawSphere(collider.transform.position, 0.2f);
        }
    }
}
