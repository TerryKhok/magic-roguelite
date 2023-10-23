using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


//遅延ありループ処理の開始ノード
public class SystemLoopStart : SkillProgress, ISkillLoopStartProgress
{
    public SystemLoopStart(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemLoopStart: {t}");
    }

    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await UniTask.Delay(0);
        return elem;
    }

    async void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
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
                foreach (ISkillProgress progress in _loopProgressList)
                {
                    try
                    {
                        progress.SkillProgressNoWait(elem, token);
                        elem = await progress.SkillProgress(elem, token);
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

    List<ISkillProgress> _loopProgressList = new List<ISkillProgress>();
    public void AddLoopProgressList(ISkillProgress progress)
    {
        _loopProgressList.Add(progress);
    }

    public Type GetLoopEndProgressType()
    {
        return typeof(SystemLoopEnd);
    }
}

//遅延ありループ処理の終了ノード
public class SystemLoopEnd : SkillProgress, ISkillProgress
{

    public SystemLoopEnd(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemLoopEnd: {t}");
    }

    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await UniTask.Delay(0);
        return elem;
    }

    void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
    }
}


//複数方向ループ処理の開始ノード
public class SystemWayStart : SkillProgress, ISkillLoopStartProgress
{
    public SystemWayStart(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemWayStart: {t}");
    }

    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await UniTask.Delay(0);
        return elem;
    }

    async void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
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
                foreach (ISkillProgress progress in _wayProgressList)
                {
                    try
                    {
                        progress.SkillProgressNoWait(elem, token);
                        elem = await progress.SkillProgress(elem, token);
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


    List<ISkillProgress> _wayProgressList = new List<ISkillProgress>();
    public void AddLoopProgressList(ISkillProgress progress)
    {
        _wayProgressList.Add(progress);
    }

    public Type GetLoopEndProgressType()
    {
        return typeof(SystemWayEnd);
    }
}

//複数方向ループ処理の終了ノード
public class SystemWayEnd : SkillProgress, ISkillProgress
{
    public SystemWayEnd(int t) : base(t)
    {
        Debug.Log($"[Generated] SystemWayEnd: {t}");
    }


    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await UniTask.Delay(0);
        return elem;
    }

    void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
    }
}