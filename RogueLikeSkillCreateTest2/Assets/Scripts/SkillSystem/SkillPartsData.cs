using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Data/SkillPartsData")]
public class SkillPartsData : ScriptableObject
{
    [Header("追加効果のID")]
    [SerializeField]
    SkillPartsId _id;

    [Header("追加効果の量")]
    [SerializeField]
    SkillCorrection _cor;

    public SkillPartsData()
    {
        _id = 0;
        _cor = new SkillCorrection(0, 1);

    }

    public SkillPartsId GetId() { return _id; }
    public SkillCorrection GetCor() { return _cor; }
    public void GenerateRandomData()
    {
        _id = (SkillPartsId)Random.Range(0, Enum.GetValues(typeof(SkillPartsId)).Length);
        string name = Enum.GetName(typeof(SkillPartsId), _id);
        if (name.Contains("Inc"))
        {
            if (Random.Range(0, 5) < 2)
            {
                _cor.AddMulti(Random.Range(1, 3));
            }
            else
            {
                _cor.AddFixed(Random.Range(1, 3));
            }
        }
    }
}

//スキルのノード補正値
public struct SkillCorrection
{
    float fixedValue;
    float multi;

    public SkillCorrection(float val, float mul)
    {
        fixedValue = val;
        multi = mul;
    }

    public void AddFixed(float value) { fixedValue += value; }
    public void AddMulti(float value) { fixedValue *= value; multi *= value; }
    public float GetFixedValue() { return fixedValue; }
    public float GetMulti() { return multi; }
}