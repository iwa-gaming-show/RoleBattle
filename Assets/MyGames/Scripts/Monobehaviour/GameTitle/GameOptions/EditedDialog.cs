using UnityEngine;

public class EditedDialog : MonoBehaviour,
    IToggleable,
    IYesButtonAction,
    INoButtonAction,
    IEditDialogSubject
{
    IEditDialogObserver _observer;

    public void OnClickYes()
    {
        _observer.Update(true);
    }

    public void OnClickNo()
    {
        _observer.Update(false);
    }

    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, transform);
    }

    public void AddObserver(IEditDialogObserver observer)
    {
        _observer = observer;
    }
}
