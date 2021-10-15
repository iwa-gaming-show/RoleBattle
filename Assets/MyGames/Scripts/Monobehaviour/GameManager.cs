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
    #endregion

    bool _isBattleFieldPlaced;//フィールドにカードが配置されたか
    bool _isMyTurn;//自身のターンか
    bool _isEnemyTurn;//相手のターンか
    bool _isMyTurnEnd;
    bool _isEnemyTurnEnd;
    int _myPoint;
    int _enemyPoint;
    GameResult _gameResult;

    #region プロパティ
    public Transform MyBattleFieldTransform => _myBattleFieldTransform;
    public bool IsBattleFieldPlaced => _isBattleFieldPlaced;
    public bool IsMyTurn => _isMyTurn;
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
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    void StartGame()
    {
        _uiManager.HideUIAtStart();
        _uiManager.ShowPoint(_myPoint, _enemyPoint);
        StartCoroutine(_uiManager.ShowRoundCountText(_roundCount, _maxRoundCount));
        ResetFieldCard();
        DistributeCards();
        MyTurn();
    }

    /// <summary>
    /// ゲームを再開する
    /// </summary>
    public void RetryGame()
    {
        InitializeGameData();
        StartGame();
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
        DestroyHandCard(_myHandTransform);
        DestroyHandCard(_enemyHandTransform);
    }

    /// <summary>
    /// 手札のカードを破壊します
    /// </summary>
    void DestroyHandCard(Transform handTransform)
    {
        foreach (CardController target in handTransform.GetComponentsInChildren<CardController>())
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
        ShuffleHandCard(_myHandTransform);
        ShuffleHandCard(_enemyHandTransform);
    }

    /// <summary>
    /// 手札のカードをシャッフルする
    /// </summary>
    void ShuffleHandCard(Transform targetTransform)
    {
        CardController[] handCards = targetTransform.GetComponentsInChildren<CardController>();

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
    /// ターンを切り替える
    /// </summary>
    public void ChangeTurn()
    {
        SetBattleFieldPlaced(false);

        if (_isMyTurn)
        {
            _isMyTurn = false;
            _isMyTurnEnd = true;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            _isEnemyTurn = false;
            _isEnemyTurnEnd = true;
        }

        if (_isMyTurnEnd && _isEnemyTurnEnd)
        {
            //自身と相手のターンが終了した時、判定処理が走る
            StartCoroutine(JudgeTheCard());
        }
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

        _isMyTurnEnd = false;
        _isEnemyTurnEnd = false;

        //todo
        //OPENのメッセージを出す
        //カードを裏から表にする

        ReflectTheResult(result);

        yield return new WaitForSeconds(TIME_BEFORE_CHANGING_ROUND);
        NextRound();
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
    /// 次のラウンドへ
    /// </summary>
    void NextRound()
    {
        if (_roundCount != _maxRoundCount)
        {
            _roundCount++;
            StartGame();
        }
        else
        {
            //最終ラウンドならゲーム終了
            EndGame();
        }
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
            _myPoint += _earnedPoint;
        }
        else
        {
            _enemyPoint += _earnedPoint;
        }
    }

    /// <summary>
    /// 自分のターン
    /// </summary>
    public void MyTurn()
    {
        Debug.Log("自分のターンです");
        _isMyTurn = true;
    }

    /// <summary>
    /// 相手のターン
    /// </summary>
    public IEnumerator EnemyTurn()
    {
        _isEnemyTurn = true;
        Debug.Log("相手のターンです");
        //エネミーの手札を取得
        CardController[] cardControllers = _enemyHandTransform.GetComponentsInChildren<CardController>();
        //カードをランダムに選択
        CardController card = cardControllers[Random.Range(0, cardControllers.Length)];
        //カードをフィールドに移動
        yield return StartCoroutine(card.CardEvent.MoveToBattleField(_enemyBattleFieldTransform));
        ChangeTurn();
    }
}
