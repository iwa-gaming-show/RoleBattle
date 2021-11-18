using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    /// <summary>
    /// インスタンスの格納場所
    /// </summary>
    static readonly Dictionary<Type, object> _instances;

    static ServiceLocator()
    {
        _instances = new Dictionary<Type, object>();
    }

    /// <summary>
    /// インスタンスを取得する
    /// </summary>
    public static T Resolve<T>()
    {
        return (T)_instances[typeof(T)];
    }

    /// <summary>
    /// インスタンスを登録する
    /// </summary>
    public static void Register<T>(T instance)
    {
        _instances[typeof(T)] = instance;
    }

    /// <summary>
    /// インスタンスを登録解除する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void UnRegister<T>(T instance)
    {
        if (Equals(_instances[typeof(T)], instance))
        {
            _instances.Remove(typeof(T));
        }
    }

    /// <summary>
    /// 登録しているインスタンスを削除する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Clear<T>()
    {
        _instances.Remove(typeof(T));
    }
}
