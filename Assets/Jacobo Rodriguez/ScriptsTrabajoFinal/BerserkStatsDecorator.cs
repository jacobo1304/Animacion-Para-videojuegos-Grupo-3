public class BerserkStatsDecorator : AttackStatsDecorator
{
    private readonly float damageMultiplier;
    private readonly float staminaCostMultiplier;
    private readonly float attackSpeedMultiplier;

    public BerserkStatsDecorator(IAttackStatsProvider inner, float damageMultiplier, float staminaCostMultiplier, float attackSpeedMultiplier)
        : base(inner)
    {
        this.damageMultiplier = damageMultiplier;
        this.staminaCostMultiplier = staminaCostMultiplier;
        this.attackSpeedMultiplier = attackSpeedMultiplier;
    }

    public override float DamageMultiplier => inner.DamageMultiplier * damageMultiplier;
    public override float StaminaCostMultiplier => inner.StaminaCostMultiplier * staminaCostMultiplier;
    public override float AttackSpeedMultiplier => inner.AttackSpeedMultiplier * attackSpeedMultiplier;
}
