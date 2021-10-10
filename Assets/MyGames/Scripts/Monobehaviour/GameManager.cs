using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WaitTimes;
using static UIStrings;
using static CardType;
using static CardJudgement;
using static GameResult;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager _instance;

    #region インスペクターから設定
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
    [Header("自身の獲得ポイントUI")]
    Text _myPointText;

    [SerializeField]
    [Header("相手の獲得ポイントUI")]
    Text _enemyPointText;

    [SerializeField]
    [Header("ラウンドの勝敗の結果表示のテキスト")]
    Text _judgementResultText;

    [SerializeField]
    [Header("ラウンド数表示テキスト")]
    Text _roundCountText;

    [SerializeField]
    [Header("ゲーム勝敗の結果表示のテキスト")]
    Text _gameResultText;

    [SerializeField]
    [Header("ラウンド毎の勝者の獲得ポイント")]
    int _earnedPoint = 1;

    [SerializeField]
    [Header("最大ラウンド数")]
    int _maxRoundCount = 5;

    [SerializeField]
    [Header("ラウンド数")]
    int _roundCount = 1;
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
        ShowPoint();
        StartCoroutine(ShowRoundCountText());
        RestFieldCard();
        DistributeCards();
        MyTurn();
    }

    /// <summary>
    /// ポイントの表示
    /// </summary>
    void ShowPoint()
    {
        _myPointText.text = _myPoint.ToString() + POINT_SUFFIX;
        _enemyPointText.text = _enemyPoint.ToString() + POINT_SUFFIX;
    }

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowRoundCountText()
    {
        ToggleRoundCountText(true);

        if (_roundCount == _maxRoundCount)
        {
            //最終ラウンド
            _roundCountText.text = FINAL_ROUND;
        }
        else
        {
            _roundCountText.text = ROUND_PREFIX + _roundCount.ToString();
        }

        yield return new WaitForSeconds(ROUND_COUNT_DISPLAY_TIME);
        ToggleRoundCountText(false);
    }

    /// <summary>
    /// 盤面をリセットします
    /// </summary>
    void RestFieldCard()
    {
        //バトル場のカードを削除します
        Destroy(GetBattleFieldCardBy(true)?.gameObject);
        Destroy(GetBattleFieldCardBy(false)?.gameObject);

        //手札のカードを削除します
        foreach (CardController target in _myHandTransform.GetComponentsInChildren<CardController>())
        {
            Destroy(target.gameObject);
        }

        foreach (CardController target in _enemyHandTransform.GetComponentsInChildren<CardController>())
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
            AddingCardToHand(_myHandTransform, i);
            AddingCardToHand(_enemyHandTransform, i);
        }
    }

    /// <summary>
    /// ラウンド数表示用テキストの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleRoundCountText(bool isActive)
    {
        _roundCountText.gameObject?.SetActive(isActive);
    }

    /// <summary>
    /// カードを手札に加えます
    /// </summary>
    /// <param name="cardIndex"></param>
    void AddingCardToHand(Transform hand, int cardIndex)
    {
        CreateCard(hand, cardIndex);
    }

    /// <summary>
    /// カードを生成する
    /// </summary>
    void CreateCard(Transform hand, int cardIndex)
    {
        CardController cardController = Instantiate(_cardPrefab, hand, false);
        cardController.Init(cardIndex);
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
        CardJudgement result = JudgmentResult(myCard, enemyCard);

        _isMyTurnEnd = false;
        _isEnemyTurnEnd = false;

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
            //盤面のリセット
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
        //ポイントの比較
        if (_myPoint > _enemyPoint)
        {
            _gameResult = GAME_WIN;
        }
        else if (_myPoint == _enemyPoint)
        {
            _gameResult = GAME_DRAW;
        }
        else
        {
            _gameResult = GAME_LOSE;
        }
        //勝ち負けの表示
        ShowGameResultText(CommonAttribute.GetStringValue(_gameResult));
        //リトライUIの表示
    }

    /// <summary>
    /// ゲームの勝敗を表示する
    /// </summary>
    /// <returns></returns>
    void ShowGameResultText(string text)
    {
        ToggleGameResultText(true);
        _gameResultText.text = text;
    }

    /// <summary>
    /// ゲーム結果の表示の切り替え
    /// </summary>
    /// <param name="isAcitve"></param>
    void ToggleGameResultText(bool isAcitve)
    {
        _gameResultText.gameObject.SetActive(isAcitve);
    }

    /// <summary>
    /// 判定結果
    /// </summary>
    /// <param name="myCard"></param>
    /// <param name="enemyCard"></param>
    /// <returns></returns>
    CardJudgement JudgmentResult(CardController myCard, CardController enemyCard)
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
        StartCoroutine(ShowJudgementResultText(result.ToString()));
        ShowPoint();
    }

    /// <summary>
    /// ラウンドの勝敗の結果を表示
    /// </summary>
    IEnumerator ShowJudgementResultText(string result)
    {
        ToggleJudgementResultText(true);
        _judgementResultText.text = result + JUDGEMENT_RESULT_SUFFIX;

        yield return new WaitForSeconds(JUDGMENT_RESULT_DISPLAY_TIME);
        ToggleJudgementResultText(false);
    }

    /// <summary>
    /// ラウンドの勝敗の結果の表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleJudgementResultText(bool isActive)
    {
        _judgementResultText.gameObject?.SetActive(isActive);
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
        StartCoroutine(card.CardEvent.MoveToBattleField(_enemyBattleFieldTransform));
        yield return null;
    }
}
