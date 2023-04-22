using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaterPipe
{
    public class WaterPipeData
    {
        public int id;
        public int x;
        public int y;
        public int upInt;       //当前方向 1上 2右 3下 4左
        public WaterPipeEnum Enum;  //水管类型
        public WaterPipeData(int _x, int _y ,int _id,WaterPipeEnum pipeEnum , int _up)
        {
            x = _x;
            y = _y;
            id = _id;            
            upInt = _up;
            Enum = pipeEnum;
        }

        public void SetDir(int _up)
        {
            upInt = _up;
        }

        /// <summary>
        /// 根据入口方向 获得输出方向的位置
        /// </summary>
        /// <param name="inputEnum"></param>
        /// <returns></returns>
        public List<int> GetDir(WaterPipeDirectionEnum inputEnum)
        {
            List<int> inout =  new List<int>();
            switch (Enum)
            {
                case WaterPipeEnum.Null:
                    if (upInt == 1)
                    {
                        inout.Add((int)WaterPipeDirectionEnum.right);
                    }
                    else
                    {
                        inout.Add((int)WaterPipeDirectionEnum.left);
                    }
                    return inout;
                case WaterPipeEnum.Line:
                    if (upInt == 1 || upInt == 3)    //渲染方向
                    {
                        if (inputEnum == WaterPipeDirectionEnum.up)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.down)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                        }
                    }
                    else
                    {
                        if (inputEnum == WaterPipeDirectionEnum.left)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                        }
                    }
                    return inout;
                    //inout.Add((upInt + 2 ) % 4);
                case WaterPipeEnum.Argle:
                    if (upInt == 1)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.left )
                        {
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.down)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                        }
                    }
                    if (upInt == 2)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.up )
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.left)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                        }
                    }
                    if (upInt == 3)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.up )
                        {
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                        }
                    }
                    if (upInt == 4)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.down )
                        {
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                    }
                    return inout;
                case WaterPipeEnum.Three:
                    if (upInt == 1)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.left)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.up)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.down)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                            inout.Add((int)WaterPipeDirectionEnum.up);
                        }
                    }

                    if (upInt == 2)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.left)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.up)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                            inout.Add((int)WaterPipeDirectionEnum.left);
                        }
                    }
                    
                    
                    if (upInt == 3)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.up)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.right);
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.down)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.right);
                            inout.Add((int)WaterPipeDirectionEnum.up);
                        }
                    }
                    
                    if (upInt == 4)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.left)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.down);
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.down)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.down);
                            inout.Add((int)WaterPipeDirectionEnum.left);
                        }
                    }
                    
                    return inout;
                case WaterPipeEnum.DoubleArgle:
                    if (upInt == 1 || upInt ==3)
                    {
                        if (inputEnum == WaterPipeDirectionEnum.up)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.left)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.down)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                        }
                    }
                    else 
                    {
                        if (inputEnum == WaterPipeDirectionEnum.up)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.left);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.left)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.up);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.right)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.down);
                        }
                        if (inputEnum == WaterPipeDirectionEnum.down)
                        {
                            inout.Add((int)WaterPipeDirectionEnum.right);
                        }
                    }
                    return inout;
                case WaterPipeEnum.Four:
                    for (int i = 1; i < 5; i++)
                    {
                        if ((int)inputEnum != i)
                        {
                            inout.Add(i);
                        }
                    }
                    return inout;
            }

            return null;
        }
    }


    public class V2Int
    {
        public int x;
        public int y;

        public V2Int(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
}