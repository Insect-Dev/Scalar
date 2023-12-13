using TNRD.Autohook;
using UnityEngine;

public class EdgeDetector : MonoBehaviour
{
	public bool isColliding = true;

    [SerializeField, AutoHook]
    private CircleCollider2D circleCollider;

    //private void Update()
    //{
    //    isColliding = circleCollider.GetContacts(null as Collider2D[]) != 0;
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    isColliding = true;
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    isColliding = false;
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }
}

