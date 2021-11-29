using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    static readonly Hashtable propsToSet = new Hashtable();
    static readonly string PointKey = "Point";
    static readonly string CanUseSpSkillKey = "CanUseSpSkill";
    static readonly string IsMyTurnKey = "IsMyTurn";
    static readonly string IsUsingSpInRoundKey = "IsUsingSpInRound";

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

    /// <summary>
    /// プレイヤーの必殺技発動可能フラグを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="canUseSpSkill"></param>
    public static void SetCanUseSpSkill(this Player player, bool canUseSpSkill)
    {
        propsToSet[CanUseSpSkillKey] = canUseSpSkill;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// プレイヤーのターンかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsMyTurn(this Player player)
    {
        return (player.CustomProperties[IsMyTurnKey] is bool isMyTurn) ? isMyTurn : false;
    }

    /// <summary>
    /// プレイヤーのターンかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    public static void SetIsMyTurn(this Player player, bool isMyTurn)
    {
        propsToSet[IsMyTurnKey] = isMyTurn;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// 必殺技の発動中かどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsUsingSpInRound(this Player player)
    {
        return (player.CustomProperties[IsUsingSpInRoundKey] is bool isUsingSpInRound) ? isUsingSpInRound : false;
    }

    /// <summary>
    /// プレイヤーのターンかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    public static void SetIsUsingSpInRound(this Player player, bool isUsingSpInRound)
    {
        propsToSet[IsUsingSpInRoundKey] = isUsingSpInRound;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
