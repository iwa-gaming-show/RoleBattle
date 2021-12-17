using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterOption : MonoBehaviour
{
    [SerializeField]
    [Header("CharaSeleWrapperを設定する")]
    GameObject _charaSeleWrapper;

    [SerializeField]
    [Header("CharacterSelectionPanelを設定する")]
    GameObject _charaSelePanel;

    [SerializeField]
    [Header("選択キャラクターのprafabを設定")]
    SelectableCharacterIcon _characterIconPrefab;

    // Start is called before the first frame update
    void Start()
    {
        SetSelectableCharacterIcons(GameManager._instance.SelectableCharacterList);
    }

    public void OnClickToToggleCharaSelePanel(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_charaSeleWrapper, isActive, transform);
        CanvasForObjectPool._instance.ToggleUIGameObject(_charaSelePanel, isActive, transform);
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
