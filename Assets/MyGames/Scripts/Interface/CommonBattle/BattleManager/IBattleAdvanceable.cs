using System.Collections;
using Cysharp.Threading.Tasks;

public interface IBattleAdvanceable
{
    /// <summary>
    /// バトルを開始する
    /// </summary>
    UniTask StartBattle(bool isFirstBattle);

    /// <summary>
    /// バトルを終了する
    /// </summary>
    void EndBattle();

    /// <summary>
    /// バトルを再開する
    /// </summary>
    void RetryBattle();
}