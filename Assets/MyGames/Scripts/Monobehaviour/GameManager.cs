using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WaitTimes;
using static InitializationData;
using static CardType;
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
    [Header("カードリストを設定する(ScriptableObjectを参照)")]
    CardEntityList _cardEntityList;

    [SerializeField]
    [Header("カードプレハブ")]
    CardController _cardPrefab;

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
    int _maxRoundCount = 5;

    [SerializeField]
    [Header("ラウンド数")]
    int _roundCount = INITIAL_ROUND_COUNT;

    [SerializeField]
    [Header("カウントダウンの秒数を設定")]
    int _defaultCountDownTime = DEFAULT_COUNT_DOWN_TIME;
    #endregion

    bool _isBattleFieldPlaced;//フィールドにカードが配置されたか
    bool _isMyTurn;//自身のターンか
    bool _isMyTurnEnd;
    bool _isEnemyTurnEnd;
    bool _canUsePlayerSpecialSkill;//必殺技が使用できるか
    bool _isUsingPlayerSkillInRound;//必殺技を使用したラウンドか
    bool _canUseEnemySpecialSkill;
    bool _isUsingEnemySkillInRound;
    bool _isDuringProductionOfSpecialSkill;//必殺技の演出中か
    int _myPoint;
    int _enemyPoint;
    int _enemySpecialSkillTurn;//敵が必殺技を使用するターン
    int _countDownTime;
    GameResult _gameResult;

    #region プロパティ
    public Transform MyBattleFieldTransform => _myBattleFieldTransform;
    public bool IsBattleFieldPlaced => _isBattleFieldPlaced;
    public bool IsMyTurn => _isMyTurn;
    public bool CanUsePlayerSpecialSkill => _canUsePlayerSpecialSkill;
    public UIManager UIManager => _uiManager;
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
            DecideTheTurn();
            DecideTheTurnOnEnemySp();
        }
        //1ラウンド目以降に行う処理
        else
        {
            _roundCount++;
        }

        _uiManager.HideUIAtStart();
        ResetFieldCard();
        yield return _uiManager.ShowRoundCountText(_roundCount, _maxRoundCount);
        DistributeCards();
        ChangeTurn();
    }

    /// <summary>
    /// 先攻、後攻のターンを決めます
    /// </summary>
    void DecideTheTurn()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            _isMyTurn = true;
        }
    }

    /// <summary>
    /// エネミーが必殺技を使用するターンを決めます
    /// </summary>
    void DecideTheTurnOnEnemySp()
    {
        _enemySpecialSkillTurn = Random.Range(INITIAL_ROUND_COUNT, _maxRoundCount + INITIAL_ROUND_COUNT);
    }

    /// <summary>
    /// ターンの終了
    /// </summary>
    public void EndTurn()
    {
        if (_isMyTurn)
        {
            _isMyTurn = false;
            _isMyTurnEnd = true;
        }
        else
        {
            _isMyTurn = true;
            _isEnemyTurnEnd = true;
        }
        
        ChangeTurn();
    }

    /// <summary>
    /// ターンを切り替える
    /// </summary>
    public void ChangeTurn()
    {
        SetBattleFieldPlaced(false);
        StopAllCoroutines();//意図しない非同期処理が走っている可能性を排除する

        if (_isMyTurn && _isMyTurnEnd == false)
        {
            StartCoroutine(CountDown());
            MyTurn();
        }
        else if (_isEnemyTurnEnd == false)
        {
            StartCoroutine(CountDown());
            StartCoroutine(EnemyTurn());
        }

        if (_isMyTurnEnd && _isEnemyTurnEnd)
        {
            //自身と相手のターンが終了した時、判定処理が走る
            StartCoroutine(JudgeTheCard());
        }
    }

    /// <summary>
    /// カウントダウン
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDown()
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
        CardController targetCard = GetRandomCardFrom(_isMyTurn);
        Transform targetTransform = GetTargetBattleFieldTransform(_isMyTurn);

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
    /// 手札からランダムなカードを取得します
    /// </summary>
    /// <returns></returns>
    CardController GetRandomCardFrom(bool isMyHand)
    {
        CardController[] handCards = GetAllHandCardsFor(isMyHand);
        int randomCardIndex = Random.Range(0, handCards.Length);
        return handCards[randomCardIndex];
    }

    /// <summary>
    /// 手札のカードを全て取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public CardController[] GetAllHandCardsFor(bool isPlayer)
    {
        if (isPlayer) return _myHandTransform.GetComponentsInChildren<CardController>();
        return _enemyHandTransform.GetComponentsInChildren<CardController>();
    }

    /// <summary>
    /// カードを判定する
    /// </summary>
    IEnumerator JudgeTheCard()
    {
        //バトル場のカードを取得
        CardController myCard = GetBattleFieldCardBy(true);
        CardController enemyCard = GetBattleFieldCardBy(false);
        //じゃんけんする
        CardJudgement result = JudgeCardResult(myCard, enemyCard);

        //OPENのメッセージを出す
        yield return _uiManager.AnnounceToOpenTheCard();
        //カードを裏から表にする
        yield return OpenTheBattleFieldCards(myCard, enemyCard);
        //結果を反映する
        ReflectTheResult(result);
        ResetRoundState();

        yield return new WaitForSeconds(TIME_BEFORE_CHANGING_ROUND);
        NextRound();
    }

    /// <summary>
    /// ラウンドの状態をリセットする
    /// </summary>
    void ResetRoundState()
    {
        //スキルの発動状態をリセット
        _isUsingPlayerSkillInRound = false;
        _isUsingEnemySkillInRound = false;
        _isMyTurnEnd = false;
        _isEnemyTurnEnd = false;
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
    /// 盤面をリセットします
    /// </summary>
    void ResetFieldCard()
    {
        //バトル場のカードを削除します
        Destroy(GetBattleFieldCardBy(true)?.gameObject);
        Destroy(GetBattleFieldCardBy(false)?.gameObject);

        //手札のカードを削除します
        DestroyHandCard(true);
        DestroyHandCard(false);
    }

    /// <summary>
    /// 手札のカードを破壊します
    /// </summary>
    void DestroyHandCard(bool isPlayer)
    {
        foreach (CardController target in GetAllHandCardsFor(isPlayer))
        {
            Destroy(target.gameObject);
        }
    }

    /// <summary>
    /// カードを配ります
    /// </summary>
    void DistributeCards()
    {
        //プレイヤーとエネミーにそれぞれ三種類のカードを作成する
        for (int i = 0; i < _cardEntityList.GetCardEntityList.Count; i++)
        {
            AddingCardToHand(_myHandTransform, i, true);
            AddingCardToHand(_enemyHandTransform, i, false);
        }
        //お互いのカードをシャッフルする
        ShuffleHandCard(true);
        ShuffleHandCard(false);
    }

    /// <summary>
    /// 手札のカードをシャッフルする
    /// </summary>
    void ShuffleHandCard(bool isPlayer)
    {
        CardController[] handCards = GetAllHandCardsFor(isPlayer);

        for (int i = 0; i < handCards.Length; i++)
        {
            int tempIndex = handCards[i].transform.GetSiblingIndex();
            int randomIndex = Random.Range(0, handCards.Length);
            handCards[i].transform.SetSiblingIndex(randomIndex);
            handCards[randomIndex].transform.SetSiblingIndex(tempIndex);
        }
    }

    /// <summary>
    /// カードを手札に加えます
    /// </summary>
    /// <param name="cardIndex"></param>
    void AddingCardToHand(Transform hand, int cardIndex, bool isPlayer)
    {
        CreateCard(hand, cardIndex, isPlayer);
    }

    /// <summary>
    /// カードを生成する
    /// </summary>
    void CreateCard(Transform hand, int cardIndex, bool isPlayer)
    {
        CardController cardController = Instantiate(_cardPrefab, hand, false);
        cardController.Init(cardIndex, isPlayer);
    }

    /// <summary>
    /// カードのフィールド配置フラグの設定
    /// </summary>
    public void SetBattleFieldPlaced(bool isBattleFieldPlaced)
    {
        _isBattleFieldPlaced = isBattleFieldPlaced;
    }

    /// <summary>
    /// 自分のターン
    /// </summary>
    public void MyTurn()
    {
        StartCoroutine(_uiManager.ShowThePlayerTurnText(true));
    }

    /// <summary>
    /// 相手のターン
    /// </summary>
    public IEnumerator EnemyTurn()
    {
        yield return _uiManager.ShowThePlayerTurnText(false);
        //エネミーの手札を取得
        CardController[] cardControllers = GetAllHandCardsFor(false);
        //カードをランダムに選択
        CardController card = cardControllers[Random.Range(0, cardControllers.Length)];

        bool useSpecialSkill = (_roundCount == _enemySpecialSkillTurn);

        if (_canUseEnemySpecialSkill && useSpecialSkill)
        {
            yield return _uiManager.ActivateSpecialSkill(false);
        }

        //カードをフィールドに移動
        yield return card.CardEvent.MoveToBattleField(_enemyBattleFieldTransform);
    }

    /// <summary>
    /// 次のラウンドへ
    /// </summary>
    void NextRound()
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
    /// バトル場のカードを表にします
    /// </summary>
    IEnumerator OpenTheBattleFieldCards(CardController myCard, CardController enemyCard)
    {
        myCard.TurnTheCardFaceUp();
        enemyCard.TurnTheCardFaceUp();
        yield return null;
    }
    /// <summary>
    /// バトル場のカードを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    CardController GetBattleFieldCardBy(bool isPlayer)
    {
        return GetTargetBattleFieldTransform(isPlayer).GetComponentInChildren<CardController>();
    }

    /// <summary>
    /// 対象のバトル場のカードのTransformを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    Transform GetTargetBattleFieldTransform(bool isPlayer)
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
    /// カードの勝敗結果を取得する
    /// </summary>
    /// <param name="myCard"></param>
    /// <param name="enemyCard"></param>
    /// <returns></returns>
    CardJudgement JudgeCardResult(CardController myCard, CardController enemyCard)
    {
        CardType myCardType = myCard.CardModel.CardType;
        CardType enemyCardType = enemyCard.CardModel.CardType;

        //じゃんけんによる勝敗の判定
        if (myCardType == enemyCardType) return DRAW;
        if (myCardType == PRINCESS && enemyCardType == BRAVE) return WIN;
        if (myCardType == BRAVE && enemyCardType == DEVIL) return WIN;
        if (myCardType == DEVIL && enemyCardType == PRINCESS) return WIN;
        return LOSE;
    }

    /// <summary>
    /// 結果を反映します
    /// </summary>
    /// <param name="result"></param>
    void ReflectTheResult(CardJudgement result)
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
