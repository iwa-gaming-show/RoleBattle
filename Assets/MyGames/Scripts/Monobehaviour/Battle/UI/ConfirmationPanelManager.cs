public class ConfirmationPanelManager : SuperConfirmationPanelManager
{
    IBattleDataManager _dataM;

    void Start()
    {
        _dataM = ServiceLocator.Resolve<IBattleDataManager>();
    }

    #region //override methods
    /// <summary>
    /// 必殺技が発動可能か返します
    /// </summary>
    /// <returns></returns>
    protected override bool canActivateSpSkill()
    {
        return (_dataM.GetCanUseSpSkillBy(true) && MySelectionTurn());
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
}
