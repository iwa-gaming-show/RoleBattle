public interface IPointManager
{
    /// <summary>
    /// ポイントの加算
    /// </summary>
    /// <param name="isPlayer"></param>
    public void AddPointTo(PlayerData targetPlayer, bool isUsingSkillInRound);

    /// <summary>
    /// 獲得ポイント
    /// </summary>
    /// <returns></returns>
    public int EarnPoint(bool isUsingSkillInRound);
}