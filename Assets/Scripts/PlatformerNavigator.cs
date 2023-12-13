using TNRD.Autohook;
using UnityEngine;

public class PlatformerNavigator : MonoBehaviour
{
    [Header("References")]

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private LivingEntity livingEntity;

    [Header("Navigator")]

	public Vector2 targetPos;

    [SerializeField]
    private float stoppingTreshold;

    [SerializeField]
    private float walkingTreshold;

    [Header("Terrain Detectors")]

    [SerializeField]
    private EdgeDetector leftDetector;

    [SerializeField]
    private EdgeDetector rightDetector;

    private void Update()
    {
        Vector2 selfPos = (Vector2)transform.position;

        float treshold = livingEntity.walkDirection == 0 ? stoppingTreshold : walkingTreshold;

        if (Mathf.Abs(selfPos.x - targetPos.x) > treshold)
        {
            livingEntity.walkDirection = targetPos.x < selfPos.x ? -1 : 1;
        } else
        {
            livingEntity.walkDirection = 0;
        }

        if (livingEntity.walkDirection < 0 && !leftDetector.isColliding)
        {
            livingEntity.Jump();
        }

        if (livingEntity.walkDirection > 0 && !rightDetector.isColliding)
        {
            livingEntity.Jump();
        }

        if (livingEntity.walkDirection != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(selfPos + Vector2.up * .5F, livingEntity.walkDirection < 0 ? Vector2.left : Vector2.right, 1, livingEntity.groundMask);

            if (hit.collider != null)
            {
                livingEntity.Jump();
            }
        }
    }
}

