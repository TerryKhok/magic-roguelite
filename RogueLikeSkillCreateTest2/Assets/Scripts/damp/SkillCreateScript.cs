using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public struct Code
{
    public int id;   //コード本体
    public List<int> variables;    //変数

    public Code(int id, List<int> variables)
    {
        this.id = id;
        this.variables = new List<int>(variables);
    }
};
public class SkillCreateScript : MonoBehaviour
{

    [SerializeField] GameObject SCCell; //cellのprefabを入れる
    [SerializeField] GameObject Player;
    private List<Code> skillCode = new List<Code>();
    private RectTransform rectTransform;//cell生成の時に使う
    public List<GameObject> cells = new List<GameObject>();    //skillCode生成用のcell保存リスト
    private Vector2 size;
    private Vector2 genePos;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        size = SCCell.GetComponent<RectTransform>().sizeDelta;  //cellのサイズを取得
        genePos = new Vector2(size.x / -2, 0);    //生成位置を左上に
        GenerateSCCells(1);
    }

    // cellの生成
    public void GenerateSCCells(int res)
    {
        GameObject cell;    //生成したprefabの一時保存用

        foreach (GameObject obj in cells)
        {
            obj.transform.localPosition += new Vector3(size.x / 2 * -1, 0, 0);
        }
        cell = Instantiate(SCCell, genePos, Quaternion.identity);   //cell生成
        cell.transform.SetParent(rectTransform, false);     //生成したcellを子にする
        cell.GetComponent<SCCellScript>().Init().setRestrict(res); //パーツの取り付け制限をつける
        cells.Add(cell);    //skillCode生成用のlistに追加
        genePos.x += size.x / 2;//cellを生成する位置を右に
    }

    public void GenerateSCAttCells(int cnt)
    {
        GameObject cell;    //生成したprefabの一時保存用

        Vector2 prevGenePos = genePos;  //genePosを最後に元に戻す
        genePos.x -= size.x / 2;    //位置調整
        for (int i = 0; i < cnt; i++)
        {
            genePos.y -= size.y;    //genePosを下に移動
            cell = Instantiate(SCCell, genePos, Quaternion.identity);   //cell生成
            cell.transform.SetParent(rectTransform, false);     //生成したcellを子にする
            cell.GetComponent<SCCellScript>().Init(true).setRestrict(4); //パーツの取り付け制限Att Onlyをつける
            cells.Add(cell);    //skillCode生成用のlistに追加
        }
        genePos = prevGenePos;
    }

    public void RefreshSCCells()    //無効なcellを削除する
    {
        bool removeFlag = false;    //削除フラグ
        int removeCnt = 0;          //削除したAtt以外のCellの数（位置調整用）
        foreach (GameObject obj in cells.ToArray())   //cellリストをループ
        {
            SCCellScript sccs = obj.GetComponent<SCCellScript>();
            if (!removeFlag)
            {
                if (sccs.getSCParts() == 0 && !sccs.isAtt())  //パーツがなくてAttCellじゃないなら
                {
                    removeFlag = true;
                }
            }
            else
            {
                //removeFlagがtrueの時
                cells.Remove(obj);
                Destroy(obj);
                if (!sccs.isAtt()) { removeCnt++; } //消した個数をカウント
            }
        }

        for (int i = 0; i < removeCnt; i++) //消した個数分Cellの位置を戻す
        {
            foreach (GameObject obj in cells)   //GenerateSCCellsの逆にずらしている
            {
                obj.transform.localPosition += new Vector3(size.x / 2, 0, 0);
            }
            genePos.x -= size.x / 2;
        }
    }

    public void SkillCompile()
    {
        skillCode.Clear();
        int id = 0;
        int standby;
        List<int> variables = new List<int>();
        foreach (GameObject cell in cells)
        {
            standby = cell.GetComponent<SCCellScript>().getSCParts();
            if (!cell.GetComponent<SCCellScript>().isAtt()) //Attと初期値を弾く
            {
                if (id != 0)
                {
                    skillCode.Add(new Code(id, variables)); //Att付与待機状態からコード完了として追加
                    //ShowSkillCodeLog();
                }
                id = standby;
                variables.Clear();
            }
            else
            {
                variables.Add(cell.GetComponent<SCCellScript>().getAttValue()); //Att追加処理
            }
        }
    }

    /*
     * スキルコードの番号(ID)について
     * 0~999    :割り当てなし。
     * 1000~1999:Trigger    スキル発動タイミングを指定する。Systemコードで変数の初期化も行う
     * 2000~2999:Target     スキルの対象となるターゲットを変数に格納したり、次の処理の位置決めを行う
     * 3000~3999:Mechanics  ターゲットの破壊などの効果付与を行う
     * 4000~4999:Attribute  ほかのコードに対して引数を与える
     * 5000~5999:System     変数の初期化などのシステム的な処理をここでまとめておく。たぶんループとか追加する
     */
    public void runSkillCode()
    {
        StartCoroutine(runSkillCodeCoroutine(0, null, Vector2.zero));
    }

    public void runSkillCode(int start, List<GameObject> targets, Vector2 pos)
    {
        StartCoroutine(runSkillCodeCoroutine(start, targets, pos));
    }

    private IEnumerator runSkillCodeCoroutine(int start, List<GameObject> targets, Vector2 pos)  //スキルコードの実行
    {
        if (start == 0)
        {
            targets = new List<GameObject>();  //Mecで効果を与えるターゲットのリスト
            pos = new Vector2();    //Tar,Mecで使うposのリスト
        }
        int cnt = 0;
        foreach (Code code in skillCode)
        {
            if (cnt < start) { cnt++; continue; }
            cnt++;
            if (code.id != 0)
            {
                int codeType = code.id / 1000;
                switch (codeType)
                {
                    case 1:
                        Debug.Log("トリガー");
                        runTriggerCode(code, ref targets, ref pos, in cnt);
                        break;
                    case 2:
                        Debug.Log("ターゲット");
                        runTargetCode(code, ref targets, ref pos, in cnt);
                        break;
                    case 3:
                        Debug.Log("メカニクス");
                        runMechanicsCode(code, ref targets, ref pos, in cnt);
                        break;
                    /*
                    case 4:
                        Debug.Log("アトリビュート");
                        runAttributeCode(code,ref targets,ref pos);
                        break;
                    */
                    case 5:
                        Debug.Log("システム");
                        runSystemCode(code, ref targets, ref pos, in cnt);
                        break;
                }
                if (code.id % 1000 >= 500) //idの100の位が5以上だったらcoroutineを止める
                {
                    Debug.Log("break");
                    yield break;
                }
            }
            yield return null;
            //yield return new WaitForSeconds(1);
        }
    }

    private Code tempCode(int id)   //変数なしの一時的なコードを簡単に生成できる
    {
        Code code;
        code.id = id;
        code.variables = null;
        return code;
    }

    private void runTriggerCode(Code code, ref List<GameObject> targets, ref Vector2 pos, in int cnt)
    {
        runSystemCode(tempCode(5001), ref targets, ref pos, in cnt);    //posのclear
        runSystemCode(tempCode(5002), ref targets, ref pos, in cnt);    //targetsのclear
        pos = Player.transform.position;        //posをプレイヤーの位置にする
    }

    private void runTargetCode(Code code, ref List<GameObject> targets, ref Vector2 pos, in int cnt)
    {
        Vector2 vec = Vector2.zero;
        switch (code.id)
        {
            case 2001:  //Rayを飛ばす
                vec = ConvertNumToVector2(GetCodeVariable(in code, 0, 0));
                int distance = GetCodeVariable(in code, 1, 10);

                RaycastHit2D hit = Physics2D.Raycast(pos, vec, distance);
                Debug.DrawRay(pos, vec, Color.red, 100);
                if (hit.collider != null)
                {
                    targets.Add(hit.collider.gameObject);
                }
                break;
            case 2501:  //球を飛ばす
                vec = ConvertNumToVector2(GetCodeVariable(in code, 0, 0));
                int speed = GetCodeVariable(in code, 1, 3);
                int pen = GetCodeVariable(in code, 2, 0);

                GameObject bullet = Player.GetComponent<PlayerScript>().getBullet();

                GameObject inst = Instantiate(bullet, Player.transform.position, Quaternion.identity);
                inst.transform.SetParent(Player.transform);
                inst.transform.position = pos;
                inst.GetComponent<Rigidbody2D>().velocity = vec * speed;
                inst.GetComponent<BulletScript>().setStart(cnt).setTargets(targets).setPos(pos).setScs(this).setPenetrate(pen);
                break;
        }
    }

    private void runMechanicsCode(Code code, ref List<GameObject> targets, ref Vector2 pos, in int cnt)
    {
        switch (code.id)
        {
            case 3001:
                foreach (GameObject target in targets.ToArray())
                {
                    targets.Remove(target);
                    Destroy(target);
                }
                break;
            case 3002:
                foreach (GameObject target in targets)
                {
                    target.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
                break;
        }
    }

    /*
    private void runAttributeCode(Code code, ref List<GameObject> targets, ref Vector2 pos)
    {
        switch (code.id)
        {
            case 4001:
                foreach (GameObject target in targets)
                {
                    Destroy(target);
                }
                break;
            case 4002:
                foreach (GameObject target in targets)
                {
                    target.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                }
                break;
        }
    }
    */

    private void runSystemCode(Code code, ref List<GameObject> targets, ref Vector2 pos, in int cnt)
    {
        switch (code.id)
        {
            case 5001:
                pos = Vector2.zero;
                break;
            case 5002:
                targets.Clear();
                break;
            case 5010:  //方向を増やす
                int way = GetCodeVariable(in code, 0, 0);
                int wait = GetCodeVariable(in code, 1, 0);
                StartCoroutine(ProcessSystem5010(way, wait, cnt, targets, pos));
                break;
        }
    }

    //Codeの指定した変数を返す関数。変数がなかったら3つ目の引数(default)を返す
    private int GetCodeVariable(in Code code, int arg, int def)
    {
        int result = def;
        if (arg < 0) { return result; }
        if (code.variables.Count > arg)
        {
            result = code.variables[arg];
        }
        return result;
    }

    //数値を与えると八方向のVector2に変換する
    private Vector2 ConvertNumToVector2(int num)
    {
        /*
         * 時計回りでVector2を返す
         * 7 0 1
         * 6   2
         * 5 4 3
         */
        while (num < 0) { num += 8; }
        Vector2 vec = new Vector2(0, 0);
        switch (num % 8)
        {
            case 0:
                vec = new Vector2(0, 1); break;
            case 1:
                vec = new Vector2(1, 1); break;
            case 2:
                vec = new Vector2(1, 0); break;
            case 3:
                vec = new Vector2(1, -1); break;
            case 4:
                vec = new Vector2(0, -1); break;
            case 5:
                vec = new Vector2(-1, -1); break;
            case 6:
                vec = new Vector2(-1, 0); break;
            case 7:
                vec = new Vector2(-1, 1); break;
        }
        vec.Normalize();
        return vec;
    }

    //デバッグ用のログ
    public void ShowSkillCodeLog()
    {
        foreach (Code code in skillCode)
        {
            int vars;
            if (code.variables.Count > 0)
            {
                vars = code.variables.Count;
            }
            else
            {
                vars = 0;
            }
            Debug.Log(code.id + " / " + vars);
        }
        Debug.Log("===================");
    }











    //==========================ちょっと特殊な処理==================================//
    private IEnumerator ProcessSystem5010(int time, int wait, int cnt, List<GameObject> targets, Vector2 pos)
    {
        for (int i = 0; i < time; i++)
        {
            yield return new WaitForSeconds(wait);
            runSkillCode(cnt, new List<GameObject>(targets), pos);
        }
    }
}
