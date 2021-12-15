using System.Collections;
using Cysharp.Threading.Tasks;

public interface ICountDowner
{
    /// <summary>
    /// 先攻、後攻のターンを決めます
    /// </summary>
    void DecideTheTurn();

    /// <summary>
    /// ターンを開始します
    /// </summary>
    void StartTurn();

    /// <summary>
    /// プレイヤーのターンを開始します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    UniTask PlayerTurn();

    /// <summary>
    /// ターンを切り替えます
    /// </summary>
    void ChangeTurn();
}