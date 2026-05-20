public abstract class AttackStatsDecorator : IAttackStatsProvider
{
    protected readonly IAttackStatsProvider inner;

    protected AttackStatsDecorator(IAttackStatsProvider inner)
    {
        this.inner = inner;
    }

    public virtual float DamageMultiplier => inner.DamageMultiplier;
    public virtual float StaminaCostMultiplier => inner.StaminaCostMultiplier;
    public virtual float AttackSpeedMultiplier => inner.AttackSpeedMultiplier;
    public virtual bool IsInvulnerable => inner.IsInvulnerable;
}
