using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("Berserk Settings")]
    [SerializeField] private float berserkDamageMultiplier = 1.5f;
    [SerializeField] private float berserkStaminaCostMultiplier = 0.5f;
    [SerializeField] private float berserkAttackSpeedMultiplier = 1.25f;

    [Header("Animator (optional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool affectAnimatorSpeed = true;

    [Header("Damage (optional)")]
    [SerializeField] private DamageController damageController;

    private IAttackStatsProvider baseProvider = new AttackStatsProviderBase();
    private IAttackStatsProvider currentProvider;
    private readonly List<ActivePowerUp> activePowerUps = new List<ActivePowerUp>();
    private bool wasInvulnerable;

    private class ActivePowerUp
    {
        public PowerUpType Type;
        public GameObject EffectInstance;
        public bool EffectIsSceneObject;
        public bool EffectWasActive;
        public Coroutine Routine;
    }

    private void Awake()
    {
        currentProvider = baseProvider;
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (damageController == null)
        {
            damageController = GetComponentInChildren<DamageController>();
        }
    }

    private void Update()
    {
        if (affectAnimatorSpeed && animator != null)
        {
            animator.speed = Mathf.Max(0.01f, currentProvider.AttackSpeedMultiplier);
        }
    }

    public void ApplyPowerUp(PowerUpType type, float duration, GameObject effectObject)
    {
        if (duration <= 0f)
        {
            return;
        }

        var active = new ActivePowerUp { Type = type };
        ActivateEffect(active, effectObject);
        activePowerUps.Add(active);
        RebuildProvider();

        active.Routine = StartCoroutine(ExpirePowerUp(active, duration));
    }

    public float GetDamageMultiplier() => currentProvider.DamageMultiplier;
    public float GetStaminaCostMultiplier() => currentProvider.StaminaCostMultiplier;
    public float GetAttackSpeedMultiplier() => currentProvider.AttackSpeedMultiplier;
    public bool IsInvulnerable() => currentProvider.IsInvulnerable;

    private IEnumerator ExpirePowerUp(ActivePowerUp active, float duration)
    {
        yield return new WaitForSeconds(duration);

        DeactivateEffect(active);
        activePowerUps.Remove(active);
        RebuildProvider();
    }

    private void RebuildProvider()
    {
        IAttackStatsProvider provider = baseProvider;
        for (int i = 0; i < activePowerUps.Count; i++)
        {
            provider = CreateDecorator(activePowerUps[i].Type, provider);
        }
        currentProvider = provider;
        UpdateInvulnerabilityState();
    }

    private void UpdateInvulnerabilityState()
    {
        if (damageController == null)
        {
            return;
        }

        bool isInvulnerable = currentProvider.IsInvulnerable;
        if (isInvulnerable == wasInvulnerable)
        {
            return;
        }

        if (isInvulnerable)
        {
            damageController.IFrameStart();
        }
        else
        {
            damageController.IFrameEnd();
        }

        wasInvulnerable = isInvulnerable;
    }

    private IAttackStatsProvider CreateDecorator(PowerUpType type, IAttackStatsProvider inner)
    {
        switch (type)
        {
            case PowerUpType.Offensive:
                return new BerserkStatsDecorator(inner, berserkDamageMultiplier, berserkStaminaCostMultiplier, berserkAttackSpeedMultiplier);
            case PowerUpType.Defensive:
                return new ShieldStatsDecorator(inner);
            default:
                return inner;
        }
    }

    private void ActivateEffect(ActivePowerUp active, GameObject effectObject)
    {
        if (effectObject == null)
        {
            return;
        }

        if (effectObject.scene.IsValid())
        {
            active.EffectIsSceneObject = true;
            active.EffectWasActive = effectObject.activeSelf;
            effectObject.SetActive(true);
            active.EffectInstance = effectObject;
        }
        else
        {
            GameObject instance = Instantiate(effectObject, transform.position, transform.rotation, transform);
            active.EffectInstance = instance;
            active.EffectIsSceneObject = false;
        }
    }

    private void DeactivateEffect(ActivePowerUp active)
    {
        if (active.EffectInstance == null)
        {
            return;
        }

        if (active.EffectIsSceneObject)
        {
            active.EffectInstance.SetActive(active.EffectWasActive);
        }
        else
        {
            Destroy(active.EffectInstance);
        }
    }
}
