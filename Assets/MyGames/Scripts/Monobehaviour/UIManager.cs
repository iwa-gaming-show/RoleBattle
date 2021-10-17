using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIStrings;
using static WaitTimes;

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
    [Header("自分のターン時に表示するテキスト")]
    Text _announceThePlayerTurnText;

    [SerializeField]
    [Header("相手のターン時に表示するテキスト")]
    Text _announceTheEnemyTurnText;

    [SerializeField]
    [Header("ゲームの勝敗の結果表示用UI")]
    GameObject _gameResultUI;

    [SerializeField]
    [Header("バトル場へ送るカードの確認UI")]
    GameObject _confirmationPanelToField;

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
    /// カードを開く演出を行います
    /// </summary>
    /// <returns></returns>
    public IEnumerator AnnounceToOpenTheCard()
    {
        ToggleOpenPhaseText(true);
        yield return new WaitForSeconds(ANNOUNCEMENT_TIME_TO_OPEN_CARD);
        ToggleOpenPhaseText(false);
    }

    /// <summary>
    /// プレイヤーのターン時にテキストを表示する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public IEnumerator ShowThePlayerTurnText(bool isPlayer)
    {
        ToggleAnnounceTurnTextFor(true, isPlayer);
        yield return new WaitForSeconds(ANNOUNCEMENT_TIME_TO_TURN_TEXT);
        ToggleAnnounceTurnTextFor(false, isPlayer);
    }

    /// <summary>
    /// プレイヤーのターン時に表示するテキストの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleAnnounceTurnTextFor(bool isActive, bool isPlayer)
    {
        if (isPlayer)
        {
            _announceThePlayerTurnText.gameObject?.SetActive(isActive);
            return;
        }
        _announceTheEnemyTurnText.gameObject?.SetActive(isActive);
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
    /// カードOPEN時のテキストの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleOpenPhaseText(bool isActive)
    {
        _openPhaseText.gameObject?.SetActive(isActive);
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
