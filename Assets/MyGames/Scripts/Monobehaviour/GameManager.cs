using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static InitializationData;
using static CardJudgement;
using static GameResult;
using static BattlePhase;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager _instance;

    #region インスペクターから設定
    [SerializeField]
    [Header("UI管理スクリプトを設定")]
    UIManager _uiManager;

    [SerializeField]
    [Header("自身の手札")]
    Transform _myHandTransform;

    [SerializeField]
    [Header("相手の手札")]
    Transform _enemyHandTransform;

    [SerializeField]
    [Header("自身のバトルフィールド")]
    Transform _myBattleFieldTransform;

    [SerializeField]
    [Header("相手のバトルフィールド")]
    Transform _enemyBattleFieldTransform;

    [SerializeField]
    [Header("カウントダウンの秒数を設定")]
    int _defaultCountDownTime = DEFAULT_COUNT_DOWN_TIME;
    #endregion

    bool _isDuringProductionOfSpecialSkill;//必殺技の演出中か
    int _countDownTime;
    GameResult _gameResult;
    BattlePhase _battlePhase;
    TurnManager _turnManager;//プレイヤーのターン管理
    CardManager _cardManager;//カードの管理
    RoundManager _roundManager;//ラウンドの管理
    PointManager _pointManager;//ポイントの管理
    PlayerData _player;
    PlayerData _enemy;
    CancellationTokenSource _tokenSource;

    #region プロパティ
    public Transform MyBattleFieldTransform => _myBattleFieldTransform;
    public Transform EnemyBattleFieldTransform => _enemyBattleFieldTransform;
    public Transform MyHandTransform => _myHandTransform;
    public Transform EnemyHandTransform => _enemyHandTransform;
    public PlayerData Player => _player;
    public PlayerData Enemy => _enemy;
    public UIManager UIManager => _uiManager;
    public TurnManager TurnManager => _turnManager;
    public CardManager CardManager => _cardManager;
    public RoundManager RoundManager => _roundManager;
    public PointManager PointManager => _pointManager;
    public BattlePhase BattlePhase => _battlePhase;
    public CancellationTokenSource TokenSource => _tokenSource;
    #endregion

    private void Awake()
    {
        //シングルトン化する
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _roundManager = GetComponent<RoundManager>();
        _turnManager = GetComponent<TurnManager>();
        _cardManager = GetComponent<CardManager>();
        _pointManager = GetComponent<PointManager>();
    }

    // Start is called before the first frame update
    async void Start()
    {
        _tokenSource = new CancellationTokenSource();
        InitPlayerData();
        await StartGame(true);
    }

    /// <summary>
    /// プレイヤーデータの初期化
    /// </summary>
    void InitPlayerData()
    {
        _player = new PlayerData(INITIAL_POINT);
        _enemy = new PlayerData(INITIAL_POINT);
    }

    /// <summary>
    /// ゲームを開始する
    /// </summary>
    /// <param name="isFirstGame"></param>
    /// <returns></returns>
    public async UniTask StartGame(bool isFirstGame)
    {
        //1ラウンド目に行う処理
        if (isFirstGame)
        {
            _player.SetCanUseSpecialSkill(true);//必殺技を使用可能に
            _enemy.SetCanUseSpecialSkill(true);
            _uiManager.ShowPoint(_player.Point, _enemy.Point);
            _uiManager.InitUIData();
            _turnManager.DecideTheTurn();
            _turnManager.DecideTheTurnOnEnemySp();
        }
        //1ラウンド目以降に行う処理
        else
        {
            _roundManager.AddRoundCount();
        }

        ResetGameState();
        _uiManager.HideUIAtStart();
        _cardManager.ResetFieldCard();
        await _uiManager.DirectionUIManager.ShowRoundCountText(_roundManager.RoundCount, _roundManager.MaxRoundCount);
        _cardManager.DistributeCards();
        _turnManager.ChangeTurn();
    }

    /// <summary>
    /// バトルの段階を切り替える
    /// </summary>
    /// <param name="phase"></param>
    public void ChangeBattlePhase(BattlePhase phase)
    {
        _battlePhase = phase;
    }

    /// <summary>
    /// カウントダウン
    /// </summary>
    /// <returns></returns>
    public IEnumerator CountDown()
    {
        _countDownTime = _defaultCountDownTime;

        while (_countDownTime > 0)
        {
            //必殺技の演出中はカウントしない
            if (_isDuringProductionOfSpecialSkill == false)
            {
                //1秒毎に減らしていきます
                yield return new WaitForSeconds(1f);
                _countDownTime--;
                _uiManager.DirectionUIManager.ShowCountDownText(_countDownTime);
            }

            yield return null;
        }

        //確認画面を全て閉じる
        _uiManager.CloseAllConfirmationPanels();

        //0になったらカードをランダムにフィールドへ移動しターンエンドする
        CardController targetCard = _cardManager.GetRandomCardFrom(_turnManager.IsMyTurn);
        Transform targetTransform = GetTargetBattleFieldTransform(_turnManager.IsMyTurn);

        yield return targetCard.CardEvent.MoveToBattleField(targetTransform);
    }

    public void Cancel()
    {
        _tokenSource.Cancel();
        Debug.Log("cancel requested");
    }

    /// <summary>
    /// 必殺技の演出中かフラグをセットする
    /// </summary>
    public void SetIsDuringProductionOfSpecialSkill(bool isDuringProduction)
    {
        _isDuringProductionOfSpecialSkill = isDuringProduction;
    }

    /// <summary>
    /// ゲームの状態をリセットする
    /// </summary>
    public void ResetGameState()
    {
        _turnManager.ResetTurn();
        _roundManager.ResetRoundState();
    }

    /// <summary>
    /// ゲームを再開する
    /// </summary>
    public async void RetryGame()
    {
        InitializeGameData();
        await StartGame(true);
    }

    /// <summary>
    /// ゲームデータを初期化する
    /// </summary>
    void InitializeGameData()
    {
        _roundManager.SetRoundCount(INITIAL_ROUND_COUNT);
        _player.SetPoint(INITIAL_POINT);
        _enemy.SetPoint(INITIAL_POINT);
    }

    /// <summary>
    /// 対象のバトル場のカードのTransformを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public Transform GetTargetBattleFieldTransform(bool isPlayer)
    {
        if (isPlayer) return _myBattleFieldTransform;
        return _enemyBattleFieldTransform;
    }

    /// <summary>
    /// ゲームを終了
    /// </summary>
    public void EndGame()
    {
        //ゲーム結果を判定
        _gameResult = JudgeGameResult();
        //勝敗の表示
        _uiManager.DirectionUIManager.ToggleGameResultUI(true);
        _uiManager.DirectionUIManager.SetGameResultText(CommonAttribute.GetStringValue(_gameResult));
    }

    /// <summary>
    /// ゲーム結果を取得する
    /// </summary>
    GameResult JudgeGameResult()
    {
        if (_player.Point > _enemy.Point) return GAME_WIN;
        if (_player.Point == _enemy.Point) return GAME_DRAW;
        return GAME_LOSE;
    }

    /// <summary>
    /// 結果を反映します
    /// </summary>
    /// <param name="result"></param>
    public async void ReflectTheResult(CardJudgement result)
    {
        ChangeBattlePhase(RESULT);

        if (result == WIN)
        {
            _pointManager.AddPointTo(true);
        }
        else if (result == LOSE)
        {
            _pointManager.AddPointTo(false);
        }

        //UIへの反映
        await _uiManager.DirectionUIManager.ShowJudgementResultText(result.ToString());
        _uiManager.ShowPoint(_player.Point, _enemy.Point);
    }

    /// <summary>
    /// 必殺技を使用します
    /// </summary>
    public void UsedSpecialSkill(bool isPlayer)
    {
        if (isPlayer)
        {
            _player.SetCanUseSpecialSkill(false);
            _roundManager.SetUsingSkillRound(isPlayer, true);
            return;
        }

        _enemy.SetCanUseSpecialSkill(false);
        _roundManager.SetUsingSkillRound(isPlayer, true);
    }
}
