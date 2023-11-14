using SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCreater : MonoBehaviour
{
    [SerializeField] List<ProgressId> _progressIdList = new();
    [SerializeField] List<SkillPartsData> _partsDataList = new();

    public Skill GetSkill()
    {
        Skill skill = new(_progressIdList);
        foreach (var part in _partsDataList)
        {
            Debug.Log(part);
            part.GenerateRandomData();
            SkillCompileElements elem = skill.GetElem();
            elem.AddPartsData(part);
        }
        skill.GiveElemEffect();
        return skill;
    }
}
