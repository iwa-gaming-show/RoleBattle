using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class MultiSpButtonEvent : MonoBehaviour, IPointerClickHandler
{
    IConfirmationPanelManager _confirmationPanelManager;

    void Start()
    {
        _confirmationPanelManager = ServiceLocator.Resolve<IConfirmationPanelManager>();
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //spSkillの確認画面を表示
        _confirmationPanelManager.ConfirmToActivateSpSkill().Forget();
    }
}
