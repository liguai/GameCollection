using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/// <summary>
/// 继承与Mono的泛型单例
/// 系统功能性
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    
    public static T getInstance()
    {
        return Instance;
    }
    
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name);

                instance = obj.AddComponent<T>();
            }
            return instance;
        }
        
    }

    protected virtual void Awake()
    {
        instance = this as T;
    }
}


/// <summary>
/// 数据类逻辑层泛型单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : Singleton<T>
{
    private static T _instance;
    private static readonly object objlock = new object();
    
    public static T getInstance()
    {
        return Instance;
    }
    

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (objlock)
                {
                    if (_instance == null)
                    {
                        ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                        ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                        if (ctor == null)
                        {
                            _instance = Activator.CreateInstance(typeof(T)) as T;
                            UnityEngine.Debug.LogWarning($"创建单例管理器: {typeof(T).Name} ");
                        }

                        else
                            _instance = ctor.Invoke(null) as T;
                        
                        //SingleTonManager.Instance.InsertEvent(typeof(T).Name);

                    }
                }
            }
            return _instance;
        }
    }
}