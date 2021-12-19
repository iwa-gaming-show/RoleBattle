public interface IGameOption: IToggleable
{
    /// <summary>
    /// 変更を保存する
    /// </summary>
    /// <returns>保存の成功フラグ</returns>
    bool Save();
}
