using System;
/// <summary>
/// ゲーム結果
/// </summary>
public enum GameResult
{
    [StringValue("YOU WIN！")]
    GAME_WIN,
    [StringValue("DRAW！")]
    GAME_DRAW,
    [StringValue("YOU LOSE！")]
    GAME_LOSE
}