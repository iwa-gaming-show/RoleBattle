using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIStrings;
using static WaitTimes;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
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
    [Header("ゲームの勝敗の結果表示のテキスト")]
    Text _gameResultText;

    [SerializeField]
    [Header("バトル場への確認画面のテキスト")]
    Text _fieldConfirmationText;

    [SerializeField]
    [Header("カードOPEN時のテキスト")]
    Text _openPhaseText;

    [SerializeField]
    [Header("カウントダウンのテキスト")]
    Text _countDownText;

    [SerializeField]
    [Header("自分のターンであることを知らせるUI")]
    GameObject _announceThePlayerTurn;

    [SerializeField]
    [Header("相手のターンであることを知らせるUI")]
    GameObject _announceTheEnemyTurn;

    [SerializeField]
    [Header("ゲームの勝敗の結果表示用UI")]
    GameObject _gameResultUI;

    [SerializeField]
    [Header("バトル場へ送るカードの確認UI")]
    GameObject _confirmationPanelToField;

    [SerializeField]
    [Header("必殺技発動の確認UI")]
    GameObject _confirmationPanelToSpecialSkill;

    [SerializeField]
    [Header("プレイヤーの必殺技発動の演出UI")]
    GameObject _playerProductionToSpecialSkill;

    [SerializeField]
    [Header("エネミーの必殺技発動の演出UI")]
    GameObject _enemyProductionToSpecialSkill;

    [SerializeField]
    [Header("必殺技の詳細を記述するテキストを格納する")]
    Text[] _descriptionsOfSpecialSkill;

    [SerializeField]
    [Header("プレイヤー、エネミーの順に必殺技ボタンを格納する")]
    Image[] _specialSkillButtonImages;

    [SerializeField]
    [Header("未使用、使用済みの順にアイコン画像を必殺技ボタンに設定する")]
    Sprite[] _specialSkillButtonIcons;

    [SerializeField]
    [Header("必殺技の説明用のテキストを設定")]

    string _specialSkillDescription;
    bool _isClickedConfirmationFieldButton;//フィールドへの確認ボタンのをクリックしたか
    bool _canMoveToField;//カードの移動ができる

    #region//プロパティ
    public bool CanMoveToField => _canMoveToField;
    public bool IsClickedConfirmationFieldButton => _isClickedConfirmationFieldButton;
    public GameObject ConfirmationPanelToField => _confirmationPanelToField;
    #endregion

    /// <summary>
    /// 開始時に非表示にするUI
    /// </summary>
    public void HideUIAtStart()
    {
        ToggleGameResultUI(false);
        ToggleJudgementResultText(false);
        ToggleRoundCountText(false);
        ToggleConfirmationPanelToField(false);
        ToggleConfirmationPanelToSpecialSkill(false);
        ToggleAnnounceTurnFor(false, true);
        ToggleAnnounceTurnFor(false, true);
        ToggleProductionToSpecialSkill(false, true);
        ToggleProductionToSpecialSkill(false, false);
        ToggleOpenPhaseText(false);
    }

    /// <summary>
    /// ポイントの表示
    /// </summary>
    public void ShowPoint(int myPoint, int enemyPoint)
    {
        _myPointText.text = myPoint.ToString() + POINT_SUFFIX;
        _enemyPointText.text = enemyPoint.ToString() + POINT_SUFFIX;
    }

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowRoundCountText(int roundCount, int maxRoundCount)
    {
        ToggleRoundCountText(true);
        SetRoundCountText(roundCount, maxRoundCount);

        yield return new WaitForSeconds(ROUND_COUNT_DISPLAY_TIME);
        ToggleRoundCountText(false);
    }

    /// <summary>
    /// ラウンドの勝敗の結果を表示
    /// </summary>
    public IEnumerator ShowJudgementResultText(string result)
    {
        ToggleJudgementResultText(true);
        _judgementResultText.text = result + JUDGEMENT_RESULT_SUFFIX;

        yield return new WaitForSeconds(JUDGMENT_RESULT_DISPLAY_TIME);
        ToggleJudgementResultText(false);
    }

    /// <summary>
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    public IEnumerator AnnounceToOpenTheCard()
    {
        RectTransform textRectTransform = _openPhaseText.rectTransform;
        float screenEdgeX = GetScreenEdgeXFor(textRectTransform.sizeDelta.x);

        //右端→真ん中→左端へ移動する
        Sequence sequence = DOTween.Sequence();
        sequence.Append(MoveAnchorPosX(textRectTransform, screenEdgeX, 0).OnStart(() => ToggleOpenPhaseText(true)));
        sequence.Append(MoveAnchorPosX(textRectTransform, 0f, 0.25f));
        sequence.Append(MoveAnchorPosX(textRectTransform, -screenEdgeX, 0.4f).SetDelay(1f).OnComplete(() => ToggleOpenPhaseText(false)));

        yield return sequence
            .Play()
            .WaitForCompletion();
    }

    /// <summary>
    /// DOTweenでアンカーのX軸を移動します
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="endValue"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    Tween MoveAnchorPosX(RectTransform rectTransform, float endValue, float duration)
    {
        return rectTransform.DOAnchorPosX(endValue, duration);
    }

    /// <summary>
    /// targetを画面端に移動した場合のX方向の値を取得
    /// </summary>
    /// <returns></returns>
    float GetScreenEdgeXFor(float targetWidthX)
    {
        //画面端 = 画面の横幅÷2 + UIの横幅分
        return (Screen.width / 2) + targetWidthX;
    }

    /// <summary>
    /// 必殺技発動の演出
    /// </summary>
    IEnumerator ShowSpecialSkillDirection(bool isPlayer)
    {
        RectTransform targetUIRectTranform = GetProductionToSpecialSkillBy(isPlayer).GetComponent<RectTransform>();
        float screenEdgeX = GetScreenEdgeXFor(targetUIRectTranform.sizeDelta.x);
        Sequence sequence = DOTween.Sequence();
        Tween firstMove;
        Tween lastMove;

        //プレイヤーなら右から左へ移動, エネミーなら左から右へ移動する
        if (isPlayer)
        {
            firstMove = MoveAnchorPosX(targetUIRectTranform, screenEdgeX, 0);
            lastMove = MoveAnchorPosX(targetUIRectTranform, -screenEdgeX, 0.4f);
        }
        else
        {
            firstMove = MoveAnchorPosX(targetUIRectTranform, -screenEdgeX, 0f);
            lastMove = MoveAnchorPosX(targetUIRectTranform, screenEdgeX, 0.4f);
        }

        sequence.Append(firstMove.OnStart(() => ToggleProductionToSpecialSkill(true, isPlayer)));
        sequence.Append(MoveAnchorPosX(targetUIRectTranform, 0f, 0.25f));
        sequence.Append(lastMove.SetDelay(SPECIAL_SKILL_PRODUCTION_DISPLAY_TIME).OnComplete(() => ToggleProductionToSpecialSkill(false, isPlayer)));

        yield return sequence
            .Play()
            .WaitForCompletion();
    }

    /// <summary>
    /// 必殺技演出UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleProductionToSpecialSkill(bool isActive, bool isPlayer)
    {
        GetProductionToSpecialSkillBy(isPlayer).SetActive(isActive);
    }

    /// <summary>
    /// 必殺技演出UIを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    GameObject GetProductionToSpecialSkillBy(bool isPlayer)
    {
        if (isPlayer)
        {
            return _playerProductionToSpecialSkill;
        }
        return _enemyProductionToSpecialSkill;
    }

    /// <summary>
    /// カードOPEN時のテキストの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleOpenPhaseText(bool isActive)
    {
        _openPhaseText.gameObject?.SetActive(isActive);
    }

    /// <summary>
    /// プレイヤーのターン時にテキストを表示する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public IEnumerator ShowThePlayerTurnText(bool isPlayer)
    {
        ToggleAnnounceTurnFor(true, isPlayer);
        yield return new WaitForSeconds(ANNOUNCEMENT_TIME_TO_TURN_TEXT);
        ToggleAnnounceTurnFor(false, isPlayer);
    }

    /// <summary>
    /// プレイヤーのターン時に表示するUIの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleAnnounceTurnFor(bool isActive, bool isPlayer)
    {
        if (isPlayer)
        {
            _announceThePlayerTurn.SetActive(isActive);
            return;
        }
        _announceTheEnemyTurn.SetActive(isActive);
    }

    /// <summary>
    /// カウントダウンを表示
    /// </summary>
    public void ShowCountDownText(int countDownTime)
    {
        _countDownText.text = countDownTime.ToString();
    }

    /// <summary>
    /// ゲーム結果の表示の切り替え
    /// </summary>
    /// <param name="isAcitve"></param>
    public void ToggleGameResultUI(bool isAcitve)
    {
        _gameResultUI.SetActive(isAcitve);
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
    /// ラウンド数表示用テキストの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleRoundCountText(bool isActive)
    {
        _roundCountText.gameObject?.SetActive(isActive);
    }

    /// <summary>
    /// フィールドへ移動するカードを選択したとき
    /// </summary>
    public void SelectedToFieldCard(CardController selectedCard)
    {
        //確認画面のメッセージを、選択したカード名にする
        _fieldConfirmationText.text = selectedCard.CardModel.Name + FIELD_CONFIRMATION_TEXT_SUFFIX;
        ToggleConfirmationPanelToField(true);
    }

    /// <summary>
    /// バトル場へ送るカードの確認画面
    /// </summary>
    public void ToggleConfirmationPanelToField(bool isActive)
    {
        _confirmationPanelToField.SetActive(isActive);
    }

    /// <summary>
    /// 必殺技発動の確認UI
    /// </summary>
    public void ToggleConfirmationPanelToSpecialSkill(bool isActive)
    {
        _confirmationPanelToSpecialSkill.SetActive(isActive);
    }

    /// <summary>
    /// バトル場への確認画面でYesを押した時
    /// </summary>
    public void OnClickYesForFieldConfirmation()
    {
        ToggleConfirmationPanelToField(false);
        _isClickedConfirmationFieldButton = true;
        _canMoveToField = true;
    }

    /// <summary>
    /// バトル場への確認画面でNoを押した時
    /// </summary>
    public void OnClickNoForFieldConfirmation()
    {
        ToggleConfirmationPanelToField(false);
        _isClickedConfirmationFieldButton = true;
        _canMoveToField = false;
    }

    /// <summary>
    /// 必殺技ボタンを押した時
    /// </summary>
    public void OnClickSpecialSkillButton()
    {
        //自分のターンのみ押せる
        if (GameManager._instance.TurnManager.IsMyTurn == false) return;
        //一度使用したら押せない
        if (GameManager._instance.Player.CanUseSpecialSkill == false) return;

        ToggleConfirmationPanelToSpecialSkill(true);
    }

    /// <summary>
    /// 必殺技発動の確認画面でYesを押した時
    /// </summary>
    public void OnClickYesForSpecialSkillConfirmation()
    {
        ToggleConfirmationPanelToSpecialSkill(false);
        StartCoroutine(ActivateSpecialSkill(true));
    }

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    public IEnumerator ActivateSpecialSkill(bool isPlayer)
    {
        UsedSpecialSkillButton(isPlayer);
        GameManager._instance.UsedSpecialSkill(isPlayer);
        //必殺技を演出、 演出中はカウントダウンが止まる
        GameManager._instance.SetIsDuringProductionOfSpecialSkill(true);

        yield return ShowSpecialSkillDirection(isPlayer);
        //カウントダウン再開
        GameManager._instance.SetIsDuringProductionOfSpecialSkill(false);
        
    }

    /// <summary>
    /// 必殺技ボタンを使用済みにする
    /// </summary>
    void UsedSpecialSkillButton(bool isPlayer)
    {
        SetSpecialSkillButtonSpriteForTarget(GetSpecialSkillButtonImageBy(isPlayer), _specialSkillButtonIcons[1]);
    }

    /// <summary>
    /// 必殺技のIconを設定する
    /// </summary>
    /// <param name="targetImage"></param>
    /// <param name="setSprite"></param>
    void SetSpecialSkillButtonSpriteForTarget(Image targetImage, Sprite setSprite)
    {
        targetImage.sprite = setSprite;
    }

    /// <summary>
    /// UIデータの初期設定
    /// </summary>
    public void InitUIData()
    {
        InitSpecialSkillButtonImageByPlayers();
        InitSpecialSkillDescriptions();
    }

    /// <summary>
    /// 必殺技の説明文の設定
    /// </summary>
    void InitSpecialSkillDescriptions()
    {
        foreach (Text description in _descriptionsOfSpecialSkill)
        {
            description.text = _specialSkillDescription;
        }
    }

    /// <summary>
    /// プレイヤー毎に必殺技のImageを初期化する
    /// </summary>
    void InitSpecialSkillButtonImageByPlayers()
    {
        foreach (Image specialSkillButtonImage in _specialSkillButtonImages)
        {
            SetSpecialSkillButtonSpriteForTarget(specialSkillButtonImage, _specialSkillButtonIcons[0]);
        }
    }

    /// <summary>
    /// プレイヤーの必殺技ボタンのImageを取得します
    /// </summary>
    /// <returns></returns>
    Image GetSpecialSkillButtonImageBy(bool isPlayer)
    {
        if (isPlayer) return _specialSkillButtonImages[0];
        return _specialSkillButtonImages[1];
    }

    /// <summary>
    /// 必殺技発動の確認画面でNoを押した時
    /// </summary>
    public void OnClickNoForSpecialSkillConfirmation()
    {
        ToggleConfirmationPanelToSpecialSkill(false);
    }

    /// <summary>
    /// カード移動フラグのセット
    /// </summary>
    public void SetCanMoveToField(bool can)
    {
        _canMoveToField = can;
    }

    /// <summary>
    /// バトル場への移動確認画面の押下フラグのセット
    /// </summary>
    /// <param name="isClicked"></param>
    public void SetIsClickedConfirmationFieldButton(bool isClicked)
    {
        _isClickedConfirmationFieldButton = isClicked;
    }

    /// <summary>
    /// ゲームの勝敗のテキストを表示する
    /// </summary>
    /// <returns></returns>
    public void SetGameResultText(string text)
    {
        _gameResultText.text = text;
    }

    /// <summary>
    /// ラウンド表示用のテキストを設定する
    /// </summary>
    void SetRoundCountText(int roundCount, int maxRoundCount)
    {
        if (roundCount == maxRoundCount)
        {
            //最終ラウンド
            _roundCountText.text = FINAL_ROUND;
        }
        else
        {
            _roundCountText.text = ROUND_PREFIX + roundCount.ToString();
        }
    }
}
