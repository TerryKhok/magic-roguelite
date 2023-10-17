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
    private SkillCreateScript scs;  //親オブジェクトのスクリプト（Cell追加生成時に使用）
    private int SCPartsID;     //Cellにドロップしたパーツの処理ID
    private int restrict;   //Cellにドロップできるパーツの制限
    /*  restrictの一覧
     * 1: Trigger Only      2: Target Only          3: Mechanics Only
     * 4: Attribute Only    5: Target or Mechanics
     */
    private bool att = false;
    private int attValue;   //変数としての値

    public int getSCParts() { return SCPartsID; }
    public int getRestrict() { return restrict; }
    public int getAttValue() { return attValue; }
    public void setSCParts(int id) { SCPartsID = id; }
    public void setRestrict(int res) { restrict = res; }

    public SCCellScript Init(bool att = false)  //初期化
    {
        scs = GetComponentInParent<SkillCreateScript>();
        this.att = att;
        return this;
    }
    public bool isRestrict(int id)  //Idがパーツ制限に引っかかっているか
    {
        int codetype = id / 1000;
        if (codetype == 5 && restrict != 1 && restrict != 4) return false;   //最初以外はシステムOK
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

    public bool isAtt() //このCellが変数か（処理ならfalse）
    {
        return att;
    }

    public void DropPartsObj(GameObject obj)    //Cellにパーツをドロップした時の処理
    {
        DragObj dragObj = obj.GetComponent<DragObj>();
        GetComponent<SCCellScript>().setSCParts(dragObj.getProcessID());
        GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
        GetComponent<Image>().color = obj.GetComponent<Image>().color;
        switch (dragObj.getProcessID() / 1000)  //パーツの種類別の処理（Tri,Tar,Mec etc..）
        {
            /*  restrictの一覧
             * 1: Trigger Only      2: Target Only          3: Mechanics Only
             * 4: Attribute Only    5: Target or Mechanics
             * SystemはTrigger/Attribute以外に対応
            */
            case 1:
                scs.GenerateSCCells(2); //Triggerの次はTarget Only
                break;
            case 2:
                scs.GenerateSCAttCells(dragObj.getReqAtt());
                scs.GenerateSCCells(5); //Targetの次はTarget or Mechanics
                break;
            case 3:
                scs.GenerateSCAttCells(dragObj.getReqAtt());
                scs.GenerateSCCells(5); //Mechanicsの次はTarget or Mechanics
                break;
            case 5:
                scs.GenerateSCAttCells(dragObj.getReqAtt());
                scs.GenerateSCCells(5); //Systemの次はTarget or Mechanics
                break;
        }
    }

    //Cellをクリックしたときの処理
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerId == -2)  //右クリック（Cellからパーツを外す処理）
        {
            setSCParts(0);
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = Color.white;
            if (!isAtt())  //消したのがAttCellじゃないなら
            {
                scs.RefreshSCCells();   //無効なCellの削除
            }
        }
        else if (eventData.pointerId == -1 && isAtt())    //左クリック（Attの値変更）
        {
            if (attValue < 7)   //0~7の8こ
            {
                attValue++;
            }
            else
            {
                attValue = 0;
            }
            TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null)    //子にTMPを持つオブジェクトがないなら生成していろいろ設定する
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
            tmp.text = attValue.ToString(); //attValueをtext表示
        }
    }
}