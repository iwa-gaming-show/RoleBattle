using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static InitializationData;
using static CardJudgement;
using static GameResult;
using static BattlePhase;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour, IBattleManager
{
    [HideInInspector]
    public static BattleManager _instance;

    #region
    [SerializeField]
    [Header("カウントダウンの秒数を設定")]
    int _defaultCountDownTime = DEFAULT_COUNT_DOWN_TIME;
    #endregion

    bool _isDuringProductionOfSpecialSkill;//必殺技の演出中か
    int _countDownTime;
    GameResult _gameResult;
    BattlePhase _battlePhase;
    PlayerData _player;
    PlayerData _enemy;
    CancellationToken _token;
    IBattleUIManager _battleUIManager;
    ITurnManager _turnManager;//プレイヤーのターン管理
    ICardManager _cardManager;//カードの管理
    IRoundManager _roundManager;//ラウンドの管理
    IFieldTransformManager _fieldTransformManager;//フィールドのTransformの管理
    IPointManager _pointManager;//ポイントの管理


    #region プロパティ
    public PlayerData Player => _player;
    public PlayerData Enemy => _enemy;
    public BattlePhase BattlePhase => _battlePhase;
    public CancellationToken Token => _token;
    public bool IsDuringProductionOfSpecialSkill => _isDuringProductionOfSpecialSkill;
    #endregion


    private void Awake()
    {
        ServiceLocator.Register<IBattleManager>(this);
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IBattleManager>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _token = this.GetCancellationTokenOnDestroy();
        _roundManager = ServiceLocator.Resolve<IRoundManager>();
        _turnManager = ServiceLocator.Resolve<ITurnManager>();
        _cardManager = ServiceLocator.Resolve<ICardManager>();
        _pointManager = ServiceLocator.Resolve<IPointManager>();
        _fieldTransformManager = ServiceLocator.Resolve<IFieldTransformManager>();
        _battleUIManager = ServiceLocator.Resolve<IBattleUIManager>();

        StartGame(true).Forget();
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
            InitPlayerData();
            _player.SetCanUseSpecialSkill(true);//必殺技を使用可能に
            _enemy.SetCanUseSpecialSkill(true);
            _roundManager.SetRoundCount(INITIAL_ROUND_COUNT);
            _battleUIManager.ShowPoint(_player.Point, _enemy.Point);
            _battleUIManager.InitUIData();
            _turnManager.DecideTheTurn();
            _turnManager.DecideTheTurnOnEnemySp(_roundManager.MaxRoundCount);
        }

        //1ラウンド目以降に行う処理
        ResetGameState(_turnManager, _roundManager);
        _battleUIManager.HideUIAtStart();
        _cardManager.ResetFieldCard(_fieldTransformManager.BattleFieldTransforms, _fieldTransformManager.HandTransforms);
        await _battleUIManager.ShowRoundCountText(_roundManager.RoundCount, _roundManager.MaxRoundCount);
        _cardManager.DistributeCards(_fieldTransformManager.MyHandTransform, _fieldTransformManager.EnemyHandTransform);
        _turnManager.ChangeTurn().Forget();
    }

    /// <summary>
    /// プレイヤーデータの初期化
    /// </summary>
    public void InitPlayerData()
    {
        _player = new PlayerData(INITIAL_POINT);
        _enemy = new PlayerData(INITIAL_POINT);
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
                _battleUIManager.ShowCountDownText(_countDownTime);
            }

            yield return null;
        }

        //確認画面を全て閉じる
        _battleUIManager.CloseAllConfirmationPanels();

        //0になったらカードをランダムにフィールドへ移動しターンエンドする
        Transform handTransform = _fieldTransformManager.GetHandTransformByTurn(_turnManager.IsMyTurn);
        CardController targetCard = _cardManager.GetRandomCardFrom(handTransform);

        yield return targetCard.CardEvent.MoveToBattleField(_fieldTransformManager.MyBattleFieldTransform).ToCoroutine();
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
    public void ResetGameState(params IGameDataResetable[] targetManagerList)
    {
        foreach (IGameDataResetable targetManager in targetManagerList)
        {
            targetManager.ResetData();
        }
    }

    /// <summary>
    /// ゲームを再開する
    /// </summary>
    public void RetryGame()
    {
        StartGame(true).Forget();
    }

    /// <summary>
    /// タイトルへ移動する
    /// </summary>
    public void OnClickToTitle()
    {
        SceneManager.LoadScene(CommonAttribute.GetStringValue(SceneType.GameTitle));
    }

    /// <summary>
    /// ゲームを終了
    /// </summary>
    public void EndGame()
    {
        //ゲーム結果を判定
        _gameResult = JudgeGameResult();
        //勝敗の表示
        _battleUIManager.ToggleGameResultUI(true);
        _battleUIManager.SetGameResultText(CommonAttribute.GetStringValue(_gameResult));
    }

    /// <summary>
    /// ゲーム結果を取得する
    /// </summary>
    public GameResult JudgeGameResult()
    {
        if (_player.Point > _enemy.Point) return GAME_WIN;
        if (_player.Point == _enemy.Point) return GAME_DRAW;
        return GAME_LOSE;
    }

    /// <summary>
    /// 結果を反映します
    /// </summary>
    /// <param name="result"></param>
    public async UniTask ReflectTheResult(CardJudgement result)
    {
        ChangeBattlePhase(RESULT);

        if (result == WIN)
        {
            _pointManager.AddPointTo(_player, _roundManager.IsUsingPlayerSkillInRound);
        }
        else if (result == LOSE)
        {
            _pointManager.AddPointTo(_enemy, _roundManager.IsUsingEnemySkillInRound);
        }

        //UIへの反映
        await _battleUIManager.ShowJudgementResultText(result.ToString());
        _battleUIManager.ShowPoint(_player.Point, _enemy.Point);
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