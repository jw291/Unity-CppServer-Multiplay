public class DefenseBoostBuff : Buff
{
    public override void Apply(MyPlayer player, int value, float duration)
        => player.AddDefenseBuff(value, duration);
}
