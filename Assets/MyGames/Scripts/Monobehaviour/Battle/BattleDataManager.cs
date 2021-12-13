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
        InitDataFor(true);
        InitDataFor(false);
    }

    /// <summary>
    /// プレイヤーごとに状態をリセットする
    /// </summary>
    /// <param name="isPlayer"></param>
    void InitDataFor(bool isPlayer)
    {
        GetPlayerDataFor(isPlayer).SetPoint(INITIAL_POINT);
        GetPlayerDataFor(isPlayer).SetCanUseSpSkill(true);
        GetPlayerDataFor(isPlayer).SetIsMyTurn(false);
    }

    /// <summary>
    /// プレイヤーの状態をリセットする
    /// </summary>
    public void ResetPlayerState()
    {
        ResetStateFor(true);
        ResetStateFor(false);
    }

    /// <summary>
    /// プレイヤーのターンを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetPlayerTurnFor(bool isPlayer)
    {
        return GetPlayerDataFor(isPlayer).IsMyTurn;
    }

    /// <summary>
    /// プレイヤーのターンを設定する
    /// </summary>
    public void SetIsPlayerTurnFor(bool isPlayer, bool isMyTurn)
    {
        GetPlayerDataFor(isPlayer).SetIsMyTurn(isMyTurn);
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetPlayerTurnEndFor(bool isPlayer)
    {
        return GetPlayerDataFor(isPlayer).IsMyTurnEnd;
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    public void SetIsPlayerTurnEndFor(bool isPlayer, bool isMyTurnEnd)
    {
        GetPlayerDataFor(isPlayer).SetIsMyTurnEnd(isMyTurnEnd);
    }

    /// <summary>
    /// プレイヤーごとに状態をリセットする
    /// </summary>
    /// <param name="isPlayer"></param>
    void ResetStateFor(bool isPlayer)
    {
        GetPlayerDataFor(isPlayer).SetIsMyTurnEnd(false);
        GetPlayerDataFor(isPlayer).SetIsUsingSpInRound(false);
        GetPlayerDataFor(isPlayer).SetIsFieldCardPlaced(false);
    }

    /// <summary>
    /// プレイヤーのデータを取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataFor(bool isPlayer)
    {
        if (isPlayer) return _player;
        return _enemy;
    }

    /// <summary>
    /// プレイヤーが必殺技が発動可能かどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetCanUseSpSkillFor(bool isPlayer)
    {
        return GetPlayerDataFor(isPlayer).CanUseSpSkill;
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
    /// プレイヤーがフィールドにカードを置いたかどうかを設定する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public void SetIsFieldCardPlacedFor(bool isPlayer, bool isFieldCardPlaced)
    {
        GetPlayerDataFor(isPlayer).SetIsFieldCardPlaced(isFieldCardPlaced);
    }

    /// <summary>
    /// プレイヤーがフィールドにカードを置いたかどうかを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public bool GetIsFieldCardPlacedFor(bool isPlayer)
    {
        return GetPlayerDataFor(isPlayer).IsFieldCardPlaced;
    }
}
