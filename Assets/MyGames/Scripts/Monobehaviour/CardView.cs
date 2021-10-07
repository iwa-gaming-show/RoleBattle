using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Image iconImage;

    /// <summary>
    /// カードを描画する
    /// </summary>
    /// <param name="cardModel"></param>
    public void SetCardView(CardModel cardModel)
    {
        nameText.text = cardModel.Name;
        iconImage.sprite = cardModel.Icon;
    }
}
