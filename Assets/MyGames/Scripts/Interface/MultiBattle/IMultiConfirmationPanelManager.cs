using Cysharp.Threading.Tasks;
/// <summary>
/// 確認画面を管理するManagerInterface
/// </summary>
public interface IMultiConfirmationPanelManager
{
    CardController MovingFieldCard
    {
        get;
    }
    /// <summary>
    /// フィールドへの移動を試みます
    /// </summary>
    /// <returns></returns>
    UniTask ConfirmToMoveToField(CardController selectedCard);

    /// <summary>
    /// フィールドへ移動するカードを削除します
    /// </summary>
    void DestroyMovingBattleCard();
}