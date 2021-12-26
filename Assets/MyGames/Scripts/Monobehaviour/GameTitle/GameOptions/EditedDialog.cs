using UnityEngine;
using static SEType;

public class EditedDialog : MonoBehaviour,
    IEditDialog
{
    [SerializeField]
    [Header("OptionCanvasを設定")]
    Transform _optionCanvasTransform;

    IEditDialogObserver _observer;

    public void OnClickYes()
    {
        GameManager._instance.PlaySE(OPTION_CLICK);
        _observer.Update(true);
    }

    public void OnClickNo()
    {
        GameManager._instance.PlaySE(OPTION_CLICK);
        _observer.Update(false);
    }

    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, _optionCanvasTransform);
    }

    public void AddObserver(IEditDialogObserver observer)
    {
        _observer = observer;
    }
}
