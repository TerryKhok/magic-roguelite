using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SkillSystem
{
    //IDを列挙。使うのはSkillCompile.csのConvertIdToISkillProgress関数
    public enum ProgressId
    {
        None,
        TargetBall,
        MechanicsDamage,
        MechanicsGenerateCube,
        SystemLoopStart,
        SystemLoopEnd,
        SystemWayStart,
        SystemWayEnd,
    }

    public enum SkillPartsId
    {
        AttackInc,
        RangeInc,
        SpeedInc,
        BootInc,
        ThreeWay,
        Loop,
    }


    public struct Skill
    {
        SkillCompileElements elem;
        List<ProgressId> idList;
        List<SkillProgress> progress;
        public Skill(List<ProgressId> ids)
        {
            elem = new SkillCompileElements(0);
            idList = new List<ProgressId>(ids);
            progress = new List<SkillProgress>(SkillCompile.Compile(idList));
        }

        public SkillCompileElements GetElem() { return elem; }
        public List<SkillProgress> GetProgress() { return progress; }

        public void GiveElemEffect() { progress = SkillCompile.ElemWrap(progress, elem); }
    }

    public struct SkillCompileElements
    {
        List<SkillPartsData> partsData;

        public SkillCompileElements(int a)
        {
            partsData = new();
        }
        public List<SkillPartsData> GetPartsData() { return partsData; }

        public void AddPartsData(SkillPartsData d) { partsData.Add(d); }
    }

    public struct SkillElements
    {
        LocationData ld;
        List<GameObject> targets;
        SkillAttributes attr;
        bool casterIsPlayer;    //true=player, false=enemy
        public SkillElements(Transform t)
        {
            ld = new LocationData(t);
            targets = new List<GameObject>();
            attr = new SkillAttributes(0);
            casterIsPlayer = true;
        }
        public LocationData GetLocationData() { return ld; }
        public List<GameObject> GetTargets() { return targets; }
        public SkillAttributes GetAttr() { return attr; }
        public bool IsPlayer() { return casterIsPlayer; }

        public void AddTargets(GameObject obj) { targets.Add(obj); }
        public void SetLocationData(Transform tf) { ld.ResetData(tf); }
        public void SetLocationData(Vector3 pos, Quaternion rotate) { ld.ResetData(pos, rotate); }
        public void SetCasterType(bool c) { casterIsPlayer = c; }

        public bool IsTarget(GameObject obj)
        {
            if (casterIsPlayer)
            {
                return obj.CompareTag("Enemy");
            }
            else
            {
                return obj.CompareTag("Player");
            }
        }
    }

    public struct LocationData
    {
        Vector3 pos;
        Quaternion rotate;
        public LocationData(Transform tf)
        {
            pos = tf.position;
            rotate = tf.rotation;
        }

        public void ResetData(Transform tf)
        {
            pos = tf.position;
            rotate = tf.rotation;
        }
        public void ResetData(Vector3 pos, Quaternion rotate)
        {
            this.pos = pos;
            this.rotate = rotate;
        }
        public Vector3 GetPos() { return pos; }
        public Quaternion GetRotate() { return rotate; }
    }

    public struct SkillAttributes
    {
        Dictionary<string, SkillCorrection> attr;
        public SkillAttributes(int a)
        {
            attr = new();
        }
        public SkillCorrection GetCorrection(string key) { return attr[key]; }
        public bool IsExist(string key) { return attr.ContainsKey(key); }

        public void AddAttr(string key, SkillCorrection value)
        {
            if (attr.ContainsKey(key))
            {
                SkillCorrection temp;
                attr.TryGetValue(key, out temp);
                temp.AddFixed(value.GetFixedValue());
                temp.AddMulti(value.GetMulti());
            }
            else
            {
                attr.Add(key, value);
            }
        }
    }


    public class SkillProgress
    {
        int _tier;

        public int GetTier() { return _tier; }
        public void GetTier(int t) { _tier = t; }

        public SkillProgress(int t)
        {
            _tier = t;
            //_args = new int[5];
        }

        public virtual async UniTask<SkillElements> RunProgress(SkillElements elem, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(0);
            return elem;
        }

        public virtual void RunProgressNoWait(SkillElements elem, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
        }
    }

    //ループ処理が含まれるノードに実装するインターフェイス
    public abstract class SkillLoopStartProgress : SkillProgress
    {
        public SkillLoopStartProgress(int t) : base(t) { }
        public abstract void AddLoopProgressList(SkillProgress progress);   //ループ内で実行する処理を追加
        public abstract Type GetLoopEndProgressType();                       //ループ処理終了ノードの取得用（要設定）
    }


    //スキルデータベース
    //1つの変数の保存形式（名前とティアに対応する値の配列）
    public struct SkillVariable
    {
        public string name;
        public int[] values;
    }

    public static class SkillDB
    {
        public static Dictionary<ProgressId, List<SkillVariable>> g_SkillVariableMap = new Dictionary<ProgressId, List<SkillVariable>>();

        //データのロード
        public static void Initialize()
        {
            TextAsset resource = Resources.Load("SkillSystem/SkillSystem_Variables") as TextAsset;  //csvをロード
            StringReader reader = new StringReader(resource.text);  //1行ずつ読み込む
            while (reader.Peek() != -1) //まだ行が読めるなら
            {
                ProgressId id = default;    //マップのKey用
                List<SkillVariable> list = new List<SkillVariable>(); //マップのValue用
                SkillVariable v = new SkillVariable();
                foreach (var str in reader.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries))    //何もないやつをスルーして,ごとの区切りでループ
                    if (id == default)
                    {
                        try
                        {
                            id = (ProgressId)Enum.Parse(typeof(ProgressId), str);   //行のはじめをProgressIdに変換
                        }
                        catch
                        {
                            Debug.Log("存在しない処理ノードIDです / " + str);       //変換できないならログ出して次の行
                            break;
                        }
                    }
                    else
                    {
                        if (v.name == default)
                        {
                            v.name = str;
                        }
                        else
                        {
                            v.values = str.Split("|").Select(int.Parse).ToArray();
                            list.Add(v);
                            v = new SkillVariable();
                        }
                    }
                if (id != default)
                    g_SkillVariableMap.Add(id, list);
            }
        }

        public static string GetSkillVariableName(ProgressId id, int idx)
        {
            string result;
            try
            {
                result = g_SkillVariableMap.GetValueOrDefault(id)[idx].name;
            }
            catch
            {
                Debug.Log("DBに値が設定されていません");
                throw;
            }
            return result;
        }

        public static int GetSkillVariableValue(ProgressId id, int idx, int tier)
        {
            int result;
            try
            {
                result = g_SkillVariableMap.GetValueOrDefault(id)[idx].values[tier];
            }
            catch
            {
                Debug.Log("DBに値が設定されていません");
                throw;
            }
            return result;
        }
    }





    //スキルコンパイル
    public static class SkillCompile
    {
        public static List<SkillProgress> Compile(List<ProgressId> idList)
        {
            List<SkillProgress> list = new List<SkillProgress>(); //最終的に返すリスト
            List<SkillProgress> surplus = new List<SkillProgress>();  //ループ処理で使うリスト
            foreach (var id in idList)
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

        static (SkillProgress r_compress, List<SkillProgress> r_surplus) CompressProcess(SkillLoopStartProgress compress, List<SkillProgress> surplus)
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

        public static List<SkillProgress> ElemWrap(List<SkillProgress> progressList, SkillCompileElements elem)
        {
            List<SkillProgress> result = new List<SkillProgress>(progressList);
            SkillLoopStartProgress prog;
            foreach (SkillPartsData part in elem.GetPartsData())
            {
                switch (part.GetId())
                {
                    case SkillPartsId.ThreeWay:
                        prog = new SystemWayStart(0);
                        foreach (var progress in result)
                        {
                            prog.AddLoopProgressList(progress);
                        }
                        result.Clear();
                        result.Add(prog);
                        break;
                    case SkillPartsId.Loop:
                        prog = new SystemLoopStart(0);
                        foreach (var progress in result)
                        {
                            prog.AddLoopProgressList(progress);
                        }
                        result.Clear();
                        result.Add(prog);
                        break;
                }
            }
            return result;
        }

        public static SkillProgress ConvertIdToISkillProgress(ProgressId id) //string型のidを渡すとSkillProgressのインスタンスで返してくれる
        {
            switch (id)
            {
                case ProgressId.TargetBall:
                    return new TargetBall(0);
                case ProgressId.MechanicsDamage:
                    return new MechanicsDamage(0);
                case ProgressId.MechanicsGenerateCube:
                    return new MechanicsGenerateCube(0);
                case ProgressId.SystemLoopStart:
                    return new SystemLoopStart(0);
                case ProgressId.SystemLoopEnd:
                    return new SystemLoopEnd(0);
                case ProgressId.SystemWayStart:
                    return new SystemWayStart(0);
                case ProgressId.SystemWayEnd:
                    return new SystemWayEnd(0);
            }
            return null;
        }
    }

}
