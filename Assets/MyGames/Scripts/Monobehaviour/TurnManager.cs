using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static InitializationData;
using static BattlePhase;

public class TurnManager : MonoBehaviour, IGameDataResetable
{
    TurnData _turnData;
    BattleManager _battleManager;

    #region プロパティ
    public bool IsMyTurn => _turnData.IsMyTurn;
    #endregion

    public TurnData TurnData => _turnData;

    void Awake()
    {
        _turnData = new TurnData();
        _battleManager = GetComponent<BattleManager>();
    }

    /// <summary>
    /// ターンの終了をリセットする
    /// </summary>
    public void ResetData()
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
        _battleManager.CardManager.SetBattleFieldPlaced(false);
        _battleManager.ChangeBattlePhase(SELECTION);
        StopAllCoroutines();//意図しないコルーチンが走っている可能性を排除する

        //自身のターン
        if (_turnData.IsMyTurn && _turnData.IsMyTurnEnd == false)
        {
            StartCoroutine(_battleManager.CountDown());
            MyTurn().Forget();
        }
        //敵のターン
        else if (_turnData.IsEnemyTurnEnd == false)
        {
            StartCoroutine(_battleManager.CountDown());
            EnemyTurn().Forget();
        }
        //カードの判定
        if (_turnData.IsMyTurnEnd && _turnData.IsEnemyTurnEnd)
        {
            //自身と相手のターンが終了した時、判定処理が走る
            await _battleManager.CardManager.JudgeTheCard(_battleManager.FieldTransformManager.MyBattleFieldTransform, _battleManager.FieldTransformManager.EnemyBattleFieldTransform);
        }
    }

    /// <summary>
    /// 自分のターン
    /// </summary>
    public async UniTaskVoid MyTurn()
    {
        await _battleManager.UIManager.ShowThePlayerTurnText(true);
    }

    /// <summary>
    /// 相手のターン
    /// </summary>
    public async UniTaskVoid EnemyTurn()
    {
        await _battleManager.UIManager.ShowThePlayerTurnText(false);

        //相手のランダムなカードを選択
        CardController targetCard = _battleManager.CardManager.GetRandomCardFrom(_battleManager.FieldTransformManager.GetHandTransformByTurn(IsMyTurn));

        //必殺技の発動
        bool useSpecialSkill = (_battleManager.RoundManager.RoundCount == _turnData.EnemySpecialSkillTurn);
        if (_battleManager.Enemy.CanUseSpecialSkill && useSpecialSkill)
        {
            await _battleManager.UIManager.ActivateSpecialSkill(false);
        }

        //カードをフィールドに移動
        await targetCard.CardEvent.MoveToBattleField(_battleManager.FieldTransformManager.EnemyBattleFieldTransform);
    }
}
