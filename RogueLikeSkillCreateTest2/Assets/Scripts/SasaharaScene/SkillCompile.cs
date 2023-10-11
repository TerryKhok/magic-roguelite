using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillCompile : MonoBehaviour
{
    [SerializeField] List<progressId> _progressIdList = new List<progressId>();

    public List<ISkillProgress> GetSkill()
    {
        return Compile();
    }



    List<ISkillProgress> Compile()
    {
        List<ISkillProgress> list = new List<ISkillProgress>(); //最終的に返すリスト
        List<ISkillProgress> surplus = new List<ISkillProgress>();  //ループ処理で使うリスト
        foreach (var id in _progressIdList)
        {       //progressId型のリストをISkillProgress型のリストへ
            surplus.Add(ConvertIdToISkillProgress(id));
        }

        do
        {
            foreach (var progress in surplus.ToArray()) //途中でsurplusを変更できるループ
            {
                surplus.Remove(progress);
                if (progress is ISkillLoopStartProgress)
                {
                    var result = CompressProcess((ISkillLoopStartProgress)progress, surplus);
                    list.Add(result.r_compress);

                    surplus = new List<ISkillProgress>(result.r_surplus);
                    Debug.Log(surplus.Count);
                    break;
                }
                else
                {
                    list.Add(progress);
                }
            }
        } while (surplus.Count > 0);
        return list;
    }

    (ISkillProgress r_compress, List<ISkillProgress> r_surplus) CompressProcess(ISkillLoopStartProgress compress, List<ISkillProgress> surplus)
    {   //ループ系処理を圧縮する処理
        (ISkillProgress r_compress, List<ISkillProgress> r_surplus) result = default;
        Type compressEnd = compress.GetLoopEndProgressType();
        do
        {
            foreach (ISkillProgress progress in surplus.ToArray())
            {
                surplus.Remove(progress);
                if (progress is ISkillLoopStartProgress)
                {
                    var compResult = CompressProcess((ISkillLoopStartProgress)progress, surplus);
                    compress.AddLoopProgressList(compResult.r_compress);
                    surplus = compResult.r_surplus;
                    break;
                }
                else if (progress.GetType().Equals(compressEnd))
                {
                    Debug.Log(progress.GetType());
                    result.r_compress = compress;
                    result.r_surplus = surplus;
                    return result;
                }
                else
                {
                    compress.AddLoopProgressList(progress);
                }
            }
        } while (surplus.Count > 0);

        return result;
    }

    public ISkillProgress ConvertIdToISkillProgress(progressId id) //string型のidを渡すとISkillProgressのインスタンスで返してくれる
    {
        switch (id)
        {
            case progressId.TargetBall:
                return new TargetBall(1);
            case progressId.MechanicsDamage:
                return new MechanicsDamage(1);
            case progressId.MechanicsGenerateCube:
                return new MechanicsGenerateCube(1);
            case progressId.SystemLoopStart:
                return new SystemLoopStart(1);
            case progressId.SystemLoopEnd:
                return new SystemLoopEnd(1);
            case progressId.SystemWayStart:
                return new SystemWayStart(1);
            case progressId.SystemWayEnd:
                return new SystemWayEnd(1);
        }
        return null;
    }
}
