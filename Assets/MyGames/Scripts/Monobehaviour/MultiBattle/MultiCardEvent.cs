using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class MultiCardEvent : MonoBehaviour, IPointerClickHandler
{
    IMultiConfirmationPanelManager _multiConfirmationPanelManager;
    CardController cardController;

    void Awake()
    {
        cardController = GetComponent<CardController>();
    }

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
        _multiConfirmationPanelManager.ConfirmToMoveToField(cardController).Forget();
    }
}