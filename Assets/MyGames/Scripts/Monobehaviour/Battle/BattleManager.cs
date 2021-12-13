using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static InitializationData;
using static BattlePhase;
using static CardJudgement;
using static CardType;
using static WaitTimes;

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

    void Update()
    {
        SwitchPlayerTurnFlg(_battleDataManager);
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
            //プレイヤーごとにpointをUIに反映
            _battleUIManager.ShowPoint(
                _battleDataManager.GetPlayerPointBy(true),
                _battleDataManager.GetPlayerPointBy(false)
            );
            //必殺技が発動可能であることをUIに反映
            _battleUIManager.SetSpButtonImage(
                _battleDataManager.GetCanUseSpSkillBy(true),
                _battleDataManager.GetCanUseSpSkillBy(false)
            );
            DecideTheTurn();
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
        await _battleUIManager.ShowThePlayerTurnText(_battleDataManager.GetPlayerTurnBy(true));
        StopAllCoroutines();//前のカウントダウンが走っている可能性があるため一度止めます
        StartCoroutine(CountDown());

        //相手のターンならアクションをする
        if (_battleDataManager.GetPlayerTurnBy(false))
        {
            await _battleUIManager.NpcEnemyAction();
        }
    }

    // <summary>
    /// プレイヤーのターンのフラグを切り替えます
    /// </summary>
    void SwitchPlayerTurnFlg(IBattleDataManager dataM)
    {
        if (dataM.CanChangeTurn == false) return;

        if (IsEachPlayerFieldCardPlaced())
        {
            JudgeTheCard().Forget();
        }
        else
        {
            ChangeTurn(dataM);
        }

        dataM.SetCanChangeTurn(false);
    }

    /// <summary>
    /// カードを判定する
    /// </summary>
    async UniTask JudgeTheCard()
    {
        _battleDataManager.SetBattlePhase(JUDGEMENT);

        CardType playerCardType = _battleDataManager.GetCardTypeBy(true);
        CardType enemyCardType = _battleDataManager.GetCardTypeBy(false);

        //じゃんけんする
        CardJudgement result = JudgeCardResult(playerCardType, enemyCardType);

        //OPENのメッセージを出す
        await _battleUIManager.AnnounceToOpenTheCard();
        //カードを表にする
        await _battleUIManager.OpenTheBattleFieldCards();
        //結果の反映
        await ReflectTheResult(result);
        //ポイントの追加
        AddPointBy(result);
        //ポイントの反映
        _battleUIManager.ShowPoint(
            _battleDataManager.GetPlayerPointBy(true),
            _battleDataManager.GetPlayerPointBy(false)
        );
        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_ROUND));
        //次のラウンドへ
        NextRound();
    }

    /// <summary>
    /// 次のラウンドへ進みます
    /// </summary>
    void NextRound()
    {
        if (_battleDataManager.RoundCount != _maxRoundCount)
        {
            _battleDataManager.AddRoundCount();
            StartBattle(false).Forget();
        }
        else
        {
            EndBattle();
        }
    }

    /// <summary>
    /// バトルを終了する
    /// </summary>
    void EndBattle()
    {
        //勝敗を表示
        _battleUIManager.ToggleBattleResultUI(true);  
        _battleUIManager.SetBattleResultText(
            CommonAttribute.GetStringValue(_battleDataManager.JudgeBattleResult())
        );
    }

    /// <summary>
    /// カードの勝敗結果を取得する
    /// </summary>
    /// <param name="myCard"></param>
    /// <param name="enemyCard"></param>
    /// <returns></returns>
    CardJudgement JudgeCardResult(CardType playerCardType, CardType enemyCardType)
    {
        //じゃんけんによる勝敗の判定
        if (playerCardType == enemyCardType) return DRAW;
        if (playerCardType == PRINCESS && enemyCardType == BRAVE) return WIN;
        if (playerCardType == BRAVE && enemyCardType == DEVIL) return WIN;
        if (playerCardType == DEVIL && enemyCardType == PRINCESS) return WIN;
        return LOSE;
    }

    /// <summary>
    /// 結果を反映します
    /// </summary>
    /// <param name="result"></param>
    public async UniTask ReflectTheResult(CardJudgement result)
    {
        _battleDataManager.SetBattlePhase(RESULT);
        await _battleUIManager.ShowJudgementResultText(result.ToString());
    }

    /// <summary>
    /// 結果によるポイントを加算する
    /// </summary>
    void AddPointBy(CardJudgement result)
    {
        if (result == WIN)
        {
            _battleDataManager.AddPointTo(true);
        }
        else if (result == LOSE)
        {
            _battleDataManager.AddPointTo(false);
        }
    }

    /// <summary>
    /// ターンを切り替えます
    /// </summary>
    void ChangeTurn(IBattleDataManager dataM)
    {
        //プレイヤーとエネミーのフラグをそれぞれ逆にします
        dataM.SetIsPlayerTurnBy(true, !dataM.GetPlayerTurnBy(true));
        dataM.SetIsPlayerTurnBy(false, !dataM.GetPlayerTurnBy(false));
        StartTurn();
    }

    /// <summary>
    /// お互いのプレイヤーがバトル場にカードを出しているか
    /// </summary>
    /// <returns></returns>
    bool IsEachPlayerFieldCardPlaced()
    {
        return _battleDataManager.GetIsFieldCardPlacedBy(true)
            && _battleDataManager.GetIsFieldCardPlacedBy(false);
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
        if (RandomBool()) _battleDataManager.SetIsPlayerTurnBy(true, true);
        else _battleDataManager.SetIsPlayerTurnBy(false, true);
    }

    /// <summary>
    /// bool型をランダムに取得する
    /// </summary>
    /// <returns></returns>
    bool RandomBool()
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }
}