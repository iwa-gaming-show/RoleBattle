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
    BattleManager _battleManager;
    CardController cardController;

    void Awake()
    {
        cardController = GetComponent<CardController>();
        _battleManager = BattleManager._instance;
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        bool controllablePlayerCard = _battleManager.TurnManager.IsMyTurn && cardController.CardModel.IsPlayerCard;
        bool selectionPhase = (_battleManager.BattlePhase == SELECTION);
        bool placeable = _battleManager.CardManager.IsBattleFieldPlaced == false;
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
        if (_battleManager.UIManager.ConfirmationPanelToField.gameObject.activeInHierarchy) return;

        //カードを選択し、確認画面を表示しYesならフィールドへ移動します
        _battleManager.UIManager.SelectedToFieldCard(cardController);
        await WaitFieldConfirmationButton();

        //yesを押した時
        if (_battleManager.UIManager.ConfirmationPanelToField.CanMoveToField)
        {
            await MoveToBattleField(_battleManager.FieldTransformManager.MyBattleFieldTransform);
        }

        _battleManager.UIManager.ConfirmationPanelToField.SetCanMoveToField(false);
        _battleManager.UIManager.ConfirmationPanelToField.SetIsClickedConfirmationButton(false);
    }

    /// <summary>
    /// フィールドへの確認画面の押下を待ちます
    /// </summary>
    /// <returns></returns>
    async UniTask WaitFieldConfirmationButton()
    {
        await UniTask.WaitUntil(() => _battleManager.UIManager.ConfirmationPanelToField.IsClickedConfirmationButton);
    }

    /// <summary>
    /// フィールドへ移動する
    /// </summary>
    public async UniTask MoveToBattleField(Transform targetTransform)
    {
        if (_battleManager.CardManager.IsBattleFieldPlaced) return;

        _battleManager.ChangeBattlePhase(SELECTED);
        cardController.TurnTheCardOver();
        //移動演出
        transform.DOMove(targetTransform.position, CARD_MOVEMENT_TIME);
        //フィールドへカードを移動
        transform.SetParent(targetTransform);
        _battleManager.CardManager.SetBattleFieldPlaced(true);
        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_TURN));
        _battleManager.TurnManager.EndTurn();
    }
}