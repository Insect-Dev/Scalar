using TNRD.Autohook;
using UnityEngine;

public class PlatformerNavigator : MonoBehaviour
{
    [Header("References")]

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private LivingEntity livingEntity;

    [Header("Terrain Detectors")]

    [SerializeField]
    private float wallRayLength;

    [SerializeField]
    private EdgeDetector leftDetector;

    [SerializeField]
    private EdgeDetector rightDetector;

    private void Update()
    {
        if ((livingEntity.walkDirection < 0 && !leftDetector.isColliding) ||
            (livingEntity.walkDirection > 0 && !rightDetector.isColliding))
        {
            livingEntity.walkDirection *= -1;
        }

        if (livingEntity.walkDirection != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.up * .5F, livingEntity.walkDirection < 0 ? Vector2.left : Vector2.right, wallRayLength, livingEntity.groundMask);

            if (hit.collider != null)
            {
                livingEntity.walkDirection *= -1;
            }
        }
    }
}

