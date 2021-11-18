using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static WaitTimes;
using static BattlePhase;

public class CardEvent : MonoBehaviour, IPointerClickHandler
{
    IBattleManager _battleManager;
    IBattleUIManager _battleUIManager;
    ICardManager _cardManager;
    IFieldTransformManager _fieldTransformManager;
    ITurnManager _turnManager;

    CardController cardController;

    void Awake()
    {
        cardController = GetComponent<CardController>();
    }

    void Start()
    {
        _battleManager = ServiceLocator.Resolve<IBattleManager>();
        _battleUIManager = ServiceLocator.Resolve<IBattleUIManager>();
        _fieldTransformManager = ServiceLocator.Resolve<IFieldTransformManager>();
        _cardManager = ServiceLocator.Resolve<ICardManager>();
        _turnManager = ServiceLocator.Resolve<ITurnManager>();
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        bool controllablePlayerCard = _turnManager.IsMyTurn && cardController.CardModel.IsPlayerCard;
        bool selectionPhase = (_battleManager.BattlePhase == SELECTION);
        bool placeable = _cardManager.IsBattleFieldPlaced == false;
        //選択フェイズで自身のカードが配置可能な場合操作可能
        bool controllable = controllablePlayerCard && selectionPhase && placeable;

        if (controllable)
        {
            TryToMoveToField().Forget();
        }
    }

    /// <summary>
    /// フィールドへの移動を試みます
    /// </summary>
    /// <returns></returns>
    async UniTask TryToMoveToField()
    {
        //すでに確認画面がでてるなら何もしない
        if (_battleUIManager.ConfirmationPanelToField.gameObject.activeInHierarchy) return;

        //カードを選択し、確認画面を表示しYesならフィールドへ移動します
        _battleUIManager.SelectedToFieldCard(cardController);
        await WaitFieldConfirmationButton();

        //yesを押した時
        if (_battleUIManager.ConfirmationPanelToField.CanMoveToField)
        {
            await MoveToBattleField(_fieldTransformManager.MyBattleFieldTransform);
        }

        _battleUIManager.ConfirmationPanelToField.SetCanMoveToField(false);
        _battleUIManager.ConfirmationPanelToField.SetIsClickedConfirmationButton(false);
    }

    /// <summary>
    /// フィールドへの確認画面の押下を待ちます
    /// </summary>
    /// <returns></returns>
    async UniTask WaitFieldConfirmationButton()
    {
        await UniTask.WaitUntil(() => _battleUIManager.ConfirmationPanelToField.IsClickedConfirmationButton);
    }

    /// <summary>
    /// フィールドへ移動する
    /// </summary>
    public async UniTask MoveToBattleField(Transform targetTransform)
    {
        if (_cardManager.IsBattleFieldPlaced) return;

        _battleManager.ChangeBattlePhase(SELECTED);
        cardController.TurnTheCardOver();
        //移動演出
        transform.DOMove(targetTransform.position, CARD_MOVEMENT_TIME);
        //フィールドへカードを移動
        transform.SetParent(targetTransform);
        _cardManager.SetBattleFieldPlaced(true);
        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_TURN));
        _turnManager.EndTurn();
    }
}