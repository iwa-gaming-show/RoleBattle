public interface IEditDialogSubject
{
    /// <summary>
    /// 観察者を登録する
    /// </summary>
    void AddObserver(IEditDialogObserver observer);
}
