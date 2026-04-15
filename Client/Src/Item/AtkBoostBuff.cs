public class AtkBoostBuff : Buff
{
    public override void Apply(MyPlayer player, int value, float duration)
        => player.AddAtkBuff(value, duration);
}
