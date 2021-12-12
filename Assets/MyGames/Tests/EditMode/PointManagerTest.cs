//using NUnit.Framework;
//using UnityEngine;
//using static InitializationData;

//public class PointManagerTest
//{
//    GameObject gameObject;
//    //PointManager pointManager;

//    [SetUp]
//    public void SetUp()
//    {
//        //事前準備
//        gameObject = new GameObject();
//        //pointManager = gameObject.AddComponent<PointManager>();
//    }

//    [Test, Description("指定したプレイヤーにポイントが加算されているか")]
//    [TestCase(true, 2)]
//    [TestCase(false, 1)]
//    public void AddPointToTest(bool isUsingSkillInRound, int expected)
//    {
//        PlayerData playerData = new PlayerData(INITIAL_POINT);
//        PlayerData enemyData = new PlayerData(INITIAL_POINT);

//        pointManager.AddPointTo(playerData, isUsingSkillInRound);
//        pointManager.AddPointTo(enemyData, isUsingSkillInRound);

//        //プレイヤーとエネミーのポイントを確認
//        Assert.That(playerData.Point, Is.EqualTo(expected));
//        Assert.That(enemyData.Point, Is.EqualTo(expected));
//    }

//    [Test, Description("必殺技を使用しているラウンドかどうかで獲得ポイントが変わるか")]
//    [TestCase(true, ExpectedResult = 2)]//必殺技使用時
//    [TestCase(false, ExpectedResult = 1)]//必殺技未使用時
//    public int EarnPointTest(bool isUsingSkillInRound)
//    {
//        return pointManager.EarnPoint(isUsingSkillInRound);
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        //後処理
//        Object.DestroyImmediate(gameObject);
//    }
//}
