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
            //todo
            //確認UIを表示
            //1pならプレイヤーのフィールドへ移動
            //1pと2pでフィールドフラグを分ける
            //フィールドには裏返して配置する
            StartCoroutine(MoveToBattleField(gameManager.MyBattleFieldTransform));
        }
    }

    /// <summary>
    /// フィールドへ移動する
    /// </summary>
    public IEnumerator MoveToBattleField(Transform targetTransform)
    {
        transform.DOMove(targetTransform.position, 0.5f);//移動演出
        transform.SetParent(targetTransform);//フィールドへカードを移動
        gameManager.SetBattleFieldPlaced(true);
        yield return new WaitForSeconds(2f);
        //相手のターンへ
        gameManager.ChangeTurn();
    }
}