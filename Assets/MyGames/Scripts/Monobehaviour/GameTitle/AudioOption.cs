using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOption : MonoBehaviour,
    IGameOption
{
    [SerializeField]
    [Header("SettingCanvasのTransformを設定する")]
    Transform _settingCanvasTransform;

    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, _settingCanvasTransform);
    }
}
