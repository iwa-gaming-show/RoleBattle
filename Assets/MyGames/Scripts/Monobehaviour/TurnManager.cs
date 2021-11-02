using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static InitializationData;
using static BattlePhase;
using GM = GameManager;

public class TurnManager : MonoBehaviour
{
    TurnData _turnData;

    #region プロパティ
    public bool IsMyTurn => _turnData.IsMyTurn;
    #endregion

    public TurnData TurnData => _turnData;

    void Awake()
    {
        _turnData = new TurnData();
    }

    /// <summary>
    /// ターンの終了をリセットする
    /// </summary>
    public void ResetTurn()
    {
        _turnData.ResetTurn();
    }

    /// <summary>
    /// 先攻、後攻のターンを決めます
    /// </summary>
    public void DecideTheTurn()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            _turnData.SetIsMyTurn(true);
        }
    }

    /// <summary>
    /// エネミーが必殺技を使用するターンを決めます
    /// </summary>
    public void DecideTheTurnOnEnemySp(int maxRoundCount)
    {
        int specialSkillTurn = Random.Range(INITIAL_ROUND_COUNT, maxRoundCount + INITIAL_ROUND_COUNT);
        _turnData.SetEnemySpecialSkillTurn(specialSkillTurn);
    }

    /// <summary>
    /// ターンの終了
    /// </summary>
    public void EndTurn()
    {
        _turnData.ChangeTurnSettings();
        ChangeTurn().Forget();
    }

    /// <summary>
    /// ターンを切り替える
    /// </summary>
    public async UniTaskVoid ChangeTurn()
    {
        GM._instance.CardManager.SetBattleFieldPlaced(false);
        GM._instance.ChangeBattlePhase(SELECTION);
        StopAllCoroutines();//意図しないコルーチンが走っている可能性を排除する

        //自身のターン
        if (_turnData.IsMyTurn && _turnData.IsMyTurnEnd == false)
        {
            StartCoroutine(GM._instance.CountDown());
            MyTurn().Forget();
        }
        //敵のターン
        else if (_turnData.IsEnemyTurnEnd == false)
        {
            StartCoroutine(GM._instance.CountDown());
            EnemyTurn().Forget();
        }
        //カードの判定
        if (_turnData.IsMyTurnEnd && _turnData.IsEnemyTurnEnd)
        {
            //自身と相手のターンが終了した時、判定処理が走る
            await GM._instance.CardManager.JudgeTheCard(GM._instance.MyBattleFieldTransform, GM._instance.EnemyBattleFieldTransform);
        }
    }

    /// <summary>
    /// 自分のターン
    /// </summary>
    public async UniTaskVoid MyTurn()
    {
        await GM._instance.UIManager.DirectionUIManager.ShowThePlayerTurnText(true);
    }

    /// <summary>
    /// 相手のターン
    /// </summary>
    public async UniTaskVoid EnemyTurn()
    {
        await GM._instance.UIManager.DirectionUIManager.ShowThePlayerTurnText(false);

        //相手のランダムなカードを選択
        CardController targetCard = GM._instance.CardManager.GetRandomCardFrom(GM._instance.GetHandTransformByTurn());

        //必殺技の発動
        bool useSpecialSkill = (GM._instance.RoundManager.RoundCount == _turnData.EnemySpecialSkillTurn);
        if (GM._instance.Enemy.CanUseSpecialSkill && useSpecialSkill)
        {
            await GM._instance.UIManager.SpecialSkillUIManager.ActivateSpecialSkill(false);
        }

        //カードをフィールドに移動
        await targetCard.CardEvent.MoveToBattleField(GM._instance.EnemyBattleFieldTransform);
    }
}
