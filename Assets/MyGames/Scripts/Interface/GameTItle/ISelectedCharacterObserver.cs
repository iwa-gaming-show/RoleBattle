public interface ISelectedCharacterObserver
{
    /// <summary>
    /// 被験者の通知を受け取る
    /// </summary>
    /// <param name="subject"></param>
    void Update(SelectableCharacter selectableCharacter);
}
