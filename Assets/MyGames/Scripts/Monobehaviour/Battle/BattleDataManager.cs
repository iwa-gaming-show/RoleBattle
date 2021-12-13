using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;

public class BattleDataManager : MonoBehaviour, IBattleDataManager
{
    PlayerData _player;
    PlayerData _enemy;
    BattlePhase _battlePhase;
    int _roundCount;
    int _earnedPoint;

    #region プロパティ
    public PlayerData Player => _player;
    public PlayerData Enemy => _enemy;
    public int RoundCount => _roundCount;
    public BattlePhase BattlePhase => _battlePhase;
    #endregion

    private void Awake()
    {
        ServiceLocator.Register<IBattleDataManager>(this);
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IBattleDataManager>(this);
    }

    /// <summary>
    /// プレイヤーデータの作成
    /// </summary>
    public void CreatePlayerData()
    {
        _player = new PlayerData(INITIAL_POINT);
        _enemy = new PlayerData(INITIAL_POINT);
    }

    /// <summary>
    /// ルームのデータの初期化
    /// </summary>
    public void InitRoomData()
    {
        _roundCount = INITIAL_ROUND_COUNT;
        _battlePhase = BattlePhase.NONE;
        _earnedPoint = INITIAL_EARNED_POINT;
    }

    /// <summary>
    /// プレイヤーデータの初期化
    /// </summary>
    public void InitPlayerData()
    {
        InitDataBy(true);
        InitDataBy(false);
    }

    /// <summary>
    /// プレイヤーごとに状態をリセットする
    /// </summary>
    /// <param name="isPlayer"></param>
    void InitDataBy(bool isPlayer)
    {
        GetPlayerDataBy(isPlayer).SetPoint(INITIAL_POINT);
        GetPlayerDataBy(isPlayer).SetCanUseSpSkill(true);
        GetPlayerDataBy(isPlayer).SetIsMyTurn(false);
    }

    /// <summary>
    /// プレイヤーの状態をリセットする
    /// </summary>
    public void ResetPlayerState()
    {
        ResetStateBy(true);
        ResetStateBy(false);
    }

    /// <summary>
    /// プレイヤーのターンを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetPlayerTurnBy(bool isPlayer)
    {
        return GetPlayerDataBy(isPlayer).IsMyTurn;
    }

    /// <summary>
    /// プレイヤーのターンを設定する
    /// </summary>
    public void SetIsPlayerTurnBy(bool isPlayer, bool isMyTurn)
    {
        GetPlayerDataBy(isPlayer).SetIsMyTurn(isMyTurn);
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetPlayerTurnEndBy(bool isPlayer)
    {
        return GetPlayerDataBy(isPlayer).IsMyTurnEnd;
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    public void SetIsPlayerTurnEndBy(bool isPlayer, bool isMyTurnEnd)
    {
        GetPlayerDataBy(isPlayer).SetIsMyTurnEnd(isMyTurnEnd);
    }

    /// <summary>
    /// プレイヤーごとに状態をリセットする
    /// </summary>
    /// <param name="isPlayer"></param>
    void ResetStateBy(bool isPlayer)
    {
        GetPlayerDataBy(isPlayer).SetIsMyTurnEnd(false);
        GetPlayerDataBy(isPlayer).SetIsUsingSpInRound(false);
        GetPlayerDataBy(isPlayer).SetIsFieldCardPlaced(false);
    }

    /// <summary>
    /// プレイヤーのデータを取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataBy(bool isPlayer)
    {
        if (isPlayer) return _player;
        return _enemy;
    }

    /// <summary>
    /// プレイヤーが必殺技が発動可能かどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetCanUseSpSkillBy(bool isPlayer)
    {
        return GetPlayerDataBy(isPlayer).CanUseSpSkill;
    }

    /// <summary>
    /// バトルの段階を設定します
    /// </summary>
    /// <param name="battlePhase"></param>
    public void SetBattlePhase(BattlePhase battlePhase)
    {
        _battlePhase = battlePhase;
    }

    /// <summary>
    /// プレイヤーがフィールドにカードを置いたかどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetIsFieldCardPlacedBy(bool isPlayer)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// プレイヤーがフィールドにカードを置いたかどうかを設定する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public void SetIsFieldCardPlacedBy(bool isPlayer, bool isFieldCardPlaced)
    {
        GetPlayerDataBy(isPlayer).SetIsFieldCardPlaced(isFieldCardPlaced);
    }

    /// <summary>
    /// プレイヤーのポイントを取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public int GetPlayerPointBy(bool isPlayer)
    {
        return GetPlayerDataBy(isPlayer).Point;
    }
}
