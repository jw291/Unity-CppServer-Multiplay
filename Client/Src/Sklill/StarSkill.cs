using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarSkill : Skill
{
    protected override IEnumerator FireLoop()
    {
        var projectiles = new List<ProjectileInfo>();

        for (int i = 0; i < 3; i++)
        {
            var star = Managers.Instance.Pool.GetPoolObject(PoolType.Star, Projectile, PoolLayer.Skill);
            star.transform.position = FirePosition;
            Vector2 dir = Random.insideUnitCircle.normalized;
            star.Launch(dir, Speed,Damage);

            projectiles.Add(new ProjectileInfo
            {
                PosX = FirePosition.x,
                PosY = FirePosition.y,
                DirX = dir.x,
                DirY = dir.y,
                Speed = Speed,
            });
        }

        Managers.Instance.Skill.SendSkillFire((uint)tableData.skillId, projectiles);
        yield break;
    }
}
