using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public GameManager instance;

    [SerializeField]
    [Header("カードリストを設定する(ScriptableObjectを参照)")]
    CardEntityList cardEntityList;

    [SerializeField]
    [Header("カードプレハブ")]
    CardController cardPrefab;

    [SerializeField]
    [Header("プレイヤーの手札")]
    Transform playerHandTransform;

    [SerializeField]
    [Header("エネミーの手札")]
    Transform enemyHandTransform;

    private void Awake()
    {
        //シングルトン化する
        if (instance == null)
        {
            instance = this;
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
        for (int i = 0; i < cardEntityList.GetCardEntityList.Count; i++)
        {
            AddingCardToHand(playerHandTransform, i);
            AddingCardToHand(enemyHandTransform, i);
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
        CardController cardController = Instantiate(cardPrefab, hand, false);
        cardController.Init(cardIndex);
    }
}
