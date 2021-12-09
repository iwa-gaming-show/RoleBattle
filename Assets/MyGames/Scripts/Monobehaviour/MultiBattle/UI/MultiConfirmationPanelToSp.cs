using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 必殺技発動の確認UI
/// </summary>
public class MultiConfirmationPanelToSp : MonoBehaviour,
    IToggleable,
    IYesButtonAction,
    INoButtonAction,
    IRequiredConfirmation
{
    [SerializeField]
    [Header("CanvasForConfirmationPanelsを設定")]
    Transform _canvasForConfirmationPanelsTransform;

    bool _canActivateSpSkill;//カードの移動ができる
    bool _isConfirmed;

    #region//プロパティ
    public bool CanActivateSpSkill => _canActivateSpSkill;
    public bool IsConfirmed => _isConfirmed;
    #endregion

    /// <summary>
    /// UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, _canvasForConfirmationPanelsTransform);
    }

    /// <summary>
    /// 必殺技発動の確認画面でYesを押した時
    /// </summary>
    public void OnClickYes()
    {
        ToggleUI(false);
        _isConfirmed = true;
        _canActivateSpSkill = true;
    }

    /// <summary>
    /// 必殺技発動の確認画面でNoを押した時
    /// </summary>
    public void OnClickNo()
    {
        ToggleUI(false);
        _isConfirmed = true;
        _canActivateSpSkill = false;
    }

    /// <summary>
    /// 必殺技発動の確認画面の押下フラグのセット
    /// </summary>
    /// <param name="isClicked"></param>
    public void SetIsConfirmed(bool isConfirmed)
    {
        _isConfirmed = isConfirmed;
    }

    /// <summary>
    /// 必殺技発動可能フラグのセット
    /// </summary>
    public void SetCanActivateSpSkill(bool can)
    {
        _canActivateSpSkill = can;
    }
}
