using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GM = GameManager;

/// <summary>
/// 必殺技発動の確認UI
/// </summary>
public class ConfirmationPanelToSpecialSkill : MonoBehaviour,
    IToggleable,
    IYesButtonAction,
    INoButtonAction
{
    /// <summary>
    /// UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleUI(bool isActive)
    {
        gameObject?.SetActive(isActive);
    }

    /// <summary>
    /// 必殺技発動の確認画面でYesを押した時
    /// </summary>
    public void OnClickYes()
    {
        GM._instance.UIManager.SpecialSkillUIManager.ActivateSpecialSkillByUI(true);
        ToggleUI(false);
    }

    /// <summary>
    /// 必殺技発動の確認画面でNoを押した時
    /// </summary>
    public void OnClickNo()
    {
        ToggleUI(false);
    }
}
