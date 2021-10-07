using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager _instance;

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

    bool _isBattleFieldPlaced;//フィールドにカードが配置されたか
    bool _isMyTurn;//自身のターンか
    bool _isEnemyTurn;//相手のターンか

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
        MyTurn();
        DistributeCards();
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
        _isBattleFieldPlaced = false;

        if (_isMyTurn)
        {
            _isMyTurn = false;
            EnemyTurn();
        }
        else
        {
            _isEnemyTurn = false;
            MyTurn();
        }

        //自身と相手のターンが終了した時、判定処理が走る
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
    public void EnemyTurn()
    {
        _isEnemyTurn = true;
        Debug.Log("相手のターンです");
        //エネミーの手札を取得
        CardController[] cardControllers = _enemyHandTransform.GetComponentsInChildren<CardController>();
        //カードをランダムに選択
        CardController card = cardControllers[Random.Range(0, cardControllers.Length)];
        //カードをフィールドに移動
        card.CardEvent.MoveToBattleField(_enemyBattleFieldTransform);
        //ターンの終了
        ChangeTurn();
        // お互いにターンが終わったら判定が入る
        //判定後自身のターンへ
    }
}
