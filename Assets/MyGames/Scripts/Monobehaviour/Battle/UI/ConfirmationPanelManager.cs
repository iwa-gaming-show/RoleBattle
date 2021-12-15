using static BattlePhase;

public class ConfirmationPanelManager : SuperConfirmationPanelManager
{
    IBattleDataManager _battleDataManager;

    void Start()
    {
        _battleDataManager = ServiceLocator.Resolve<IBattleDataManager>();
    }

    #region //override methods
    /// <summary>
    /// 必殺技が発動可能か返します
    /// </summary>
    /// <returns></returns>
    protected override bool canActivateSpSkill()
    {
        return (_battleDataManager.GetCanUseSpSkillBy(true) && MySelectionTurn());
    }

    /// <summary>
    /// 自身の選択ターンかどうかを返します
    /// </summary>
    /// <returns></returns>
    protected override bool MySelectionTurn()
    {
        bool myTurn = _battleDataManager.GetPlayerTurnBy(true);
        bool selectionPhase = (_battleDataManager.BattlePhase == SELECTION);
        bool placeable = (_battleDataManager.GetIsFieldCardPlacedBy(true) == false);

        return myTurn && selectionPhase && placeable;
    }
    #endregion
}
