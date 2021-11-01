using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static InitializationData;
using GM = GameManager;

public class RoundManager : MonoBehaviour
{
    #region インスペクターから設定
    [SerializeField]
    [Header("最大ラウンド数")]
    int _maxRoundCount = 3;

    [SerializeField]
    [Header("ラウンド数")]
    int _roundCount = INITIAL_ROUND_COUNT;
    #endregion

    bool _isUsingPlayerSkillInRound;//必殺技を使用したラウンドか
    bool _isUsingEnemySkillInRound;

    #region プロパティ
    public int RoundCount => _roundCount;
    public int MaxRoundCount => _maxRoundCount;
    public bool IsUsingPlayerSkillInRound => _isUsingPlayerSkillInRound;
    public bool IsUsingEnemySkillInRound => _isUsingEnemySkillInRound;
    #endregion

    /// <summary>
    /// ラウンドの状態をリセットする
    /// </summary>
    public void ResetRoundState()
    {
        //スキルの発動状態をリセット
        _isUsingPlayerSkillInRound = false;
        _isUsingEnemySkillInRound = false;
    }

    /// <summary>
    /// ラウンドの設定
    /// </summary>
    public void SetRoundCount(int count)
    {
        _roundCount = count;
    }

    /// <summary>
    /// ラウンドの加算
    /// </summary>
    public void AddRoundCount()
    {
        _roundCount++;
    }

    /// <summary>
    /// 次のラウンドへ
    /// </summary>
    public async UniTask NextRound()
    {
        if (_roundCount != _maxRoundCount)
        {
            AddRoundCount();
            await GM._instance.StartGame(false);
        }
        else
        {
            //最終ラウンドならゲーム終了
            GM._instance.EndGame();
        }
    }

    /// <summary>
    /// 必殺技を使用したラウンドかどうかの設定
    /// </summary>
    public void SetUsingSkillRound(bool isPlayer, bool isUsingSkill)
    {
        if (isPlayer)
        {
            _isUsingPlayerSkillInRound = isUsingSkill;
            return;
        }

        _isUsingEnemySkillInRound = isUsingSkill;
    }
}

