using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Reflection;

/// <summary>
/// Windows窗体管理类
/// </summary>
public class WindowsManager : Singleton<WindowsManager>
{

    [SerializeField]
    public GameObject currWindow;
    
    //记录打开的窗体
    private List<GameObject> winStack = new List<GameObject>();     //打开的窗口栈
    private List<string> winNameStack = new List<string>();         //窗口名字
    

    //固定窗口数量
    private int staticWinCount = 0;
    //常开启窗口类型
    //private string[] staticWinInfo = null; //new[] {""};
    private List<string> staticWinInfo = new List<string>() { };

    /// <summary>
    /// 打开的窗体入链表
    /// </summary>
    /// <param name="windows"></param>
    public void SaveWindow(GameObject windows)
    {
        if (!winStack.Contains(windows))
        {
            winStack.Add(windows);
            winNameStack.Add(windows.name);
        }
        currWindow = windows;
        currWindow.transform.SetAsLastSibling();
        
        SureCameraCanMove();
    }
    
    /// <summary>
    /// 出链
    /// </summary>
    /// <param name="windows"></param>
    public void RemoveWindow(GameObject windows)
    {
        if (winStack.Contains(windows))
        {
            winStack.Remove(windows);
            winNameStack.Remove(windows.name);
        }
        
        SureCameraCanMove();
    }


    /// <summary>
    /// 根据窗口打开的数量判断视角是否可移动
    /// </summary>
    private void SureCameraCanMove()
    {
        bool cant = false;
        if (winStack != null)
        {
            cant = winStack.Count > staticWinCount;
        }
        //CameraControll.isCameraMove = !cant ; 
    }


    /// <summary>
    /// 关闭所有已经打开的窗体
    /// </summary>
    /// <param name="withOut">除了 某些窗口 不传递即为关闭所有</param>
    public void CloseAllWindows(params string[] withOut)
    {
        if (winStack.Count > staticWinCount)
        {
            if (withOut != null)
            {
                var addList = withOut.Union(staticWinInfo).ToList();
                var except = winNameStack.Except(addList).ToList();
                for (int i = 0; i < winStack.Count; i++)
                {
                    if (except.Contains(winStack[i].name))
                    {
                        winStack[i].SendMessage("Hide", SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            else
            {
                for (int i = 0; i < winStack.Count; i++)
                {
                    winStack[i].SendMessage("Hide", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
    
    
    private void OnApplicationQuit()
    {
        winStack.Clear();
        winNameStack.Clear();
    }
}
