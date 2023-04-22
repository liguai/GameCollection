using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 对象池
 */
public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    private Dictionary<string, List<GameObject>> ObjectPoolsDic = new Dictionary<string, List<GameObject>>();

    public GameObject Load(string objName, Transform parent)
    {
        GameObject obj = FindPoolByKey(objName);
        if (obj != null)
        {
            obj.SetActive(true);
            
        }
        else
        {
            obj = Resources.Load<GameObject>(objName);
            obj = Instantiate(obj);
            Add(objName, obj);
        }
        
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        return obj;
    }


    public void UnLoad(GameObject obj)
    {
        obj.SetActive(false);
    }
    
    
    private GameObject FindPoolByKey(string key)
    {
        if (ObjectPoolsDic.ContainsKey(key))
            return ObjectPoolsDic[key].Find(p => !p.activeSelf);
        return null;
    }

    private void Add(string key,GameObject obj)
    {
        if (!ObjectPoolsDic.ContainsKey(key))
        {
            ObjectPoolsDic.Add(key, new List<GameObject>());
        }
        ObjectPoolsDic[key].Add(obj);
    }
    
    private void RemoveKey(string key)
    {
        if (ObjectPoolsDic.ContainsKey(key))
        {
            for (int i = 0; i < ObjectPoolsDic[key].Count; i++)
            {
                Destroy(ObjectPoolsDic[key][i]);
            }
            ObjectPoolsDic.Remove(key);
        }
    }
    
    public void UnSpawnAllObject()
    {   
        List<string> list = new List<string>(ObjectPoolsDic.Keys);
        for(int i = 0; i < ObjectPoolsDic.Keys.Count; i++)
        {
            if(list[i] != "Prefabs/MainScene/friendWin/privateChatPlay" && list[i] != "Prefabs/MainScene/friendWin/emojiPrefab")
                RemoveKey(list[i]);
        }
    }
}