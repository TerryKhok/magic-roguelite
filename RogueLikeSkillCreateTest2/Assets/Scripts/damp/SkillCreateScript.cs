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
    public int id;   //�R�[�h�{��
    public List<int> variables;    //�ϐ�

    public Code(int id, List<int> variables)
    {
        this.id = id;
        this.variables = new List<int>(variables);
    }
};
public class SkillCreateScript : MonoBehaviour
{

    [SerializeField] GameObject SCCell; //cell��prefab������
    [SerializeField] GameObject Player;
    private List<Code> skillCode = new List<Code>();
    private RectTransform rectTransform;//cell�����̎��Ɏg��
    public List<GameObject> cells = new List<GameObject>();    //skillCode�����p��cell�ۑ����X�g
    private Vector2 size;
    private Vector2 genePos;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        size = SCCell.GetComponent<RectTransform>().sizeDelta;  //cell�̃T�C�Y���擾
        genePos = new Vector2(size.x / -2, 0);    //�����ʒu�������
        GenerateSCCells(1);
    }

    // cell�̐���
    public void GenerateSCCells(int res)
    {
        GameObject cell;    //��������prefab�̈ꎞ�ۑ��p

        foreach (GameObject obj in cells)
        {
            obj.transform.localPosition += new Vector3(size.x / 2 * -1, 0, 0);
        }
        cell = Instantiate(SCCell, genePos, Quaternion.identity);   //cell����
        cell.transform.SetParent(rectTransform, false);     //��������cell���q�ɂ���
        cell.GetComponent<SCCellScript>().Init().setRestrict(res); //�p�[�c�̎��t������������
        cells.Add(cell);    //skillCode�����p��list�ɒǉ�
        genePos.x += size.x / 2;//cell�𐶐�����ʒu���E��
    }

    public void GenerateSCAttCells(int cnt)
    {
        GameObject cell;    //��������prefab�̈ꎞ�ۑ��p

        Vector2 prevGenePos = genePos;  //genePos���Ō�Ɍ��ɖ߂�
        genePos.x -= size.x / 2;    //�ʒu����
        for (int i = 0; i < cnt; i++)
        {
            genePos.y -= size.y;    //genePos�����Ɉړ�
            cell = Instantiate(SCCell, genePos, Quaternion.identity);   //cell����
            cell.transform.SetParent(rectTransform, false);     //��������cell���q�ɂ���
            cell.GetComponent<SCCellScript>().Init(true).setRestrict(4); //�p�[�c�̎��t������Att Only������
            cells.Add(cell);    //skillCode�����p��list�ɒǉ�
        }
        genePos = prevGenePos;
    }

    public void RefreshSCCells()    //������cell���폜����
    {
        bool removeFlag = false;    //�폜�t���O
        int removeCnt = 0;          //�폜����Att�ȊO��Cell�̐��i�ʒu�����p�j
        foreach (GameObject obj in cells.ToArray())   //cell���X�g�����[�v
        {
            SCCellScript sccs = obj.GetComponent<SCCellScript>();
            if (!removeFlag)
            {
                if (sccs.getSCParts() == 0 && !sccs.isAtt())  //�p�[�c���Ȃ���AttCell����Ȃ��Ȃ�
                {
                    removeFlag = true;
                }
            }
            else
            {
                //removeFlag��true�̎�
                cells.Remove(obj);
                Destroy(obj);
                if (!sccs.isAtt()) { removeCnt++; } //�����������J�E���g
            }
        }

        for (int i = 0; i < removeCnt; i++) //����������Cell�̈ʒu��߂�
        {
            foreach (GameObject obj in cells)   //GenerateSCCells�̋t�ɂ��炵�Ă���
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
            if (!cell.GetComponent<SCCellScript>().isAtt()) //Att�Ə����l��e��
            {
                if (id != 0)
                {
                    skillCode.Add(new Code(id, variables)); //Att�t�^�ҋ@��Ԃ���R�[�h�����Ƃ��Ēǉ�
                    //ShowSkillCodeLog();
                }
                id = standby;
                variables.Clear();
            }
            else
            {
                variables.Add(cell.GetComponent<SCCellScript>().getAttValue()); //Att�ǉ�����
            }
        }
    }

    /*
     * �X�L���R�[�h�̔ԍ�(ID)�ɂ���
     * 0~999    :���蓖�ĂȂ��B
     * 1000~1999:Trigger    �X�L�������^�C�~���O���w�肷��BSystem�R�[�h�ŕϐ��̏��������s��
     * 2000~2999:Target     �X�L���̑ΏۂƂȂ�^�[�Q�b�g��ϐ��Ɋi�[������A���̏����̈ʒu���߂��s��
     * 3000~3999:Mechanics  �^�[�Q�b�g�̔j��Ȃǂ̌��ʕt�^���s��
     * 4000~4999:Attribute  �ق��̃R�[�h�ɑ΂��Ĉ�����^����
     * 5000~5999:System     �ϐ��̏������Ȃǂ̃V�X�e���I�ȏ����������ł܂Ƃ߂Ă����B���Ԃ񃋁[�v�Ƃ��ǉ�����
     */
    public void runSkillCode()
    {
        StartCoroutine(runSkillCodeCoroutine(0, null, Vector2.zero));
    }

    public void runSkillCode(int start, List<GameObject> targets, Vector2 pos)
    {
        StartCoroutine(runSkillCodeCoroutine(start, targets, pos));
    }

    private IEnumerator runSkillCodeCoroutine(int start, List<GameObject> targets, Vector2 pos)  //�X�L���R�[�h�̎��s
    {
        if (start == 0)
        {
            targets = new List<GameObject>();  //Mec�Ō��ʂ�^����^�[�Q�b�g�̃��X�g
            pos = new Vector2();    //Tar,Mec�Ŏg��pos�̃��X�g
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
                        Debug.Log("�g���K�[");
                        runTriggerCode(code, ref targets, ref pos, in cnt);
                        break;
                    case 2:
                        Debug.Log("�^�[�Q�b�g");
                        runTargetCode(code, ref targets, ref pos, in cnt);
                        break;
                    case 3:
                        Debug.Log("���J�j�N�X");
                        runMechanicsCode(code, ref targets, ref pos, in cnt);
                        break;
                    /*
                    case 4:
                        Debug.Log("�A�g���r���[�g");
                        runAttributeCode(code,ref targets,ref pos);
                        break;
                    */
                    case 5:
                        Debug.Log("�V�X�e��");
                        runSystemCode(code, ref targets, ref pos, in cnt);
                        break;
                }
                if (code.id % 1000 >= 500) //id��100�̈ʂ�5�ȏゾ������coroutine���~�߂�
                {
                    Debug.Log("break");
                    yield break;
                }
            }
            yield return null;
            //yield return new WaitForSeconds(1);
        }
    }

    private Code tempCode(int id)   //�ϐ��Ȃ��̈ꎞ�I�ȃR�[�h���ȒP�ɐ����ł���
    {
        Code code;
        code.id = id;
        code.variables = null;
        return code;
    }

    private void runTriggerCode(Code code, ref List<GameObject> targets, ref Vector2 pos, in int cnt)
    {
        runSystemCode(tempCode(5001), ref targets, ref pos, in cnt);    //pos��clear
        runSystemCode(tempCode(5002), ref targets, ref pos, in cnt);    //targets��clear
        pos = Player.transform.position;        //pos���v���C���[�̈ʒu�ɂ���
    }

    private void runTargetCode(Code code, ref List<GameObject> targets, ref Vector2 pos, in int cnt)
    {
        Vector2 vec = Vector2.zero;
        switch (code.id)
        {
            case 2001:  //Ray���΂�
                vec = ConvertNumToVector2(GetCodeVariable(in code, 0, 0));
                int distance = GetCodeVariable(in code, 1, 10);

                RaycastHit2D hit = Physics2D.Raycast(pos, vec, distance);
                Debug.DrawRay(pos, vec, Color.red, 100);
                if (hit.collider != null)
                {
                    targets.Add(hit.collider.gameObject);
                }
                break;
            case 2501:  //�����΂�
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
            case 5010:  //�����𑝂₷
                int way = GetCodeVariable(in code, 0, 0);
                int wait = GetCodeVariable(in code, 1, 0);
                StartCoroutine(ProcessSystem5010(way, wait, cnt, targets, pos));
                break;
        }
    }

    //Code�̎w�肵���ϐ���Ԃ��֐��B�ϐ����Ȃ�������3�ڂ̈���(default)��Ԃ�
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

    //���l��^����Ɣ�������Vector2�ɕϊ�����
    private Vector2 ConvertNumToVector2(int num)
    {
        /*
         * ���v����Vector2��Ԃ�
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

    //�f�o�b�O�p�̃��O
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











    //==========================������Ɠ���ȏ���==================================//
    private IEnumerator ProcessSystem5010(int time, int wait, int cnt, List<GameObject> targets, Vector2 pos)
    {
        for (int i = 0; i < time; i++)
        {
            yield return new WaitForSeconds(wait);
            runSkillCode(cnt, new List<GameObject>(targets), pos);
        }
    }
}
