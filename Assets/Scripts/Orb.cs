using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TNRD.Autohook;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Orb : MonoBehaviour
{
    [Header("Sprites")]

    [SerializeField] private Sprite minusOrb;
    [SerializeField] private Sprite plusOrb;

    [Header("Orb")]

    [SerializeField] private OrbType type;

    [SerializeField] private bool thrown;

    [SerializeField] private LayerMask blockMask;

    [Header("References")]

    [SerializeField, AutoHook(searchArea: AutoHookSearchArea.Parent, ReadOnlyWhenFound = true)]
    private PlayerController player;

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private SpriteRenderer spriteRenderer;

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private Collider2D coll;

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private Rigidbody2D rb;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Trajectory Plot")]

    public bool drawTrajectory = false;

    [SerializeField] private Texture2D lineTexture;

    [Range(0.05F, 1)]
    [SerializeField] private float timestep = 0.1F;

    [SerializeField] private float maxTime = 3;

    [Header("Throwing")]

    [SerializeField] private float forceScale;

    [SerializeField] private float throwAngle;

    [SerializeField] private float throwForce;

    [SerializeField] private LayerMask groundMask;

    private void Update()
    {
        if (thrown)
        {
            rb.WakeUp();
        }
        else
        {
            rb.Sleep();
            float height = player.transform.position.y + 1.5F;

            height -= (1.0f / 16) * player.orbPixelOffset;

            transform.position = new Vector2(player.transform.position.x, height);

            throwForce = Vector2.Distance(transform.position, GetMousePosition()) * forceScale;
        }

        spriteRenderer.enabled = type != OrbType.None;

        spriteRenderer.sprite = type == OrbType.Minus ? minusOrb : plusOrb;

        coll.enabled = thrown;

        if (drawTrajectory) DrawTrajectory();

        if (transform.position.y < GameManager.instance.worldBarrier)
        {
            thrown = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(GetMousePosition(), .1F);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, .1F);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, GetMousePosition());
    }

    public void Throw()
    {
        if (thrown) return;

        thrown = true;

        drawTrajectory = false;

        rb.velocity = -((Vector2)transform.position - GetMousePosition()).normalized * throwForce;
    }

    private void DrawTrajectory()
    {
        Vector2 sourcePosition = transform.position;

        Vector2 targetPosition = GetMousePosition();

        lineRenderer.SetPositions(new Vector3[0]);

        throwAngle = Mathf.Atan2(targetPosition.x - sourcePosition.x, targetPosition.y - sourcePosition.y);

        Vector3[] trajectory = PlotTrajectory(sourcePosition, throwAngle, throwForce).Select(pos => (Vector3)pos).ToArray();

        lineRenderer.positionCount = trajectory.Length;

        lineRenderer.SetPositions(trajectory);

        float width = lineRenderer.startWidth;
        float aspectRatio = lineTexture.height / (float)lineTexture.width;

        lineRenderer.material.SetTextureScale("_MainTex", new Vector2(1 / width * aspectRatio, 1.0f));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if ((groundMask & (1 << other.gameObject.layer)) != 0)
        {
            thrown = false;
            drawTrajectory = false;
            lineRenderer.positionCount = 0;
        }

        if ((blockMask & (1 << other.gameObject.layer)) != 0)
        {
            thrown = false;
            drawTrajectory = false;
            lineRenderer.positionCount = 0;

            float scale = Mathf.Clamp(type == OrbType.Minus ? .5F : 2, .5F, 4);

            other.transform.LeanScale(other.transform.localScale * scale, .5F).setEaseInOutExpo();

            player.livingEntity.soundPlayer.PlaySound(type == OrbType.Minus ? "scaleDown" : "scaleUp");

            type = OrbType.None;
        }
    }

    List<Vector2> PlotTrajectory(Vector2 startPos, float angle, float initVel)
    {
        List<Vector2> trajectory = new() { startPos };
        Vector2 prev = startPos;
        Vector2 startVelocity = new Vector2(initVel * Mathf.Sin(angle), initVel * Mathf.Cos(angle));


        for (int i = 1; ; i++)
        {
            float t = timestep * i;
            if (t > maxTime) break;
            Vector2 pos = PlotTrajectoryAtTime(startPos, startVelocity, t);
            if (Physics.Linecast(prev, pos)) break;
            trajectory.Add(pos);
            prev = pos;
        }

        return trajectory;
    }

    Vector2 GetMousePosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    public Vector2 PlotTrajectoryAtTime(Vector2 start, Vector2 startVelocity, float time)
    {
        return start + startVelocity * time + ((Vector2)Physics.gravity) * time * time * 0.5f;
    }

    public OrbType GetOrbType() => type;

    public void SetOrbType(OrbType type) { this.type = type; }

    public enum OrbType
    {
        Minus,
        Plus,
        None
    }
}

