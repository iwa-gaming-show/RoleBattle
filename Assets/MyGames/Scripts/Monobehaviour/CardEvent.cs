using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static WaitTimes;

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
    public void OnPointerClick(PointerEventData eventData)
    {
        bool controllablePlayerCard = gameManager.IsMyTurn && cardController.CardModel.IsPlayerCard;
        if (gameManager.IsBattleFieldPlaced == false && controllablePlayerCard)
        {
            StartCoroutine(TryToMoveToField());
        }
    }

    /// <summary>
    /// フィールドへの移動を試みます
    /// </summary>
    /// <returns></returns>
    IEnumerator TryToMoveToField()
    {
        //すでに確認画面がでてるなら何もしない
        if (gameManager.UIManager.ConfirmationPanelToField.activeInHierarchy) yield break;

        //カードを選択し、確認画面を表示しYesならフィールドへ移動します
        gameManager.UIManager.SelectedToFieldCard(cardController);
        yield return StartCoroutine(WaitFieldConfirmationButton());

        //yesを押した時
        if (gameManager.UIManager.CanMoveToField)
        {
            //todo フィールドにはカードを裏返して配置する

            yield return StartCoroutine(MoveToBattleField(gameManager.MyBattleFieldTransform));
            gameManager.ChangeTurn();
        }

        gameManager.UIManager.SetCanMoveToField(false);
        gameManager.UIManager.SetIsClickedConfirmationFieldButton(false);
    }

    /// <summary>
    /// フィールドへの確認画面の押下を待ちます
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitFieldConfirmationButton()
    {
        yield return new WaitUntil(() => gameManager.UIManager.IsClickedConfirmationFieldButton);
    }

    /// <summary>
    /// フィールドへ移動する
    /// </summary>
    public IEnumerator MoveToBattleField(Transform targetTransform)
    {
        transform.DOMove(targetTransform.position, CARD_MOVEMENT_TIME);//移動演出
        transform.SetParent(targetTransform);//フィールドへカードを移動
        gameManager.SetBattleFieldPlaced(true);
        yield return new WaitForSeconds(TIME_BEFORE_CHANGING_TURN);
    }
}