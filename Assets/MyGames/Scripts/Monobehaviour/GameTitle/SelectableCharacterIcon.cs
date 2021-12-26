using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterIconSizes;
using UnityEngine.EventSystems;

public class SelectableCharacterIcon : MonoBehaviour,
    IPointerClickHandler,
    ISelectedCharacterSubject
{
    Image _iconImage;
    SelectableCharacter _selectableCharacter;
    ISelectedCharacterObserver _observer;

    public SelectableCharacter SelectableCharacter => _selectableCharacter;

    /// <summary>
    /// 選択可能キャラクターを設定する
    /// </summary>
    /// <param name="selectableCharacter"></param>
    public void SetSelectableCharacter(SelectableCharacter selectableCharacter)
    {
        _selectableCharacter = selectableCharacter;
        SetIconImage(_selectableCharacter.FindIconImageBy(S_SIZE));
    }

    /// <summary>
    /// アイコン画像を設定する
    /// </summary>
    /// <param name="sprite"></param>
    void SetIconImage(Sprite sprite)
    {
        if (sprite == null) return;

        if (TryGetComponent(out Image imageComp))
        {
            _iconImage = imageComp;
            _iconImage.sprite = sprite;
        }
    }

    /// <summary>
    /// 観察者を登録する
    /// </summary>
    public void AddObserver(ISelectedCharacterObserver observer)
    {
        _observer = observer;
    }

    /// <summary>
    /// 観察者にイベントを通知する
    /// </summary>
    public void NotifyObserver()
    {
        _observer.Update(_selectableCharacter);
    }

    /// <summary>
    /// クリック時に呼び出します
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        NotifyObserver();
    }
}
