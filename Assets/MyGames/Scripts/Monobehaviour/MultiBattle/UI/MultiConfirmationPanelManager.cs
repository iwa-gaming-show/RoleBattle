using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using static BattlePhase;

public class MultiConfirmationPanelManager : MonoBehaviour,
    IMultiConfirmationPanelManager
{
    [SerializeField]
    [Header("バトル場へ移動する確認画面を設定する")]
    MultiConfirmationPanelToField _confirmationPanelToField;

    [SerializeField]
    [Header("必殺技発動の確認画面を設定する")]
    MultiConfirmationPanelToSp _confirmationPanelToSp;

    CardController _movingFieldCard;//フィールドへ移動するカードを保存する

    public CardController MovingFieldCard => _movingFieldCard;

    void Awake()
    {
        ServiceLocator.Register<IMultiConfirmationPanelManager>(this);
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IMultiConfirmationPanelManager>(this);
    }

    /// <summary>
    /// フィールドへの移動を試みます
    /// </summary>
    /// <returns></returns>
    public async UniTask ConfirmToMoveToField(CardController selectedCard)
    {
        //todo メソッド化する
        bool myTurn = PhotonNetwork.LocalPlayer.GetIsMyTurn();
        bool myCard = selectedCard.IsPlayerCard;
        bool selectionPhase = (PhotonNetwork.CurrentRoom.GetBattlePhase() == (int)SELECTION);
        bool placeable = PhotonNetwork.LocalPlayer.GetCanPlaceCardToField();
        //選択フェイズで自身のカードが配置可能な場合操作可能
        bool controllable = myTurn && myCard && selectionPhase && placeable;

        if (controllable == false) return;


        //すでに確認画面が表示されているなら何もしない
        if (_confirmationPanelToField.gameObject.activeInHierarchy) return;

        //カードを選択し、確認画面を表示しYesならフィールドへ移動します
        SelectedToFieldCard(selectedCard);
        await WaitFieldConfirmationButton();

        //yesを押した時、フィールドへ移動するカードとして保持
        if (_confirmationPanelToField.CanMoveToField)
        {
            _movingFieldCard = selectedCard;
        }

        _confirmationPanelToField.SetCanMoveToField(false);
        _confirmationPanelToField.SetIsClickedConfirmationButton(false);
    }

    /// <summary>
    /// フィールドへ移動するカードを選択したとき
    /// </summary>
    public void SelectedToFieldCard(CardController selectedCard)
    {
        _confirmationPanelToField.ToggleUI(true);//確認画面を表示
        _confirmationPanelToField.SetFieldConfirmationText(selectedCard);
    }

    /// <summary>
    /// フィールドへの確認画面の押下を待ちます
    /// </summary>
    /// <returns></returns>
    async UniTask WaitFieldConfirmationButton()
    {
        await UniTask.WaitUntil(() => _confirmationPanelToField.IsClickedConfirmationButton);
    }

    /// <summary>
    /// バトル場へ移動するカードを削除します
    /// </summary>
    public void DestroyMovingBattleCard()
    {
        _movingFieldCard = null;
    }
}
