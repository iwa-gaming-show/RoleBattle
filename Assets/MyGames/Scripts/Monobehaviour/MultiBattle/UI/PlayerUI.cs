using System.Collections;
using System.Collections.Generic;
using static UIStrings;
using static WaitTimes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    [Header("自身の獲得ポイントUI")]
    TextMeshProUGUI _myPointText;

    [SerializeField]
    [Header("必殺技ボタンを設定する")]
    Image _spButtonImage;

    [SerializeField]
    [Header("プレイヤーアイコンの配置場所を設定する")]
    Transform _playerIconField;

    [SerializeField]
    [Header("カードを加える手札の設置場所を設定する")]
    Transform _handPanel;

    [SerializeField]
    [Header("カードのバトル場の配置場所を設定する")]
    Transform _battleField;

    [SerializeField]
    [Header("必殺技の演出を設定する")]
    GameObject _spProduction;

    int _cachePoint;
    Sprite _cacheSpButtonSprite;

    /// <summary>
    /// ポイントの表示
    /// </summary>
    public void ShowPoint(int myPoint)
    {
        if (_cachePoint == myPoint) return;//同じ値なら描画しない
        _cachePoint = myPoint;
        _myPointText.text = myPoint.ToString() + POINT_SUFFIX;
    }

    /// <summary>
    /// 必殺技のImageを初期化する
    /// </summary>
    //public void InitSpButtonImage(Sprite unUsedIcon)
    //{
    //    SetSpButtonSprite(unUsedIcon);
    //}

    /// <summary>
    /// 必殺技のIconを設定する
    /// </summary>
    /// <param name="targetImage"></param>
    /// <param name="setSprite"></param>
    public void SetSpButtonSprite(Sprite setSprite)
    {
        if (_cacheSpButtonSprite == setSprite) return;
        _cacheSpButtonSprite = setSprite;
        _spButtonImage.sprite = setSprite;
    }

    /// <summary>
    /// 必殺技ボタンを使用済みにする
    /// </summary>
    void UsedSpecialSkillButton(Sprite usedIcon)
    {
        SetSpButtonSprite(usedIcon);
    }

    /// <summary>
    /// プレイヤーアイコンを配置する
    /// </summary>
    public void PlacePlayerIcon(GameObject targetGo)
    {
        targetGo.transform.SetParent(_playerIconField, false);
    }

    /// <summary>
    /// カードを手札に加えます
    /// </summary>
    public void AddingCardToHand(CardController card)
    {
        card.transform.SetParent(_handPanel, false);
    }

    /// <summary>
    /// 手札のカードを全て取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public CardController[] GetAllHandCards()
    {
        return _handPanel.GetComponentsInChildren<CardController>();
    }

    /// <summary>
    /// カードをバトル場へ移動します
    /// </summary>
    /// <param name="movingCard"></param>
    public async UniTask MoveToBattleField(CardController movingCard)
    {
        //カードを裏にする
        movingCard.TurnTheCardOver();
        //フィールドへカードを移動
        movingCard.transform.DOMove(_battleField.position, CARD_MOVEMENT_TIME);
        movingCard.transform.SetParent(_battleField);
        await UniTask.Yield(); 
    }

    /// <summary>
    /// 手札からランダムにカードを取得します
    /// </summary>
    /// <returns></returns>
    public CardController GetRandomHandCard()
    {
        CardController[] handCards = _handPanel.GetComponentsInChildren<CardController>();
        int randomCardIndex = Random.Range(0, handCards.Length);
        return handCards[randomCardIndex];
    }
}
