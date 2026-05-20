public class AttackStatsProviderBase : IAttackStatsProvider
{
    public float DamageMultiplier => 1f;
    public float StaminaCostMultiplier => 1f;
    public float AttackSpeedMultiplier => 1f;
    public bool IsInvulnerable => false;
}
