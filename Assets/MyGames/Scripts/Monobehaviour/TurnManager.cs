using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;
using static BattlePhase;
using GM = GameManager;

public class TurnManager : MonoBehaviour
{
    bool _isMyTurn;//自身のターンか
    bool _isMyTurnEnd;
    bool _isEnemyTurnEnd;
    int _enemySpecialSkillTurn;//敵が必殺技を使用するターン

    #region プロパティ
    public bool IsMyTurn => _isMyTurn;
    #endregion

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
        _enemySpecialSkillTurn = Random.Range(INITIAL_ROUND_COUNT, GM._instance.RoundManager.MaxRoundCount + INITIAL_ROUND_COUNT);
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
        GM._instance.CardManager.SetBattleFieldPlaced(false);
        GM._instance.ChangeBattlePhase(SELECTION);
        StopAllCoroutines();//意図しない非同期処理が走っている可能性を排除する

        //自身のターン
        if (_isMyTurn && _isMyTurnEnd == false)
        {
            StartCoroutine(GM._instance.CountDown());
            MyTurn();
        }
        //敵のターン
        else if (_isEnemyTurnEnd == false)
        {
            StartCoroutine(GM._instance.CountDown());
            StartCoroutine(EnemyTurn());
        }
        //カードの判定
        if (_isMyTurnEnd && _isEnemyTurnEnd)
        {
            //自身と相手のターンが終了した時、判定処理が走る
            StartCoroutine(GM._instance.CardManager.JudgeTheCard());
        }
    }

    /// <summary>
    /// 自分のターン
    /// </summary>
    public void MyTurn()
    {
        StartCoroutine(GM._instance.UIManager.DirectionUIManager.ShowThePlayerTurnText(true));
    }

    /// <summary>
    /// 相手のターン
    /// </summary>
    public IEnumerator EnemyTurn()
    {
        yield return GM._instance.UIManager.DirectionUIManager.ShowThePlayerTurnText(false);

        //エネミーの手札を取得
        CardController[] cardControllers = GM._instance.CardManager.GetAllHandCardsFor(false);
        //カードをランダムに選択
        CardController card = cardControllers[Random.Range(0, cardControllers.Length)];

        //必殺技の発動
        bool useSpecialSkill = (GM._instance.RoundManager.RoundCount == _enemySpecialSkillTurn);
        if (GM._instance.Enemy.CanUseSpecialSkill && useSpecialSkill)
        {
            yield return GM._instance.UIManager.SpecialSkillUIManager.ActivateSpecialSkill(false);
        }

        //カードをフィールドに移動
        yield return card.CardEvent.MoveToBattleField(GM._instance.EnemyBattleFieldTransform);
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

    /// <summary>
    /// ターンの終了をリセットする
    /// </summary>
    public void ResetTurn()
    {
        SetIsMyTurnEnd(false);
        SetIsEnemyTurnEnd(false);
    }
}
