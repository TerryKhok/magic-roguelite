using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor;

public class MechanicsDamage : SkillProgress
{
    public MechanicsDamage(int t) : base(t)
    {
        Debug.Log($"[Generated] MechanicsDamage: {t}");
    }

    public override void RunProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        ProgressId id = ProgressId.MechanicsDamage;
        int dmg = SkillDB.GetSkillVariableValue(id, 0, GetTier());

        SkillAttributes attr = elem.GetAttr();
        if (attr.IsExist("AttackInc"))
        {
            dmg = (int)(dmg * attr.GetCorrection("AttackInc").GetMulti());
            dmg = (int)(dmg + attr.GetCorrection("AttackInc").GetFixedValue());
        }

        elem.GetTargets().ForEach(t =>
        {
            if (elem.IsTarget(t))
            {
                if (elem.IsPlayer())
                {
                    t.GetComponent<EnemyManager>().EnemyTakeDamage(dmg);
                    Debug.Log("A");
                }
                else
                {
                    t.GetComponent<PlayerMovement>().PlayerTakeDamage(dmg);
                    Debug.Log("B");
                }

            }
        });
    }
}

public class MechanicsGenerateCube : SkillProgress
{
    public MechanicsGenerateCube(int t) : base(t)
    {
        Debug.Log($"[Generated] MechanicsGenerateCube: {t}");
    }

    public override async void RunProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        ProgressId id = ProgressId.MechanicsGenerateCube;
        int lifeTime = SkillDB.GetSkillVariableValue(id, 0, GetTier());

        SkillAttributes attr = elem.GetAttr();
        if (attr.IsExist("RangeInc"))
        {
            lifeTime = (int)(lifeTime * attr.GetCorrection("RangeInc").GetMulti());
            lifeTime = (int)(lifeTime + attr.GetCorrection("RangeInc").GetFixedValue());
        }

        GameObject resource = Resources.Load("SkillSystem/SkillSystem_Mechanics_GenerateCube_Stub") as GameObject;
        GameObject cube = Object.Instantiate(resource, elem.GetLocationData().GetPos(), elem.GetLocationData().GetRotate());
        await UniTask.Delay(lifeTime);
        Object.Destroy(cube);
    }
}