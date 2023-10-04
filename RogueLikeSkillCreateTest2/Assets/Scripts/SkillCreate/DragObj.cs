using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class DragObj : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{

    [SerializeField] int processID;     //実行する処理のID
    [SerializeField] int reqAtt;
    private Vector2 prevPos; //保存しておく初期position
    private RectTransform rectTransform; // 移動したいオブジェクトのRectTransform
    private RectTransform parentRectTransform; // 移動したいオブジェクトの親(Panel)のRectTransform

    public int getProcessID() { return processID; }
    public int getReqAtt() { return reqAtt; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent as RectTransform;
    }


    // ドラッグ開始時の処理
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ドラッグ前の位置を記憶しておく
        // RectTransformの場合はpositionではなくanchoredPositionを使う
        prevPos = rectTransform.anchoredPosition;

    }

    // ドラッグ中の処理
    public void OnDrag(PointerEventData eventData)
    {
        // eventData.positionから、親に従うlocalPositionへの変換を行う
        // オブジェクトの位置をlocalPositionに変更する

        Vector2 localPosition = GetLocalPosition(eventData.position);
        rectTransform.anchoredPosition = localPosition;
    }

    // ドラッグ終了時の処理
    public void OnEndDrag(PointerEventData eventData)
    {
        // オブジェクトをドラッグ前の位置に戻す
        rectTransform.anchoredPosition = prevPos;


    }

    // ScreenPositionからlocalPositionへの変換関数
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;

        // screenPositionを親の座標系(parentRectTransform)に対応するよう変換する.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPosition, Camera.main, out result);

        return result;
    }

    // ドロップした位置を基準に発生
    public void OnDrop(PointerEventData eventData)
    {
        GameObject scc = GetSCCellOnMouse(eventData);   //SCCellがあればsccに代入
        if (scc != null && !scc.GetComponent<SCCellScript>().isRestrict(processID))
        {
            scc.GetComponent<SCCellScript>().DropPartsObj(gameObject);
        }
    }

    GameObject GetSCCellOnMouse(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == "SCCell")
            {
                return result.gameObject;
            }
        }
        return null;
    }
}


