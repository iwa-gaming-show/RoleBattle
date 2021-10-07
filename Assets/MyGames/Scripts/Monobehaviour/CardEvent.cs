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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager.IsBattleFieldPlaced == false)
        {
            //確認UIを表示
            //1pならプレイヤーのフィールドへ移動
            //フィールドには裏返して配置する
            MoveToBattleField();
            gameManager.SetBattleFieldPlaced(true);
            //相手のターンへ
            gameManager.EnemyTurn();
        }
    }

    /// <summary>
    /// フィールドへ移動する
    /// </summary>
    void MoveToBattleField()
    {
        transform.DOMove(gameManager.P1BattleFieldTransform.position, 0.5f);//移動演出
        transform.SetParent(gameManager.P1BattleFieldTransform);//フィールドへカードを移動
    }
}