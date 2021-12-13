//using System.Collections;
//using System.Collections.Generic;
//using Moq;
//using NUnit.Framework;
//using UnityEngine;
//using static InitializationData;
//using static BattleResult;

//public class BattleManagerTest
//{
//    BattleManager battleManager;

//    [SetUp]
//    public void SetUp()
//    {
//        battleManager = new GameObject().AddComponent<BattleManager>();
//    }

//    [Test, Description("プレイヤーとエネミーのデータが初期化されているか")]
//    public void InitPlayerDataTest()
//    {
//        battleManager.InitPlayerData();

//        Assert.That(battleManager.Player, Is.Not.Null);
//        Assert.That(battleManager.Enemy, Is.Not.Null);
//        Assert.That(battleManager.Player.Point, Is.EqualTo(INITIAL_POINT));
//        Assert.That(battleManager.Enemy.Point, Is.EqualTo(INITIAL_POINT));
//    }

//    [Test, Description("バトルの段階が正しく切り替わるか")]
//    public void ChangeBattlePhaseTest([Values] BattlePhase battlePhase)
//    {
//        battleManager.ChangeBattlePhase(battlePhase);

//        Assert.That(battleManager.BattlePhase, Is.EqualTo(battlePhase));
//    }

//    [Test, Description("必殺技の演出中フラグの切り替えができる")]
//    [TestCase(true)]
//    [TestCase(false)]
//    public void SetIsDuringProductionOfSpecialSkillTest(bool input)
//    {
//        battleManager.SetIsDuringProductionOfSpecialSkill(input);

//        Assert.That(battleManager.IsDuringProductionOfSpecialSkill, Is.EqualTo(input));
//    }

//    [Test, Description("ゲームリセット処理が正しく呼ばれているか")]
//    public void ResetGameStateTest()
//    {
//        Mock<IGameDataResetable> turnManagerMock = new Mock<IGameDataResetable>();
//        Mock<IGameDataResetable> roundManagerMock = new Mock<IGameDataResetable>();

//        battleManager.ResetGameState(turnManagerMock.Object, roundManagerMock.Object);

//        //1回ずつ呼ばれているか
//        turnManagerMock.Verify(m => m.ResetData(), Times.Exactly(1));
//        roundManagerMock.Verify(m => m.ResetData(), Times.Exactly(1));
//    }

//    [Test, Description("ゲーム結果が正しく取得できているか")]
//    [TestCase(1, 0, BATTLE_WIN)]
//    [TestCase(2, 2, BATTLE_DRAW)]
//    [TestCase(3, 4, BATTLE_LOSE)]
//    public void JudgeBattleResultTest(int playerPoint, int enemyPoint, BattleResult expected)
//    {
//        //playerとenemyのデータを作る
//        battleManager.InitPlayerData();
//        //pointをセットする
//        battleManager.Player.SetPoint(playerPoint);
//        battleManager.Enemy.SetPoint(enemyPoint);

//        //勝敗を取得
//        BattleResult actual = battleManager.JudgeBattleResult();

//        Assert.That(actual, Is.EqualTo(expected));
//    }
//}
