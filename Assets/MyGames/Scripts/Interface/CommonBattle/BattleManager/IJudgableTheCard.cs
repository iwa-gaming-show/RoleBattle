using Cysharp.Threading.Tasks;

public interface IJudgableTheCard
{
    /// <summary>
    /// カードを判定する
    /// </summary>
    UniTask JudgeTheCard();

    /// <summary>
    /// カードの勝敗結果を取得する
    /// </summary>
    /// <param name="myCard"></param>
    /// <param name="enemyCard"></param>
    /// <returns></returns>
    CardJudgement JudgeCardResult(CardType playerCardType, CardType enemyCardType);

    /// <summary>
    /// 結果を反映します
    /// </summary>
    /// <param name="result"></param>
    UniTask ReflectTheResult(CardJudgement result);
}