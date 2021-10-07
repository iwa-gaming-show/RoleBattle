using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

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
            //確認UIを表示
            //1pならプレイヤーのフィールドへ移動
            //1pと2pでフィールドフラグを分ける
            //フィールドには裏返して配置する
            MoveToBattleField(gameManager.MyBattleFieldTransform);
            gameManager.SetBattleFieldPlaced(true);
            //相手のターンへ
            gameManager.ChangeTurn();
        }
    }

    /// <summary>
    /// フィールドへ移動する
    /// </summary>
    public void MoveToBattleField(Transform targetTransform)
    {
        transform.DOMove(targetTransform.position, 0.5f);//移動演出
        transform.SetParent(targetTransform);//フィールドへカードを移動
    }
}