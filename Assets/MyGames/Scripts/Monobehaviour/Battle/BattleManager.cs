using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static InitializationData;
using static BattlePhase;

public class BattleManager : MonoBehaviour
{
    #region
    [SerializeField]
    [Header("最大ラウンド数")]
    int _maxRoundCount = 3;

    [SerializeField]
    [Header("カウントダウンの秒数を設定")]
    int _defaultCountDownTime = DEFAULT_COUNT_DOWN_TIME;

    [SerializeField]
    [Header("ゲーム盤のCanvasを設定する")]
    BattleUIManager _battleUIManager;
    #endregion

    int _countDownTime;
    GameObject _playerIcon;

    //bool _isDuringProductionOfSpecialSkill;//必殺技の演出中か
    //bool _isOnline;//falseはオフライン
    ////BattleResult _battleResult;
    //CancellationToken _token;
    //ITurnManager _turnManager;//プレイヤーのターン管理
    //ICardManager _cardManager;//カードの管理
    //IRoundManager _roundManager;//ラウンドの管理
    //IFieldTransformManager _fieldTransformManager;//フィールドのTransformの管理
    //IPointManager _pointManager;//ポイントの管理
    IBattleDataManager _battleDataManager;


    #region プロパティ
    //public CancellationToken Token => _token;
    //public bool IsDuringProductionOfSpecialSkill => _isDuringProductionOfSpecialSkill;
    #endregion


    private void Awake()
    {
        //ServiceLocator.Register<IBattleManager>(this);
    }

    void OnDestroy()
    {
        //ServiceLocator.UnRegister<IBattleManager>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        //_token = this.GetCancellationTokenOnDestroy();
        _battleDataManager = ServiceLocator.Resolve<IBattleDataManager>();
        _battleDataManager.CreatePlayerData();
        _battleDataManager.InitPlayerData();
        StartBattle(true).Forget();
    }

    /// <summary>
    /// バトルを開始する
    /// </summary>
    public async UniTask StartBattle(bool isFirstBattle)
    {
        //1ラウンド目に行う処理
        if (isFirstBattle)
        {
            _battleDataManager.InitRoomData();
            _battleUIManager.InitSpSkillDescriptions();
            DecideTheTurn();
            //pointの表示
            //enemyが必殺技を使用するタイミングを決める
        }
        _battleDataManager.ResetPlayerState();
        _battleUIManager.HideUIAtStart();
        _battleUIManager.ResetFieldCards();
        await _battleUIManager.ShowRoundCountText(_battleDataManager.RoundCount, _maxRoundCount);
        _battleUIManager.DistributeCards();
        StartTurn();
    }

    /// <summary>
    /// ターンを開始します
    /// </summary>
    void StartTurn()
    {
        PlayerTurn().Forget();
    }

    /// <summary>
    /// プレイヤーのターンを開始します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    async UniTask PlayerTurn()
    {
        _battleDataManager.SetBattlePhase(SELECTION);//カード選択フェイズへ
        await _battleUIManager.ShowThePlayerTurnText(_battleDataManager.GetPlayerTurnFor(true));
        StopAllCoroutines();//前のカウントダウンが走っている可能性があるため一度止めます
        StartCoroutine(CountDown());
    }

    /// <summary>
    /// カウントダウン
    /// </summary>
    public IEnumerator CountDown()
    {
        _countDownTime = _defaultCountDownTime;
        while (_countDownTime > 0)
        {
            //1秒毎に減らしていきます
            yield return new WaitForSeconds(1f);
            _countDownTime--;
            _battleUIManager.ShowCountDownText(_countDownTime);
            yield return null;
        }

        //DoIfCountDownTimeOut();
    }

    /// <summary>
    /// 先攻、後攻のターンを決めます
    /// </summary>
    public void DecideTheTurn()
    {
        //trueなら自身を先攻にする
        if (RandomBool()) _battleDataManager.SetIsPlayerTurnFor(true, true);
        else _battleDataManager.SetIsPlayerTurnFor(false, true);
    }

    /// <summary>
    /// bool型をランダムに取得する
    /// </summary>
    /// <returns></returns>
    bool RandomBool()
    {
        return Random.Range(0, 2) == 0;
    }
}