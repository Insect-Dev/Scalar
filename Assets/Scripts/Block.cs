using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class Block : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
