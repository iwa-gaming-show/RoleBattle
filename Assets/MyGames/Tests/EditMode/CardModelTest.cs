using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

public class CardModelTest
{
    CardModel cardModel;

    [SetUp]
    public void SetUp()
    {
        cardModel = new GameObject().AddComponent<CardModel>();
    }

    [Test, Description("カードのデータが正しく設定されているか")]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void SetCardDataTest(bool isPlayer, bool expected)
    {
        //ScriptableObjectのアセットを読み込みます
        string cardEntityListAssetPath = "Assets/MyGames/ScriptableObjects/CardEntityList.asset";
        CardEntityList cardEntityList = AssetDatabase.LoadAssetAtPath(cardEntityListAssetPath, typeof(CardEntityList)) as CardEntityList;
        List<CardEntity> cardEntityData = cardEntityList.GetCardEntityList;

        //cardEntityDataの数分周し、i時のEntityDataが正しくmodelに設定されていればAssert
        cardModel.SetCardEntityListTestData(cardEntityList);
        for (int i = 0; i < cardEntityData.Count; i++)
        {
            cardModel.SetCardData(i, isPlayer);

            Assert.That(cardModel.Name, Is.EqualTo(cardEntityData[i].Name));
            Assert.That(cardModel.Icon, Is.EqualTo(cardEntityData[i].Icon));
            Assert.That(cardModel.CardType, Is.EqualTo(cardEntityData[i].CardType));
            Assert.That(cardModel.IsPlayerCard, Is.EqualTo(expected));
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(cardModel);
    }
}
