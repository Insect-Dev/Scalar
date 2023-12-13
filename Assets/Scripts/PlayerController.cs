using TNRD.Autohook;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LivingEntity))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    public LivingEntity livingEntity;

    [SerializeField, AutoHook(searchArea:AutoHookSearchArea.Children, ReadOnlyWhenFound = true)]
    private Orb orb;

    [SerializeField] private Collider2D platformsCollider;

    [SerializeField] private Tilemap tilemap;

    [Header("Movement")]

    [SerializeField] private bool sneaking;

    [SerializeField] private float sneakSpeedMultiplier;

    [Header("Orb Animation")]

    public int orbPixelOffset = 0;

    public void MoveInput(InputAction.CallbackContext ctx)
    {
        livingEntity.walkDirection = ctx.canceled ? 0 : ctx.ReadValue<float>() * (sneaking ? sneakSpeedMultiplier : 1);
    }

    public void JumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            livingEntity.Jump();
        }
    }

    public void SneakInput(InputAction.CallbackContext ctx)
    {
        sneaking = ctx.canceled ? false : true;

        livingEntity.animator.SetBool("Sneaking", sneaking);

        platformsCollider.enabled = !sneaking;
    }

    public void ThrowInput(InputAction.CallbackContext ctx)
    {
        if (orb.GetOrbType() == Orb.OrbType.None) return;

        if (ctx.performed) orb.drawTrajectory = true;

        if (ctx.canceled) orb.Throw();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OrbItem orbItem = collision.GetComponent<OrbItem>();

        if (orbItem == null) return;

        if (orbItem.PickUp())
        {
            orb.SetOrbType(orbItem.GetOrbType());

            livingEntity.soundPlayer.PlaySound("pickUp");
        }
    }
}