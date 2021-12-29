using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using static SEType;

public class MultiConfirmationPanelManager : SuperConfirmationPanelManager
{
    [SerializeField]
    [Header("部屋を退室する確認画面を設定する")]
    ConfirmationPanelToLeaveRoom _confirmationPanelToLeaveRoom;
    

    IMultiBattleDataManager _dataM;

    void Start()
    {
        _dataM = ServiceLocator.Resolve<IMultiBattleDataManager>();
    }

    #region //override methods
    /// <summary>
    /// 必殺技が発動可能か返します
    /// </summary>
    /// <returns></returns>
    protected override bool canActivateSpSkill()
    {
        return (_dataM.GetCanUseSpSkill(_dataM.GetPlayerBy(true)) && MySelectionTurn());
    }

    /// <summary>
    /// 自身の選択ターンかどうかを返します
    /// </summary>
    /// <returns></returns>
    protected override bool MySelectionTurn()
    {
        return _dataM.MySelectionTurn();
    }
    #endregion

    /// <summary>
    /// 部屋を退室するか確認します
    /// </summary>
    public void OnClickToConfirmLeavingRoom()
    {
        ConfirmToLeaveRoom().Forget();
    }

    /// <summary>
    /// ルームを退室するか確認します
    /// </summary>
    async UniTask ConfirmToLeaveRoom()
    {
        GameManager._instance.PlaySE(STANDARD_CLICK);
        //確認画面を表示しYesなら部屋を退室します
        ViewConfirmationPanelFor(_confirmationPanelToLeaveRoom);
        await WaitFieldConfirmationButton(_confirmationPanelToLeaveRoom);

        //yesなら部屋を退室します
        if (_confirmationPanelToLeaveRoom.CanLeaveRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        _confirmationPanelToLeaveRoom.SetIsConfirmed(false);
        _confirmationPanelToLeaveRoom.SetCanLeaveRoom(false);
    }
}
