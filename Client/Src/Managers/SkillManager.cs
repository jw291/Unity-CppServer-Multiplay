using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10f;

    private readonly Collider2D[] hitBuffer = new Collider2D[64];
    private ContactFilter2D contactFilter;
    private Coroutine detectionRoutine;
    private Transform MyPlayer => Managers.Instance.Player?.myPlayer?.transform;

    public Dictionary<int, Skill> OwnedSkills => ownedSkills;
    private Dictionary<int, Skill> ownedSkills = new();
    
    private SkillTable skillTableData => Managers.Instance.Data.SkillTable;
    
    private readonly Dictionary<uint, Projectile> prefabMap = new();
    public event Action OnSkillRefresh;

    public void Init()
    {
        contactFilter = new ContactFilter2D().NoFilter();

        foreach (SkillTableData data in skillTableData.GetAll())
            prefabMap[(uint)data.skillId] = data.SkillPrefab.Projectile;
    }

    public void ExecuteSkills()
    {
        foreach (Skill skill in ownedSkills.Values)
        {
            skill.Create();
            skill.Execute();
        }

        if (detectionRoutine != null)
            StopCoroutine(detectionRoutine);
        detectionRoutine = StartCoroutine(DetectionRoutine());
    }

    public void RequestSkillInfo()
    {
        var req = new REQ_SKILL_INFO { AccountId = Managers.Instance.Network.AccountId };
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_SKILL_INFO, req);
    }

    public void RequestSkillAdd(uint skillId)
    {
        var req = new REQ_SKILL_ADD { AccountId = Managers.Instance.Network.AccountId, SkillId = skillId };
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_SKILL_ADD, req);
    }

    public void RequestSkillSubtract(uint skillId)
    {
        var req = new REQ_SKILL_SUBTRACT { AccountId = Managers.Instance.Network.AccountId, SkillId = skillId };
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_SKILL_SUBTRACT, req);
    }

    public void PACKET_RES_SKILL_INFO(RES_SKILL_INFO res)
    {
        SetOwnedSkills(res.Skills.ToDictionary(x => (int)x.Key,  x => (int)x.Value));
        OnSkillRefresh?.Invoke();
        Debug.Log($"[SkillManager] Skill info received: {ownedSkills.Count} skills");
    }

    public void PACKET_RES_SKILL_ADD(RES_SKILL_ADD res)
    {
        if (!res.Success)
        {
            Debug.LogWarning($"[SkillManager] Skill add failed: skillId={res.SkillId}");
            return;
        }

        SetOwnedSkill((int)res.SkillId, (int)res.SkillLevel);
        OnSkillRefresh?.Invoke();
        Debug.Log($"[SkillManager] Skill added/upgraded: skillId={res.SkillId}, level={res.SkillLevel}");
    }

    public void PACKET_RES_SKILL_SUBTRACT(RES_SKILL_SUBTRACT res)
    {
        if (!res.Success)
        {
            Debug.LogWarning($"[SkillManager] Skill subtract failed: skillId={res.SkillId}");
            return;
        }

        SetOwnedSkill((int)res.SkillId, (int)res.SkillLevel);
        OnSkillRefresh?.Invoke();
        Debug.Log($"[SkillManager] Skill subtracted: skillId={res.SkillId}, level={res.SkillLevel}");
    }

    public void SendSkillFire(uint skillId, List<ProjectileInfo> projectiles)
    {
        var req = new REQ_SKILL_FIRE
        {
            AccountId = Managers.Instance.Network.AccountId,
            SkillId = skillId,
        };
        req.Projectiles.AddRange(projectiles);
        Managers.Instance.Network.Tcp.Send(PacketId.MSG_REQ_SKILL_FIRE, req);
    }

    public void PACKET_RES_SKILL_FIRE(RES_SKILL_FIRE res)
    {
        if (!prefabMap.TryGetValue(res.SkillId, out var prefab)) 
            return;

        SkillTableData data = Managers.Instance.Data.SkillTable.Get((int)res.SkillId);
        if (data == null)
            return;

        PoolType poolType = data.GetPoolType();

        foreach (var p in res.Projectiles)
        {
            var obj = Managers.Instance.Pool.GetPoolObject(poolType, prefab, PoolLayer.Skill);
            if (obj == null) 
                continue;

            obj.transform.position = new Vector3(p.PosX, p.PosY, 0f);
            if (obj is Projectile projectile)
                projectile.Launch(new Vector2(p.DirX, p.DirY), p.Speed);
        }
    }
    
    private IEnumerator DetectionRoutine()
    {
        while (true)
        {
            if (MyPlayer == null)
                yield break;

            Monster nearest = GetNearestActiveMonster();
            foreach (Skill skill in ownedSkills.Values)
                skill.SetNearestMonster(nearest);

            yield return null;
        }
    }

    private Monster GetNearestActiveMonster()
    {
        int count = Physics2D.OverlapCircle(MyPlayer.position, detectionRadius, contactFilter, hitBuffer);

        Monster nearest = null;
        float minDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = hitBuffer[i];
            if (!hit.gameObject.activeInHierarchy)
                continue;

            Monster monster = hit.GetComponent<Monster>();
            if (monster == null) 
                continue;

            float dist = Vector2.Distance(MyPlayer.position, monster.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = monster;
            }
        }

        return nearest;
    }

    private void SetOwnedSkills(Dictionary<int,int> skills)
    {
        foreach (var skill in skills)
        {
            SetOwnedSkill(skill.Key, skill.Value);
        }
    }
    
    private void SetOwnedSkill(int skillId, int skillLevel)
    {
        var data = skillTableData.Get((int)skillId);
        if (data == null)
            return;

        if (skillLevel <= 0)
        {
            ownedSkills.Remove(skillId);
            return;
        }

        var skillPrefab = data.SkillPrefab;
        var skillobject = Instantiate(skillPrefab, Managers.Instance.Player.myPlayer.transform);
        skillobject.gameObject.SetActive(false);
        skillobject.SetTableData(data);
        skillobject.SetLevel((int)skillLevel);
        ownedSkills[skillId] = skillobject;
    }
}