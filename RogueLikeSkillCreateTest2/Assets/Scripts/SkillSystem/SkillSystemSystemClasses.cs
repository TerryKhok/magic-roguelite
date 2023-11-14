using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


//遅延ありループ処理の開始ノード
public class SystemLoopStart : SkillLoopStartProgress
{
    public SystemLoopStart(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemLoopStart: {t}");
    }

    public override async void RunProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        ProgressId id = ProgressId.SystemLoopStart;
        int loopTime = SkillDB.GetSkillVariableValue(id, 0, GetTier());
        int delay = SkillDB.GetSkillVariableValue(id, 1, GetTier());
        for (int i = 0; i < loopTime; i++)
        {
            SkillElements elemPrev = elem;
            UniTask.Create(async () =>
            {
                foreach (SkillProgress progress in _loopProgressList)
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
            }).Forget();
            elem = elemPrev;
            await UniTask.Delay(delay);
        }
    }

    List<SkillProgress> _loopProgressList = new List<SkillProgress>();
    public override void AddLoopProgressList(SkillProgress progress)
    {
        _loopProgressList.Add(progress);
    }

    public override Type GetLoopEndProgressType()
    {
        return typeof(SystemLoopEnd);
    }
}

//遅延ありループ処理の終了ノード
public class SystemLoopEnd : SkillProgress
{
    public SystemLoopEnd(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemLoopEnd: {t}");
    }
}


//複数方向ループ処理の開始ノード
public class SystemWayStart : SkillLoopStartProgress
{
    public SystemWayStart(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemWayStart: {t}");
    }

    public override async void RunProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        ProgressId id = ProgressId.SystemWayStart;
        int way = SkillDB.GetSkillVariableValue(id, 0, GetTier());
        int angleBetween = SkillDB.GetSkillVariableValue(id, 1, GetTier());

        for (int i = 0; i < way; i++)
        {
            SkillElements elemPrev = elem;
            LocationData ld = elem.GetLocationData();
            elem.SetLocationData(ld.GetPos(), Quaternion.Euler(ld.GetRotate().eulerAngles + new Vector3(0, 0, (angleBetween / 2 * (way - 1) * -1) + (angleBetween * i))));
            UniTask.Create(async () =>
            {
                foreach (SkillProgress progress in _wayProgressList)
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
            }).Forget();
            elem = elemPrev;
        }
        await UniTask.Delay(100);
    }


    List<SkillProgress> _wayProgressList = new List<SkillProgress>();
    public override void AddLoopProgressList(SkillProgress progress)
    {
        _wayProgressList.Add(progress);
    }

    public override Type GetLoopEndProgressType()
    {
        return typeof(SystemWayEnd);
    }
}

//複数方向ループ処理の終了ノード
public class SystemWayEnd : SkillProgress
{
    public SystemWayEnd(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemWayEnd: {t}");
    }
}