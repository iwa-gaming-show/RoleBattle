/// <summary>
/// ターンに関するデータ
/// </summary>
public class TurnData
{
    bool _isMyTurn;//自身のターンか
    bool _isMyTurnEnd;
    bool _isEnemyTurnEnd;
    int _enemySpecialSkillTurn;//敵が必殺技を使用するターン

    #region プロパティ
    public bool IsMyTurn => _isMyTurn;
    public bool IsMyTurnEnd => _isMyTurnEnd;
    public bool IsEnemyTurnEnd => _isEnemyTurnEnd;
    public int EnemySpecialSkillTurn => _enemySpecialSkillTurn;
    #endregion

    /// <summary>
    /// 自身のターンか設定する
    /// </summary>
    public void SetIsMyTurn(bool isMyTurn)
    {
        _isMyTurn = isMyTurn;
    }

    /// <summary>
    ///　エネミーが必殺技を使用するターンを設定します
    /// </summary>
    /// <param name="turn"></param>
    public void SetEnemySpecialSkillTurn(int turn)
    {
        _enemySpecialSkillTurn = turn;
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
