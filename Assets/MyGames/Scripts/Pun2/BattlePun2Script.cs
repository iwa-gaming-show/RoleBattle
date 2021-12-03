using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Cysharp.Threading.Tasks;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using static InitializationData;
using static BattlePhase;

public class BattlePun2Script : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    [SerializeField]
    MultiBattleManager _multiBattleManager;

    [SerializeField]
    byte _maxPlayers = 2;

    [SerializeField]
    [Header("ゲーム盤のCanvasを設定する")]
    GameObject _multiBattleCanvas;

    [SerializeField]
    MultiBattleUIManager _multiBattleUIManager;

    bool _canStartBattle;
    bool _decidedTurn;
    bool _isEnemyIconPlaced;//エネミーのアイコンが設置されているか
    GameObject _playerIcon;//todo あとでスクリプト名になる可能性あり
    Player _player;
    Player _enemy;
    Room _room;
    PunTurnManager _punTurnManager;
    PhotonView _photonView;
    BattlePhase _battlePhase;

    void Awake()
    {
        _punTurnManager = GetComponent<PunTurnManager>();
        _punTurnManager.TurnManagerListener = this;
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        if (_enemy == null) return;
        if (_canStartBattle) return;
        _canStartBattle = true;
        StartBattle(true).Forget();
    }

    /// <summary>
    /// バトルを開始する
    /// </summary>
    public async UniTask StartBattle(bool isFirstGame)
    {
        //1ラウンド目に行う処理
        if (isFirstGame)
        {
            InitRoomData();
            //_battleUIManager.InitUIData();//中身は必殺技のdescriptionsの文言を設定できる、一旦保留
            DecideTheTurn();
        }
        ResetPlayerState();
        //_multiBattleUIManager.HideUIAtStart();
        _multiBattleUIManager.ResetFieldCards();
        await _multiBattleUIManager.ShowRoundCountText(_room.GetRoundCount());
        _multiBattleUIManager.DistributeCards();
    }

    /// <summary>
    /// ターンを開始します
    /// </summary>
    void StartTurn()
    {
        if (_player == null || _enemy == null) return;
        if (_decidedTurn) return;
        _decidedTurn = true;

        if (PhotonNetwork.IsMasterClient)
        {
            _punTurnManager.BeginTurn();
        }
    }

    /// <summary>
    /// プレイヤーのカスタムプロパティが呼び出された時
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        //自身と相手のデータへそれぞれ紐付けする
        bool isPlayer = IsUpdatePlayer(targetPlayer);

        _multiBattleUIManager.ShowPointBy(isPlayer, targetPlayer.GetPoint());
        _multiBattleUIManager.SetSpButtonImageBy(isPlayer, targetPlayer.GetCanUseSpSkill());
        CheckEnemyIcon();
        CheckPlayerTurnEnd(targetPlayer);

        StartTurn();
    }

    /// <summary>
    /// 更新プレイヤーかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool IsUpdatePlayer(Player targetPlayer)
    {
        return (_player.UserId == targetPlayer.UserId);
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
    /// プレイヤーのターンの終了を確認します
    /// </summary>
    /// <param name="player"></param>
    void CheckPlayerTurnEnd(Player player)
    {
        if (player.GetIsMyTurnEnd() == false) return;

        Debug.Log(player.GetCanPlaceCardToField());
        _punTurnManager.SendMove(null, true);
    }

    /// <summary>
    /// プレイヤーの状態をリセットする
    /// </summary>
    public void ResetPlayerState()
    {
        _player.SetIsMyTurnEnd(false);
        _player.SetIsUsingSpInRound(false);
    }

    /// <summary>
    /// 先攻、後攻のターンを決めます
    /// </summary>
    public void DecideTheTurn()
    {
        if (_player.IsMasterClient == false) return;

        //trueならmasterClientを先攻にする
        if (RandomBool()) PhotonNetwork.MasterClient.SetIsMyTurn(true);
        else PhotonNetwork.PlayerListOthers[0].SetIsMyTurn(true);
    }

    /// <summary>
    /// bool型をランダムに取得する
    /// </summary>
    /// <returns></returns>
    bool RandomBool()
    {
        return Random.Range(0, 2) == 0;
    }

    // <summary>
    /// ターンを切り替える
    /// </summary>
    void ChangeTurn()
    {
        _player.SetIsMyTurnEnd(false);

        //各プレイヤーのターンのフラグを逆にする
        _player.SetIsMyTurn(!_player.GetIsMyTurn());
        _enemy.SetIsMyTurn(!_enemy.GetIsMyTurn());
        _decidedTurn = false;
    }

    /// <summary>
    /// ターン開始時に呼ばれます
    /// </summary>
    /// <param name="turn"></param>
    void IPunTurnManagerCallbacks.OnTurnBegins(int turn)
    {
        _player.SetCanPlaceCardToField(true);//バトル場へのカード配置フラグ
        _room.SetIntBattlePhase(SELECTION);//カード選択フェイズへ
        PlayerTurn(_player.GetIsMyTurn()).Forget();
    }

    /// <summary>
    /// プレイヤーのターンを開始します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    async UniTask PlayerTurn(bool isPlayer)
    {
        await _multiBattleUIManager.ShowThePlayerTurnText(isPlayer);
        StopAllCoroutines();//前のカウントダウンが走っている可能性があるため一度止めます
        StartCoroutine(CountDown());
    }

    /// <summary>
    /// カウントダウン
    /// </summary>
    public IEnumerator CountDown()
    {
        while (_punTurnManager.RemainingSecondsInTurn > 0)
        {
            _multiBattleUIManager.ShowCountDownText((int)_punTurnManager.RemainingSecondsInTurn);

            //必殺技の演出中はカウントしない
            //if (_isDuringProductionOfSpecialSkill == false)
            //{
            //    //1秒毎に減らしていきます
            //    yield return new WaitForSeconds(1f);
            //    _countDownTime--;
            //    _battleUIManager.ShowCountDownText(_countDownTime);
            //}

            yield return null;
        }
    }

    /// <summary>
    /// ターン完了時に呼ばれます
    /// </summary>
    /// <param name="turn"></param>
    void IPunTurnManagerCallbacks.OnTurnCompleted(int turn)
    {
        Debug.Log("ターン完了");
        bool placedCardEachPlayer =
            (_player.GetCanPlaceCardToField() == false
            && _enemy.GetCanPlaceCardToField() == false);

        Debug.Log("お互いにカードを置いたか" + placedCardEachPlayer);
        ChangeTurn();


        //各プレイヤーがカードをフィールドに配置した場合
        //if (placedCardEachPlayer)
        //{
        //    //お互いのプレイヤーがカードを場に出しているならターンを切り替えずバトルする
        //}
        //else
        //{
        //    ChangeTurn();
        //}
    }

    /// <summary>
    /// プレイヤーが移動したときに呼び出されます
    /// </summary>
    /// <param name="player"></param>
    /// <param name="turn"></param>
    /// <param name="move"></param>
    void IPunTurnManagerCallbacks.OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("プレイヤーが移動");
    }

    /// <summary>
    /// プレイヤーがターンを終了したときに呼ばれます
    /// </summary>
    /// <param name="player"></param>
    /// <param name="turn"></param>
    /// <param name="move"></param>
    void IPunTurnManagerCallbacks.OnPlayerFinished(Player player, int turn, object move)
    {
        Debug.Log("ターン終了");
    }

    /// <summary>
    /// タイムアウトでターンが終了した時に呼ばれます
    /// </summary>
    /// <param name="turn"></param>
    void IPunTurnManagerCallbacks.OnTurnTimeEnds(int turn)
    {
        Debug.Log("カウント終了!");
        //ChangeTurn();
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
        InitPlayerIcon();
        _player = PhotonNetwork.LocalPlayer;
        _room = PhotonNetwork.CurrentRoom;
        InitPlayerData();

        //対戦相手が既に入室している場合
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (_player.UserId != player.UserId)
            {
                _multiBattleUIManager.ShowPointBy(false, player.GetPoint());
                _multiBattleUIManager.SetSpButtonImageBy(false, player.GetCanUseSpSkill());
                _enemy = player;
                break;
            }
        }
    }

    /// <summary>
    /// プレイヤーアイコンの初期設定
    /// </summary>
    void InitPlayerIcon()
    {
        _playerIcon = PhotonNetwork.Instantiate("PlayerIcon", Vector3.zero, Quaternion.identity);
        _multiBattleUIManager.PlacePlayerIconBy(true, _playerIcon);
    }

    /// <summary>
    /// プレイヤーのデータの初期化
    /// </summary>
    void InitPlayerData()
    {
        _player.SetPoint(INITIAL_POINT);
        _player.SetCanUseSpSkill(true);
        _player.SetCanPlaceCardToField(false);
    }

    /// <summary>
    /// ルームのデータの初期化
    /// </summary>
    void InitRoomData()
    {
        if (_player.IsMasterClient == false) return;
        _room.SetRoundCount(INITIAL_ROUND_COUNT);
        _room.SetIntBattlePhase(NONE);
    }

    /// <summary>
    /// ルームのカスタムプロパティが呼び出された時
    /// </summary>
    /// <param name="propertiesThatChanged"></param>
    public override void OnRoomPropertiesUpdate(PhotonHashTable propertiesThatChanged)
    {
        Debug.Log("updateBattlePhase" + propertiesThatChanged["BattlePhase"]);
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
