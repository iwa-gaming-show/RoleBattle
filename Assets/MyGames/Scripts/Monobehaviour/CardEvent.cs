using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static WaitTimes;

public class CardEvent : MonoBehaviour, IPointerClickHandler
{
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager._instance;
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager.IsBattleFieldPlaced == false && gameManager.IsMyTurn)
        {
            StartCoroutine(TryToMoveToField());
            //1pならプレイヤーのフィールドへ移動
            //1pと2pでフィールドフラグを分ける
            //フィールドには裏返して配置する
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

        //確認画面を表示しYesならフィールドへ移動します
        gameManager.UIManager.ToggleConfirmationPanelToField(true);
        yield return StartCoroutine(WaitFieldConfirmationButton());

        //yesを押した時
        if (gameManager.UIManager.CanMoveToField)
        {
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