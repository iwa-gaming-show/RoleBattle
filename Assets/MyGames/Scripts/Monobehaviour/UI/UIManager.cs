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
    ConfirmationPanelToField _confirmationPanelToField;

    [SerializeField]
    [Header("必殺技のUIマネージャーを設定")]
    SpecialSkillUIManager _specialSkillUIManager;

    [SerializeField]
    [Header("バトル中に使用する確認画面のUIを格納する")]
    GameObject[] BattleConfirmationPanels;

    #region//プロパティ
    public ConfirmationPanelToField ConfirmationPanelToField => _confirmationPanelToField;
    public SpecialSkillUIManager SpecialSkillUIManager => _specialSkillUIManager;
    #endregion

    /// <summary>
    /// 開始時に非表示にするUI
    /// </summary>
    public void HideUIAtStart()
    {
        ToggleGameResultUI(false);
        ToggleJudgementResultText(false);
        ToggleRoundCountText(false);
        ToggleAnnounceTurnFor(false, true);
        ToggleAnnounceTurnFor(false, true);
        ToggleOpenPhaseText(false);
        _confirmationPanelToField.ToggleUI(false);
        _specialSkillUIManager.ConfirmationPanel.ToggleUI(false);
        _specialSkillUIManager.ToggleProductionToSpecialSkill(false, true);
        _specialSkillUIManager.ToggleProductionToSpecialSkill(false, false);
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
    public Tween MoveAnchorPosX(RectTransform rectTransform, float endValue, float duration)
    {
        return rectTransform.DOAnchorPosX(endValue, duration);
    }

    /// <summary>
    /// targetを画面端に移動した場合のX方向の値を取得
    /// </summary>
    /// <returns></returns>
    public float GetScreenEdgeXFor(float targetWidthX)
    {
        //画面端 = 画面の横幅÷2 + UIの横幅分
        return (Screen.width / 2) + targetWidthX;
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
        _confirmationPanelToField.ToggleUI(true);
    }

    /// <summary>
    /// 確認画面UIを全てを非表示にする
    /// </summary>
    public void CloseAllConfirmationPanels()
    {
        foreach (GameObject targetPanel in BattleConfirmationPanels)
        {
            targetPanel.GetComponent<IToggleable>()?.ToggleUI(false);
        }
    }

    /// <summary>
    /// UIデータの初期設定
    /// </summary>
    public void InitUIData()
    {
        _specialSkillUIManager.InitSpecialSkillButtonImageByPlayers();
        _specialSkillUIManager.InitSpecialSkillDescriptions();
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
