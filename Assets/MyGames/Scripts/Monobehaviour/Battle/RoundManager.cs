using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RoundManager : MonoBehaviour, IRoundManager
{
    #region インスペクターから設定
    [SerializeField]
    [Header("最大ラウンド数")]
    int _maxRoundCount = 3;
    #endregion

    IBattleManager _battleManager;
    RoundData _roundData;

    #region プロパティ
    public int RoundCount => _roundData.RoundCount;
    public int MaxRoundCount => _maxRoundCount;
    public bool IsUsingPlayerSkillInRound => _roundData.IsUsingPlayerSkillInRound;
    public bool IsUsingEnemySkillInRound => _roundData.IsUsingEnemySkillInRound;
    #endregion

    void Awake()
    {
        ServiceLocator.Register<IRoundManager>(this);
        _roundData = new RoundData(_maxRoundCount);
    }

    void Start()
    {
        _battleManager = ServiceLocator.Resolve<IBattleManager>();
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IRoundManager>(this);
    }

    /// <summary>
    /// ラウンドの状態をリセットする
    /// </summary>
    public void ResetData()
    {
        _roundData.ResetRoundState();
    }

    /// <summary>
    /// ラウンドの設定
    /// </summary>
    public void SetRoundCount(int count)
    {
        _roundData.SetRoundCount(count);
    }

    /// <summary>
    /// 必殺技を使用したラウンドかどうかの設定
    /// </summary>
    public void SetUsingSkillRound(bool isPlayer, bool isUsingSkill)
    {
        _roundData.SetUsingSkillRound(isPlayer, isUsingSkill);
    }

    /// <summary>
    /// 次のラウンドへ
    /// </summary>
    public async UniTask NextRound()
    {
        if (_roundData.RoundCount != _maxRoundCount)
        {
            _roundData.AddRoundCount();
            await _battleManager.StartGame(false);
        }
        else
        {
            //最終ラウンドならゲーム終了
            _battleManager.EndGame();
        }
    }
}