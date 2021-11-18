using UnityEngine;

public class FieldTransformManager : MonoBehaviour, IFieldTransformManager
{
    #region インスペクターから設定
    [SerializeField]
    [Header("自身の手札")]
    Transform _myHandTransform;

    [SerializeField]
    [Header("相手の手札")]
    Transform _enemyHandTransform;

    [SerializeField]
    [Header("自身のバトルフィールド")]
    Transform _myBattleFieldTransform;

    [SerializeField]
    [Header("相手のバトルフィールド")]
    Transform _enemyBattleFieldTransform;
    #endregion

    #region プロパティ
    public Transform MyBattleFieldTransform => _myBattleFieldTransform;
    public Transform EnemyBattleFieldTransform => _enemyBattleFieldTransform;
    public Transform MyHandTransform => _myHandTransform;
    public Transform EnemyHandTransform => _enemyHandTransform;
    public Transform[] BattleFieldTransforms => new Transform[] { _myBattleFieldTransform, _enemyBattleFieldTransform };
    public Transform[] HandTransforms => new Transform[] { _myHandTransform, _enemyHandTransform };
    #endregion

    void Awake()
    {
        ServiceLocator.Register<IFieldTransformManager>(this);
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IFieldTransformManager>(this);
    }

    /// <summary>
    /// ターンになったプレイヤーの手札のTransformを取得する
    /// </summary>
    /// <returns></returns>
    public Transform GetHandTransformByTurn(bool IsMyTurn)
    {
        if (IsMyTurn) return _myHandTransform;
        return _enemyHandTransform;
    }

    /// <summary>
    /// 対象のバトル場のカードのTransformを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public Transform GetTargetBattleFieldTransform(bool isPlayer)
    {
        if (isPlayer) return _myBattleFieldTransform;
        return _enemyBattleFieldTransform;
    }
}
