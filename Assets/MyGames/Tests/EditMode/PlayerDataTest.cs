using NUnit.Framework;
using static InitializationData;

public class PlayerDataTest
{
    private PlayerData playerData;

    [SetUp]
    public void SetUP()
    {
        playerData = new PlayerData(0);
        Assert.That(playerData, Is.Not.Null);
    }

    [Test, Description("ポイントに初期値が正しく設定されているか")]
    public void InitialPointTest()
    {
        Assert.That(playerData.Point, Is.EqualTo(INITIAL_POINT));
    }

    [Test, Description("ポイントを設定できるか")]
    [TestCase(0, 0)]
    [TestCase(-1, -1)]
    [TestCase(5, 5)]
    public void SetPointTest(int input, int expected)
    {
        playerData.SetPoint(input);
        Assert.That(playerData.Point, Is.EqualTo(expected));
    }

    [Test, Description("ポイントの加算ができているか")]
    [TestCase(1, 1, 2)]
    [TestCase(0, 0, 0)]
    [TestCase(-1, -1, -2)]
    public void AddPointTest(int initial, int input, int expected)
    {
        playerData.SetPoint(initial);
        playerData.AddPoint(input);
        Assert.That(playerData.Point, Is.EqualTo(expected));
    }

    [Test, Description("必殺技の使用可能設定を切り替えられるか")]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void SetCanUseSpSkillTest(bool input, bool expected)
    {
        playerData.SetCanUseSpSkill(input);
        Assert.That(playerData.CanUseSpSkill, Is.EqualTo(expected));
    }
}
