using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using static InitializationData;
using static GameResult;

public class GameManagerTest
{
    GameManager gameManager;

    [SetUp]
    public void SetUp()
    {
        gameManager = new GameObject().AddComponent<GameManager>();
    }

    [Test, Description("プレイヤーとエネミーのデータが初期化されているか")]
    public void InitPlayerDataTest()
    {
        gameManager.InitPlayerData();

        Assert.That(gameManager.Player, Is.Not.Null);
        Assert.That(gameManager.Enemy, Is.Not.Null);
        Assert.That(gameManager.Player.Point, Is.EqualTo(INITIAL_POINT));
        Assert.That(gameManager.Enemy.Point, Is.EqualTo(INITIAL_POINT));
    }

    [Test, Description("バトルの段階が正しく切り替わるか")]
    public void ChangeBattlePhaseTest([Values] BattlePhase battlePhase)
    {
        gameManager.ChangeBattlePhase(battlePhase);

        Assert.That(gameManager.BattlePhase, Is.EqualTo(battlePhase));
    }

    [Test, Description("必殺技の演出中フラグの切り替えができる")]
    [TestCase(true)]
    [TestCase(false)]
    public void SetIsDuringProductionOfSpecialSkillTest(bool input)
    {
        gameManager.SetIsDuringProductionOfSpecialSkill(input);

        Assert.That(gameManager.IsDuringProductionOfSpecialSkill, Is.EqualTo(input));
    }

    [Test, Description("ゲームリセット処理が正しく呼ばれているか")]
    public void ResetGameStateTest()
    {
        Mock<IGameDataResetable> turnManagerMock = new Mock<IGameDataResetable>();
        Mock<IGameDataResetable> roundManagerMock = new Mock<IGameDataResetable>();

        gameManager.ResetGameState(turnManagerMock.Object, roundManagerMock.Object);

        //1回ずつ呼ばれているか
        turnManagerMock.Verify(m => m.ResetData(), Times.Exactly(1));
        roundManagerMock.Verify(m => m.ResetData(), Times.Exactly(1));
    }

    [Test, Description("ゲーム結果が正しく取得できているか")]
    [TestCase(1, 0, GAME_WIN)]
    [TestCase(2, 2, GAME_DRAW)]
    [TestCase(3, 4, GAME_LOSE)]
    public void JudgeGameResultTest(int playerPoint, int enemyPoint, GameResult expected)
    {
        //playerとenemyのデータを作る
        gameManager.InitPlayerData();
        //pointをセットする
        gameManager.Player.SetPoint(playerPoint);
        gameManager.Enemy.SetPoint(enemyPoint);

        //勝敗を取得
        GameResult actual = gameManager.JudgeGameResult();

        Assert.That(actual, Is.EqualTo(expected));
    }
}
