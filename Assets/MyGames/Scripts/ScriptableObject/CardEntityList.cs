using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEntityList", menuName = "Create CardEntityList")]
public class CardEntityList : ScriptableObject
{
    [SerializeField]
    List<CardEntity> _cardEntityList = new List<CardEntity>();

    public List<CardEntity> GetCardEntityList => _cardEntityList;
}

[System.Serializable]
public class CardEntity
{
    [SerializeField]
    [Header("カード名")]
    string _name = "カード名";

    [SerializeField]
    [Header("アイコン画像")]
    Sprite _icon;

    [SerializeField]
    [Header("カードの種類")]
    CardType _cardType;

    #region //プロパティ
    public string Name => _name;
    public Sprite Icon => _icon;
    public CardType CardType => _cardType;
    #endregion
}