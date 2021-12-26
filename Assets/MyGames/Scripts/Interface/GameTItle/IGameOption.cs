public interface IGameOption
{
    /// <summary>
    /// 変更を保存する
    /// </summary>
    /// <returns>保存の成功フラグ</returns>
    bool Save();

    /// <summary>
    /// 保存しない場合
    /// </summary>
    /// <returns></returns>
    void DoNotSave();
}
