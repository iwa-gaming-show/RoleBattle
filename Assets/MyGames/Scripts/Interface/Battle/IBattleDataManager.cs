using System.Collections;
using Cysharp.Threading.Tasks;

public interface IBattleDataManager
{
    PlayerData Player
    {
        get;
    }
    PlayerData Enemy
    {
        get;
    }

    int RoundCount
    {
        get;
    }

    BattlePhase BattlePhase
    {
        get;
    }

    bool CanChangeTurn
    {
        get;
    }

    /// <summary>
    /// プレイヤーデータの作成
    /// </summary>
    void CreatePlayerData();

    /// <summary>
    /// ルームのデータの初期化
    /// </summary>
    void InitRoomData();

    /// <summary>
    /// プレイヤーデータの初期化
    /// </summary>
    void InitPlayerData();

    /// <summary>
    /// プレイヤーのターンを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    bool GetPlayerTurnBy(bool isPlayer);

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    bool GetPlayerTurnEndBy(bool isPlayer);

    /// <summary>
    /// プレイヤーの状態をリセットする
    /// </summary>
    void ResetPlayerState();

    /// <summary>
    /// プレイヤーのターンを設定する
    /// </summary>
    void SetIsPlayerTurnBy(bool isPlayer, bool isMyTurn);

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    void SetIsPlayerTurnEndBy(bool isPlayer, bool isMyTurnEnd);

    /// <summary>
    /// プレイヤーが必殺技が発動可能かどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    bool GetCanUseSpSkillBy(bool isPlayer);

    /// <summary>
    /// バトルの段階を設定します
    /// </summary>
    /// <param name="battlePhase"></param>
    void SetBattlePhase(BattlePhase battlePhase);

    /// <summary>
    /// プレイヤーがフィールドにカードを置いたかどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    bool GetIsFieldCardPlacedBy(bool isPlayer);

    /// <summary>
    /// プレイヤーがフィールドにカードを置いたかどうかを設定する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    void SetIsFieldCardPlacedBy(bool isPlayer, bool isFieldCardPlaced);

    /// <summary>
    /// プレイヤーのポイントを取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    int GetPlayerPointBy(bool isPlayer);

    /// <summary>
    /// ターンを切り替えることができるかどうかを設定する
    /// </summary>
    /// <param name="can"></param>
    /// <returns></returns>
    void SetCanChangeTurn(bool can);
}