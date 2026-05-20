using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour
{
    [SerializeField] private bool ignoreDamage;
    [SerializeField] private EnemyHealth health;
    [SerializeField] private Animator animator;
    [SerializeField] private string damageTrigger = "Damage";
    [SerializeField] private string dieTrigger = "Die";
    [SerializeField] private string damageDirectionParam = "DamageDirection";
    [SerializeField] private string damageLevelParam = "DamageLevel";
    [SerializeField] private float hitPointOffset = 0.3f;
    [SerializeField] private float deathShrinkDelay = 0.4f;
    [SerializeField] private float deathShrinkDuration = 0.6f;
    [SerializeField] private Vector3 deathScaleTarget = Vector3.zero;
    [SerializeField] private Transform deathScaleRoot;
    [SerializeField] private GameObject deathDisableTarget;

    private readonly List<DamageMessage> damageList = new List<DamageMessage>();
    private Vector3 lastHitPoint;
    private bool isDying;
    private Coroutine deathRoutine;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>(includeInactive: true);
        }

        if (health == null)
        {
            health = GetComponent<EnemyHealth>();
        }

        if (deathScaleRoot == null)
        {
            deathScaleRoot = animator != null ? animator.transform : transform;
        }

        if (deathDisableTarget == null)
        {
            deathDisableTarget = gameObject;
        }
    }

    public void OnHit(DamageMessage damage)
    {
        EnqueueDamage(damage);
    }

    public void EnqueueDamage(DamageMessage damage)
    {
        if (ignoreDamage) return;
        if (damage.sender != null && damageList.Exists(dmg => dmg.sender == damage.sender)) return;

        if (damage.sender != null)
        {
            var dirFromAttacker = (transform.position - damage.sender.transform.position).normalized;
            lastHitPoint = transform.position + dirFromAttacker * hitPointOffset;
        }

        damageList.Add(damage);
    }

    private void Update()
    {
        if (damageList.Count == 0 || animator == null || health == null) return;

        Vector3 damageDirection = Vector3.zero;
        int damageLevel = 0;
        bool isDead = false;

        foreach (DamageMessage message in damageList)
        {
            health.ApplyDamage(message.amount, out isDead);
            if (message.sender != null)
            {
                damageDirection += (message.sender.transform.position - transform.position).normalized;
            }

            damageLevel = Mathf.Max(damageLevel, (int)message.damageLevel);
        }

        if (damageDirection.sqrMagnitude > 0.001f)
        {
            damageDirection = Vector3.ProjectOnPlane(damageDirection.normalized, transform.up);
            float damageAngle = Vector3.SignedAngle(transform.forward, damageDirection, transform.up);
            animator.SetFloat(damageDirectionParam, (damageAngle / 180f) * 0.5f + 0.5f);
        }

        animator.SetInteger(damageLevelParam, damageLevel);
        animator.SetTrigger(damageTrigger);

        if (isDead)
        {
            animator.ResetTrigger(damageTrigger);
            animator.SetTrigger(dieTrigger);
            BeginDeathSequence();
        }

        if (damageList[0].sender != null)
        {
            var attacker = damageList[0].sender.transform;
            GetComponent<HitTargetFollower>()?.Moveto(lastHitPoint, attacker, pulse: 0.18f);
        }

        damageList.Clear();
    }

    public void IFrameStart()
    {
        ignoreDamage = true;
    }

    public void IFrameEnd()
    {
        ignoreDamage = false;
    }

    private void BeginDeathSequence()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
        }

        deathRoutine = StartCoroutine(ScaleDownAndDisable());
    }

    private System.Collections.IEnumerator ScaleDownAndDisable()
    {
        if (deathShrinkDelay > 0f)
        {
            yield return new WaitForSeconds(deathShrinkDelay);
        }

        Transform root = deathScaleRoot != null ? deathScaleRoot : transform;
        Vector3 startScale = root.localScale;
        float duration = Mathf.Max(0.01f, deathShrinkDuration);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            root.localScale = Vector3.Lerp(startScale, deathScaleTarget, lerp);
            yield return null;
        }

        root.localScale = deathScaleTarget;
        deathDisableTarget.SetActive(false);
    }
}
