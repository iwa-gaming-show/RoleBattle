using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GM = GameManager;

public class RoundManager : MonoBehaviour
{
    #region インスペクターから設定
    [SerializeField]
    [Header("最大ラウンド数")]
    int _maxRoundCount = 3;
    #endregion

    RoundData _roundData;

    #region プロパティ
    public int RoundCount => _roundData.RoundCount;
    public int MaxRoundCount => _maxRoundCount;
    public bool IsUsingPlayerSkillInRound => _roundData.IsUsingPlayerSkillInRound;
    public bool IsUsingEnemySkillInRound => _roundData.IsUsingEnemySkillInRound;
    public RoundData RoundData => _roundData;
    #endregion

    void Awake()
    {
        _roundData = new RoundData(_maxRoundCount);
    }

    /// <summary>
    /// 次のラウンドへ
    /// </summary>
    public async UniTask NextRound()
    {
        if (_roundData.RoundCount != _maxRoundCount)
        {
            _roundData.AddRoundCount();
            await GM._instance.StartGame(false);
        }
        else
        {
            //最終ラウンドならゲーム終了
            GM._instance.EndGame();
        }
    }
}