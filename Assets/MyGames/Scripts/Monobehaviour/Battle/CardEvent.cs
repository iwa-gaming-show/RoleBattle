using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvent : MonoBehaviour, IPointerClickHandler
{
    IConfirmationPanelManager _confirmationPanelManager;
    CardController cardController;

    void Awake()
    {
        cardController = GetComponent<CardController>();
    }

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
        _confirmationPanelManager.ConfirmToMoveToField(cardController).Forget();
    }
}