public interface IEditable
{
    /// <summary>
    /// 編集済み
    /// </summary>
    bool IsEdited
    {
        get;
    }

    /// <summary>
    /// 編集済みフラグの設定
    /// </summary>
    /// <param name="isEdited"></param>
    void SetEdited(bool isEdited);
}
