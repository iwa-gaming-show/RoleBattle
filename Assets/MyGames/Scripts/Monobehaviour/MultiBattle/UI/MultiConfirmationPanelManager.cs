using Photon.Pun;
using static BattlePhase;

public class MultiConfirmationPanelManager : SuperConfirmationPanelManager
{
    IMultiBattleDataManager _multiBattleDataManager;

    void Start()
    {
        _multiBattleDataManager = ServiceLocator.Resolve<IMultiBattleDataManager>();
    }

    #region //override methods
    /// <summary>
    /// 必殺技が発動可能か返します
    /// </summary>
    /// <returns></returns>
    protected override bool canActivateSpSkill()
    {
        return (PhotonNetwork.LocalPlayer.GetCanUseSpSkill() && MySelectionTurn());
    }

    /// <summary>
    /// 自身の選択ターンかどうかを返します
    /// </summary>
    /// <returns></returns>
    protected override bool MySelectionTurn()
    {
        bool myTurn = PhotonNetwork.LocalPlayer.GetIsMyTurn();
        bool selectionPhase = (PhotonNetwork.CurrentRoom.GetIntBattlePhase() == (int)SELECTION);
        bool placeable = PhotonNetwork.LocalPlayer.GetIsFieldCardPlaced() == false;
        return myTurn && selectionPhase && placeable;
    }
    #endregion
}
