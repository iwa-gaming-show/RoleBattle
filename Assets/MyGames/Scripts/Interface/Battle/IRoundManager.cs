using Cysharp.Threading.Tasks;

public interface IRoundManager: IGameDataResetable
{
    int RoundCount
    {
        get;
    }
    int MaxRoundCount
    {
        get;
    }
    bool IsUsingPlayerSkillInRound
    {
        get;
    }
    bool IsUsingEnemySkillInRound
    {
        get;
    }

    /// <summary>
    /// 次のラウンドへ
    /// </summary>
    UniTask NextRound();

    /// <summary>
    /// ラウンドの設定
    /// </summary>
    void SetRoundCount(int count);

    /// <summary>
    /// 必殺技を使用したラウンドかどうかの設定
    /// </summary>
    void SetUsingSkillRound(bool isPlayer, bool isUsingSkill);
}