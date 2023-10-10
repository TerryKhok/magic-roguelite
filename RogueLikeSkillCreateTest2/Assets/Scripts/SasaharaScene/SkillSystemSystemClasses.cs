using Cysharp.Threading.Tasks;
using Mono.Cecil.Cil;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


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
        int loopTime = 6;
        int delay = 100;
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
        int angleBetween = 60;
        int way = 6;
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