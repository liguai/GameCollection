using System.Collections.Generic;

namespace MieMieMie
{
    public class MieData
    {
        public int rid;
        public int posId;       
        public int type;        //生成的类型
        public bool mask;       //遮罩
        
        public MieData(int _rid,int _type, int _pos)
        {
            rid = _rid;
            posId = _pos;
            type = _type;
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