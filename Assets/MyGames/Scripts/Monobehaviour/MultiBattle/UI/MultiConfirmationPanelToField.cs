using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIStrings;

/// <summary>
/// バトル場へ送るカードの確認UI
/// </summary>
public class MultiConfirmationPanelToField : MonoBehaviour,
    IToggleable,
    IYesButtonAction,
    INoButtonAction
{
    [SerializeField]
    [Header("CanvasForConfirmationPanelsを設定")]
    Transform _canvasForConfirmationPanelsTransform;

    [SerializeField]
    [Header("バトル場への確認画面のテキスト")]
    Text _fieldConfirmationText;

    bool _isClickedConfirmationButton;//確認ボタンをクリックしたか
    bool _canMoveToField;//カードの移動ができる

    #region//プロパティ
    public bool CanMoveToField => _canMoveToField;
    public bool IsClickedConfirmationButton => _isClickedConfirmationButton;
    #endregion

    /// <summary>
    /// UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance
            .ToggleUIGameObject(gameObject, isActive, _canvasForConfirmationPanelsTransform);
    }

    /// <summary>
    /// バトル場への確認画面でYesを押した時
    /// </summary>
    public void OnClickYes()
    {
        ToggleUI(false);
        _isClickedConfirmationButton = true;
        _canMoveToField = true;
    }

    /// <summary>
    /// バトル場への確認画面でNoを押した時
    /// </summary>
    public void OnClickNo()
    {
        ToggleUI(false);
        _isClickedConfirmationButton = true;
        _canMoveToField = false;
    }

    /// <summary>
    /// バトル場への移動確認画面の押下フラグのセット
    /// </summary>
    /// <param name="isClicked"></param>
    public void SetIsClickedConfirmationButton(bool isClicked)
    {
        _isClickedConfirmationButton = isClicked;
    }

    /// <summary>
    /// カード移動フラグのセット
    /// </summary>
    public void SetCanMoveToField(bool can)
    {
        _canMoveToField = can;
    }

    /// <summary>
    /// 確認画面のメッセージを、選択したカード名にする
    /// </summary>
    /// <param name="selectedCard"></param>
    public void SetFieldConfirmationText(CardController selectedCard)
    {
        _fieldConfirmationText.text = selectedCard.CardModel.Name + FIELD_CONFIRMATION_TEXT_SUFFIX;
    }
}
