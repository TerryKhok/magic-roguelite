using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SCCellScript : MonoBehaviour, IPointerClickHandler
{
    private SkillCreateScript scs;  //�e�I�u�W�F�N�g�̃X�N���v�g�iCell�ǉ��������Ɏg�p�j
    private int SCPartsID;     //Cell�Ƀh���b�v�����p�[�c�̏���ID
    private int restrict;   //Cell�Ƀh���b�v�ł���p�[�c�̐���
    /*  restrict�̈ꗗ
     * 1: Trigger Only      2: Target Only          3: Mechanics Only
     * 4: Attribute Only    5: Target or Mechanics
     */
    private bool att = false;
    private int attValue;   //�ϐ��Ƃ��Ă̒l

    public int getSCParts() { return SCPartsID; }
    public int getRestrict() { return restrict; }
    public int getAttValue() { return attValue; }
    public void setSCParts(int id) { SCPartsID = id; }
    public void setRestrict(int res) { restrict = res; }

    public SCCellScript Init(bool att = false)  //������
    {
        scs = GetComponentInParent<SkillCreateScript>();
        this.att = att;
        return this;
    }
    public bool isRestrict(int id)  //Id���p�[�c�����Ɉ����������Ă��邩
    {
        int codetype = id / 1000;
        if (codetype == 5 && restrict != 1 && restrict != 4) return false;   //�ŏ��ȊO�̓V�X�e��OK
        switch (restrict)
        {
            case 1:
                if (codetype == 1) return false; break;
            case 2:
                if (codetype == 2) return false; break;
            case 3:
                if (codetype == 3) return false; break;
            case 4:
                if (codetype == 4) return false; break;
            case 5:
                if (codetype == 2 || codetype == 3) return false; break;
        }
        return true;
    }

    public bool isAtt() //����Cell���ϐ����i�����Ȃ�false�j
    {
        return att;
    }

    public void DropPartsObj(GameObject obj)    //Cell�Ƀp�[�c���h���b�v�������̏���
    {
        DragObj dragObj = obj.GetComponent<DragObj>();
        GetComponent<SCCellScript>().setSCParts(dragObj.getProcessID());
        GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
        GetComponent<Image>().color = obj.GetComponent<Image>().color;
        switch (dragObj.getProcessID() / 1000)  //�p�[�c�̎�ޕʂ̏����iTri,Tar,Mec etc..�j
        {
            /*  restrict�̈ꗗ
             * 1: Trigger Only      2: Target Only          3: Mechanics Only
             * 4: Attribute Only    5: Target or Mechanics
             * System��Trigger/Attribute�ȊO�ɑΉ�
            */
            case 1:
                scs.GenerateSCCells(2); //Trigger�̎���Target Only
                break;
            case 2:
                scs.GenerateSCAttCells(dragObj.getReqAtt());
                scs.GenerateSCCells(5); //Target�̎���Target or Mechanics
                break;
            case 3:
                scs.GenerateSCAttCells(dragObj.getReqAtt());
                scs.GenerateSCCells(5); //Mechanics�̎���Target or Mechanics
                break;
            case 5:
                scs.GenerateSCAttCells(dragObj.getReqAtt());
                scs.GenerateSCCells(5); //System�̎���Target or Mechanics
                break;
        }
    }

    //Cell���N���b�N�����Ƃ��̏���
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerId == -2)  //�E�N���b�N�iCell����p�[�c���O�������j
        {
            setSCParts(0);
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = Color.white;
            if (!isAtt())  //�������̂�AttCell����Ȃ��Ȃ�
            {
                scs.RefreshSCCells();   //������Cell�̍폜
            }
        }
        else if (eventData.pointerId == -1 && isAtt())    //���N���b�N�iAtt�̒l�ύX�j
        {
            if (attValue < 7)   //0~7��8��
            {
                attValue++;
            }
            else
            {
                attValue = 0;
            }
            TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null)    //�q��TMP�����I�u�W�F�N�g���Ȃ��Ȃ琶�����Ă��낢��ݒ肷��
            {
                GameObject obj = new GameObject("text");
                obj.AddComponent<RectTransform>().AddComponent<TextMeshProUGUI>();
                obj.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                rectTransform.sizeDelta = new Vector2(80, 50);
                rectTransform.localScale = Vector3.one;
                tmp = obj.GetComponent<TextMeshProUGUI>();
                tmp.alignment = TextAlignmentOptions.Center;
            }
            tmp.text = attValue.ToString(); //attValue��text�\��
        }
    }
}