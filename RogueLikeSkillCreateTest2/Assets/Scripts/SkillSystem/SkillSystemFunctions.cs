using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

public class SkillSystemFunctions
{
    List<ProgressId> m_progressIds = new() { ProgressId.TargetBall, ProgressId.MechanicsDamage };
    List<SkillProgress> m_mechanicsAttackFunction = new();
}
