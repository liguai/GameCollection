using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MieMieMie
{
    
    
    /// <summary>
    /// 设
    /// 一个item占2*2四格
    /// 一行暂定为20 * 20， 即每行最多10个item
    /// 在中心点范围 生成  数量左5右5
    /// 坐标 左偏移10 右偏移10
    ///  
    /// </summary>
    
    
    public class MieMieMieGameWin : MonoSingleton<MieMieMieGameWin>
    {
        public Transform mapContent;
        private Vector3 mapContentPos;
        public Transform boxContnet;
        public Button createBtn;
        public GameObject overMask;
        public Text titleTex;
        private int rid;

        
        private string itemPath = "02MieMieMie/Item ";
        

        private Dictionary<int, GameObject> ObjsDic;
        private Dictionary<int, List<MieData>> DataListDic;
        private Dictionary<int, MieData> MieDatasDic;
        private List<int> typeList;
        private Dictionary<int,GameObject> typeObjListDic;


        private void Start()
        {
            mapContentPos = mapContent.position;
            ObjsDic = new Dictionary<int, GameObject>();
            DataListDic = new Dictionary<int, List<MieData>>();
            MieDatasDic = new Dictionary<int, MieData>();
            typeList = new List<int>();
            typeObjListDic = new Dictionary<int, GameObject>();
            createBtn.onClick.AddListener(CreateMap);
            
            
            titleTex.text ="点击按钮开始游戏";
        }

        
        private List<MieData> GetMieDataById(int _id)
        {
            DataListDic.TryGetValue(_id, out var data);
            return data;
        }
        
        private MieData GetMieDataByRId(int _id)
        {
            MieDatasDic.TryGetValue(_id, out var data);
            return data;
        }
        
        private int GetIdByXy(int x, int y)
        {
            return x * 100 + y;
        }

        private V2Int GetXyById(int id)
        {
            return new V2Int(id / 100, id % 100);
        }

        
        /// <summary>
        /// 获得该坐标下的最后一个data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private MieData GetDataByidLast(int id)
        {
            if (DataListDic.ContainsKey(id) && DataListDic[id].Count != 0) 
            {
                var len = GetMieDataById(id);
                return GetMieDataById(id)[len.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// 数据层获得
        /// </summary>
        private void CreateMap()
        {
            overMask.SetActive(false);
            rid = 0;
            DataListDic.Clear();
            MieDatasDic.Clear();
            ResetBoxContent();
            
            
            
            foreach (var item in ObjsDic.Values)
            {
                ObjectPoolManager.Instance.UnLoad(item);
            }
            ObjsDic.Clear();
            
            
            //中心趋向 每20个/层 趋向范围向外扩大
            for (int lay = 3; lay < 11; lay++)
            {
                //必须为3的倍数
                for (int i = 0; i < 12 + lay * 3; i++)
                //for (int i = 0; i < 8; i++)
                {
                    rid++;
                    var type = Random.Range(1, 12);
                    //pos [10,10]为中心  以此中心做偏移量
                      var posX = Random.Range(-lay, lay);
                       var posY = Random.Range(-lay, lay);
                    //var pos = new V2Int(5 + i,  10); 
                    var id = GetIdByXy(10 + posX,10 + posY);
                    
                    var data = new MieData(rid, type, id);
                    data.mask = true;
                    MieDatasDic.Add(rid, data);

                    //数据层
                    if (DataListDic.ContainsKey(id))
                    {
                        DataListDic[id].Add(data);  
                    }
                    else
                    {
                        DataListDic.Add(id, new List<MieData> { data });
                    }

                }
            }
            
            for (int i = 1; i < MieDatasDic.Count + 1; i++)
            {
                var data = MieDatasDic[i];
                MieDatasDic[i] = SetUpLayerData(data);
            }

            Debug.Log(MieDatasDic);
            Debug.Log(DataListDic);

            CreateItem();
        }

        /// <summary>
        /// 渲染层
        /// </summary>
        private void CreateItem()
        {
            //渲染层
            foreach (var data in MieDatasDic.Values)
            {
                var  obj = ObjectPoolManager.Instance.Load(itemPath + data.type, mapContent);
                obj.name = data.rid.ToString();
                ObjsDic.Add(data.rid, obj );
                obj.GetComponent<MieMieItem>().InitData(data);
                obj.transform.SetAsLastSibling();
            }
        }
        
        /// <summary>
        /// 上面有节点 且比自己的rid大
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private MieData SetUpLayerData(MieData data)
        {
            var vint = GetXyById(data.posId);
            var leftUp = GetIdByXy(vint.x - 1, vint.y + 1);
            var leftDown = GetIdByXy(vint.x - 1, vint.y - 1);
            var rightUp = GetIdByXy(vint.x + 1, vint.y + 1);
            var rightDown = GetIdByXy(vint.x + 1, vint.y - 1);
            var left = GetIdByXy(vint.x - 1, vint.y);
            var up = GetIdByXy(vint.x, vint.y + 1);
            var right = GetIdByXy(vint.x + 1, vint.y);
            var down = GetIdByXy(vint.x, vint.y - 1);

            var bol1 = HasUpLayer(leftUp,data);
            var bol2 = HasUpLayer(leftDown,data);
            var bol3 = HasUpLayer(rightUp,data);
            var bol4 = HasUpLayer(rightDown,data);
            var bol5 = HasUpLayer(left,data);
            var bol6 = HasUpLayer(up,data);
            var bol7 = HasUpLayer(right,data);
            var bol8 = HasUpLayer(down,data);
            var bol9 = HasUpLayer(data.posId, data);
            
            //该节点上方有点 返回ture 
            data.mask = bol1 | bol2 | bol3 | bol4 | bol5 | bol6 | bol7 | bol8 | bol9;
            
            return data;
        }

        /// <summary>
        /// 是否存在上层方块
        /// </summary>
        /// <param name="upId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool HasUpLayer(int upId, MieData data)
        {
            if (GetMieDataById(upId) == null 
                || GetMieDataById(upId).Count == 0 
                || GetDataByidLast(upId).rid == data.rid
                || GetDataByidLast(upId).rid < data.rid) 
            {
                return false;
            }
            return true;
        }
        
        
        
        
        
        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector3 GetPositionByPos(int posId)
        {
            var pos = GetXyById(posId);
            return new Vector3(mapContentPos.x + pos.x * 50 + 25, mapContentPos.y + pos.y * 50 + 25);
        }
        
        /// <summary>
        /// 当方块点击后
        /// </summary>
        /// <param name="rid"></param>
        public void RemoveItem(int rid)
        {
            List<int> cenInts = new List<int>();
            var data = GetMieDataByRId(rid);
            
            DataListDic[data.posId].Remove(data);
            ObjsDic.Remove(rid);
            
            //获得周围的邻居  邻居各自再算一遍 自己是否被遮挡
            var vint =  GetXyById(GetMieDataByRId(rid).posId);
            cenInts.Add(GetIdByXy(vint.x - 1, vint.y + 1));            
            cenInts.Add(GetIdByXy(vint.x - 1, vint.y - 1));            
            cenInts.Add(GetIdByXy(vint.x + 1, vint.y - 1));            
            cenInts.Add(GetIdByXy(vint.x + 1, vint.y + 1));            
            cenInts.Add(GetIdByXy(vint.x, vint.y + 1));            
            cenInts.Add(GetIdByXy(vint.x, vint.y - 1));            
            cenInts.Add(GetIdByXy(vint.x + 1, vint.y));            
            cenInts.Add(GetIdByXy(vint.x - 1, vint.y));
            cenInts.Add(data.posId);

            for (int i = 0; i < cenInts.Count; i++)
            {
                var single = GetDataByidLast(cenInts[i]);
                if (single != null)
                {
                    var  thisData =  SetUpLayerData(single);
                    ObjsDic[single.rid].GetComponent<MieMieItem>().InitData(thisData);
                }
            } 

            //最后移除当前
            MieDatasDic.Remove(rid);


            //添加方块
            InsertBoxItem(data.type);
        }

        Dictionary<int, int> listDic = new Dictionary<int, int>(); 

        /// <summary>
        /// 插入box  三消判断
        /// </summary>
        /// <param name="type"></param>
        private void InsertBoxItem(int type)
        {
            typeList.Add(type);
            typeList.Sort();
            
            //处理逻辑层
            int allNum = 0;
            int num = 0;

            listDic.Clear();
            //判断是否有相同的三个
            for (int i = 0; i < typeList.Count; i++)
            {
                if (listDic.ContainsKey(typeList[i]))
                {
                    listDic[typeList[i]] += 1;
                    if (listDic[typeList[i]] == 3)
                    {
                        listDic.Remove(typeList[i]);
                    }
                }
                else
                {
                    listDic.Add(typeList[i], 1);
                }
            }
            
            foreach (var listKey in listDic.Keys)
            {
                allNum += listDic[listKey];
            }

            if (allNum == 9)
            {
                Debug.Log("游戏结束");
                titleTex.text ="游戏结束,点击按钮重新开始";

                overMask.SetActive(true);
                return;
            }
            
            //刷新渲染层数据
            foreach (var item in typeObjListDic.Values)
            {
                ObjectPoolManager.getInstance().UnLoad(item);
            }
            typeObjListDic.Clear();
            
            foreach (var typeID in listDic.Keys)
            {
                for (int i = 0; i < listDic[typeID]; i++)
                {
                    num++;
                    var obj = ObjectPoolManager.Instance.Load(itemPath + typeID, boxContnet);
                    obj.GetComponent<MieMieItem>().SetRayCast();
                    obj.transform.SetAsLastSibling();
                    typeObjListDic.Add(num, obj);
                }
            }

        }


        /// <summary>
        /// 三消初始化
        /// </summary>
        private void ResetBoxContent()
        {
            listDic.Clear();
            typeList.Clear();
            foreach (var item in typeObjListDic.Values)
            {
                ObjectPoolManager.getInstance().UnLoad(item);
            }
            typeObjListDic.Clear();
        }
    }
}
