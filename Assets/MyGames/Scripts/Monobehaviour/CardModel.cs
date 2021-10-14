using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    [SerializeField]
    [Header("カードリストを設定する(ScriptableObjectを参照)")]
    CardEntityList cardEntityList;

    string _name;
    Sprite _icon;
    CardType _cardType;
    bool _isPlayerCard;

    #region //プロパティ
    public string Name => _name;
    public Sprite Icon => _icon;
    public CardType CardType => _cardType;
    public bool IsPlayerCard => _isPlayerCard;
    #endregion

    /// <summary>
    /// カードのデータを設定する
    /// </summary>
    public void SetCardData(int cardIndex, bool isPlayer)
    {
        CardEntity cardEntity = cardEntityList.GetCardEntityList[cardIndex];

        _name = cardEntity.Name;
        _icon = cardEntity.Icon;
        _cardType = cardEntity.CardType;
        _isPlayerCard = isPlayer;
    }
}
