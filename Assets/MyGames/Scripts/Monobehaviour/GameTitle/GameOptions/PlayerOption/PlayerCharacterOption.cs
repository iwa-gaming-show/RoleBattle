using UnityEngine;
using UnityEngine.UI;
using static CharacterIconSizes;
using static CharacterVoiceSituations;
using static SEType;
using static PlayerPrefsKey;

public class PlayerCharacterOption : MonoBehaviour,
    IPlayerCharacterOption
{
    [SerializeField]
    [Header("CharaSeleWrapperを設定する")]
    GameObject _charaSeleWrapper;

    [SerializeField]
    [Header("CharacterSelectionPanelを設定する")]
    GameObject _charaSelePanel;

    [SerializeField]
    [Header("選択キャラクターアイコンのprafabを設定する")]
    SelectableCharacterIcon _characterIconPrefab;

    [SerializeField]
    [Header("選択したキャラクター名を設定するテキストを設定")]
    Text _selectedCharacterName;

    [SerializeField]
    [Header("選択したキャラクターを表示するImageUIを設定")]
    Image _selectedCharacterImage;

    bool _isEdited;
    SelectableCharacter _selectedCharacter;//選択したキャラクターを保持する

    public bool IsEdited => _isEdited;

    // Start is called before the first frame update
    void Start()
    {
        ViewSelectableCharacterIcons(GameManager._instance.SelectableCharacterList);
        InitSelectedCharacterWindow(GameManager._instance.GetPlayerCharacter());//選択済みキャラの初期化
    }

    /// <summary>
    /// クリックでキャラ選択画面の表示を切り替えます
    /// </summary>
    /// <param name="isActive"></param>
    public void OnClickToToggleCharaSelePanel(bool isActive)
    {
        GameManager._instance.PlaySE(OPTION_CLICK);
        ToggleUI(isActive);
    }

    /// <summary>
    /// キャラ選択画面の表示を切り替えます
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="pool"></param>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_charaSeleWrapper, isActive, transform);
        CanvasForObjectPool._instance.ToggleUIGameObject(_charaSelePanel, isActive, transform);
    }

    /// <summary>
    /// 選択可能なキャラクターのアイコンを一覧表示する
    /// </summary>
    void ViewSelectableCharacterIcons(SelectableCharacterList seleCharaList)
    {
        seleCharaList.GetSelectableCharacterList.ForEach(target => CreateCharacterIcon(target));
    }

    /// <summary>
    /// キャラクターアイコンを作成します
    /// </summary>
    /// <param name="target"></param>
    void CreateCharacterIcon(SelectableCharacter target)
    {
        SelectableCharacterIcon characterIcon = Instantiate(_characterIconPrefab, _charaSelePanel.transform, false);
        ISelectedCharacterSubject subject = characterIcon.GetComponent<ISelectedCharacterSubject>();
        subject?.AddObserver(this);
        characterIcon?.SetSelectableCharacter(target);
    }

    /// <summary>
    /// 観察対象の通知を受け取ります
    /// </summary>
    /// <param name="subject"></param>
    void ISelectedCharacterObserver.Update(SelectableCharacter selectedCharacter)
    {
        //パネルを閉じ、選択したキャラを保持し、UIに反映する
        ToggleUI(false);
        GameManager._instance.PlayVoiceBy(selectedCharacter, GLEETING);
        ViewSelectedCharacter(selectedCharacter);
        _isEdited = true;
    }

    /// <summary>
    /// 選択したキャラクターを表示します
    /// </summary>
    /// <param name="selectableCharacter"></param>
    void ViewSelectedCharacter(SelectableCharacter selectedCharacter)
    {
        //選択したキャラクターを保持する
        _selectedCharacter = selectedCharacter;
        //UIに反映する
        _selectedCharacterName.text = _selectedCharacter.Name;
        _selectedCharacterImage.sprite = _selectedCharacter.FindIconImageBy(M_SIZE);
    }

    /// <summary>
    /// クリックでボイスを再生します
    /// </summary>
    public void OnClickToPlayCharacterVoice()
    {
        GameManager._instance.PlayRandomVoiceBy(_selectedCharacter);
    }

    /// <summary>
    /// 選択済みキャラクターの情報をUIに表示します
    /// </summary>
    void InitSelectedCharacterWindow(SelectableCharacter selectedCharacter)
    {
        ViewSelectedCharacter(selectedCharacter);
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    /// <returns>保存の成功フラグ</returns>
    public bool Save()
    {
        //未選択なら保存扱いにして何もしない
        if (_selectedCharacter == null) return true;
        if (_isEdited == false) return true;

        PlayerPrefs.SetInt(SELECTED_CHARACTER_ID, _selectedCharacter.Id);
        PlayerPrefs.Save();
        return true;
    }

    /// <summary>
    /// 保存しない場合
    /// </summary>
    /// <returns></returns>
    public void DoNotSave()
    {
        //設定を未編集時に初期化します
        InitSelectedCharacterWindow(GameManager._instance.GetPlayerCharacter());
    }

    public void SetEdited(bool isEdited)
    {
        _isEdited = isEdited;
    }
}
