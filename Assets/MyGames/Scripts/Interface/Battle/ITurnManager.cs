using Cysharp.Threading.Tasks;

public interface ITurnManager: IGameDataResetable
{
    bool IsMyTurn
    {
        get;
    }

    /// <summary>
    /// ターンの終了
    /// </summary>
    void EndTurn();

    /// <summary>
    /// 先攻、後攻のターンを決めます
    /// </summary>
    void DecideTheTurn();

    /// <summary>
    /// エネミーが必殺技を使用するターンを決めます
    /// </summary>
    void DecideTheTurnOnEnemySp(int maxRoundCount);

    /// <summary>
    /// ターンを切り替える
    /// </summary>
    UniTaskVoid ChangeTurn();
}