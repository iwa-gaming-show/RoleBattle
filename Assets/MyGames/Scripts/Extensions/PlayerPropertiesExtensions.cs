using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    static readonly Hashtable propsToSet = new Hashtable();
    static readonly string PointKey = "Point";
    static readonly string CanUseSpSkillKey = "CanUseSpSkill";

    /// <summary>
    /// プレイヤーのポイントを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetPoint(this Player player)
    {
        return (player.CustomProperties[PointKey] is int point) ? point : 0;
    }

    /// <summary>
    /// プレイヤーのポイントを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="point"></param>
    public static void SetPoint(this Player player, int point)
    {
        propsToSet[PointKey] = point;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// プレイヤーの必殺技発動可能フラグを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetCanUseSpSkill(this Player player)
    {
        return (player.CustomProperties[CanUseSpSkillKey] is bool canUseSpSkill) ? canUseSpSkill : false;
    }

    public static void SetCanUseSpSkill(this Player player, bool canUseSpSkill)
    {
        propsToSet[CanUseSpSkillKey] = canUseSpSkill;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
