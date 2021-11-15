using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;

public class CardViewTest
{
    CardView cardView;

    [SetUp]
    public void SetUp()
    {
        cardView = new GameObject().AddComponent<CardView>();
        //カードの回転速度とカードの初期時の角度を設定
        cardView.SetRotationalSpeedTestData(300f);
        cardView.SetCardInversionAngleTestData(180f);
    }

    [UnityTest, Description("カードが表側になっているか")]
    public IEnumerator OpenTheCardTest() => UniTask.ToCoroutine(async () =>
    {
        //カードの裏面を作成
        cardView.CreateBackSideTestData();
        Assert.That(cardView.BackSide.activeInHierarchy, Is.True);

        //カードを裏から表に
        await cardView.OpenTheCard();

        //裏面が非表示になり、カードが正しく表になっている
        Assert.That(cardView.BackSide.activeInHierarchy, Is.False);
        Assert.That(cardView.gameObject.transform.eulerAngles, Is.EqualTo(new Vector3(0, 0, 0)));
    });

    [UnityTest, Description("引数で渡された角度までカードが回転するか")]
    public IEnumerator RotateTheCardToTest([Values(-90f, 0)] float value) => UniTask.ToCoroutine(async () =>
    {
        //カードを予め裏返しておく
        cardView.SetCardInversionAngleTestData(-180f);

        //引数で指定した角度まで回転する
        await cardView.RotateTheCardTo(value);

        //カードが引数で指定した角度より回転している
        Assert.That(cardView.gameObject.transform.eulerAngles.y, Is.GreaterThanOrEqualTo(value));
    });
}
