using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCreater : MonoBehaviour
{
    [SerializeField] List<ProgressId> _progressIdList = new List<ProgressId>();

    public List<SkillProgress> GetSkill()
    {
        return SkillCompile.Compile(_progressIdList);
    }
}
