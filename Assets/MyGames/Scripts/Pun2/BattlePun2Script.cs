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
    [Header("カードリストを設定する(ScriptableObjectを参照)")]
    CardEntityList _cardEntityList;

    [SerializeField]
    [Header("自身の手札")]
    Transform _handTransform;

    [SerializeField]
    byte _maxPlayers = 2;

    [SerializeField]
    [Header("ゲーム盤のCanvasを設定する")]
    GameObject _multiBattleCanvas;

    [SerializeField]
    MultiBattleUIManager _multiBattleUIManager;

    bool _canStartBattle;
    bool _isEnemyIconPlaced;//エネミーのアイコンが設置されているか
    GameObject _playerIcon;//todo あとでスクリプト名になる可能性あり
    Player _player;
    Player _enemy;


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (_enemy == null) return;
        if (_canStartBattle) return;
        //_multiBattleManager.StartGame(true).Forget();
        //_multiBattleManager.PhotonView.RPC("StartGame", RpcTarget.All, true);
        _canStartBattle = true;
        Debug.Log("バトル開始!");
    }

    /// <summary>
    /// 接続完了時
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 入室失敗時
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions opt = new RoomOptions();
        opt.MaxPlayers = _maxPlayers;
        opt.PublishUserId = true;//お互いにuserIdを見えるようにする
        PhotonNetwork.CreateRoom(null, opt);
    }

    /// <summary>
    /// 自クライアントのルーム入室時
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        _playerIcon = PhotonNetwork.Instantiate("PlayerIcon", Vector3.zero, Quaternion.identity);
        _multiBattleUIManager.PlacePlayerIconBy(true, _playerIcon);

        InitPlayer();


        //対戦相手が既に入室している場合
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (_player.UserId != player.UserId)
            {
                _multiBattleUIManager.ShowPointBy(false, player.GetPoint());
                _multiBattleUIManager.SetSpButtonImageBy(false, player.GetCanUseSpSkill());
                //_multiBattleUIManager.SetPlayerIconBy(false, player.GetPlayerIcon());
                _enemy = player;
                Debug.Log("他クライアントを見つけました");
                break;
            }
        }
    }

    /// <summary>
    /// プレイヤーのデータの初期化
    /// </summary>
    void InitPlayer()
    {
        _player = PhotonNetwork.LocalPlayer;
        _player.SetPoint(INITIAL_POINT);
        _player.SetCanUseSpSkill(true);
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

        if (_player.UserId == targetPlayer.UserId)
            isPlayer = true;

        //ポイントをUIへ反映する
        _multiBattleUIManager.ShowPointBy(isPlayer, targetPlayer.GetPoint());
        _multiBattleUIManager.SetSpButtonImageBy(isPlayer, targetPlayer.GetCanUseSpSkill());
        CheckEnemyIcon();
    }

    /// <summary>
    /// エネミーのアイコンを調べます
    /// </summary>
    void CheckEnemyIcon()
    {
        if (_isEnemyIconPlaced) return;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerIcon"))
        {
            if (go != _playerIcon)
            {
                _multiBattleUIManager.PlacePlayerIconBy(false, go);
                _isEnemyIconPlaced = true;
            }
        }
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
        _enemy = newPlayer;
    }

    /// <summary>
    /// 他クライアントがルームから離れた時、もしくは非アクティブになった時
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

    }
}
