using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class BattlePun2Script : MonoBehaviourPunCallbacks
{
    [SerializeField]
    [Header("最大対戦プレイヤー数")]
    byte _maxPlayers = 2;

    [SerializeField]
    [Header("ゲーム盤のCanvasを設定する")]
    MultiBattleUIManager _multiBattleUIManager;

    [SerializeField]
    [Header("BattleManagerを設定する")]
    MultiBattleManager _multiBattleManager;

    IMultiBattleDataManager _multiBattleDataManager;

    async UniTask Start()
    {
        await Fade._instance.StartFadeIn();
        Loading._instance.ToggleUI(true);
        _multiBattleUIManager.ToggleDisplayLeaveRoomButton(true);
        PhotonNetwork.ConnectUsingSettings();
        _multiBattleDataManager = ServiceLocator.Resolve<IMultiBattleDataManager>();
    }

    /// <summary>
    /// 接続完了時
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 切断時
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        //タイトルへ
        Loading._instance.ToggleUI(false);
        GameManager._instance.ClickToLoadScene(SceneType.GameTitle);
    }

    /// <summary>
    /// 自クライアントのルーム入室時
    /// </summary>
    public override void OnJoinedRoom()
    {
        _multiBattleManager.InitPlayerIcon();
        _multiBattleDataManager.Init();

        if (PhotonNetwork.PlayerList.Length == _maxPlayers)
        {
            _multiBattleManager.PhotonView.RPC("RpcPrepareBattle", RpcTarget.All);
        }
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
    /// 自クライアントがルームから退室した時
    /// </summary>
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
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
        LeaveRoomAfterPlayerDisconnected().Forget();
    }

    /// <summary>
    /// 切断された時、部屋を退室します
    /// </summary>
    /// <returns></returns>
    async UniTask LeaveRoomAfterPlayerDisconnected()
    {
        await _multiBattleUIManager.ViewDisconectedDialog();
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// プレイヤーのカスタムプロパティが呼び出された時
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        //自身と相手のデータへそれぞれ紐付けする
        bool isPlayer = _multiBattleDataManager.IsUpdatePlayer(targetPlayer);

        _multiBattleUIManager.ShowPointBy(isPlayer, targetPlayer.GetPoint());
        _multiBattleUIManager.SetSpButtonImageBy(isPlayer, targetPlayer.GetCanUseSpSkill());
        _multiBattleManager.CheckPlayerTurnEnd(_multiBattleDataManager);
        _multiBattleManager.ChangeTurn();
        _multiBattleManager.CheckToNextRound(_multiBattleDataManager);
        _multiBattleManager.CheckRetryingBattle(_multiBattleDataManager);
    }

    /// <summary>
    /// ルームのカスタムプロパティが呼び出された時
    /// </summary>
    /// <param name="propertiesThatChanged"></param>
    public override void OnRoomPropertiesUpdate(PhotonHashTable propertiesThatChanged)
    {
        _multiBattleManager.CheckActivatingSpSkill(_multiBattleDataManager);
    }
}