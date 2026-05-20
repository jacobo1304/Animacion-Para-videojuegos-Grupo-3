public interface IAttackStatsProvider
{
    float DamageMultiplier { get; }
    float StaminaCostMultiplier { get; }
    float AttackSpeedMultiplier { get; }
    bool IsInvulnerable { get; }
}
