using System.Collections;
using System.Collections.Generic;
using static UIStrings;
using static WaitTimes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Random = UnityEngine.Random;

public class MultiPlayerUI : MonoBehaviour
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
    [Header("カードのフィールドの配置場所を設定する")]
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
    void UsedSpSkillButton(Sprite usedIcon)
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
    /// カードをフィールドへ移動します
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

    /// <summary>
    /// フィールドのカードを取得します
    /// </summary>
    /// <returns></returns>
    public CardController GetFieldCard()
    {
        return _battleField.GetComponentInChildren<CardController>();
    }

    /// <summary>
    /// フィールドのカードを破棄します
    /// </summary>
    public void DestroyFieldCard()
    {
        Destroy(GetFieldCard().gameObject);
    }

    /// <summary>
    /// フィールドへカードをセットします
    /// </summary>
    public void SetFieldCard(CardController cardController)
    {
        cardController.transform.SetParent(_battleField, false);
    }

    /// <summary>
    /// 必殺技を発動します
    /// </summary>
    public async UniTask ActivateDirectingOfSpSkill(bool isPlayer)
    {
        RectTransform targetUIRectTranform = _spProduction.GetComponent<RectTransform>();
        float screenEdgeX = UIUtil.GetScreenEdgeXFor(targetUIRectTranform.sizeDelta.x);
        Sequence sequence = DOTween.Sequence();
        Tween firstMove;
        Tween lastMove;

        //自分なら右端から左端へ移動, 相手なら左から右へ移動する
        firstMove = UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, GetScreenEdgeXForPlayer(isPlayer, true, screenEdgeX), 0);
        lastMove = UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, GetScreenEdgeXForPlayer(isPlayer, false, screenEdgeX) , 0.4f);

        sequence.Append(firstMove.OnStart(() => ToggleProductionToSpSkill(true)));
        sequence.Append(UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, 0f, 0.25f));
        sequence.Append(lastMove.SetDelay(SPECIAL_SKILL_PRODUCTION_DISPLAY_TIME).OnComplete(() => ToggleProductionToSpSkill(false)));

        await sequence
           .Play()
           .AsyncWaitForCompletion();
    }

    /// <summary>
    /// 必殺技演出UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleProductionToSpSkill(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_spProduction, isActive, transform);
    }

    /// <summary>
    /// プレイヤーの演出のための画面の横幅を取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="isFirstMove"></param>
    /// <param name="screenEdgeX"></param>
    /// <returns></returns>
    public float GetScreenEdgeXForPlayer(bool isPlayer, bool isFirstMove, float screenEdgeX)
    {
        if (isPlayer && isFirstMove) return screenEdgeX;
        if (isPlayer == false && isFirstMove == false) return screenEdgeX;
        return -screenEdgeX;
    }
}
