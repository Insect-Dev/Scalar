using System.Collections;
using TNRD.Autohook;
using UnityEngine;
using static Orb;

public class OrbItem : MonoBehaviour
{
    [Header("References")]

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private SpriteRenderer spriteRenderer;

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private Animator animator;

    [Header("Orb")]

    [SerializeField] private bool active = true;

    [SerializeField] private bool animatingOut = false;

    [SerializeField] private OrbType type;

    [Header("Sprites")]

    [SerializeField] private Sprite minusOrb;
    [SerializeField] private Sprite plusOrb;

    void Update()
    {
        spriteRenderer.enabled = !(type == OrbType.None || !active);

        spriteRenderer.sprite = type == OrbType.Minus ? minusOrb : plusOrb;
    }

    public bool PickUp()
    {
        if (!active || animatingOut) return false;

        animator.SetTrigger("Pick Up");

        StartCoroutine(WaitToDelete());

        return true;
    }

    public IEnumerator WaitToDelete()
    {
        animatingOut = true;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        active = false;

        animatingOut = false;
    }


    public OrbType GetOrbType() => type;
}

