using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// 部屋の退室の確認UI
/// </summary>
public class ConfirmationPanelToLeaveRoom : MonoBehaviour,
    IToggleable,
    IYesButtonAction,
    INoButtonAction,
    IRequiredConfirmation
{
    [SerializeField]
    [Header("CanvasForConfirmationPanelsを設定")]
    Transform _canvasForConfirmationPanelsTransform;

    bool _isConfirmed;
    bool _canLeaveRoom;

    #region//プロパティ
    public bool IsConfirmed => _isConfirmed;
    public bool CanLeaveRoom => _canLeaveRoom;
    #endregion

    public void OnClickNo()
    {
        ToggleUI(false);
        _isConfirmed = true;
    }

    /// <summary>
    /// 部屋を退室します
    /// </summary>
    public void OnClickYes()
    {
        ToggleUI(false);
        _isConfirmed = true;
        _canLeaveRoom = true;
    }

    /// <summary>
    /// UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, _canvasForConfirmationPanelsTransform);
    }

    /// <summary>
    /// 確認画面の押下フラグのセット
    /// </summary>
    /// <param name="isClicked"></param>
    public void SetIsConfirmed(bool isConfirmed)
    {
        _isConfirmed = isConfirmed;
    }

    /// <summary>
    /// 部屋の退室フラグのセット
    /// </summary>
    public void SetCanLeaveRoom(bool can)
    {
        _canLeaveRoom = can;
    }
}
