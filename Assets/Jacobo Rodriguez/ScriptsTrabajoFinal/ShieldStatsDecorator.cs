public class ShieldStatsDecorator : AttackStatsDecorator
{
    public ShieldStatsDecorator(IAttackStatsProvider inner) : base(inner)
    {
    }

    public override bool IsInvulnerable => true;
}
