using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillCompile : MonoBehaviour
{
    [SerializeField] List<ProgressId> _progressIdList = new List<ProgressId>();

    public List<SkillProgress> GetSkill()
    {
        return Compile();
    }



    List<SkillProgress> Compile()
    {
        List<SkillProgress> list = new List<SkillProgress>(); //最終的に返すリスト
        List<SkillProgress> surplus = new List<SkillProgress>();  //ループ処理で使うリスト
        foreach (var id in _progressIdList)
        {       //progressId型のリストをSkillProgress型のリストへ
            surplus.Add(ConvertIdToISkillProgress(id));
        }

        do
        {
            foreach (var progress in surplus.ToArray()) //途中でsurplusを変更できるループ
            {
                surplus.Remove(progress);
                if (progress is SkillLoopStartProgress)
                {
                    var result = CompressProcess((SkillLoopStartProgress)progress, surplus);
                    list.Add(result.r_compress);

                    surplus = new List<SkillProgress>(result.r_surplus);
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

    (SkillProgress r_compress, List<SkillProgress> r_surplus) CompressProcess(SkillLoopStartProgress compress, List<SkillProgress> surplus)
    {   //ループ系処理を圧縮する処理
        (SkillProgress r_compress, List<SkillProgress> r_surplus) result = default;
        Type compressEnd = compress.GetLoopEndProgressType();
        do
        {
            foreach (SkillProgress progress in surplus.ToArray())
            {
                surplus.Remove(progress);
                if (progress is SkillLoopStartProgress)
                {
                    var compResult = CompressProcess((SkillLoopStartProgress)progress, surplus);
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

    public SkillProgress ConvertIdToISkillProgress(ProgressId id) //string型のidを渡すとSkillProgressのインスタンスで返してくれる
    {
        switch (id)
        {
            case ProgressId.TargetBall:
                return new TargetBall(1);
            case ProgressId.MechanicsDamage:
                return new MechanicsDamage(1);
            case ProgressId.MechanicsGenerateCube:
                return new MechanicsGenerateCube(1);
            case ProgressId.SystemLoopStart:
                return new SystemLoopStart(1);
            case ProgressId.SystemLoopEnd:
                return new SystemLoopEnd(1);
            case ProgressId.SystemWayStart:
                return new SystemWayStart(1);
            case ProgressId.SystemWayEnd:
                return new SystemWayEnd(1);
        }
        return null;
    }
}
