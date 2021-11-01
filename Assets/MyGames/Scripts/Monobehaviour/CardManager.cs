using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static WaitTimes;
using static CardType;
using static CardJudgement;
using static BattlePhase;
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
    public CardController GetRandomCardFrom(Transform targetHandTransform)
    {
        CardController[] handCards = GetAllHandCardsFor(targetHandTransform);
        int randomCardIndex = UnityEngine.Random.Range(0, handCards.Length);
        return handCards[randomCardIndex];
    }

    /// <summary>
    /// カードを判定する
    /// </summary>
    public async UniTask JudgeTheCard(Transform myBattleFieldTransform, Transform enemyBattleFieldTransform)
    {
        GM._instance.ChangeBattlePhase(JUDGEMENT);
        //バトル場のカードを取得
        CardController myCard = GetBattleFieldCardBy(myBattleFieldTransform);
        CardController enemyCard = GetBattleFieldCardBy(enemyBattleFieldTransform);
        //じゃんけんする
        CardJudgement result = JudgeCardResult(myCard, enemyCard);

        //OPENのメッセージを出す
        await GM._instance.UIManager.DirectionUIManager.AnnounceToOpenTheCard();
        //カードを裏から表にする
        await OpenTheBattleFieldCards(myCard, enemyCard);

        //結果を反映する
        GM._instance.ReflectTheResult(result);

        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_ROUND));

        await GM._instance.RoundManager.NextRound();
    }

    /// <summary>
    /// 盤面をリセットします
    /// </summary>
    public void ResetFieldCard(
        Transform[] battleFieldTransforms,
        Transform[] handTrandforms
    )
    {
        //プレイヤーごとにバトル場のカードを削除します
        foreach (Transform fieldTransform in battleFieldTransforms)
        {
            Destroy(GetBattleFieldCardBy(fieldTransform)?.gameObject);
        }

        //プレイヤーごとに手札のカードを削除します
        foreach (Transform handTransform in handTrandforms)
        {
            DestroyHandCard(handTransform);
        }

        //await UniTask.Yield();
    }

    /// <summary>
    /// バトル場のカードを取得する
    /// </summary>
    /// <param name="targetBattleFieldTransform"></param>
    /// <returns></returns>
    CardController GetBattleFieldCardBy(Transform targetBattleFieldTransform)
    {
        return targetBattleFieldTransform.GetComponentInChildren<CardController>();
    }

    /// <summary>
    /// 手札のカードを破壊します
    /// </summary>
    void DestroyHandCard(Transform targetHandTransform)
    {
        foreach (CardController target in GetAllHandCardsFor(targetHandTransform))
        {
            Destroy(target.gameObject);
        }
    }

    /// <summary>
    /// 手札のカードを全て取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public CardController[] GetAllHandCardsFor(Transform targetHandTransform)
    {
        return targetHandTransform.GetComponentsInChildren<CardController>();
    }

    /// <summary>
    /// カードを配ります
    /// </summary>
    public void DistributeCards(Transform myHandTransform, Transform enemyHandTransform)
    {
        //プレイヤーとエネミーにそれぞれ三種類のカードを作成する
        for (int i = 0; i < _cardEntityList.GetCardEntityList.Count; i++)
        {
            AddingCardToHand(myHandTransform, i, true);
            AddingCardToHand(enemyHandTransform, i, false);
        }
        //お互いのカードをシャッフルする
        ShuffleHandCard(myHandTransform);
        ShuffleHandCard(enemyHandTransform);
    }

    /// <summary>
    /// 手札のカードをシャッフルする
    /// </summary>
    void ShuffleHandCard(Transform targetTransform)
    {
        CardController[] handCards = GetAllHandCardsFor(targetTransform);

        for (int i = 0; i < handCards.Length; i++)
        {
            int tempIndex = handCards[i].transform.GetSiblingIndex();
            int randomIndex = UnityEngine.Random.Range(0, handCards.Length);
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
    async UniTask OpenTheBattleFieldCards(CardController myCard, CardController enemyCard)
    {
        UniTask opened1 = myCard.TurnTheCardFaceUp();
        UniTask opened2 = enemyCard.TurnTheCardFaceUp();
        await UniTask.WhenAll(opened1, opened2);
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
