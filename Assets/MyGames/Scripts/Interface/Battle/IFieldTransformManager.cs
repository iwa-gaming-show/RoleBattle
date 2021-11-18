using UnityEngine;

public interface IFieldTransformManager
{
    #region プロパティ
    Transform MyBattleFieldTransform
    {
        get;
    }
    Transform EnemyBattleFieldTransform
    {
        get;
    }
    Transform MyHandTransform
    {
        get;
    }
    Transform EnemyHandTransform
    {
        get;
    }
    Transform[] BattleFieldTransforms
    {
        get;
    }
    /// <summary>
    /// MyHandTransformとEnemyHandTransformを格納した配列を返します
    /// </summary>
    Transform[] HandTransforms
    {
        get;
    }
    #endregion

    /// <summary>
    /// ターンになったプレイヤーの手札のTransformを取得する
    /// </summary>
    /// <returns></returns>
    Transform GetHandTransformByTurn(bool IsMyTurn);
}