using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;
using static BattleResult;

public class BattleDataManager : MonoBehaviour, IBattleDataManager
{
    PlayerData _player;
    PlayerData _enemy;
    BattlePhase _battlePhase;
    int _roundCount;
    int _earnedPoint = INITIAL_EARNED_POINT;
    int _enemySpSkillRound;//エネミーが必殺技を使用するラウンド
    bool _canChangeTurn;
    bool _isDuringDirectingSpSkill;//必殺技の演出中か

    #region プロパティ
    public PlayerData Player => _player;
    public PlayerData Enemy => _enemy;
    public int RoundCount => _roundCount;
    public int EarnedPoint => _earnedPoint;
    public int EnemySpSkillRound => _enemySpSkillRound;
    public BattlePhase BattlePhase => _battlePhase;
    public bool CanChangeTurn => _canChangeTurn;
    public bool IsDuringDirectingSpSkill => _isDuringDirectingSpSkill;
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
    /// エネミーが必殺技を使用するラウンドを設定します
    /// </summary>
    /// <param name="spSkillRound"></param>
    public void SetEnemySpSkillRound(int spSkillRound)
    {
        _enemySpSkillRound = spSkillRound;
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
    /// 必殺技を発動する状態にする
    /// </summary>
    public void ActivatingSpSkillState(bool isPlayer)
    {
        GetPlayerDataBy(isPlayer).SetIsUsingSpInRound(true);
        GetPlayerDataBy(isPlayer).SetCanUseSpSkill(false);
        _isDuringDirectingSpSkill = true;
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
        return GetPlayerDataBy(isPlayer).IsFieldCardPlaced;
    }

    /// <summary>
    /// プレイヤーがフィールドにカードを置いたかどうかを設定する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="isFieldCardPlaced"></param>
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

    /// <summary>
    /// ターンを切り替えることができるかどうかを設定する
    /// </summary>
    /// <param name="can"></param>
    /// <returns></returns>
    public void SetCanChangeTurn(bool can)
    {
        _canChangeTurn = can;
    }

    /// <summary>
    /// プレイヤーのフィールドに配置したカードの種類を取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    public CardType GetCardTypeBy(bool isPlayer)
    {
        return GetPlayerDataBy(isPlayer).BattleCardType;
    }

    /// <summary>
    /// プレイヤーのフィールドに配置したカードの種類を設定します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="cardType"></param>
    public void RegisterCardTypeBy(bool isPlayer, CardType cardType)
    {
        GetPlayerDataBy(isPlayer).SetCardType(cardType);
    }

    /// <summary>
    /// ポイントを加算します
    /// </summary>
    /// <param name="isPlayer"></param>
    public void AddPointTo(bool isPlayer)
    {
        // 獲得ポイント含めたこれまでの取得ポイント
        int totalPoint = GetPlayerDataBy(isPlayer).Point + EarnPoint(GetPlayerDataBy(isPlayer).IsUsingSpInRound);

        GetPlayerDataBy(isPlayer).SetPoint(totalPoint);
    }

    /// <summary>
    /// 獲得ポイント
    /// </summary>
    /// <returns></returns>
    public int EarnPoint(bool isUsingSkillInRound)
    {
        int earnPoint = _earnedPoint;
        //このラウンドの間必殺技を使用していた場合
        if (isUsingSkillInRound)
            earnPoint *= SPECIAL_SKILL_MAGNIFICATION_BONUS;

        return earnPoint;
    }

    /// <summary>
    /// ラウンドの増加
    /// </summary>
    public void AddRoundCount()
    {
        _roundCount++;
    }

    /// <summary>
    /// バトルの結果を取得する
    /// </summary>
    public BattleResult JudgeBattleResult()
    {
        int playerPoint = _player.Point;
        int enemyPoint = _enemy.Point;

        if (playerPoint > enemyPoint) return BATTLE_WIN;
        if (playerPoint == enemyPoint) return BATTLE_DRAW;
        return BATTLE_LOSE;
    }

    /// <summary>
    /// 必殺技発動の演出中かどうかを設定する
    /// </summary>
    /// <param name="_isDuring"></param>
    public void SetIsDuringDirectingSpSkill(bool _isDuring)
    {
        _isDuringDirectingSpSkill = _isDuring;
    }
}