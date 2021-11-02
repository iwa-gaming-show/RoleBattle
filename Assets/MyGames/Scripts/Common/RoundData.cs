using static InitializationData;

/// <summary>
/// ラウンドに関するデータ
/// </summary>
public class RoundData
{
    int _roundCount = INITIAL_ROUND_COUNT;
    int _maxRoundCount;
    bool _isUsingPlayerSkillInRound;//必殺技を使用したラウンドか
    bool _isUsingEnemySkillInRound;

    #region プロパティ
    public int RoundCount => _roundCount;
    public bool IsUsingPlayerSkillInRound => _isUsingPlayerSkillInRound;
    public bool IsUsingEnemySkillInRound => _isUsingEnemySkillInRound;
    #endregion

    public RoundData(int maxRoundCount)
    {
        _maxRoundCount = maxRoundCount;
    }

    /// <summary>
    /// プレイヤーがラウンド中にスキルを発動したかの設定
    /// </summary>
    public void SetIsUsingPlayerSkillInRound(bool isUsing)
    {
        _isUsingPlayerSkillInRound = isUsing;
    }

    /// <summary>
    /// エネミーがラウンド中にスキルを発動したかの設定
    /// </summary>
    public void SetIsUsingEnemySkillInRound(bool isUsing)
    {
        _isUsingEnemySkillInRound = isUsing;
    }

    /// <summary>
    /// ラウンドの設定
    /// </summary>
    public void SetRoundCount(int count)
    {
        if (count < INITIAL_ROUND_COUNT)
        {
            //初期値以下なら初期値にする
            _roundCount = INITIAL_ROUND_COUNT;
            return;
        }

        if (count > _maxRoundCount)
        {
            //最大値を超えたら最大値にする
            _roundCount = _maxRoundCount;
            return;
        }

        _roundCount = count;
    }

    /// <summary>
    /// ラウンドの加算
    /// </summary>
    public void AddRoundCount()
    {
        _roundCount++;
    }
}

