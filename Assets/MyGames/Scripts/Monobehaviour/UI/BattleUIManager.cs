using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIStrings;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class BattleUIManager : MonoBehaviour, IBattleUIManager
{
    [SerializeField]
    [Header("自身の獲得ポイントUI")]
    TextMeshProUGUI _myPointText;

    [SerializeField]
    [Header("相手の獲得ポイントUI")]
    TextMeshProUGUI _enemyPointText;

    [SerializeField]
    [Header("バトル場への確認画面のテキスト")]
    Text _fieldConfirmationText;

    [SerializeField]
    [Header("バトル場へ送るカードの確認UI")]
    ConfirmationPanelToField _confirmationPanelToField;

    [SerializeField]
    [Header("ゲームの進行に関するUIマネージャーを設定")]
    DirectionUIManager _directionUIManager;

    [SerializeField]
    [Header("必殺技のUIマネージャーを設定")]
    SpecialSkillUIManager _specialSkillUIManager;

    [SerializeField]
    [Header("バトル中に使用する確認画面のUIを格納する")]
    GameObject[] BattleConfirmationPanels;

    [SerializeField]
    [Header("オブジェクトプールに使用する非表示にしたUIを格納するCanvasを設定")]
    GameObject CanvasForObjectPool;

    IHideableUIsAtStart _hideableUIsAtStartByDir;
    IHideableUIsAtStart _hideableUIsAtStartBySP;

    #region//プロパティ
    public ConfirmationPanelToField ConfirmationPanelToField => _confirmationPanelToField;
    public SpecialSkillUIManager SpecialSkillUIManager => _specialSkillUIManager;
    public DirectionUIManager DirectionUIManager => _directionUIManager;
    #endregion

    void Awake()
    {
        ServiceLocator.Register<IBattleUIManager>(this);
        _hideableUIsAtStartByDir = _directionUIManager.GetComponent<IHideableUIsAtStart>();
        _hideableUIsAtStartBySP = _specialSkillUIManager.GetComponent<IHideableUIsAtStart>();
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IBattleUIManager>(this);
    }

    /// <summary>
    /// 開始時に非表示にするUI
    /// </summary>
    public void HideUIAtStart()
    {
        _hideableUIsAtStartByDir.HideUIsAtStart();
        _hideableUIsAtStartBySP.HideUIsAtStart();
        _confirmationPanelToField.ToggleUI(false);
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
    /// ポイントの表示
    /// </summary>
    public void ShowPointBy(int point, bool isPlayer)
    {
        if (isPlayer)
        {
            _myPointText.text = point.ToString() + POINT_SUFFIX;
        }
        else
        {
            _enemyPointText.text = point.ToString() + POINT_SUFFIX;
        }
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
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    public async UniTask AnnounceToOpenTheCard()
    {
        await _directionUIManager.AnnounceToOpenTheCard();
    }

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <param name="roundCount"></param>
    /// <param name="maxRoundCount"></param>
    /// <returns></returns>
    public async UniTask ShowRoundCountText(int roundCount, int maxRoundCount)
    {
        await _directionUIManager.ShowRoundCountText(roundCount, maxRoundCount);
    }

    /// <summary>
    /// カウントダウンを表示
    /// </summary>
    public void ShowCountDownText(int countDownTime)
    {
        _directionUIManager.ShowCountDownText(countDownTime);
    }

    /// <summary>
    /// ゲーム結果の表示の切り替え
    /// </summary>
    /// <param name="isAcitve"></param>
    public void ToggleGameResultUI(bool isActive)
    {
        _directionUIManager.ToggleGameResultUI(isActive);
    }

    /// <summary>
    /// ゲームの勝敗のテキストを表示する
    /// </summary>
    /// <returns></returns>
    public void SetGameResultText(string text)
    {
        _directionUIManager.SetGameResultText(text);
    }

    /// <summary>
    /// ラウンドの勝敗の結果を表示
    /// </summary>
    public async UniTask ShowJudgementResultText(string result)
    {
        await _directionUIManager.ShowJudgementResultText(result);
    }

    /// <summary>
    /// プレイヤーのターン時にテキストを表示する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public async UniTask ShowThePlayerTurnText(bool isPlayer)
    {
        await _directionUIManager.ShowThePlayerTurnText(isPlayer);
    }

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    public async UniTask ActivateSpecialSkill(bool isPlayer)
    {
        await _specialSkillUIManager.ActivateSpecialSkill(isPlayer);
    }
}
