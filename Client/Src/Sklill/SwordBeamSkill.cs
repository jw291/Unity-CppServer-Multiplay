using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordBeamSkill : Skill
{
    private static readonly Vector2[] Directions =
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2( 1f,  1f).normalized,
        new Vector2(-1f,  1f).normalized,
        new Vector2( 1f, -1f).normalized,
        new Vector2(-1f, -1f).normalized,
    };

    protected override IEnumerator FireLoop()
    {
        while (true)
        {
            FireAllDirections();
            yield return new WaitForSeconds(Interval);
        }
    }

    private void FireAllDirections()
    {
        var projectiles = new List<ProjectileInfo>();

        foreach (Vector2 dir in Directions)
        {
            var beam = Managers.Instance.Pool.GetPoolObject(PoolType.SwordBeam, Projectile, PoolLayer.Skill);
            beam.transform.position = FirePosition;
            beam.Launch(dir, Speed,Damage);

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
    }
}
