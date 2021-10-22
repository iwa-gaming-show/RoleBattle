using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;

public class TurnManager : MonoBehaviour
{
    bool _isMyTurn;//自身のターンか
    bool _isMyTurnEnd;
    bool _isEnemyTurnEnd;
    int _enemySpecialSkillTurn;//敵が必殺技を使用するターン
    GameManager gameManager;

    #region プロパティ
    public bool IsMyTurn => _isMyTurn;
    #endregion

    void Start()
    {
        //Awakeで取得予定だったがGameManagerのAwakeより先に呼ばれると参照が取得できないためStartで取得する
        gameManager = GameManager._instance;
    }

    /// <summary>
    /// 先攻、後攻のターンを決めます
    /// </summary>
    public void DecideTheTurn()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            _isMyTurn = true;
        }
    }

    /// <summary>
    /// エネミーが必殺技を使用するターンを決めます
    /// </summary>
    public void DecideTheTurnOnEnemySp()
    {
        _enemySpecialSkillTurn = Random.Range(INITIAL_ROUND_COUNT, gameManager.MaxRoundCount + INITIAL_ROUND_COUNT);
    }

    /// <summary>
    /// ターンの終了
    /// </summary>
    public void EndTurn()
    {
        if (_isMyTurn)
        {
            _isMyTurn = false;
            _isMyTurnEnd = true;
        }
        else
        {
            _isMyTurn = true;
            _isEnemyTurnEnd = true;
        }

        ChangeTurn();
    }

    /// <summary>
    /// ターンを切り替える
    /// </summary>
    public void ChangeTurn()
    {
        gameManager.CardManager.SetBattleFieldPlaced(false);
        StopAllCoroutines();//意図しない非同期処理が走っている可能性を排除する

        if (_isMyTurn && _isMyTurnEnd == false)
        {
            StartCoroutine(gameManager.CountDown());
            MyTurn();
        }
        else if (_isEnemyTurnEnd == false)
        {
            StartCoroutine(gameManager.CountDown());
            StartCoroutine(EnemyTurn());
        }

        if (_isMyTurnEnd && _isEnemyTurnEnd)
        {
            //自身と相手のターンが終了した時、判定処理が走る
            StartCoroutine(gameManager.CardManager.JudgeTheCard());
        }
    }

    /// <summary>
    /// 自分のターン
    /// </summary>
    public void MyTurn()
    {
        StartCoroutine(gameManager.UIManager.ShowThePlayerTurnText(true));
    }

    /// <summary>
    /// 相手のターン
    /// </summary>
    public IEnumerator EnemyTurn()
    {
        yield return gameManager.UIManager.ShowThePlayerTurnText(false);
        //エネミーの手札を取得
        CardController[] cardControllers = gameManager.CardManager.GetAllHandCardsFor(false);
        //カードをランダムに選択
        CardController card = cardControllers[Random.Range(0, cardControllers.Length)];

        bool useSpecialSkill = (gameManager.RoundCount == _enemySpecialSkillTurn);

        if (gameManager.Enemy.CanUseSpecialSkill && useSpecialSkill)
        {
            yield return gameManager.UIManager.ActivateSpecialSkill(false);
        }

        //カードをフィールドに移動
        yield return card.CardEvent.MoveToBattleField(gameManager.EnemyBattleFieldTransform);
    }

    /// <summary>
    /// 自身のターンが終わったか
    /// </summary>
    /// <param name="isEnd"></param>
    public void SetIsMyTurnEnd(bool isEnd)
    {
        _isMyTurnEnd = isEnd;
    }

    /// <summary>
    /// エネミーのターンが終わったか
    /// </summary>
    /// <param name="isEnd"></param>
    public void SetIsEnemyTurnEnd(bool isEnd)
    {
        _isEnemyTurnEnd = isEnd;
    }
}
