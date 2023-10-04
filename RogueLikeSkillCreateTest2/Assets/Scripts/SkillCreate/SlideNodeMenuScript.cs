using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideNodeMenuScript : MonoBehaviour
{

    [SerializeField] GameObject btn;
    private RectTransform rectTransform;
    private bool open;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width / 2 * -1, 0);
        CloseMenu();
    }
    public void OnClick()
    {
        if (open)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        rectTransform.anchoredPosition -= new Vector2(rectTransform.rect.width, 0);
        open = true;
        btn.GetComponentInChildren<TextMeshProUGUI>().text = "Å®";
    }

    private void CloseMenu()
    {
        rectTransform.anchoredPosition += new Vector2(rectTransform.rect.width, 0);
        open = false;
        btn.GetComponentInChildren<TextMeshProUGUI>().text = "Å©";
    }
}
