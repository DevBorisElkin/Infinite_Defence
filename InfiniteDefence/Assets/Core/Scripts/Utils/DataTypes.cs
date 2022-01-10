using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public static class DataTypes
{
    [System.Serializable]
    public class RandomChallengeButton
    {
        public Color neutralColor;
        public Color selectedColor;
        public Color ignoredColor;

        [Space(5f)]
        public Image backgroundImage;
        public TMP_Text resultTxt;
        public string resultString;
        public Challenge challenge;

        public void Hide()
        {
            backgroundImage.color = neutralColor;
            resultTxt.text = "?";
        }

        public void Unveil(bool selected = false)
        {
            backgroundImage.color = selected ? selectedColor : ignoredColor;
            resultTxt.text = resultString;
        }
    }
}
