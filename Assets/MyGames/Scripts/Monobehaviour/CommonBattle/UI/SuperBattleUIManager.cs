using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using static SEType;

public abstract class SuperBattleUIManager : MonoBehaviour
{
    [SerializeField]
    [Header("プレイヤーのUI")]
    protected PlayerUI _playerUI;

    [SerializeField]
    [Header("エネミーのUI")]
    protected PlayerUI _enemyUI;

    [SerializeField]
    [Header("未使用、使用済みの順にアイコン画像を必殺技ボタンに設定する")]
    Sprite[] _spButtonIcons;

    [SerializeField]
    [Header("カードリストを設定する(ScriptableObjectを参照)")]
    protected CardEntityList _cardEntityList;

    [SerializeField]
    [Header("カードプレハブ")]
    CardController _cardPrefab;

    [SerializeField]
    [Header("開始時に非表示にするUIを設定します")]
    GameObject[] _hideUIs;

    [SerializeField]
    [Header("必殺技の詳細を記述するテキストを格納する")]
    Text[] _descriptionsOfSpSkill;

    [SerializeField]
    [Header("必殺技の説明用のテキストを設定")]
    string _spSkillDescription;

    [SerializeField]
    [Header("CanvasForDirectionを設定します")]
    BattleDirectionalityUI _battleDirectionalityUI;

    IConfirmationPanelManager _confirmationPanelManager;
    protected IBattleDirectionalityUI _IdirectionalityUI;

    protected void Awake()
    {
        _IdirectionalityUI = _battleDirectionalityUI;
    }

    protected void Start()
    {
        _confirmationPanelManager = ServiceLocator.Resolve<IConfirmationPanelManager>();
    }

    void Update()
    {
        TryToMoveToField(_confirmationPanelManager.MovingFieldCard);
        TryToActivateSpSkill(_confirmationPanelManager.IsSpSkillActivating);
    }

    #region // abstract methods
    /// <summary>
    /// カードを移動する
    /// </summary>
    public abstract UniTask MoveToBattleField(bool isPlayer, CardController movingCard);

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    /// <returns></returns>
    public abstract UniTask ActivateSpSkill(bool isPlayer);
    #endregion

    /// <summary>
    /// プレイヤーかエネミーのUIを取得する
    /// </summary>
    /// <returns></returns>
    public PlayerUI GetPlayerUI(bool isPlayer)
    {
        if (isPlayer) return _playerUI;
        return _enemyUI;
    }

    /// <summary>
    /// プレイヤーごとのポイントの表示
    /// </summary>
    public void ShowPointBy(bool isPlayer, int point)
    {
        GetPlayerUI(isPlayer).ShowPoint(point);
    }

    ///<summary>
    //プレイヤーの必殺技のImageの状態を設定する
    ///</summary>
    public void SetSpButtonImageBy(bool isPlayer, bool canUseSpSkill)
    {
        Sprite setSprite = null;
        if (canUseSpSkill) setSprite = _spButtonIcons[0];//未使用
        else setSprite = _spButtonIcons[1];//使用済み

        GetPlayerUI(isPlayer).SetSpButtonSprite(setSprite);
    }

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <param name="roundCount"></param>
    /// <returns></returns>
    public async UniTask ShowRoundCountText(int roundCount, int maxRoundCount)
    {
        await _IdirectionalityUI.ShowRoundCountText(roundCount, maxRoundCount);
    }

    /// <summary>
    /// カードを配ります
    /// </summary>
    public void DistributeCards()
    {
        //プレイヤーとエネミーにそれぞれ三種類のカードを作成する
        for (int i = 0; i < _cardEntityList.GetCardEntityList.Count; i++)
        {
            AddingCardToHand(i, true);
            AddingCardToHand(i, false);
        }
        //お互いのカードをシャッフルする
        //※実際には手札は同期されていないので不要な処理だが
        //相手に手の内がバレているのはないかといった不安を与えないよう演出させる
        ShuffleHandCard(true);
        ShuffleHandCard(false);
    }

    /// <summary>
    /// カードを手札に加えます
    /// </summary>
    /// <param name="cardIndex"></param>
    void AddingCardToHand(int cardIndex, bool isPlayer)
    {
        CardController card = CreateCard(cardIndex, isPlayer);
        GetPlayerUI(isPlayer).AddingCardToHand(card);
    }

    /// <summary>
    /// カードを生成する
    /// </summary>
    protected CardController CreateCard(int cardIndex, bool isPlayer)
    {
        CardController card = Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity);
        card.Init(cardIndex, isPlayer);
        return card;
    }

    /// <summary>
    /// 手札のカードをシャッフルする
    /// </summary>
    void ShuffleHandCard(bool isPlayer)
    {
        CardController[] handCards = GetPlayerUI(isPlayer).GetAllHandCards();

        for (int i = 0; i < handCards.Length; i++)
        {
            int tempIndex = handCards[i].transform.GetSiblingIndex();
            int randomIndex = UnityEngine.Random.Range(0, handCards.Length);
            handCards[i].transform.SetSiblingIndex(randomIndex);
            handCards[randomIndex].transform.SetSiblingIndex(tempIndex);
        }
    }

    /// <summary>
    /// 盤面をリセットします
    /// </summary>
    public void ResetFieldCards()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Card"))
        {
            Destroy(go);
        }
    }

    /// <summary>
    /// プレイヤーのターン時にテキストを表示する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public async UniTask ShowThePlayerTurnText(bool isPlayer)
    {
        if (isPlayer) GameManager._instance.PlaySE(MY_TURN);
        await _IdirectionalityUI.ShowThePlayerTurnText(isPlayer);
    }

    /// <summary>
    /// カウントダウンを表示
    /// </summary>
    public void ShowCountDownText(int countDownTime)
    {
        _IdirectionalityUI.ShowCountDownText(countDownTime);
    }

    /// <summary>
    /// フィールドへの移動を試みます
    /// </summary>
    /// <returns></returns>
    void TryToMoveToField(CardController movingCard)
    {
        if (movingCard == null) return;
        _confirmationPanelManager.DestroyMovingBattleCard();

        MoveToBattleField(true, movingCard).Forget();
    }

    /// <summary>
    /// 必殺技発動を試みます
    /// </summary>
    void TryToActivateSpSkill(bool isSpSkillActivating)
    {
        if (isSpSkillActivating == false) return;
        _confirmationPanelManager.SetIsSpSkillActivating(false);

        ActivateSpSkill(true).Forget();
    }

    /// <summary>
    /// ランダムなカードをフィールドに移動します
    /// </summary>
    public async UniTask MoveRandomCardToField(bool isPlayer)
    {
        CardController movingCard = GetPlayerUI(isPlayer).GetRandomHandCard();
        await MoveToBattleField(isPlayer, movingCard);
    }

    /// <summary>
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    public async UniTask AnnounceToOpenTheCard()
    {
        GameManager._instance.PlaySE(BATTLE);
        await _IdirectionalityUI.AnnounceToOpenTheCard();
    }

    /// <summary>
    /// 開始時にUIを非表示にします
    /// </summary>
    public void HideUIAtStart()
    {
        _IdirectionalityUI.HideUIAtStart();
    }

    /// <summary>
    /// フィールドのカードを表にします
    /// </summary>
    public async UniTask OpenTheBattleFieldCards()
    {
        CardController playerCard = _playerUI.GetFieldCard();
        CardController enemyCard = _enemyUI.GetFieldCard();

        UniTask opened1 = playerCard.TurnTheCardFaceUp();
        UniTask opened2 = enemyCard.TurnTheCardFaceUp();

        await UniTask.WhenAll(opened1, opened2);
    }

    /// <summary>
    /// ラウンドの勝敗の結果を表示
    /// </summary>
    public async UniTask ShowJudgementResultText(string result)
    {
        await _IdirectionalityUI.ShowJudgementResultText(result);
    }

    /// <summary>
    /// 確認画面UIを全てを非表示にする
    /// </summary>
    public void InactiveUIIfCountDownTimeOut()
    {
        _confirmationPanelManager.InactiveUIIfCountDownTimeOut();
    }

    /// <summary>
    /// バトル結果の表示
    /// </summary>
    /// <param name="isAcitve"></param>
    public void ShowBattleResultUI(bool isActive, string resultText)
    {
        _IdirectionalityUI.ShowBattleResultUI(isActive, resultText);
    }

    /// <summary>
    /// 必殺技の説明文の設定
    /// </summary>
    public void InitSpSkillDescriptions()
    {
        foreach (Text description in _descriptionsOfSpSkill)
        {
            description.text = _spSkillDescription;
        }
    }
}
