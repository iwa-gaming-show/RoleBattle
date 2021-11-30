using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 必殺技発動の確認UI
/// </summary>
public class MultiConfirmationPanelToSp : MonoBehaviour,
    IToggleable,
    IYesButtonAction,
    INoButtonAction
{
    [SerializeField]
    [Header("CanvasForSpecialSkillを設定")]
    Transform SpecialSkillUIManagerTransform;

    IBattleUIManager _battleUIManager;
    
    void Start()
    {
        _battleUIManager = ServiceLocator.Resolve<IBattleUIManager>();
    }

    /// <summary>
    /// UIの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, SpecialSkillUIManagerTransform);
    }

    /// <summary>
    /// 必殺技発動の確認画面でYesを押した時
    /// </summary>
    public async void OnClickYes()
    {
        ToggleUI(false);
        await _battleUIManager.ActivateSpecialSkill(true);
    }

    /// <summary>
    /// 必殺技発動の確認画面でNoを押した時
    /// </summary>
    public void OnClickNo()
    {
        ToggleUI(false);
    }
}
