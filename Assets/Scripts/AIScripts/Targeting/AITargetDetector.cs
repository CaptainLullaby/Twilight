using System.Collections.Generic;
using UnityEngine;

public class AITargetDetector : AITargeting
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private LayerMask obstacleLayerMask, playerLayerMask;
    [SerializeField] private bool showGizmos = false;

    private List<Transform> targetColliders;

    public override void Detect(AIData aiData)
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayerMask);
        if (playerCollider != null)
        {
            Vector2 dir = (playerCollider.transform.position - transform.position).normalized;
            //Debug.Log(dir);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRange, obstacleLayerMask);
            if (hit.collider != null && (playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
            {
                Debug.DrawLine(transform.position, hit.transform.position, Color.magenta);
                //Debug.Log(playerCollider.transform.position);
                targetColliders = new List<Transform>() { playerCollider.transform };
            }
            else
                targetColliders = null;
        }
        else
            targetColliders = null;

        aiData.targets = targetColliders;
    }

    private void OnDrawGizmos()
    {
        if(showGizmos == false) 
            return;

        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (targetColliders == null)
            return;

        Gizmos.color = Color.magenta;
        foreach (var collider in targetColliders)
            Gizmos.DrawSphere(collider.position, 0.3f);
    }
}
