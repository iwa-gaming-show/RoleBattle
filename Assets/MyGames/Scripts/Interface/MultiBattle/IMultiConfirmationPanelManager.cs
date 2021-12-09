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
    /// フィールドへの移動を確認します
    /// </summary>
    /// <returns></returns>
    UniTask ConfirmToMoveToField(CardController selectedCard);

    /// <summary>
    /// フィールドへ移動するカードを削除します
    /// </summary>
    void DestroyMovingBattleCard();

    /// <summary>
    /// 必殺技発動の確認をします
    /// </summary>
    UniTask ConfirmToActivateSpSkill();
}