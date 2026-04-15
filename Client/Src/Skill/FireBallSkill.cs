using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBallSkill : Skill
{
    protected override IEnumerator FireLoop()
    {
        while (true)
        {
            if (NearestMonster != null)
            {
                var ball = Managers.Instance.Pool.GetPoolObject(PoolType.FireBall, Projectile, PoolLayer.Skill);
                ball.transform.position = FirePosition;

                Vector2 direction = (NearestMonster.transform.position - FirePosition).normalized;
                ball.Launch(direction, Speed,Damage);

                Managers.Instance.Skill.SendSkillFire((uint)tableData.skillId, new List<ProjectileInfo>
                {
                    new ProjectileInfo
                    {
                        PosX = FirePosition.x,
                        PosY = FirePosition.y,
                        DirX = direction.x,
                        DirY = direction.y,
                        Speed = Speed,
                    }
                });
            }

            yield return new WaitForSeconds(Interval);
        }
    }
}
