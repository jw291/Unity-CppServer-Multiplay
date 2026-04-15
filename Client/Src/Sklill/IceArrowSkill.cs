using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceArrowSkill : Skill
{
    protected override IEnumerator FireLoop()
    {
        while (true)
        {
            if (NearestMonster != null)
            {
                var arrow = Managers.Instance.Pool.GetPoolObject(PoolType.IceArrow, Projectile, PoolLayer.Skill);
                arrow.transform.position = FirePosition;

                Vector2 direction = (NearestMonster.transform.position - FirePosition).normalized;
                arrow.Launch(direction, Speed,Damage);

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
