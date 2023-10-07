using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SkillSystem
{
    struct SkillElements
    {
        Transform tf;
        List<GameObject> targets;
        public SkillElements(Transform t)
        {
            tf = t;
            targets = new List<GameObject>();
        }
        public Transform GetTransform() { return  tf; }
        public List<GameObject> GetTargets() {  return targets; }

        public void AddTargets(GameObject obj) { targets.Add(obj); }
        public void SetTransform(Transform tf) { this.tf = tf; }
    }

    public class SkillProgress
    {
		int _tier;
        
		public int getTier() { return _tier; }
        public void setTier(int t) { _tier = t; }

       public SkillProgress(int t) { _tier = t; }
    }

    interface ISkillProgress
    {
        public UniTask<SkillElements> SkillProgress(SkillElements elem, CancellationToken token);

        public void SkillProgressNoWait(SkillElements elem, CancellationToken token);
    }
}
