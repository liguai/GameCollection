using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 窗体接口约束 TODO
/// </summary>
public interface WindowsDataInterface
{
    /// <summary>
    /// 初始化数据  客户端 + 服务端 以及记录在本地的数据
    /// </summary>
    void InitData();
    
    /// <summary>
    /// 在相应的Windows OnShow里调用即页面开始时获取数据 可不用
    /// </summary>
    void GetData();

    /// <summary>
    /// 更新相应的数据，部分或者全部
    /// </summary>
    /// <param name="_data">存在即为部分，Null为全部更新</param>
    void UpdateData(params object[] _data);
    
    

}
