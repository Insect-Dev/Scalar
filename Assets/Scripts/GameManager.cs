using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public float worldBarrier;

    [SerializeField] private Tilemap tilemap;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;

        worldBarrier = tilemap.size.y * .25F * -1;
    }
}

