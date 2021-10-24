using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;
using static CardJudgement;
using static GameResult;

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
    [Header("ラウンド毎の勝者の獲得ポイント")]
    int _earnedPoint = 1;

    [SerializeField]
    [Header("カウントダウンの秒数を設定")]
    int _defaultCountDownTime = DEFAULT_COUNT_DOWN_TIME;
    #endregion

    bool _isDuringProductionOfSpecialSkill;//必殺技の演出中か
    int _countDownTime;
    GameResult _gameResult;
    TurnManager _turnManager;//プレイヤーのターン管理スクリプト
    CardManager _cardManager;//カードの管理スクリプト
    RoundManager _roundManager;//ラウンドの管理スクリプト
    PlayerData _player;
    PlayerData _enemy;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        InitPlayerData();
        StartCoroutine(StartGame(true));
    }

    /// <summary>
    /// プレイヤーデータの初期化
    /// </summary>
    void InitPlayerData()
    {
        _player = new PlayerData(INITIAL_POINT);
        _enemy = new PlayerData(INITIAL_POINT);
    }

    public IEnumerator StartGame(bool isFirstGame)
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

        _uiManager.HideUIAtStart();
        _cardManager.ResetFieldCard();
        yield return _uiManager.ShowRoundCountText(_roundManager.RoundCount, _roundManager.MaxRoundCount);
        _cardManager.DistributeCards();
        _turnManager.ChangeTurn();
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
                _uiManager.ShowCountDownText(_countDownTime);
            }

            yield return null;
        }

        //0になったらカードをランダムにフィールドへ移動しターンエンドする
        CardController targetCard = _cardManager.GetRandomCardFrom(_turnManager.IsMyTurn);
        Transform targetTransform = GetTargetBattleFieldTransform(_turnManager.IsMyTurn);

        yield return targetCard.CardEvent.MoveToBattleField(targetTransform);
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
    public void RetryGame()
    {
        InitializeGameData();
        StartCoroutine(StartGame(true));
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
        _uiManager.ToggleGameResultUI(true);
        _uiManager.SetGameResultText(CommonAttribute.GetStringValue(_gameResult));
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
    public void ReflectTheResult(CardJudgement result)
    {
        if (result == WIN)
        {
            AddPointTo(true);
        }
        else if (result == LOSE)
        {
            AddPointTo(false);
        }

        //UIへの反映
        StartCoroutine(_uiManager.ShowJudgementResultText(result.ToString()));
        _uiManager.ShowPoint(_player.Point, _enemy.Point);
    }

    /// <summary>
    /// ポイントの加算
    /// </summary>
    /// <param name="isPlayer"></param>
    void AddPointTo(bool isPlayer)
    {
        if (isPlayer)
        {
            _player.AddPoint(EarnPoint(_roundManager.IsUsingPlayerSkillInRound));
            return;
        }

        _enemy.AddPoint(EarnPoint(_roundManager.IsUsingEnemySkillInRound));
    }

    /// <summary>
    /// 獲得ポイント
    /// </summary>
    /// <returns></returns>
    int EarnPoint(bool isUsingSkillInRound)
    {
        //このラウンドの間必殺技を使用していた場合
        if (isUsingSkillInRound)
        {
            return _earnedPoint * SPECIAL_SKILL_MAGNIFICATION_BONUS;
        }
        return _earnedPoint;
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
