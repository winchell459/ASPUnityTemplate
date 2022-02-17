using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundUIHandler : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text roundText;
    public int Round { set { roundText.text = value.ToString(); } }

    [SerializeField] UnityEngine.UI.Text timeRemainingText;
    public float TimeRemaining { set { timeRemainingText.text = value.ToString("F1"); } }

    string formatTime(float time, int decimalCount)
    {
        int formatedTime = (int)(time * Mathf.Pow(10, decimalCount));
        time = formatedTime / (Mathf.Pow(10, decimalCount));

        return time.ToString();
    }
}
