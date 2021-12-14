using NUnit.Framework;

public class TurnDataTest
{
    TurnData turnData;

    [SetUp]
    public void SetUp()
    {
        turnData = new TurnData();
    }

    [Test, Description("自身のターンかの設定が正しくできているか")]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void SetIsMyTurnTest(bool input, bool expected)
    {
        //初期値はfalseになっているか
        Assert.That(turnData.IsMyTurn, Is.False);

        turnData.SetIsMyTurn(input);
        Assert.That(turnData.IsMyTurn, Is.EqualTo(expected));
    }

    [Test, Description("エネミーが必殺技を使用するターンの設定が正しくできているか")]
    [TestCase(1, 1)]
    [TestCase(2, 2)]
    public void SetEnemySpSkillTurnTest(int input, int expected)
    {
        turnData.SetEnemySpSkillTurn(input);
        Assert.That(turnData.EnemySpSkillTurn, Is.EqualTo(expected));
    }

    [Test, Description("自身のターンが終わったか")]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void SetIsMyTurnEndTest(bool input, bool expected)
    {
        //初期値はfalseになっているか
        Assert.That(turnData.IsMyTurnEnd, Is.False);

        turnData.SetIsMyTurnEnd(input);
        Assert.That(turnData.IsMyTurnEnd, Is.EqualTo(expected));
    }

    [Test, Description("エネミーのターンが終わったか")]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void SetIsEnemyTurnEnd(bool input, bool expected)
    {
        //初期値はfalseになっているか
        Assert.That(turnData.IsEnemyTurnEnd, Is.False);

        turnData.SetIsEnemyTurnEnd(input);
        Assert.That(turnData.IsEnemyTurnEnd, Is.EqualTo(expected));
    }

    [Test, Description("ターンの終了フラグがリセットされているか")]
    public void ResetTurnTest()
    {
        //まずは終了フラグをtrueにする
        turnData.SetIsMyTurnEnd(true);
        turnData.SetIsEnemyTurnEnd(true);
        Assert.That(turnData.IsMyTurnEnd, Is.True);
        Assert.That(turnData.IsEnemyTurnEnd, Is.True);

        //リセットする
        turnData.ResetTurn();
        Assert.That(turnData.IsMyTurnEnd, Is.False);
        Assert.That(turnData.IsEnemyTurnEnd, Is.False);
    }

    [Test, Description("ターンの設定を正しく切り替えられているか")]
    [TestCase(true, false, true, false)]
    [TestCase(false, true, false, true)]
    public void ChangeTurnSettingsTest(bool input, bool expected1, bool expected2, bool expected3)
    {
        turnData.SetIsMyTurn(input);
        turnData.ChangeTurnSettings();

        //自身のターンの時はIsMyTurnがfalse, IsMyTurnEndがtrue, IsEnemyTurnEndがfalse
        //相手のターンの時はIsMyTurnがtrue, IsMyTurnEndがfalse, IsEnemyTurnEndがtrue
        Assert.That(turnData.IsMyTurn, Is.EqualTo(expected1));
        Assert.That(turnData.IsMyTurnEnd, Is.EqualTo(expected2));
        Assert.That(turnData.IsEnemyTurnEnd, Is.EqualTo(expected3));
    }
}
