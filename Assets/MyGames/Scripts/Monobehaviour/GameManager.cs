using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WaitTimes;
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
    [Header("最大ラウンド数")]
    int _maxRoundCount = 3;

    [SerializeField]
    [Header("ラウンド数")]
    int _roundCount = INITIAL_ROUND_COUNT;

    [SerializeField]
    [Header("カウントダウンの秒数を設定")]
    int _defaultCountDownTime = DEFAULT_COUNT_DOWN_TIME;
    #endregion

    bool _canUsePlayerSpecialSkill;//必殺技が使用できるか
    bool _canUseEnemySpecialSkill;
    bool _isUsingPlayerSkillInRound;//必殺技を使用したラウンドか
    bool _isUsingEnemySkillInRound;
    bool _isDuringProductionOfSpecialSkill;//必殺技の演出中か
    int _myPoint;
    int _enemyPoint;
    int _countDownTime;
    GameResult _gameResult;
    TurnManager _turnManager;//プレイヤーのターン管理スクリプト
    CardManager _cardManager;//カードの管理スクリプト

    #region プロパティ
    public Transform MyBattleFieldTransform => _myBattleFieldTransform;
    public Transform EnemyBattleFieldTransform => _enemyBattleFieldTransform;
    public Transform MyHandTransform => _myHandTransform;
    public Transform EnemyHandTransform => _enemyHandTransform;
    public bool CanUsePlayerSpecialSkill => _canUsePlayerSpecialSkill;
    public bool CanUseEnemySpecialSkill => _canUseEnemySpecialSkill;
    public int RoundCount => _roundCount;
    public int MaxRoundCount => _maxRoundCount;
    public UIManager UIManager => _uiManager;
    public TurnManager TurnManager => _turnManager;
    public CardManager CardManager => _cardManager;
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

        _turnManager = GetComponent<TurnManager>();
        _cardManager = GetComponent<CardManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame(true));
    }

    IEnumerator StartGame(bool isFirstGame)
    {
        //1ラウンド目に行う処理
        if (isFirstGame)
        {
            _canUsePlayerSpecialSkill = true;//必殺技を使用可能に
            _canUseEnemySpecialSkill = true;
            _uiManager.ShowPoint(_myPoint, _enemyPoint);
            _uiManager.InitUIData();
            _turnManager.DecideTheTurn();
            _turnManager.DecideTheTurnOnEnemySp();
        }
        //1ラウンド目以降に行う処理
        else
        {
            _roundCount++;
        }

        _uiManager.HideUIAtStart();
        _cardManager.ResetFieldCard();
        yield return _uiManager.ShowRoundCountText(_roundCount, _maxRoundCount);
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
    /// ラウンドの状態をリセットする
    /// </summary>
    public void ResetRoundState()
    {
        //スキルの発動状態をリセット
        _isUsingPlayerSkillInRound = false;
        _isUsingEnemySkillInRound = false;
        _turnManager.SetIsMyTurnEnd(false);
        _turnManager.SetIsEnemyTurnEnd(false);
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
        _roundCount = INITIAL_ROUND_COUNT;
        _myPoint = INITIAL_POINT;
        _enemyPoint = INITIAL_POINT;
    }

    /// <summary>
    /// 次のラウンドへ
    /// </summary>
    public void NextRound()
    {
        if (_roundCount != _maxRoundCount)
        {
            StartCoroutine(StartGame(false));
        }
        else
        {
            //最終ラウンドならゲーム終了
            EndGame();
        }
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
    void EndGame()
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
        if (_myPoint > _enemyPoint) return GAME_WIN;
        if (_myPoint == _enemyPoint) return GAME_DRAW;
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
        _uiManager.ShowPoint(_myPoint, _enemyPoint);
    }

    /// <summary>
    /// ポイントの加算
    /// </summary>
    /// <param name="isPlayer"></param>
    void AddPointTo(bool isPlayer)
    {
        if (isPlayer)
        {
            _myPoint += EarnPlayerPoint();
            return;
        }

        _enemyPoint += EarnEnemyPoint();
    }

    /// <summary>
    /// エネミーの獲得ポイント
    /// </summary>
    /// <returns></returns>
    int EarnEnemyPoint()
    {
        if (_isUsingEnemySkillInRound)
        {
            return _earnedPoint * SPECIAL_SKILL_MAGNIFICATION_BONUS;
        }

        return _earnedPoint;
    }

    /// <summary>
    /// プレイヤーの獲得ポイント
    /// </summary>
    /// <returns></returns>
    int EarnPlayerPoint()
    {
        if (_isUsingPlayerSkillInRound)
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
            _canUsePlayerSpecialSkill = false;//使用済みに
            _isUsingPlayerSkillInRound = true;
            return;
        }

        _canUseEnemySpecialSkill = false;
        _isUsingEnemySkillInRound = true;
    }
}
