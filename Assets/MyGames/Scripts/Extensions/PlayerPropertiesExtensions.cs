using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    static readonly Hashtable propsToSet = new Hashtable();
    static readonly string PointKey = "Point";
    static readonly string CanUseSpecialSkillKey = "CanUseSpecialSkill";

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
    public static bool GetCanUseSpecialSkill(this Player player)
    {
        return (player.CustomProperties[CanUseSpecialSkillKey] is bool canUseSpecialSkill) ? canUseSpecialSkill : false;
    }

    public static void SetCanUseSpecialSkill(this Player player, bool canUseSpecialSkill)
    {
        propsToSet[CanUseSpecialSkillKey] = canUseSpecialSkill;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
