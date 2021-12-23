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
using static SEType;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour,
    IBattleAdvanceable,
    ITurnAdvanceable,
    IJudgableTheCard,
    ICountDowner
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
    bool _isFirstClick;
    IBattleDataManager _battleDataManager;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        ChangeTurn();
        CheckActivatingSpSkill();
    }

    void Init()
    {
        _battleDataManager = ServiceLocator.Resolve<IBattleDataManager>();
        _battleDataManager.CreatePlayerData();
        _battleDataManager.InitPlayerData();
        _battleUIManager.InitPlayerCharacter();
        PrepareBattle().Forget();
    }

    /// <summary>
    /// バトルの準備をします
    /// </summary>
    async UniTask PrepareBattle()
    {
        await Fade._instance.StartFadeIn();
        GameManager._instance.PlayBgm(BgmType.BATTLE);
        await StartBattle(true);
    }

    /// <summary>
    /// バトルを再開する
    /// </summary>
    public void OnClickToRetryBattle()
    {
        //二重送信防止
        if (_isFirstClick) return;
        _isFirstClick = true;

        GameManager._instance.PlaySE(BATTLE);
        RetryBattle();
    }

    /// <summary>
    /// タイトルへ移動する
    /// </summary>
    public void OnClickToTitle()
    {
        //二重送信防止
        if (_isFirstClick) return;
        _isFirstClick = true;

        GameManager._instance.PlaySE(TO_TITLE);
        GameManager._instance.ClickToLoadScene(SceneType.GameTitle);
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
            DecideSpSkillRoundForEnemy();
        }
        _battleDataManager.ResetPlayerState();
        _battleUIManager.HideUIAtStart();
        _battleUIManager.ResetFieldCards();
        await _battleUIManager.ShowRoundCountText(_battleDataManager.RoundCount, _maxRoundCount);
        _battleUIManager.DistributeCards();
        StartTurn();
    }

    /// <summary>
    /// バトルを終了する
    /// </summary>
    public void EndBattle()
    {
        //勝敗を表示
        _battleUIManager.ShowBattleResultUI(true, CommonAttribute.GetStringValue(_battleDataManager.JudgeBattleResult()));
    }

    /// <summary>
    /// バトルを再開する
    /// </summary>
    public void RetryBattle()
    {
        _battleDataManager.InitPlayerData();
        StartBattle(true).Forget();
        _isFirstClick = false;
    }

    /// <summary>
    /// ターンを開始します
    /// </summary>
    public void StartTurn()
    {
        PlayerTurn().Forget();
    }

    /// <summary>
    /// プレイヤーのターンを開始します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public async UniTask PlayerTurn()
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

    /// <summary>
    /// ターンを切り替えます
    /// </summary>
    public void ChangeTurn()
    {
        IBattleDataManager dataM = _battleDataManager;

        if (dataM.CanChangeTurn == false) return;

        SwitchPlayerTurnFlg(dataM);

        if (IsEachPlayerFieldCardPlaced())
        {
            JudgeTheCard().Forget();
        }
        else
        {
            StartTurn();
        }

        dataM.SetCanChangeTurn(false);
    }

    /// <summary>
    /// プレイヤーのターンのフラグを切り替えます
    /// </summary>
    void SwitchPlayerTurnFlg(IBattleDataManager dataM)
    {
        //プレイヤーとエネミーのフラグをそれぞれ逆にします
        dataM.SetIsPlayerTurnBy(true, !dataM.GetPlayerTurnBy(true));
        dataM.SetIsPlayerTurnBy(false, !dataM.GetPlayerTurnBy(false));
    }

    /// <summary>
    /// カードを判定する
    /// </summary>
    public async UniTask JudgeTheCard()
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
    /// カードの勝敗結果を取得する
    /// </summary>
    /// <param name="myCard"></param>
    /// <param name="enemyCard"></param>
    /// <returns></returns>
    public CardJudgement JudgeCardResult(CardType playerCardType, CardType enemyCardType)
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
    /// お互いのプレイヤーがフィールドにカードを出しているか
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

        DoIfCountDownTimeOut();
    }

    /// <summary>
    /// 必殺技が発動していることを確認します
    /// </summary>
    void CheckActivatingSpSkill()
    {
        if (_battleDataManager.IsDuringDirectingSpSkill == false) return;
        _battleDataManager.SetIsDuringDirectingSpSkill(false);

        //発動後カウントダウンをリセットします
        ResetCountDown();
    }

    /// <summary>
    /// カウントダウンをリセットします
    /// </summary>
    void ResetCountDown()
    {
        StopAllCoroutines();//前のカウントダウンが走っている可能性があるため一度止めます
        StartCoroutine(CountDown());
    }

    /// <summary>
    /// カウントダウン終了時の処理
    /// </summary>
    public void DoIfCountDownTimeOut()
    {
        //自分のターンではない場合
        if (_battleDataManager.GetPlayerTurnBy(true) == false) return;

        //確認画面を全て閉じ、ランダムにカードを移動
        _battleUIManager.InactiveUIIfCountDownTimeOut();
        _battleUIManager.MoveRandomCardToField(true).Forget();
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
    /// エネミーが必殺技を使用するラウンドを決めます
    /// </summary>
    public void DecideSpSkillRoundForEnemy()
    {
        int spSkillRound = UnityEngine.Random.Range(INITIAL_ROUND_COUNT, _maxRoundCount + INITIAL_ROUND_COUNT);
        _battleDataManager.SetEnemySpSkillRound(spSkillRound);
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