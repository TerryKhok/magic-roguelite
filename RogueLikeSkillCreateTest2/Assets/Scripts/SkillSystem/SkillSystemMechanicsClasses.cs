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
        ProgressId id = ProgressId.MechanicsDamage;
        int dmg = SkillDB.GetSkillVariableValue(id, 0, GetTier());
        token.ThrowIfCancellationRequested();
        elem.GetTargets().ForEach(t => {
            if (t.CompareTag("Enemy"))
            {
                //t.GetComponent<EnemyManager>().EnemyTakeDamage(dmg);
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

        GameObject resource = Resources.Load("SkillSystem/SkillSystem_Mechanics_GenerateCube_Stub") as GameObject;
        GameObject cube = Object.Instantiate(resource, elem.GetLocationData().GetPos(), elem.GetLocationData().GetRotate());
        await UniTask.Delay(lifeTime);
        Object.Destroy(cube);
    }
}




//----------------------------ここからエネミー----------------------//
public class EnemyMechanicsDamage : SkillProgress
{
    public EnemyMechanicsDamage(int t) : base(t)
    {
        Debug.Log($"[Generated] EnemyMechanicsDamage: {t}");
    }

    public override void RunProgressNoWait(SkillElements elem, CancellationToken token)
    {
        ProgressId id = ProgressId.EnemyMechanicsDamage;
        int dmg = SkillDB.GetSkillVariableValue(id, 0, GetTier());
        token.ThrowIfCancellationRequested();
        elem.GetTargets().ForEach(t =>
        {
            if (t.CompareTag("Player"))
            {
                t.GetComponent<PlayerMovement>().PlayerTakeDamage(dmg);
            }
        });
    }
}

