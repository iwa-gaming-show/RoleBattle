using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] [Header("カード名")] Text _nameText;
    [SerializeField] [Header("カードの画像")] Image _iconImage;
    [SerializeField] [Header("カードの裏側")] GameObject _backSide;
    [SerializeField] [Header("カードの回転速度")] float _rotationalSpeed = 300f;

    float _cardInversionAngle = 180f;//カードが反転する角度

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
    public IEnumerator OpenTheCard()
    {
        //最初にカードを裏返した時の角度を設定する
        _cardInversionAngle *= -1;

        //-90度を越えるまで回転
        yield return StartCoroutine(RotateTheCardTo(-90f));

        //90度あたりで裏面画像を非表示にする
        ToggleBackSide(false);

        //0度まで回転する
        yield return StartCoroutine(RotateTheCardTo(0f));

        //綺麗に0度にならないことがあるので明示的に0度に補正する
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 引数で決められた角度までカードを回転する
    /// </summary>
    /// <param name="targetAngle"></param>
    /// <returns></returns>
    IEnumerator RotateTheCardTo(float targetAngle)
    {
        while (_cardInversionAngle < targetAngle)
        {
            _cardInversionAngle += _rotationalSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, _cardInversionAngle, 0);
            yield return null;
        }
    }
}
