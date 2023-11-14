using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject _textPrefab;
    [SerializeField] GameObject _contentHolder;

    public void UpdateUI(Inventory inventory)
    {
        //表示されているテキストの数を返す
        int currentTextCount = _contentHolder.transform.childCount;
        //今インベントリにあるアイテムの数を返す
        int currentItemCount = inventory.g_itemList.Count;

        //アイテムの数とUIの数を合わせる
        if(currentTextCount < currentItemCount)
        {
            int num = currentItemCount - currentTextCount;
            for(int i = 0;i < num;i++)
            {
                //textObject生成
                GameObject newTextObject = Instantiate(_textPrefab);
                //パネルのコンテントにくっつける
                newTextObject.transform.SetParent(_contentHolder.transform, false);
            }
        }
        else 
        if(currentTextCount > currentItemCount)
        {
            for(int i = currentTextCount - 1;i > currentItemCount - 1;i--)
            {
                //i番目のオブジェクトを破壊
                Destroy(_contentHolder.transform.GetChild(i).gameObject);
            }
        }

        //textにアイテムの名前を表示
        for (int i = 0 ; i < currentItemCount; i++)
        {
            ItemData itemData = inventory.g_itemList[i].g_data;
            GameObject textObject = _contentHolder.transform.GetChild(i).gameObject;
            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = itemData.getItemName() + "  ×  " + inventory.g_itemList[i].g_value;
        }
    }
}
