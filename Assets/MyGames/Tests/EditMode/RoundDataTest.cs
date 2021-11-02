using NUnit.Framework;

public class RoundDataTest
{
    RoundData roundData;

    [SetUp]
    public void SetUp()
    {
        roundData = new RoundData(3);//3はラウンドの最大数
    }

    [Test, Description("プレイヤーがラウンド中にスキルを発動したかの設定")]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void SetIsUsingPlayerSkillInRoundTest(bool input, bool expected)
    {
        //初期値はfalseになっているか
        Assert.That(roundData.IsUsingPlayerSkillInRound, Is.False);

        roundData.SetIsUsingPlayerSkillInRound(input);
        Assert.That(roundData.IsUsingPlayerSkillInRound, Is.EqualTo(expected));
    }

    [Test, Description("エネミーがラウンド中にスキルを発動したかの設定")]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void SetIsUsingEnemySkillInRoundTest(bool input, bool expected)
    {
        //初期値はfalseになっているか
        Assert.That(roundData.IsUsingPlayerSkillInRound, Is.False);

        roundData.SetIsUsingPlayerSkillInRound(input);
        Assert.That(roundData.IsUsingPlayerSkillInRound, Is.EqualTo(expected));
    }

    [Test, Description("ラウンドの設定")]
    [TestCase(0, 1)]
    [TestCase(2, 2)]
    [TestCase(-1, 1)]
    [TestCase(9999, 3)]
    public void SetRoundCountTest(int input, int expected)
    {
        roundData.SetRoundCount(input);
        Assert.That(roundData.RoundCount, Is.EqualTo(expected));
    }

    [Test, Description("ラウンドの加算がされているか")]
    [TestCase(1, 2, 3)]
    [TestCase(-1, 2, 3)]
    [TestCase(9999, 4, 5)]
    public void AddRoundCountTest(int input, int expected1, int expected2)
    {
        roundData.SetRoundCount(input);
        roundData.AddRoundCount();
        Assert.That(roundData.RoundCount, Is.EqualTo(expected1));

        //連続して呼び出して問題なく加算されているか
        roundData.AddRoundCount();
        Assert.That(roundData.RoundCount, Is.EqualTo(expected2));
    }

    [Test, Description("ラウンドの状態が正しくリセットされているか")]
    public void ResetRoundStateTest()
    {
        roundData.ResetRoundState();
        Assert.That(roundData.IsUsingPlayerSkillInRound, Is.False);
        Assert.That(roundData.IsUsingEnemySkillInRound, Is.False);
    }

    [Test, Description("プレイヤーが必殺技を使用したラウンドかどうか正しく設定されているか")]
    [TestCase(true, true, true)]
    [TestCase(true, false, false)]
    public void SetUsingSkillRoundByPlayerTest(bool isPlayer, bool isUsingSkill, bool expected)
    {
        roundData.SetUsingSkillRound(isPlayer, isUsingSkill);

        //プレイヤーの場合
        Assert.That(roundData.IsUsingPlayerSkillInRound, Is.EqualTo(expected));
        //エネミーは常にfalseになる
        Assert.That(roundData.IsUsingEnemySkillInRound, Is.False);
    }

    [Test, Description("エネミーが必殺技を使用したラウンドかどうか正しく設定されているか")]
    [TestCase(false, true, true)]
    [TestCase(false, false, false)]
    public void SetUsingSkillRoundByEnemyTest(bool isPlayer, bool isUsingSkill, bool expected)
    {
        roundData.SetUsingSkillRound(isPlayer, isUsingSkill);

        //エネミーの場合
        Assert.That(roundData.IsUsingEnemySkillInRound, Is.EqualTo(expected));
        //プレイヤーは常にfalseになる
        Assert.That(roundData.IsUsingPlayerSkillInRound, Is.False);
    }
}
