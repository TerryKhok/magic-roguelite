using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillUser : MonoBehaviour
{
    private List<List<ISkillProgress>> _skillSlot = new List<List<ISkillProgress>>();

    void Start()
    {
        List<ISkillProgress> list = new List<ISkillProgress>();
        list.Add(new TargetBall(1));
        list.Add(new MechanicsDamage(1));
        _skillSlot.Add(new List<ISkillProgress>(list));
    }

    public async void RunSkill(int num)
    {
        List<ISkillProgress> code = new List<ISkillProgress>(_skillSlot[num]);
        SkillElements elem = new SkillElements(gameObject.transform);
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;

        foreach (ISkillProgress progress in code)
        {
            progress.SkillProgressNoWait(elem, token);
            elem = await progress.SkillProgress(elem, token);
        }
    }
}
