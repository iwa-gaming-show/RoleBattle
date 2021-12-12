using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Linq;
using UnityEngine.UI;
using static UIStrings;
using static WaitTimes;
using static BattlePhase;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField]
    [Header("プレイヤーのUI")]
    MultiPlayerUI _playerUI;

    [SerializeField]
    [Header("エネミーのUI")]
    MultiPlayerUI _enemyUI;

    [SerializeField]
    [Header("未使用、使用済みの順にアイコン画像を必殺技ボタンに設定する")]
    Sprite[] _spButtonIcons;

    [SerializeField]
    [Header("ラウンド数表示テキスト")]
    TextMeshProUGUI _roundCountText;

    [SerializeField]
    [Header("カードリストを設定する(ScriptableObjectを参照)")]
    CardEntityList _cardEntityList;

    [SerializeField]
    [Header("カードプレハブ")]
    CardController _cardPrefab;

    [SerializeField]
    [Header("自分のターンであることを知らせるUI")]
    GameObject _announceThePlayerTurn;

    [SerializeField]
    [Header("相手のターンであることを知らせるUI")]
    GameObject _announceTheEnemyTurn;

    [SerializeField]
    [Header("カウントダウンのテキスト")]
    TextMeshProUGUI _countDownText;

    [SerializeField]
    [Header("開始時に非表示にするUIを設定します")]
    GameObject[] _hideUIs;

    [SerializeField]
    [Header("カードOPEN時のテキスト")]
    TextMeshProUGUI _openPhaseText;

    [SerializeField]
    [Header("ラウンドの勝敗の結果表示のテキスト")]
    TextMeshProUGUI _judgementResultText;

    [SerializeField]
    [Header("バトルの勝敗の結果表示用UI")]
    GameObject _battleResultUI;

    [SerializeField]
    [Header("バトルの勝敗の結果表示のテキスト")]
    TextMeshProUGUI _battleResultText;

    [SerializeField]
    [Header("必殺技の詳細を記述するテキストを格納する")]
    Text[] _descriptionsOfSpSkill;

    [SerializeField]
    [Header("必殺技の説明用のテキストを設定")]
    string _spSkillDescription;


    #region//プロパティ
    IMultiConfirmationPanelManager _multiConfirmationPanelManager;
    PhotonView _photonView;
    #endregion

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        _multiConfirmationPanelManager = ServiceLocator.Resolve<IMultiConfirmationPanelManager>();
    }

    void Update()
    {
        TryToMoveToField(_multiConfirmationPanelManager.MovingFieldCard);
        TryToActivateSpSkill(_multiConfirmationPanelManager.IsSpSkillActivating);
    }

    /// <summary>
    /// プレイヤーかエネミーのUIを取得する
    /// </summary>
    /// <returns></returns>
    public MultiPlayerUI GetPlayerUI(bool isPlayer)
    {
        if (isPlayer) return _playerUI;
        return _enemyUI;
    }

    /// <summary>
    /// ポイントの表示
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
    /// プレイヤーアイコンを配置する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="targetGo"></param>
    public void PlacePlayerIconBy(bool isPlayer, GameObject targetGo)
    {
        GetPlayerUI(isPlayer).PlacePlayerIcon(targetGo);
    }

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <param name="roundCount"></param>
    /// <returns></returns>
    public async UniTask ShowRoundCountText(int roundCount, int maxRoundCount)
    {
        ToggleRoundCountText(true);
        SetRoundCountText(roundCount, maxRoundCount);

        await UniTask.Delay(TimeSpan.FromSeconds(ROUND_COUNT_DISPLAY_TIME));
        ToggleRoundCountText(false);
    }

    /// <summary>
    /// ラウンド表示用のテキストを設定する
    /// </summary>
    void SetRoundCountText(int roundCount, int maxRoundCount)
    {
        if (roundCount == maxRoundCount)
            _roundCountText.text = FINAL_ROUND;//最終ラウンド
        else
            _roundCountText.text = ROUND_PREFIX + roundCount.ToString();
    }

    /// <summary>
    /// ラウンド数表示用テキストの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleRoundCountText(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_roundCountText.gameObject, isActive, transform);
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
    /// 相手のフィールドのカードを置き換えます
    /// </summary>
    /// <param name="cardType"></param>
    public async UniTask ReplaceEnemyFieldCard(CardType cardType)
    {
        CardController replacingCard = CreateCardFor(cardType, false);
        //元々フィールドに配置したカードは削除します
        _enemyUI.DestroyFieldCard();
        _enemyUI.SetFieldCard(replacingCard);
        await UniTask.Yield();
    }

    /// <summary>
    /// カードの種類に対応したカードを作成します
    /// </summary>
    /// <param name="cardType"></param>
    /// <returns></returns>
    CardController CreateCardFor(CardType cardType, bool isPlayer)
    {
        //cardTypeに対応したカードのindex番号を取得します
        var cardEntities = _cardEntityList.GetCardEntityList
            .Select((ce, i) => new { CardType = ce.CardType, Index = i });

        int cardIndex = (cardEntities.Where(ce => ce.CardType == cardType)
            .First().Index is int index) ? index : 0;

        return CreateCard(cardIndex, isPlayer);
    }

    /// <summary>
    /// カードを生成する
    /// </summary>
    CardController CreateCard(int cardIndex, bool isPlayer)
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
        ToggleAnnounceTurnFor(true, isPlayer);
        await UniTask.Delay(TimeSpan.FromSeconds(ANNOUNCEMENT_TIME_TO_TURN_TEXT));
        ToggleAnnounceTurnFor(false, isPlayer);
    }

    /// <summary>
    /// プレイヤーのターン時に表示するUIの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleAnnounceTurnFor(bool isActive, bool isPlayer)
    {
        GameObject AnnounceThePlayerTurn = GetAnnounceThePlayerTurnBy(isPlayer);
        CanvasForObjectPool._instance.ToggleUIGameObject(AnnounceThePlayerTurn, isActive, transform);
    }

    /// <summary>
    /// プレイヤーのターンのアナウンス用のゲームオブジェクトを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    GameObject GetAnnounceThePlayerTurnBy(bool isPlayer)
    {
        if (isPlayer) return _announceThePlayerTurn;
        return _announceTheEnemyTurn;
    }

    /// <summary>
    /// カウントダウンを表示
    /// </summary>
    public void ShowCountDownText(int countDownTime)
    {
        _countDownText.text = countDownTime.ToString();
    }

    /// <summary>
    /// フィールドへの移動を試みます
    /// </summary>
    /// <returns></returns>
    void TryToMoveToField(CardController movingCard)
    {
        if (movingCard == null) return;
        _multiConfirmationPanelManager.DestroyMovingBattleCard();

        MoveToBattleField(movingCard).Forget();
    }

    /// <summary>
    /// 必殺技発動を試みます
    /// </summary>
    void TryToActivateSpSkill(bool IsSpSkillActivating)
    {
        if (IsSpSkillActivating == false) return;
        _multiConfirmationPanelManager.SetIsSpSkillActivating(false);

        ActivateSpSkill().Forget();
    }

    /// <summary>
    /// カードを移動する
    /// </summary>
    async UniTask MoveToBattleField(CardController movingCard)
    {
        //すでにバトル場にカードが置かれているなら何もしない
        if (PhotonNetwork.LocalPlayer.GetIsFieldCardPlaced()) return;

        RegisterCardType(movingCard.CardType);
        //カードを配置済みにする
        PhotonNetwork.LocalPlayer.SetIsFieldCardPlaced(true);
        PhotonNetwork.CurrentRoom.SetIntBattlePhase(SELECTED);

        //playerのカードを移動する、対戦相手の視点ではEnemyのカードを移動する
        _photonView.RPC("RpcMoveEnemyCardToField", RpcTarget.Others);
        await _playerUI.MoveToBattleField(movingCard);

        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_TURN));
        //ターンを終了する
        PhotonNetwork.LocalPlayer.SetIsMyTurnEnd(true);
    }

    /// <summary>
    /// ランダムなカードをフィールドに移動します
    /// </summary>
    public void MoveRandomCardToField()
    {
        CardController movingCard = _playerUI.GetRandomHandCard();
        MoveToBattleField(movingCard).Forget();
    }

    /// <summary>
    /// 相手のカードをフィールドに移動します
    /// </summary>
    [PunRPC]
    void RpcMoveEnemyCardToField()
    {
        //演出用にランダムなカードを選び移動させる。
        //※実際にフィールドに出すカードは異なります、カンニングを阻止する意もあります。
        CardController randomFieldCard = _enemyUI.GetRandomHandCard();
        _enemyUI.MoveToBattleField(randomFieldCard).Forget();
    }

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    /// <returns></returns>
    async UniTask ActivateSpSkill()
    {
        PhotonNetwork.LocalPlayer.SetIsUsingSpInRound(true);
        PhotonNetwork.LocalPlayer.SetCanUseSpSkill(false);
        PhotonNetwork.CurrentRoom.SetIsDuringDirecting(true);

        _photonView.RPC("RpcActivateEnemySpSkill", RpcTarget.Others);
        await _playerUI.ActivateDirectingOfSpSkill(true);
    }

    /// <summary>
    /// 相手のカードをフィールドに移動します
    /// </summary>
    [PunRPC]
    void RpcActivateEnemySpSkill()
    {
        _enemyUI.ActivateDirectingOfSpSkill(false).Forget();
    }

    /// <summary>
    /// カードタイプを登録します
    /// </summary>
    void RegisterCardType(CardType cardType)
    {
        PhotonNetwork.LocalPlayer.SetIntBattleCardType(cardType);
    }

    /// <summary>
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    public async UniTask AnnounceToOpenTheCard()
    {
        RectTransform textRectTransform = _openPhaseText.rectTransform;
        float screenEdgeX = UIUtil.GetScreenEdgeXFor(textRectTransform.sizeDelta.x);

        //右端→真ん中→左端へ移動する
        Sequence sequence = DOTween.Sequence();
        sequence.Append(UIUtil.MoveAnchorPosXByDOT(textRectTransform, screenEdgeX, 0)
            .OnStart(() => ToggleOpenPhaseText(true)));

        sequence.Append(UIUtil.MoveAnchorPosXByDOT(textRectTransform, 0f, 0.25f));

        sequence.Append(UIUtil.MoveAnchorPosXByDOT(textRectTransform, -screenEdgeX, 0.4f).SetDelay(1f)
            .OnComplete(() => ToggleOpenPhaseText(false)));

        await sequence
            .Play()
            .AsyncWaitForCompletion();
    }

    /// <summary>
    /// カードOPEN時のテキストの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleOpenPhaseText(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_openPhaseText.gameObject, isActive, transform);
    }

    /// <summary>
    /// 開始時にUIを非表示にします
    /// </summary>
    public void HideUIAtStart()
    {
        CanvasForObjectPool._instance
            .ToggleUIGameObject(_battleResultUI, false, transform);
    }

    /// <summary>
    /// バトル場のカードを表にします
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
        ToggleJudgementResultText(true);
        _judgementResultText.text = result + JUDGEMENT_RESULT_SUFFIX;

        await UniTask.Delay(TimeSpan.FromSeconds(JUDGMENT_RESULT_DISPLAY_TIME));
        ToggleJudgementResultText(false);
    }

    /// <summary>
    /// ラウンドの勝敗の結果の表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleJudgementResultText(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_judgementResultText.gameObject, isActive, transform);
    }

    /// <summary>
    /// 確認画面UIを全てを非表示にする
    /// </summary>
    public void InactiveUIIfCountDownTimeOut()
    {
        _multiConfirmationPanelManager.InactiveUIIfCountDownTimeOut();
    }

    /// <summary>
    /// バトル結果の表示の切り替え
    /// </summary>
    /// <param name="isAcitve"></param>
    public void ToggleBattleResultUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_battleResultUI, isActive, transform);
    }

    /// <summary>
    /// バトルの勝敗のテキストを表示する
    /// </summary>
    /// <returns></returns>
    public void SetBattleResultText(string text)
    {
        _battleResultText.text = text;
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