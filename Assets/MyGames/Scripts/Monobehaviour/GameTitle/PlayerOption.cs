using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOption : MonoBehaviour,
    IGameOption
{
    [SerializeField]
    [Header("SettingCanvasのTransformを設定する")]
    Transform _settingCanvasTransform;

    [SerializeField]
    [Header("CharacterSelectionPanelを設定する")]
    GameObject _charaSelePanel;

    [SerializeField]
    [Header("選択キャラクターのprafabを設定")]
    SelectableCharacterIcon _characterIconPrefab;

    void Start()
    {
        SetSelectableCharacterIcons(GameManager._instance.SelectableCharacterList);
    }

    public void Show()
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, true, _settingCanvasTransform);
    }

    public void Close()
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, false, _settingCanvasTransform);
    }

    public void OnClickToShowCharaSelePanel()
    {
        _charaSelePanel.SetActive(true);
    }

    /// <summary>
    /// 選択可能なキャラクターのアイコンを設定する
    /// </summary>
    void SetSelectableCharacterIcons(SelectableCharacterList seleCharaList)
    {
        //todo キャラごとのデータを埋め込む
        for (int i = 0; i < seleCharaList.GetSelectableCharacterList.Count; i++)
        {
            SelectableCharacterIcon characterIcon = Instantiate(_characterIconPrefab, _charaSelePanel.transform, false);
        }
    }
}
