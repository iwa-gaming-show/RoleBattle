using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;
using static BattleResult;
using Photon.Realtime;
using Photon.Pun;

public class MultiBattleDataManager : MonoBehaviour,
    IMultiBattleDataManager
{
    Player _player;
    Player _enemy;
    Room _room;

    #region プロパティ
    public Player Player => _player;
    public Player Enemy => _enemy;
    public Room Room => _room;
    #endregion

    private void Awake()
    {
        ServiceLocator.Register<IMultiBattleDataManager>(this);
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IMultiBattleDataManager>(this);
    }

    /// <summary>
    /// プレイヤーを設定します
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// 対戦相手を設定します
    /// </summary>
    /// <param name="player"></param>
    public void SetEnemy(Player enemy)
    {
        _enemy = enemy;
    }

    /// <summary>
    /// ルームを設定します
    /// </summary>
    /// <param name="room"></param>
    public void SetRoom(Room room)
    {
        _room = room;
    }

    /// <summary>
    /// プレイヤーのデータの初期化
    /// </summary>
    public void InitPlayerData()
    {
        _player.SetPoint(INITIAL_POINT);
        _player.SetCanUseSpSkill(true);
        _player.SetIsMyTurn(false);
    }

    /// <summary>
    /// ルームのデータの初期化
    /// </summary>
    public void InitRoomData()
    {
        if (_player.IsMasterClient == false) return;
        _room.SetRoundCount(INITIAL_ROUND_COUNT);
        _room.SetIntBattlePhase(BattlePhase.NONE);
        _room.SetEarnedPoint(INITIAL_EARNED_POINT);
    }

    /// <summary>
    /// プレイヤーのデータを取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public Player GetPlayerBy(bool isPlayer)
    {
        if (isPlayer) return _player;
        return _enemy;
    }

    /// <summary>
    /// 更新プレイヤーかどうか
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsUpdatePlayer(Player targetPlayer)
    {
        return (_player.UserId == targetPlayer.UserId);
    }

    /// <summary>
    /// マスタークライアントかどうか
    /// </summary>
    /// <returns></returns>
    public bool IsMasterClient()
    {
        return (_player.IsMasterClient);
    }

    /// <summary>
    /// お互いのプレイヤーがカードを判定したかどうか
    /// </summary>
    /// <returns></returns>
    public bool IsCardJudgedForEachPlayer()
    {
        return (_player.GetIsCardJudged() && _enemy.GetIsCardJudged());
    }

    /// <summary>
    /// カードの判定をしたかどうかを設定する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="isCardJudged"></param>
    public void SetIsCardJudged(Player player, bool isCardJudged)
    {
        player.SetIsCardJudged(isCardJudged);
    }

    /// <summary>
    /// プレイヤーのターンかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool GetIsMyTurn(Player player)
    {
        return player.GetIsMyTurn();
    }

    /// <summary>
    /// プレイヤーのターンかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    public void SetIsMyTurn(Player player, bool isMyTurn)
    {
        player.SetIsMyTurn(isMyTurn);
    }

    // <summary>
    /// プレイヤーのターンが終了したかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool GetIsMyTurnEnd(Player player)
    {
        return player.GetIsMyTurnEnd();
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurnEnd"></param>
    public void SetIsMyTurnEnd(Player player, bool isMyTurnEnd)
    {
        player.SetIsMyTurnEnd(isMyTurnEnd);
    }

    /// <summary>
    /// お互いのプレイヤーが再戦を希望したかどうか
    /// </summary>
    /// <returns></returns>
    public bool IsRetryingBattleForEachPlayer()
    {
        return (_player.GetIsRetryingBattle() && _enemy.GetIsRetryingBattle());
    }

    /// <summary>
    /// 再戦するかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isRetryingBattle"></param>
    public void SetIsRetryingBattle(Player player, bool isRetryingBattle)
    {
        player.SetIsRetryingBattle(isRetryingBattle);
    }

    /// <summary>
    /// プレイヤーの状態をリセットする
    /// </summary>
    public void ResetPlayerState()
    {
        _player.SetIsMyTurnEnd(false);
        _player.SetIsUsingSpInRound(false);
        _player.SetIsFieldCardPlaced(false);
    }

    /// <summary>
    /// 対戦相手かどうか
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsEnemy(string userId)
    {
        return (_player.UserId != userId);
    }

    /// <summary>
    /// マスタークライアントを取得します
    /// </summary>
    /// <returns></returns>
    public Player GetMasterClient()
    {
        return PhotonNetwork.MasterClient;
    }

    /// <summary>
    /// 自分以外のプレイヤーを取得します
    /// </summary>
    /// <returns></returns>
    public Player GetOtherPlayer()
    {
        return PhotonNetwork.PlayerListOthers[0];
    }

    /// <summary>
    /// お互いのプレイヤーがフィールドにカードを出しているか
    /// </summary>
    /// <returns></returns>
    public bool IsEachPlayerFieldCardPlaced()
    {
        return _player.GetIsFieldCardPlaced() && _enemy.GetIsFieldCardPlaced();
    }

    /// <summary>
    /// プレイヤーのポイントを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public int GetPoint(Player player)
    {
        return player.GetPoint();
    }

    /// <summary>
    /// プレイヤーのポイントを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="point"></param>
    public void SetPoint(Player player, int point)
    {
        player.SetPoint(point);
    }

    /// <summary>
    /// 必殺技の発動中かどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool GetIsUsingSpInRound(Player player)
    {
        return player.GetIsUsingSpInRound();
    }

    /// <summary>
    /// 必殺技の発動中かどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isUsingSpInRound"></param>
    public void SetIsUsingSpInRound(Player player, bool isUsingSpInRound)
    {
        player.SetIsUsingSpInRound(isUsingSpInRound);
    }

    /// <summary>
    /// フィールドへのカードのタイプを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public int GetIntBattleCardType(Player player)
    {
        return player.GetIntBattleCardType();
    }

    /// <summary>
    /// フィールドへのカードのタイプを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cardType"></param>
    public void SetIntBattleCardType(Player player, CardType cardType)
    {
        player.SetIntBattleCardType(cardType);
    }
}