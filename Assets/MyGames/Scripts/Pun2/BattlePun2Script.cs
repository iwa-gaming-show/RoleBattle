using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;

public class BattlePun2Script : MonoBehaviourPunCallbacks
{
    [SerializeField]
    MultiBattleManager _multiBattleManager;

    [SerializeField]
    byte maxPlayers = 2;

    // Start is called before the first frame update
    void Start()
    {
        //初期化処理及び、接続

        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 接続完了時
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("接続したお");
        PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayers);
    }

    /// <summary>
    /// 自クライアントのロビー入室時
    /// </summary>
    public override void OnJoinedLobby()
    {
        
    }

    /// <summary>
    /// 自クライアントのルーム入室時
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("ランダムに入室");
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
        
    }

    /// <summary>
    /// 他クライアントがルームから離れた時、もしくは非アクティブになった時
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }
}
