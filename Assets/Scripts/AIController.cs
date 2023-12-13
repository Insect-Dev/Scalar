using TNRD.Autohook;
using UnityEngine;

[RequireComponent(typeof(PlatformerNavigator))]
public class AIController : MonoBehaviour
{
	[Header("References")]

	[SerializeField, AutoHook(ReadOnlyWhenFound = true)]
	private PlatformerNavigator navigator;

    [SerializeField, AutoHook(searchArea: AutoHookSearchArea.Children, ReadOnlyWhenFound = true)]
    private NavigatorPath navigatorPath;

    [Header("Movement")]

    [SerializeField]
    private float pointArrivalTreshold;

    private void Update()
    {
        navigator.targetPos = navigatorPath.currentPoint.pos;

        if (Mathf.Abs(transform.position.x - navigator.targetPos.x) < pointArrivalTreshold)
        {
            StartCoroutine(navigatorPath.currentPoint.OnArrive(navigatorPath));
        }
    }
}

