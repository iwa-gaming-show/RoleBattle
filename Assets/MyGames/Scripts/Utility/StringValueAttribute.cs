using System;
using System.Reflection;
/// <summary>
/// カスタム属性、Enumに付与して使用する
/// </summary>
public class StringValueAttribute : Attribute
{
    private readonly string _value;

    public string Value => _value;

    public StringValueAttribute(string value)
    {
        _value = value;
    }
}

/// <summary>
/// StringValue属性に渡した文字列を返します
/// </summary>
public static class CommonAttribute
{
    public static string GetStringValue(Enum value)
    {
        string output = null;
        Type type = value.GetType();

        //渡した列挙体の対象のフィールドを取得
        FieldInfo fieldInfo = type.GetField(value.ToString());

        //カスタム属性を取得
        StringValueAttribute[] stringValueAttributes
            = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

        if (stringValueAttributes.Length > 0)
        {
            output = stringValueAttributes[0].Value;
        }

        return output;
    }
}
