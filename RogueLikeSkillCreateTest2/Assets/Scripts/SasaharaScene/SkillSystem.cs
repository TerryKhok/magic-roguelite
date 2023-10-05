using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISkillCell{
    public async void RunProcess();
}

interface ISkill{
    public async void Run();
}

public class Skill : ISkill
{
    public async void Run(){
        
    }
}