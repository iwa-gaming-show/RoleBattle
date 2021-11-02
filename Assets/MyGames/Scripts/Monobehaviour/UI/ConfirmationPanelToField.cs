using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GM = GameManager;

/// <summary>
/// バトル場へ送るカードの確認UI
/// </summary>
public class ConfirmationPanelToField : MonoBehaviour,
    IToggleable,
    IYesButtonAction,
    INoButtonAction
{
    [SerializeField]
    [Header("CanvasForDirectionを設定")]
    Transform directionUIManagerTransform;

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
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, directionUIManagerTransform);
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
}
