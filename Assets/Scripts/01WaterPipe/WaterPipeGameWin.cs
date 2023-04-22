using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace WaterPipe
{
    /// <summary>
    /// 随机生成地图
    /// </summary>
    public class WaterPipeGameWin : MonoSingleton<WaterPipeGameWin>
    {
        public Button resetMap;

        //地图 1000*1000  单个未知
        private const int maxWeight = 1000;

        //行列的数量
        public int lineNum;

        //father
        public Transform content;

        public Dictionary<int, WaterPipeData> DatasDic;


        private int fistId;
        private int lastId;

        private Dictionary<int, GameObject> itemsDic;
        private const string path = "01WaterPipeRes/Item";

        void Start()
        {
            resetMap.onClick.AddListener(ResetGameMap);
            ImageAtlasManager.Instance.LoadAtlas();
            itemsDic = new Dictionary<int, GameObject>();
            DatasDic = new Dictionary<int, WaterPipeData>();
        }

        public WaterPipeData GetWaterPipeDataById(int id)
        {
            DatasDic.TryGetValue(id, out var data);
            return data;
        }

        public WaterPipeItem GetWaterPipeItemById(int id)
        {
            itemsDic.TryGetValue(id, out var data);
            if (data != null)
                return data.GetComponent<WaterPipeItem>();
            else
                return null;
        }

        private void ResetGameMap()
        {
            //先清除之前的
            foreach (var item in itemsDic.Values)
                ObjectPoolManager.Instance.UnLoad(item);
            itemsDic.Clear();
            DatasDic.Clear();


            //初始化规格
            var weight = maxWeight / lineNum;
            var size = new Vector2(weight, weight);

            //随机生成开始和结束点位
            var start = Random.Range(0, lineNum);
            var startObj = ObjectPoolManager.Instance.Load(path, content);
            fistId = GetIdByXy(0, start + 1);
            var wStartData = new WaterPipeData(0, start + 1, fistId, WaterPipeEnum.Null, 1);
            startObj.GetComponent<WaterPipeItem>().InitData(wStartData, size, new Vector2(weight * -0.5f, weight * (start + 0.5f)), 1);
            startObj.GetComponent<WaterPipeItem>().OnEnableBtn(false);
            itemsDic.Add(fistId,startObj);
            DatasDic.Add(fistId, wStartData);

            var end = Random.Range(0, lineNum);
            var endObj = ObjectPoolManager.Instance.Load(path, content);
            lastId = GetIdByXy(lineNum + 1, end + 1);
            var wEndData = new WaterPipeData(lineNum + 1, end + 1, lastId, WaterPipeEnum.Null, 3);
            endObj.GetComponent<WaterPipeItem>().InitData(wEndData, size, new Vector2(weight * (lineNum + 0.5f), weight * end + weight * 0.5f), 3);
            endObj.GetComponent<WaterPipeItem>().OnEnableBtn(false);
            itemsDic.Add(lastId,endObj);
            DatasDic.Add(lastId, wEndData);


            //阵列随机
            for (int i = 0; i < lineNum; i++)
            {
                for (int j = 0; j < lineNum; j++)
                {
                    Vector2 position = new Vector2(weight * i + weight * 0.5f, weight * j + weight * 0.5f);
                    int random = Random.Range(1, 5);
                    var rotation = Random.Range(1, 4);
                    var obj = ObjectPoolManager.Instance.Load(path, content);
                    var dicI = i + 1;
                    var dicJ = j + 1;
                    var totlaID = GetIdByXy(dicI, dicJ);
                    var wData = new WaterPipeData(dicI, dicJ, totlaID, (WaterPipeEnum)random, rotation);
                    obj.GetComponent<WaterPipeItem>().InitData(wData, size, position, rotation);
                    itemsDic.Add(totlaID,obj);
                    DatasDic.Add(totlaID, wData);
                }
            }
        }
        
        private List<int> openStack = new List<int>();

        /// <summary>
        /// 判断路径是否通
        /// </summary>
        /// <returns></returns>
        public bool CheackPath()
        {
            openStack.Clear();
            //增加第一个
            openStack.Add(fistId);
            var data = DatasDic[fistId];
            CheckStep(data, WaterPipeDirectionEnum.left);
            
            return true;
        }
        
        
        /// <summary>
        /// 递归
        /// </summary>
        /// <param name="data"></param>
        /// <param name="inputEnum"></param>
        /// <returns></returns>
        private bool CheckStep(WaterPipeData data, WaterPipeDirectionEnum inputEnum)
        {
            var list = data.GetDir(inputEnum);
            if (list == null || list.Count == 0)
            {
                return false;
            }
            
            //获取流向的邻居节点
            for (int i = 0; i < list.Count; i++)
            {
                var childData = GetDataBy(data.x, data.y, (WaterPipeDirectionEnum)(list[i]));

                if (childData == null)
                {
                    openStack.Remove(data.id);
                    break;
                }
                if (openStack.Contains(childData.id))
                {
                    continue;
                }

                if (childData.id == lastId)
                {
                    Debug.Log("找到了 游戏胜利");
                    for (int j = 0; j < openStack.Count; j++)
                    {
                        GetWaterPipeItemById(openStack[j]).SetColor(true);
                    }
                    
                    return true;
                }
                else
                {
                    openStack.Add(childData.id);
                    CheckStep(childData,GetResert(list[i]));
                }
            }
            return false;
        }

        /// <summary>
        /// 获得流向位置的坐标id
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="directionEnum"></param>
        /// <returns></returns>
        private WaterPipeData GetDataBy(int x, int y, WaterPipeDirectionEnum directionEnum)
        {
            switch (directionEnum)
            {
                case WaterPipeDirectionEnum.up:
                    return GetWaterPipeDataById(GetIdByXy(x, y + 1));
                case WaterPipeDirectionEnum.right:
                    return GetWaterPipeDataById(GetIdByXy(x + 1, y));
                case WaterPipeDirectionEnum.down:
                    return GetWaterPipeDataById(GetIdByXy(x, y - 1));
                case WaterPipeDirectionEnum.left:
                    return GetWaterPipeDataById(GetIdByXy(x - 1, y));
            }
            return null;
        }

        /// <summary>
        /// 获得流向下一个位置的开始入口
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private WaterPipeDirectionEnum GetResert(int dir)
        {
            dir = (dir + 2) % 5;
            if (dir == 0)
            {
                dir = 1;
            }
            return (WaterPipeDirectionEnum)dir;
        }
        
        /// <summary>
        /// 坐标和id转换
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int GetIdByXy(int x, int y)
        {
            return x * lineNum + y;
        }
        
        /// <summary>
        /// 生成时的位置转换
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Quaternion GetRotationPos(int pos)
        {
            return Quaternion.Euler(new Vector3(0, 0, -90 * (pos - 1)));
        }

    }
}