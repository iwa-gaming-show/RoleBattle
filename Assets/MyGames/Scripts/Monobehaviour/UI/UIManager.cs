using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIStrings;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
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
        _hideableUIsAtStartByDir = _directionUIManager.GetComponent<IHideableUIsAtStart>();
        _hideableUIsAtStartBySP = _specialSkillUIManager.GetComponent<IHideableUIsAtStart>();
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
}
