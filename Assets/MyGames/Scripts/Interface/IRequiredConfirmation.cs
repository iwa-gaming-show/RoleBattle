public interface IRequiredConfirmation
{
    bool IsConfirmed
    {
        get;
    }

    /// <summary>
    /// 確認済みかどうかのフラグを設定
    /// </summary>
    /// <param name="isClicked"></param>
    void SetIsConfirmed(bool isConfirmed);
}
