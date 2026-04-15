public class SpeedBoostBuff : Buff
{
    public override void Apply(MyPlayer player, int value, float duration)
        => player.AddSpeedBuff(value, duration);
}
