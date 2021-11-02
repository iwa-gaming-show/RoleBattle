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
}
