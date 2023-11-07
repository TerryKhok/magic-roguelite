using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject textPrefab;
    public GameObject contentHolder;

    public void UpdateUI(Inventory inventory)
    {
        //�\������Ă���e�L�X�g�̐���Ԃ�
        int currentTextCount = contentHolder.transform.childCount;
        //���C���x���g���ɂ���A�C�e���̐���Ԃ�
        int currentItemCount = inventory.itemList.Count;

        //�A�C�e���̐���UI�̐������킹��
        if(currentTextCount < currentItemCount)
        {
            int num = currentItemCount - currentTextCount;
            for(int i = 0;i < num;i++)
            {
                //textObject����
                GameObject newTextObject = Instantiate(textPrefab);
                //�p�l���̃R���e���g�ɂ�������
                newTextObject.transform.SetParent(contentHolder.transform, false);
            }
        }
        else 
        if(currentTextCount > currentItemCount)
        {
            for(int i = currentTextCount - 1;i > currentItemCount - 1;i--)
            {
                //i�Ԗڂ̃I�u�W�F�N�g��j��
                Destroy(contentHolder.transform.GetChild(i).gameObject);
            }
        }

        //text�ɃA�C�e���̖��O��\��
        for (int i = 0 ; i < currentItemCount; i++)
        {
            ItemData itemData = inventory.itemList[i].data;
            GameObject textObject = contentHolder.transform.GetChild(i).gameObject;
            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = itemData.getItemName() + "  �~  " + inventory.itemList[i].value;
        }
    }
}
