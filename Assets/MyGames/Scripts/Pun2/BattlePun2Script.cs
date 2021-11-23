using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using static InitializationData;
using static PlayerPropertiesExtensions;

public class BattlePun2Script : MonoBehaviourPunCallbacks
{
    [SerializeField]
    MultiBattleManager _multiBattleManager;

    [SerializeField]
    byte _maxPlayers = 2;

    bool _canStartBattle;
    //Player _player;
    //Player _enemy;


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (_multiBattleManager.EnemyM == null) return;
        if (_canStartBattle) return;

        _multiBattleManager.StartGame(true).Forget();
        _canStartBattle = true;
        Debug.Log("バトル開始!");
    }

    /// <summary>
    /// 接続完了時
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(null, _maxPlayers);
    }

    /// <summary>
    /// 自クライアントのルーム入室時
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.LocalPlayer.SetPoint(INITIAL_POINT);
        PhotonNetwork.LocalPlayer.SetCanUseSpecialSkill(true);

        //対戦相手が既に入室している場合
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (PhotonNetwork.LocalPlayer.UserId != player.UserId)
            {
                _multiBattleManager.SetPlayer(player, false);
                break;
            }
        }
    }

    /// <summary>
    /// カスタムプロパティが呼び出された時
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        //自身と相手のデータへそれぞれ紐付けする
        bool isPlayer = false;

        if (PhotonNetwork.LocalPlayer.UserId == targetPlayer.UserId)
        {
            isPlayer = true;
        }
        
        _multiBattleManager.SetPlayer(targetPlayer, isPlayer);

        //todo 検証用後で消す
        //_multiBattleManager.SetPlayer(targetPlayer, false);
    }

    /// <summary>
    /// 自クライアントがルームから退室した時
    /// </summary>
    public override void OnLeftRoom()
    {
        
    }

    /// <summary>
    /// 他クライアントがルームに入室した時
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _multiBattleManager.SetPlayer(newPlayer, false);
    }

    /// <summary>
    /// 他クライアントがルームから離れた時、もしくは非アクティブになった時
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }
}
