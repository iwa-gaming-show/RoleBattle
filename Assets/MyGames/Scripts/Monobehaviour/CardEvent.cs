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
    GameManager gameManager;
    CardController cardController;

    void Awake()
    {
        cardController = GetComponent<CardController>();
        gameManager = GameManager._instance;
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    /// <param name="eventData"></param>
    public async void OnPointerClick(PointerEventData eventData)
    {
        bool controllablePlayerCard = gameManager.TurnManager.IsMyTurn && cardController.CardModel.IsPlayerCard;
        bool selectionPhase = (gameManager.BattlePhase == SELECTION);
        bool placeable = gameManager.CardManager.IsBattleFieldPlaced == false;
        //選択フェイズで自身のカードが配置可能な場合操作可能
        bool controllable = controllablePlayerCard && selectionPhase && placeable;

        if (controllable)
        {
            await TryToMoveToField();
        }
    }

    /// <summary>
    /// フィールドへの移動を試みます
    /// </summary>
    /// <returns></returns>
    async UniTask TryToMoveToField()
    {
        //すでに確認画面がでてるなら何もしない
        if (gameManager.UIManager.ConfirmationPanelToField.gameObject.activeInHierarchy) return;

        //カードを選択し、確認画面を表示しYesならフィールドへ移動します
        gameManager.UIManager.SelectedToFieldCard(cardController);
        await WaitFieldConfirmationButton();

        //yesを押した時
        if (gameManager.UIManager.ConfirmationPanelToField.CanMoveToField)
        {
            await MoveToBattleField(gameManager.MyBattleFieldTransform);
        }

        gameManager.UIManager.ConfirmationPanelToField.SetCanMoveToField(false);
        gameManager.UIManager.ConfirmationPanelToField.SetIsClickedConfirmationButton(false);
    }

    /// <summary>
    /// フィールドへの確認画面の押下を待ちます
    /// </summary>
    /// <returns></returns>
    async UniTask WaitFieldConfirmationButton()
    {
        await UniTask.WaitUntil(() => gameManager.UIManager.ConfirmationPanelToField.IsClickedConfirmationButton);
    }

    /// <summary>
    /// フィールドへ移動する
    /// </summary>
    public async UniTask MoveToBattleField(Transform targetTransform)
    {
        if (gameManager.CardManager.IsBattleFieldPlaced) return;

        gameManager.ChangeBattlePhase(SELECTED);
        cardController.TurnTheCardOver();
        transform.DOMove(targetTransform.position, CARD_MOVEMENT_TIME);//移動演出
        transform.SetParent(targetTransform);//フィールドへカードを移動
        gameManager.CardManager.SetBattleFieldPlaced(true);
        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_TURN));
        gameManager.TurnManager.EndTurn();
    }
}