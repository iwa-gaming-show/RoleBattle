using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WaitTimes;
using static CardType;
using static CardJudgement;
using GM = GameManager;

public class CardManager : MonoBehaviour
{
    #region インスペクターから設定
    [SerializeField]
    [Header("カードリストを設定する(ScriptableObjectを参照)")]
    CardEntityList _cardEntityList;

    [SerializeField]
    [Header("カードプレハブ")]
    CardController _cardPrefab;
    #endregion

    bool _isBattleFieldPlaced;//フィールドにカードが配置されたか

    #region プロパティ
    public bool IsBattleFieldPlaced => _isBattleFieldPlaced;
    #endregion

    /// <summary>
    /// 手札からランダムなカードを取得します
    /// </summary>
    /// <returns></returns>
    public CardController GetRandomCardFrom(bool isMyHand)
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
        if (isPlayer) return GM._instance.MyHandTransform.GetComponentsInChildren<CardController>();
        return GM._instance.EnemyHandTransform.GetComponentsInChildren<CardController>();
    }

    /// <summary>
    /// カードを判定する
    /// </summary>
    public IEnumerator JudgeTheCard()
    {
        //バトル場のカードを取得
        CardController myCard = GetBattleFieldCardBy(true);
        CardController enemyCard = GetBattleFieldCardBy(false);
        //じゃんけんする
        CardJudgement result = JudgeCardResult(myCard, enemyCard);

        //OPENのメッセージを出す
        yield return GM._instance.UIManager.AnnounceToOpenTheCard();
        //カードを裏から表にする
        yield return OpenTheBattleFieldCards(myCard, enemyCard);
        //結果を反映する
        GM._instance.ReflectTheResult(result);
        GM._instance.ResetGameState();

        yield return new WaitForSeconds(TIME_BEFORE_CHANGING_ROUND);
        GM._instance.RoundManager.NextRound();
    }

    /// <summary>
    /// 盤面をリセットします
    /// </summary>
    public void ResetFieldCard()
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
    public void DistributeCards()
    {
        //プレイヤーとエネミーにそれぞれ三種類のカードを作成する
        for (int i = 0; i < _cardEntityList.GetCardEntityList.Count; i++)
        {
            AddingCardToHand(GM._instance.MyHandTransform, i, true);
            AddingCardToHand(GM._instance.EnemyHandTransform, i, false);
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
        return GM._instance.GetTargetBattleFieldTransform(isPlayer).GetComponentInChildren<CardController>();
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
}
