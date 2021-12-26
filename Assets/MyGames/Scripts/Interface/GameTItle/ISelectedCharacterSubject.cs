public interface ISelectedCharacterSubject
{
    /// <summary>
    /// 観察者を登録する
    /// </summary>
    void AddObserver(ISelectedCharacterObserver observer);

    /// <summary>
    /// 観察者にイベントを通知する
    /// </summary>
    void NotifyObserver();
}
