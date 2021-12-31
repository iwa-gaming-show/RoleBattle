using System;
using System.Reflection;

/// <summary>
/// あらゆる方法でbool値を返す、共通で使用できるクラス
/// </summary>
public static class BooleanUtil
{
    /// <summary>
    /// bool型をランダムに取得する
    /// </summary>
    /// <returns></returns>
    public static bool RandomBool()
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

    /// <summary>
    /// 文字列の入力チェックをします
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ValidateString(string input)
    {
        //nullや空文字では無ければtrue
        return (string.IsNullOrWhiteSpace(input) == false);
    }
}
