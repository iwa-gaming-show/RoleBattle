using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] [Header("カード名")] Text nameText;
    [SerializeField] [Header("カードの画像")] Image iconImage;
    [SerializeField] [Header("カードの裏側")] GameObject backSide;

    /// <summary>
    /// カードを描画する
    /// </summary>
    /// <param name="cardModel"></param>
    public void SetCardView(CardModel cardModel)
    {
        nameText.text = cardModel.Name;
        iconImage.sprite = cardModel.Icon;
        backSide.SetActive(!cardModel.IsPlayerCard);//相手のカードは裏側で表示する
    }
}
