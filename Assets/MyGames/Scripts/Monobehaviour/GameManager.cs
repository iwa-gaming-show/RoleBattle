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
    [Header("プレイヤーの手札")]
    Transform _playerHandTransform;

    [SerializeField]
    [Header("エネミーの手札")]
    Transform _enemyHandTransform;

    [SerializeField]
    [Header("プレイヤーのバトルフィールド")]
    Transform _p1BattleFieldTransform;

    [SerializeField]
    [Header("エネミーのバトルフィールド")]
    Transform _p2BattleFieldTransform;

    bool _isBattleFieldPlaced;//フィールドにカードが配置されたか


    #region プロパティ
    public Transform P1BattleFieldTransform => _p1BattleFieldTransform;
    public bool IsBattleFieldPlaced => _isBattleFieldPlaced;
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
            AddingCardToHand(_playerHandTransform, i);
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
    /// 相手のターン
    /// </summary>
    public void EnemyTurn()
    {
        //カードをランダムに選択
        //カードをフィールドに移動
        //ターンの終了
        //判定へ
    }
}
