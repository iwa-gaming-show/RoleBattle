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
    [Header("未使用、使用済みの順にColorを必殺技ボタンに設定する")]
    Color[] _specialSkillButtonColors;

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
    /// 必殺技発動の確認UI
    /// </summary>
    public void ToggleConfirmationPanelToSpecialSkill(bool isActive)
    {
        _confirmationPanelToSpecialSkill.SetActive(isActive);
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
    /// 必殺技ボタンを押した時
    /// </summary>
    public void OnClickSpecialSkillButton()
    {
        //自分のターンのみ押せる
        if (GameManager._instance.IsMyTurn == false) return;
        //一度使用したら押せない
        if (GameManager._instance.CanUsePlayerSpecialSkill == false) return;

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
        SetSpecialSkillButtonImageForTarget(GetSpecialSkillButtonImageBy(isPlayer), _specialSkillButtonColors[1]);
    }

    /// <summary>
    /// 必殺技のImageを設定する
    /// </summary>
    /// <param name="targetImage"></param>
    /// <param name="setColor"></param>
    void SetSpecialSkillButtonImageForTarget(Image targetImage, Color setColor)
    {
        targetImage.color = setColor;
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
            SetSpecialSkillButtonImageForTarget(specialSkillButtonImage, _specialSkillButtonColors[0]);
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
    /// 必殺技発動の演出
    /// </summary>
    IEnumerator ShowSpecialSkillDirection(bool isPlayer)
    {
        ToggleProductionToSpecialSkill(true, isPlayer);
        yield return new WaitForSeconds(SPECIAL_SKILL_PRODUCTION_DISPLAY_TIME);
        ToggleProductionToSpecialSkill(false, isPlayer);
        yield return null;
    }

    /// <summary>
    /// 必殺技演出UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleProductionToSpecialSkill(bool isActive, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerProductionToSpecialSkill.SetActive(isActive);
            return;
        }

        _enemyProductionToSpecialSkill.SetActive(isActive);
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
