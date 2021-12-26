public interface IGameOption
{
    /// <summary>
    /// 編集済み
    /// </summary>
    bool IsEdited
    {
        get;
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    /// <returns>保存の成功フラグ</returns>
    bool Save();
}
