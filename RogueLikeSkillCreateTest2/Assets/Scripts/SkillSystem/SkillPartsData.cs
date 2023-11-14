using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Data/SkillPartsData")]
public class SkillPartsData : ScriptableObject
{
    [Header("�ǉ����ʂ�ID")]
    [SerializeField]
    SkillPartsId m_id;

    [Header("�ǉ����ʂ̗�")]
    [SerializeField]
    SkillCorrection m_cor;

    public SkillPartsData()
    {
        m_id = 0;
        m_cor = new SkillCorrection(0, 1);

    }

    public SkillPartsId GetId() { return m_id; }
    public SkillCorrection GetCor() { return m_cor; }
    public void GenerateRandomData()
    {
        m_id = (SkillPartsId)Random.Range(0, Enum.GetValues(typeof(SkillPartsId)).Length);
        string name = Enum.GetName(typeof(SkillPartsId), m_id);
        if (name.Contains("Inc"))
        {
            if (Random.Range(0, 5) < 2)
            {
                m_cor.AddMulti(Random.Range(1, 3));
            }
            else
            {
                m_cor.AddFixed(Random.Range(1, 3));
            }
        }
    }
}

//�X�L���̃m�[�h�␳�l
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