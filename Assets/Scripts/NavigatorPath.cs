using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;

public class NavigatorPath : MonoBehaviour
{
    public List<NavigatorPoint> path = new();

    public int currentPointIndex = 0;

    public NavigatorPoint currentPoint { get { return path[currentPointIndex]; } }

    public LoopMode loopMode;

    public bool goesForward;

    private void OnDrawGizmos()
    {
        if (path.Count == 0) return;

        Vector2 prev = path[0].pos;

        Gizmos.color = currentPoint.GetColor(currentPointIndex == 0);
        Gizmos.DrawSphere(prev, .1F);

        for (int i = 1; i < path.Count; i++)
        {
            NavigatorPoint point = path[i];

            Gizmos.color = currentPoint.GetColor(currentPointIndex == i);
            Gizmos.DrawSphere(point.pos, .1F);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(prev, point.pos);
        }
    }
}
[Serializable]
public class NavigatorPoint
{
    public Vector2 pos;

    public float pauseTime;

    public bool arrived;

    public virtual IEnumerator OnArrive(NavigatorPath navigatorPath)
    {
        if (arrived) yield return null;

        arrived = true;

        Debug.Log("Arrived at the Navigation Point");

        if (pauseTime > 0)
        {
            yield return new WaitForSeconds(pauseTime);
        }

        if (navigatorPath.loopMode == LoopMode.FromStart)
        {
            navigatorPath.currentPointIndex = (navigatorPath.currentPointIndex + 1) % navigatorPath.path.Count - 1;
        }

        //if (navigatorPath.loopMode == LoopMode.Bounce)
        //{
        //    if (navigatorPath.goesForward && ((navigatorPath.currentPointIndex + 1) == navigatorPath.path.Count))
        //    {
        //        navigatorPath.goesForward = false;
        //    }
        //    else if (!navigatorPath.goesForward && navigatorPath.currentPointIndex == 0)
        //    {
        //        navigatorPath.goesForward = true;
        //    }

        //    navigatorPath.currentPointIndex += navigatorPath.goesForward ? 1 : -1;
        //}

        yield return null;
    }

    public Color GetColor(bool isCurrent)
    {
        return isCurrent ? Color.blue : Color.cyan;
    }
}

public enum LoopMode
{
    FromStart,
    Bounce
}
