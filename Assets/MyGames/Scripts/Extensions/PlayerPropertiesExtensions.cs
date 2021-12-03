using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PlayerPropertiesExtensions
{
    static readonly Hashtable propsToSet = new Hashtable();
    static readonly string PointKey = "Point";
    static readonly string CanUseSpSkillKey = "CanUseSpSkill";
    static readonly string CanPlaceCardToFieldKey = "CanPlaceCardToField";
    static readonly string IsMyTurnKey = "IsMyTurn";
    static readonly string IsMyTurnEndKey = "IsMyTurnEnd";
    static readonly string IsUsingSpInRoundKey = "IsUsingSpInRound";
    static readonly string BattleCardTypeKey = "BattleCardType";


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

    // <summary>
    /// プレイヤーのターンが終了したかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsMyTurnEnd(this Player player)
    {
        return (player.CustomProperties[IsMyTurnEndKey] is bool isMyTurnEnd) ? isMyTurnEnd : false;
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    public static void SetIsMyTurnEnd(this Player player, bool isMyTurnEnd)
    {
        propsToSet[IsMyTurnEndKey] = isMyTurnEnd;
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
    /// 必殺技の発動中かどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    public static void SetIsUsingSpInRound(this Player player, bool isUsingSpInRound)
    {
        propsToSet[IsUsingSpInRoundKey] = isUsingSpInRound;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// カードをフィールドに置くことができるかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetCanPlaceCardToField(this Player player)
    {
        return (player.CustomProperties[CanPlaceCardToFieldKey] is bool canPlaceCardToField) ? canPlaceCardToField : false;
    }

    /// <summary>
    /// カードをフィールドに置くことができるかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    public static void SetCanPlaceCardToField(this Player player, bool canPlaceCardToField)
    {
        propsToSet[CanPlaceCardToFieldKey] = canPlaceCardToField;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// バトル場へのカードのタイプを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetIntBattleCardType(this Player player)
    {
        return (player.CustomProperties[BattleCardTypeKey] is int cardType) ? cardType : 0;
    }

    /// <summary>
    /// バトル場へのカードのタイプを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="point"></param>
    public static void SetIntBattleCardType(this Player player, CardType cardType)
    {
        propsToSet[BattleCardTypeKey] = (int)cardType;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
