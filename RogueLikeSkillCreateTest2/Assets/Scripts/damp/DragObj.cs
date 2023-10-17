using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class DragObj : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{

    [SerializeField] int processID;     //���s���鏈����ID
    [SerializeField] int reqAtt;
    private Vector2 prevPos; //�ۑ����Ă�������position
    private RectTransform rectTransform; // �ړ��������I�u�W�F�N�g��RectTransform
    private RectTransform parentRectTransform; // �ړ��������I�u�W�F�N�g�̐e(Panel)��RectTransform

    public int getProcessID() { return processID; }
    public int getReqAtt() { return reqAtt; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent as RectTransform;
    }


    // �h���b�O�J�n���̏���
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �h���b�O�O�̈ʒu���L�����Ă���
        // RectTransform�̏ꍇ��position�ł͂Ȃ�anchoredPosition���g��
        prevPos = rectTransform.anchoredPosition;

    }

    // �h���b�O���̏���
    public void OnDrag(PointerEventData eventData)
    {
        // eventData.position����A�e�ɏ]��localPosition�ւ̕ϊ����s��
        // �I�u�W�F�N�g�̈ʒu��localPosition�ɕύX����

        Vector2 localPosition = GetLocalPosition(eventData.position);
        rectTransform.anchoredPosition = localPosition;
    }

    // �h���b�O�I�����̏���
    public void OnEndDrag(PointerEventData eventData)
    {
        // �I�u�W�F�N�g���h���b�O�O�̈ʒu�ɖ߂�
        rectTransform.anchoredPosition = prevPos;


    }

    // ScreenPosition����localPosition�ւ̕ϊ��֐�
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;

        // screenPosition��e�̍��W�n(parentRectTransform)�ɑΉ�����悤�ϊ�����.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPosition, Camera.main, out result);

        return result;
    }

    // �h���b�v�����ʒu����ɔ���
    public void OnDrop(PointerEventData eventData)
    {
        GameObject scc = GetSCCellOnMouse(eventData);   //SCCell�������scc�ɑ��
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


