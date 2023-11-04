using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillUser : MonoBehaviour
{
    private List<List<SkillProgress>> _skillSlot = new List<List<SkillProgress>>();

    void Start()
    {
        SkillDB.Initialize();
        _skillSlot.Add(GetComponent<SkillCompile>().GetSkill());
    }

    public async void RunSkill(int num)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        List<SkillProgress> code = new List<SkillProgress>(_skillSlot[num]);
        SkillElements elem = new SkillElements(gameObject.transform);
        foreach (SkillProgress progress in code)
        {
            try
            {
                progress.RunProgressNoWait(elem, token);
                elem = await progress.RunProgress(elem, token);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString() + " / スキル実行中にキャンセル");
            }
        }
    }
}
