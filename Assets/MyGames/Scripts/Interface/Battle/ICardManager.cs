using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ICardManager
{
    bool IsBattleFieldPlaced
    {
        get;
    }

    /// <summary>
    /// カードのフィールド配置フラグの設定
    /// </summary>
    void SetBattleFieldPlaced(bool isBattleFieldPlaced);

    /// <summary>
    /// カードを判定する
    /// </summary>
    UniTask JudgeTheCard(Transform myBattleFieldTransform, Transform enemyBattleFieldTransform);

    /// <summary>
    /// 手札からランダムなカードを取得します
    /// </summary>
    /// <returns></returns>
    CardController GetRandomCardFrom(Transform targetHandTransform);

    /// <summary>
    /// 盤面をリセットします
    /// </summary>
    public void ResetFieldCard(
        Transform[] battleFieldTransforms,
        Transform[] handTrandforms
    );

    //// <summary>
    /// カードを配ります
    /// </summary>
    void DistributeCards(Transform myHandTransform, Transform enemyHandTransform);
}