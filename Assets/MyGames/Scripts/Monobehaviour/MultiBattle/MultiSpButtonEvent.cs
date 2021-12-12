using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class MultiSpButtonEvent : MonoBehaviour, IPointerClickHandler
{
    IMultiConfirmationPanelManager _multiConfirmationPanelManager;

    void Start()
    {
        _multiConfirmationPanelManager = ServiceLocator.Resolve<IMultiConfirmationPanelManager>();
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //spSkillの確認画面を表示
        _multiConfirmationPanelManager.ConfirmToActivateSpSkill().Forget();
    }
}
