using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] [Header("カード名")] TextMeshProUGUI _nameText;
    [SerializeField] [Header("カードの画像")] Image _iconImage;
    [SerializeField] [Header("カードの裏側")] GameObject _backSide;
    [SerializeField] [Header("カードの回転速度")] float _rotationalSpeed = 300f;

    float _cardInversionAngle = 180f;//カードが反転する角度

    public float CardInversionAngle => _cardInversionAngle;
    public GameObject BackSide => _backSide;

    /// <summary>
    /// カードを描画する
    /// </summary>
    /// <param name="cardModel"></param>
    public void SetCardView(CardModel cardModel)
    {
        _nameText.text = cardModel.Name;
        _iconImage.sprite = cardModel.Icon;
        ToggleBackSide(!cardModel.IsPlayerCard);//相手のカードは裏側で表示する
    }

    /// <summary>
    /// 裏側への切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleBackSide(bool isActive)
    {
        _backSide.SetActive(isActive);
    }

    /// <summary>
    /// カードを表側にする
    /// </summary>
    /// <returns></returns>
    public async UniTask OpenTheCard()
    {
        //最初にカードを裏返した時の角度を設定する
        _cardInversionAngle *= -1;

        //-90度を越えるまで回転
        await RotateTheCardTo(-90f);

        //90度あたりで裏面画像を非表示にする
        ToggleBackSide(false);

        //0度まで回転する
        await RotateTheCardTo(0f);

        //綺麗に0度にならないことがあるので明示的に0度に補正する
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 引数で決められた角度までカードを回転する
    /// </summary>
    /// <param name = "targetAngle" ></ param >
    /// < returns ></ returns >
    public async UniTask RotateTheCardTo(float targetAngle)
    {
        while (_cardInversionAngle < targetAngle)
        {
            _cardInversionAngle += _rotationalSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, _cardInversionAngle, 0);
            await UniTask.Yield();
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// テスト用:カードの回転速度を設定します
    /// </summary>
    /// <param name="speed"></param>
    public void SetRotationalSpeedTestData(float speed)
    {
        _rotationalSpeed = speed;
    }

    /// <summary>
    /// テスト用:カードの回転する角度を設定します
    /// </summary>
    /// <param name="speed"></param>
    public void SetCardInversionAngleTestData(float angle)
    {
        _cardInversionAngle = angle;
    }

    /// <summary>
    /// テスト用:カードの裏面を作成します
    /// </summary>
    /// <param name="speed"></param>
    public void CreateBackSideTestData()
    {
        _backSide = new GameObject();
    }
#endif
}
