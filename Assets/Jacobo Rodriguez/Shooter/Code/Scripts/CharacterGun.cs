using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterGun : MonoBehaviour, ICharacterComponent
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private RecoilCameraKick recoil;

    [Header("Shooting")]
    [SerializeField] private bool automatic;
    [SerializeField] private bool requiereAim = true;
    [SerializeField] private float fireRate = 10f;
    [SerializeField] private float range = 200f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float debugDuration;

    [Header("RecoilCamera")]
    [SerializeField] private float camShake = 0.6f;
    [SerializeField] private float camKick = 0.12f;
    [SerializeField] private float camRecover = 0.18f;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 1.5f;

    [SerializeField] private Transform tracerOrigin;

    private float _nextShootTime;
    private CharacterStealth _characterStealth;

    public Character ParentCharacter { get; set; }

    private void Awake()
    {
        _characterStealth = GetComponent<CharacterStealth>();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ParentCharacter == null)
            return;

        if (ctx.started)
        {
            ParentCharacter.IsFiring = true;
            if (_characterStealth != null)
                _characterStealth.ForceExitStealth();
        }

        if (ctx.canceled) ParentCharacter.IsFiring = false;
        if (!automatic && ctx.performed) TryShoot();
    }


    private void TryShoot()
    {
        if (ParentCharacter.IsReloading) return;
        if (requiereAim && (ParentCharacter == null || !ParentCharacter.IsAiming)) return;
        if(Time.time < _nextShootTime) return;
        _nextShootTime = Time.time + 1f / Mathf.Max(1f,fireRate);
        

        ShootOnce();
    }
    private void ShootOnce()
    {

        if (animator != null) animator.SetTrigger("Fire");

        if (recoil) recoil.Kick(camShake, camKick, camRecover);


        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 from = tracerOrigin ? tracerOrigin.position : ray.origin;

        if (Physics.Raycast(ray, out var hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 to = hit.point;

            Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(ray.origin, to), Color.magenta, debugDuration);

            Debug.DrawLine(from, to, Color.yellow, debugDuration);

            Debug.DrawRay(to, hit.normal, Color.red, debugDuration);

            var info = new HitInfo
            {
                point = hit.point,
                normal = hit.normal,
                damage = 10f
            };


            if (hit.collider.TryGetComponent<IHittable>(out var h))
            {
                h.ApplyHit(info);
            }
            else
            {
                var rb = hit.collider.attachedRigidbody;
                if (rb != null && rb.TryGetComponent<IHittable>(out var hRb))
                {
                    hRb.ApplyHit(info);
                }
                else
                {
                    var hParent = hit.collider.GetComponentInParent<IHittable>();
                    if (hParent != null) hParent.ApplyHit(info);
                }
            }
        }
        else
        {
            Vector3 to = ray.origin + ray.direction * range;
            Debug.DrawRay(ray.origin, ray.direction * range, Color.gray, debugDuration);

            Debug.DrawLine(from, to, Color.cyan, debugDuration);
        }

       
    }

    public void OnReload(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && ParentCharacter != null
            && !ParentCharacter.IsReloading
            && !ParentCharacter.IsFiring
            && !ParentCharacter.IsEmoting)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        ParentCharacter.IsReloading = true;

        if (animator != null)
            animator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        ParentCharacter.IsReloading = false;
    }

    private void Update()
    {
        if (automatic && ParentCharacter.IsFiring) TryShoot();
    }
}
