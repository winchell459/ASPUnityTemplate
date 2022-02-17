using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMove : MonoBehaviour
{
    [SerializeField] int count;
    int maxCount = 9;
    int maxLength = 400;
    [SerializeField] GameObject[] countIcons;
    public void SetMoveCount(int count)
    {
        //RectTransform rect = GetComponent<RectTransform>();
        //rect.sizeDelta = new Vector2(count * maxLength / maxCount, rect.sizeDelta.y);
        for(int i = 0; i < countIcons.Length; i += 1)
        {
            if(count <= i)
            {
                countIcons[i].SetActive(false);
            }
            else
            {
                countIcons[i].SetActive(true);
            }
        }
        this.count = count;
    }

    
}
