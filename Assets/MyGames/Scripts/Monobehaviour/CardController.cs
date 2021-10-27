using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardModel _cardModel;//カードデータ操作用
    CardView _cardView;//カード表示用
    CardEvent _cardEvent;

    public CardModel CardModel => _cardModel;
    public CardEvent CardEvent => _cardEvent;

    void Awake()
    {
        _cardModel = GetComponent<CardModel>();
        _cardView = GetComponent<CardView>();
        _cardEvent = GetComponent<CardEvent>();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="cardIndex"></param>
    public void Init(int cardIndex, bool isPlayer)
    {
        _cardModel.SetCardData(cardIndex, isPlayer);
        _cardView.SetCardView(_cardModel);
    }

    /// <summary>
    /// カードを表側にする
    /// </summary>
    public async UniTask TurnTheCardFaceUp()
    {
        await _cardView.OpenTheCard();
    }

    /// <summary>
    /// カードを裏側にする
    /// </summary>
    public void TurnTheCardOver()
    {
        _cardView.ToggleBackSide(true);
    }
}
